using System.Collections.Generic;

namespace HS_Lib
{
    public class HS_TestDeck : HS_Deck
    {

        public override int Count
        {
            get { return 10; }
        }

        protected override HS_CardInstance DrawCard()
        {
            byte r = 1;
            byte a = (byte)HS_Game.RAND.Next(15);
            if(a < 10)
            {
                r = (byte)(HS_Game.RAND.Next(5) + 1);
            }
            else
            {
                r = (byte)(HS_Game.RAND.Next(10) + 1);
            }
            return new HS_CreatureInstance("Slime v" + r, new HS_CreatureCard(r, r, 
                new List<HS_CreatureType>(new HS_CreatureType[] { HS_CreatureType.Beast }),
                "", r, "a", HS_CardRarity.Common, HS_CardType.Creature, null));
        }
    }
}
