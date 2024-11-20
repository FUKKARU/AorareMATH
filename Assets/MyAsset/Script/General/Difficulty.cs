namespace General
{
    public static class Difficulty
    {
        public enum Type
        {
            EasyWithAssist,
            EasyWithoutAssist,
            Normal,
        }

        public static Type Value = Type.EasyWithAssist;
    }
}
