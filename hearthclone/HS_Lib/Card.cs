namespace HS_Lib
{
    public class HS_Card
    {

        protected string copy;
        public string Copy
        {
            get { return copy; }
            set { copy = value; }
        }

        protected byte cost;
        public int Cost
        {
            get { return cost; }
            set { cost = (byte)value; }
        }

        protected string id;
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        protected string imageid;
        public string ImageId
        {
            get { return imageid; }
            set { imageid = value; }
        }

        protected HS_CardRarity rarity;
        public HS_CardRarity Rarity
        {
            get { return rarity; }
            set { rarity = value; }
        }

        protected HS_CardType type;
        public HS_CardType Type
        {
            get { return type; }
            set { type = value; }
        }

        public HS_CreatureCard CreatureCard
        {
            get
            {
                if (this is HS_CreatureCard)
                {
                    return (HS_CreatureCard)this;
                }
                else return null;
            }
        }

        public HS_Card(string copy, byte cost, string id, HS_CardRarity rarity, HS_CardType type, string imageid)
        {
            this.copy = copy;
            this.cost = cost;
            this.id = id;
            this.rarity = rarity;
            this.type = type;
            this.imageid = imageid;
        }

        public HS_Card(HS_Card card)
        {
            this.copy = card.copy;
            this.cost = card.cost;
            this.id = card.id;
            this.rarity = card.rarity;
            this.type = card.type;
            this.imageid = card.imageid;
        }

    }
}
