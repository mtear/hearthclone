﻿using System.Collections.Generic;

namespace HS_Lib
{
    public class HS_Hand
    {
        private static byte MAX_CARDS = 10;

        private List<HS_CardInstance> cards;
        public List<HS_CardInstance> Cards
        {
            get { return cards; }
        }
        public int Count
        {
            get { return cards.Count; }
        }

        public HS_Hand()
        {
            cards = new List<HS_CardInstance>();
        }

        public void Add(HS_CardInstance card)
        {
            if (cards.Count < MAX_CARDS)
            {
                cards.Add(card);
            }
        }

    }
}
