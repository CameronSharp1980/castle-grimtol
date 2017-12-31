

using System;

namespace CastleGrimtol.Project
{

    public class Chime
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int Frequency { get; set; }

        public void PlayChime()
        {
            if (this.Type == "Success")
            {
                for (int i = 500; i <= this.Frequency; i += 500)
                {
                    Console.Beep(i, 50);
                }
            }
            else if (this.Type == "Failure")
            {
                for (int i = this.Frequency; i >= 500; i -= 500)
                {
                    Console.Beep(i, 50);
                }
            }
        }

        public Chime(string name, string type, int frequency)
        {
            Name = name;
            Type = type;
            Frequency = frequency;
        }


    }
}