namespace HS_Lib
{
    public enum HS_CardType
    {
        Creature
    }

    public static class HS_CardTypeUtil
    {
        public static char GetCode(HS_CardType type)
        {
            switch (type)
            {
                case HS_CardType.Creature: return 'C';
            }
            return '?';
        }
    }
}
