using System.Collections.Generic;

namespace HS_Lib
{
    public class HS_CreatureCard : HS_Card
    {
        int power;
        public int Power
        {
            get { return power; }
            set { power = value; }
        }

        int health;
        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        List<HS_CreatureType> creatureTypes;
        public List<HS_CreatureType> CreatureTypes
        {
            get { return creatureTypes; }
        }

        public void AddCreatureType(HS_CreatureType type)
        {
            creatureTypes.Add(type);
        }

        public void RemoveCreatureType(HS_CreatureType type)
        {
            creatureTypes.Remove(type);
        }

        public HS_CreatureCard(int power, int health, List<HS_CreatureType> creatureTypes, string copy,
            byte cost, string id, HS_CardRarity rarity, HS_CardType type, string imageid)
            : base(copy, cost, id, rarity, type, imageid)
        {
            this.power = power;
            this.health = health;
            this.creatureTypes = creatureTypes;
        }

        public HS_CreatureCard(int power, int health, List<HS_CreatureType> creatureTypes, HS_Card card) : base(card)
        {
            this.power = power;
            this.health = health;
            this.creatureTypes = creatureTypes;
        }

    }
}
