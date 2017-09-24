namespace HS_Lib
{
    public class HS_PlayerInstance
    {
        private HS_Deck deck;
        private static byte MAX_CRYSTALS = 10;

        private bool usedHero = false;
        public bool UsedHero
        {
            get { return usedHero; }
            set { usedHero = value; }
        }

        private HS_Hand hand;
        public HS_Hand Hand
        {
            get { return hand; }
        }

        private HS_Avatar avatar;
        private sbyte currentlife;
        public sbyte Life
        {
            get { return currentlife; }
        }

        private string name = "";
        public string Name
        {
            get { return name; }
        }

        private byte crystals;
        public int Crystals
        {
            get { return crystals; }
        }

        private byte mana;
        public int Mana
        {
            get { return mana; }
        }

        private bool alive = true;
        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }

        public HS_PlayerInstance(string name, HS_Avatar avatar, HS_Deck deck)
        {
            this.deck = deck;
            this.hand = new HS_Hand();
            this.avatar = avatar;
            currentlife = 30;
            this.name = name;
            crystals = 0;
            mana = 0;
        }

        public void Draw()
        {
            deck.Draw(hand);
        }

        public void SpendMana(byte cost)
        {
            if (mana >= cost)
            {
                mana -= cost;
            }
        }

        public void AddManaCrystal()
        {
            if (crystals < MAX_CRYSTALS)
            {
                crystals++;
                mana++;
            }
        }

        public void ResetMana()
        {
            mana = crystals;
            usedHero = false;
        }

        public void Play(HS_Game game, int index)
        {
            HS_CardInstance card = game.CurrentPlayer.Hand.Cards[index];
            SpendMana((byte)card.Stats.Cost);
            game.Battlefield.AddCreature(this, (HS_CreatureInstance)card);
            //game.CurrentBattlefield.Add((HS_CreatureInstance)card);
            Hand.Cards.RemoveAt(index);
            //TODO redo this
        }

        public void TakeDamage(int dmg)
        {
            int cl = (int)currentlife;
            cl -= dmg;
            currentlife = (sbyte)cl;
        }

    }
}
