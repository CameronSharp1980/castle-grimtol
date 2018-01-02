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

// To do:
// ...
// ...

// Optional To do:
// Add additional combat (rpg turn based?)
// Add enemy inventory that drops into room on death.
// Add the ability to use items directly from the room? (Such as "use bed")
// Add equipment for combat? (armor accessories etc. You already have "items" for use in attacking in a text adventure fashion)
// Add events?
// Add disquises? (Could be done via your players "status" property like your sneak status.)
// Add puzzles? (Aside from your maze)
// Add ability to inspect specific parts of the room in more detail (Or items of interest in the room.)
// Add points / score?

// Design:
// Add colors
// ** Add beeps

// Clean up:
// Separate room instantiation from exit and item (and other) instantiation? (generate rooms method)
// Move methods to other classes (Sort them by where / what is using them)
