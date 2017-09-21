namespace HS_Lib
{
    enum HS_CardType
    {
        Creature
    }

    static class HS_CardTypeUtil
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
