namespace HS_Lib
{
    public abstract class HS_Deck
    {
        public void Draw(HS_Hand hand)
        {
            HS_CardInstance cardInstance = DrawCard();
            hand.Add(cardInstance);
        }

        protected abstract HS_CardInstance DrawCard();

        public abstract int Count
        {
            get;
        }
    }
}
