namespace Main.Data
{
    internal sealed class Question
    {
        internal (int N1, int N2, int N3, int N4) N { get; set; }
        internal int Target { get; set; }

        internal Question()
        {
            N = (0, 0, 0, 0);
            Target = 0;
        }
    }
}