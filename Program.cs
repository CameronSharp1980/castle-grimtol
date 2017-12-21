using System;
using CastleGrimtol.Project;

namespace CastleGrimtol
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Game MainGame = new Game();
            MainGame.Setup();
            MainGame.Start();

        }
    }
}
