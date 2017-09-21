using System.Collections.Generic;

namespace HS_Lib
{
    class HS_TestDeck : HS_Deck
    {
        public override int Count
        {
            get { return 10; }
        }

        protected override HS_CardInstance DrawCard()
        {
            return new HS_CreatureInstance("wolfie", new HS_CreatureCard(1, 3, 
                new List<HS_CreatureType>(new HS_CreatureType[] { HS_CreatureType.Beast }),
                "", (char)1, "a", HS_CardRarity.Common, HS_CardType.Creature, null));
        }
    }
}
