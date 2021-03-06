﻿using FileParser;
using GoogleHashCode.Model;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoogleHashCode
{
    public class BookManager
    {
        private readonly string _filePath;
        private int _time;

        private IList<Library> _libraries;
        private ICollection<Library> _scannedLibraries;

        private string _outputFilePath => Path.GetFullPath(Path.Combine(
            $"{Directory.GetParent(_filePath).FullName}",
            $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}" +
                $"Outputs{Path.DirectorySeparatorChar}output_{Path.GetFileName(_filePath)}"));

        public BookManager(string filePath)
        {
            _filePath = filePath;

            LoadData(_filePath);
        }

        public void Manage()
        {
            int t = 0;
            int signingUpLibIndex = -1;

            _libraries.ForEach(l => l.OrderBooksByScore());
            _scannedLibraries = _libraries.Where(l => l.IsSignedUp).Select(l => new Library(l)).ToList();

            while (t < _time)
            {
                ScanBooks();

                signingUpLibIndex = HandleLibSignUp(t, signingUpLibIndex);

                ++t;
            }

            PrintResult();
        }

        private int HandleLibSignUp(int currentTime, int signingUpLibIndex)
        {
            if (signingUpLibIndex == -1)
            {
                var libToSignUp = SelectLibToSignUpWithMaxNumberOfTheoreticallyScannableBooks(currentTime);
                //var libToSignUp = SelectLibToSignUpWithMaxNumberOfTheoreticalScore(currentTime); // d
                //var libToSignUp = SelectLibToSignUpWithMininumSignupTime();    // c

                return libToSignUp != null
                    ? SignUpProcess(libToSignUp)
                    : -1;
            }
            else
            {
                var libSigningUp = _libraries[signingUpLibIndex];
                return SignUpProcess(libSigningUp);
            }
        }

        private int SignUpProcess(Library libToSignUp)
        {
            --libToSignUp.RemainingSignUpTime;

            if (libToSignUp.IsSignedUp)
            {
                _scannedLibraries.Add(new Library(libToSignUp));
                return -1;
            }
            return libToSignUp.Index;
        }

        /// <summary>
        /// Works better with input sets e and f
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        private Library SelectLibToSignUpWithMaxNumberOfTheoreticallyScannableBooks(int currentTime)
        {
            var remainingTime = _time - currentTime;

            var candidateLibs = _libraries.Where(l => !l.IsSignedUp);

            return
                candidateLibs.Any()
                    ? candidateLibs.MaxBy(l => l.ParallelBooks * (remainingTime - l.SignupTime)).FirstOrDefault()
                    : null;
        }

        /// <summary>
        /// Works better with input set d
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        private Library SelectLibToSignUpWithMaxNumberOfTheoreticalScore(int currentTime)
        {
            var remainingTime = _time - currentTime;

            var candidateLibs = _libraries.Where(l => !l.IsSignedUp);

            var maxScore = -1;
            Library chosenLib = null;

            foreach (var lib in candidateLibs)
            {
                var scanTime = remainingTime - lib.SignupTime;
                var bookSlots = Math.Min(lib.ParallelBooks * scanTime, lib.Books.Count);

                var scores = lib.Books
                    .Where(b => !b.IsScanned)
                    .Take(bookSlots)
                    .Sum(b => b.Score);

                if (scores > maxScore)
                {
                    maxScore = scores;
                    chosenLib = lib;
                }
            }

            return chosenLib;
        }

        /// <summary>
        /// Worts better with input set c
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        private Library SelectLibToSignUpWithMininumSignupTime()
        {
            var candidateLibs = _libraries.Where(l => !l.IsSignedUp);

            return
                candidateLibs.Any()
                    ? candidateLibs.MinBy(l => l.SignupTime).FirstOrDefault()
                    : null;
        }

        private void ScanBooks()
        {
            foreach (var lib in _libraries.Where(l => l.IsSignedUp))
            {
                var unscannedBooks = lib.Books.Where(b => !b.IsScanned);

                var booksToScan = unscannedBooks
                    .Take(Math.Min(lib.ParallelBooks, unscannedBooks.Count()));

                foreach (var b in booksToScan)
                {
                    b.IsScanned = true;
                    _scannedLibraries.Single(l => l.Index == lib.Index).Books.Add(b);
                }
            }
        }

        private void LoadData(string filePath)
        {
            var file = new ParsedFile(filePath);

            var firstLine = file.NextLine();

            var numberOfBooks = firstLine.NextElement<int>();
            var numberOfLibraries = firstLine.NextElement<int>();
            _time = firstLine.NextElement<int>();

            EnsureEmptyLine(firstLine);

            var books = new List<Book>(numberOfBooks);

            var secondLine = file.NextLine();
            for (int i = 0; i < numberOfBooks; ++i)
            {
                books.Add(new Book(i, secondLine.NextElement<int>()));
            }
            EnsureEmptyLine(secondLine);

            _libraries = new List<Library>(numberOfLibraries);
            for (int i = 0; i < numberOfLibraries; ++i)
            {
                var firstLibLine = file.NextLine();
                var library = new Library(i, firstLibLine.NextElement<int>(), firstLibLine.NextElement<int>(), firstLibLine.NextElement<int>());
                EnsureEmptyLine(firstLibLine);

                var secondLibLine = file.NextLine();

                while (!secondLibLine.Empty)
                {
                    var bookPosition = secondLibLine.NextElement<int>();
                    library.Books.Add(books[bookPosition]);
                }

                _libraries.Add(library);
            }

            if (!file.Empty)
            {
                throw new ArgumentException("Error parsing file");
            }
        }

        private static void EnsureEmptyLine(IParsedLine firstLine)
        {
            if (!firstLine.Empty)
            {
                throw new ArgumentException("Error parsing file");
            }
        }

        private void PrintResult()
        {
            Console.WriteLine($"Writing output to {_outputFilePath}");

#pragma warning disable S2930 // "IDisposables" should be disposed - Sonar doesn't know about C# 8 features
            using StreamWriter sw = new StreamWriter(_outputFilePath, append: false);
#pragma warning restore S2930 // "IDisposables" should be disposed

            var librariesWithBooks = _scannedLibraries.Where(l => l.Books.Any()).ToList();

            sw.WriteLine(librariesWithBooks.Count);

            foreach (var lib in librariesWithBooks)
            {
                sw.WriteLine($"{lib.Index} {lib.Books.Count}");
                foreach (var book in lib.Books.Take(lib.Books.Count - 1))
                {
                    sw.Write(book.Index);
                    sw.Write(" ");
                }

                sw.WriteLine(lib.Books.Last().Index);
            }
        }
    }
}
