using System.Collections.Generic;

namespace HS_Lib
{
    public class HS_CardInstance
    {
        private List<HS_Enchantment> modifiers;
        private HS_Card baseCard;
        public HS_Card Base
        {
            get { return baseCard; }
        }

        private HS_Card modifiedCard;
        public HS_Card Stats
        {
            get { return modifiedCard; }
        }

        private string name;
        public string Name
        {
            get { return name; }
        }

        public HS_Card clone(HS_Card card)
        {
            if(card.GetType() == typeof(HS_Card))
            {
                return new HS_Card(card);
            }
            else if(card.GetType() == typeof(HS_CreatureCard))
            {
                HS_CreatureCard ncard = (HS_CreatureCard)card;
                return new HS_CreatureCard(ncard.Power, ncard.Health, 
                    new List<HS_CreatureType>(ncard.CreatureTypes), new HS_Card(card));
            }
            return null;
        }

        public HS_CardInstance(string name, HS_Card baseCard)
        {
            initialize(name, baseCard, clone(baseCard));
        }

        public HS_CardInstance(string name, HS_Card baseCard, HS_Card modifiedCard)
        {
            initialize(name, baseCard, modifiedCard);
        }

        private void initialize(string name, HS_Card baseCard, HS_Card modifiedCard)
        {
            this.name = name;
            this.baseCard = baseCard;
            this.modifiedCard = modifiedCard;
        }
    }
}
