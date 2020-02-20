namespace GoogleHashCode.Model
{
    public sealed class Book
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
    }
}
