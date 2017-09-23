namespace HS_Lib
{
    public class HS_PlayerInstance
    {
        private HS_Deck deck;

        private HS_Hand hand;
        public HS_Hand Hand
        {
            get { return hand; }
        }

        private HS_Avatar avatar;
        private char currentlife;
        public char Life
        {
            get { return currentlife; }
        }

        private string name = "";
        public string Name
        {
            get { return name; }
        }

        private char crystals;
        public int Crystals
        {
            get { return crystals; }
        }

        private char mana;
        public int Mana
        {
            get { return mana; }
        }

        public HS_PlayerInstance(string name, HS_Avatar avatar, HS_Deck deck)
        {
            this.deck = deck;
            this.hand = new HS_Hand();
            this.avatar = avatar;
            currentlife = (char)30;
            this.name = name;
            crystals = (char)0;
            mana = (char)0;
        }

        public void Draw()
        {
            deck.Draw(hand);
        }

        public void SpendMana(char cost)
        {
            if (mana >= cost)
            {
                mana -= cost;
                if (mana < 0) mana = (char)0;
            }
        }

        public void AddManaCrystal()
        {
            crystals++;
            mana++;
        }

        public void ResetMana()
        {
            mana = crystals;
        }

        public void Play(HS_Game game, int index)
        {
            HS_CardInstance card = game.CurrentPlayer.Hand.Cards[index];
            game.CurrentBattlefield.Add((HS_CreatureInstance)card);
            Hand.Cards.RemoveAt(index);
        }

        public void TakeDamage(int dmg)
        {
            int cl = (int)currentlife;
            cl -= dmg;
            currentlife = (char)cl;
        }

    }
}
