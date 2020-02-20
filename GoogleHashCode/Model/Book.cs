using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleHashCode.Model
{
    public sealed class Book : IEquatable<Book>
    {
        public int Index { get; }

        public int Score { get; }

        public bool IsScanned { get; set; }

        public Book(int index, int score)
        {
            Index = index;
            Score = score;

            IsScanned = false;
        }

        #region Equals override
        // https://docs.microsoft.com/en-us/visualstudio/code-quality/ca1815-override-equals-and-operator-equals-on-value-types?view=vs-2017

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Book))
            {
                return false;
            }

            return Equals((Book)obj);
        }

        public bool Equals(Book other)
        {
            if (other == null)
            {
                return false;
            }

            return Score == other.Score;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Score);
        }

        public static bool operator ==(Book book1, Book book2)
        {
            if (book1 is null)
            {
                return book2 is null;
            }

            return book1.Equals(book2);
        }

        public static bool operator !=(Book book1, Book book2)
        {
            if (book1 is null)
            {
                return book2 is object;
            }

            return !book1.Equals(book2);
        }
        #endregion
    }
}
