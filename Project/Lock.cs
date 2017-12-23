using System.Collections.Generic;

namespace CastleGrimtol.Project
{

    public class Lock
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public bool Locked { get; set; }


        public Lock(string name, string type, bool locked)
        {
            Name = name;
            Type = type;
            Locked = locked;
        }


    }
}