using System.Collections.Generic;

namespace CastleGrimtol.Project
{

    public class Hazard
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string DeathMessage { get; set; }

        public bool Nullified { get; set; }


        public Hazard(string name, string type, string deathMessage, bool nullified)
        {
            Name = name;
            Type = type;
            DeathMessage = deathMessage;
            Nullified = nullified;
        }


    }
}