using System.Collections.Generic;

namespace HS_Lib
{
    public class HS_TestDeck : HS_Deck
    {

        public static System.Random rand = new System.Random();

        public override int Count
        {
            get { return 10; }
        }

        protected override HS_CardInstance DrawCard()
        {
            int r = rand.Next(10) + 1;
            return new HS_CreatureInstance("Slime v" + r, new HS_CreatureCard(r, r, 
                new List<HS_CreatureType>(new HS_CreatureType[] { HS_CreatureType.Beast }),
                "", (char)r, "a", HS_CardRarity.Common, HS_CardType.Creature, null));
        }
    }
}
