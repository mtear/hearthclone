using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HS_Lib
{
    public class HS_CreatureInstance : HS_CardInstance
    {
        private bool canAttack;
        private bool shielded;
        private bool frozen;
        private bool stealthed;

        public HS_CreatureCard CreatureStats
        {
            get { return Stats.CreatureCard; }
        }

        public HS_CreatureCard BaseCreatureStats
        {
            get { return Base.CreatureCard; }
        }

        public HS_CreatureInstance(string name, HS_Card baseCard) : base(name, baseCard)
        {
            initialize();
        }

        public HS_CreatureInstance(string name, HS_Card baseCard, HS_Card modifiedCard)
            : base(name, baseCard, modifiedCard)
        {
            initialize();
        }

        private void initialize()
        {
            canAttack = false;
            shielded = false;
            frozen = false;
            stealthed = false;
        }

    }
}
