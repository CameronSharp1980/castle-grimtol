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
// ** Win conditions
// ** Lose conditions
// ** Lose conditions due to bad decisions
// FINISH THE HELP FUNCTION
// ** Add riddles (Maze / passphrase door) 
// ** Add enemies
// ** Add attack method
// ** Add sneak status toggle?

// Optional To do:
// Add combat (rpg turn based?)
// Add enemy inventory that drops into room on death.
// Add the ability to use items directly from the room? (Such as "use bed")
// Add equipment for combat? (armor accessories etc. You already have "items" for use in attacking in a text adventure fashion)
// Add events?
// Add disquises?
// Add puzzles? (Aside from your maze)

// Design:
// Add colors and perhaps beeps
// Add points / score?

// To consider:
// **Currently if appropriate items are in your inventory, you are passing off to the Rooms use item method.
// **   - What about if the item does not act on the room? What about the player?

// Clean up:
// Separate room instantiation from exit and item (and other) instantiation? (generate rooms method)
// Move methods to other classes (Sort them by where / what is using them)

/******** START HERE!!! *********/
// Attack method, Kill enemy, enemy drops item(s)
