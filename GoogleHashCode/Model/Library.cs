using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleHashCode.Model
{
    public class Library
    {
        public int BookCapacity { get; }

        public int Index { get; }

        public Guid Id { get; }

        public IList<Book> Books { get; private set; }

        public int SignupTime { get; }

        public int RemainingSignUpTime { get; set; }

        public int ParallelBooks { get; }

        public bool IsSignedUp => RemainingSignUpTime == 0;

        public Library(int index, int bookCapacity, int signupTime, int parallelBooks)
        {
            BookCapacity = bookCapacity;
            SignupTime = signupTime;
            ParallelBooks = parallelBooks;
            Books = new List<Book>(BookCapacity);

            Id = Guid.NewGuid();
            RemainingSignUpTime = signupTime;
            Index = index;
        }

        public Library(Library library)
        {
            Index = library.Index;
            BookCapacity = library.BookCapacity;
            SignupTime = library.SignupTime;
            ParallelBooks = library.ParallelBooks;

            Books = new List<Book>(BookCapacity);
            Id = Guid.NewGuid();
        }

        public void OrderBooksByScore()
        {
            Books = Books.OrderByDescending(b => b.Score).ToList();
        }
    }
}
