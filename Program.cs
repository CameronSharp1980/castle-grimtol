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
// Win conditions
// Lose conditions
// Lose conditions due to bad decisions
// Add ability to peer into next room without triggering events?
// Add events?
// Add disquises?
// Add riddles and / or puzzles?
// Add enemies / combat?
// Add sneak status toggle?
// Add the ability to use items directly from the room? (Such as "use bed")
// Add equipment for combat?

// Design:
// Add colors and perhaps beeps
// Add points / score?


// To consider:
// Currently if appropriate items are in your inventory, you are passing off to the Rooms use item method.
//    - What about if the item does not act on the room? What about the player?
// Are there any other "Actions" you might add to the game? or do the "use" and "take" commands suffice?

// Clean up:
// Separate room instantiation from exit and item (and other) instantiation? (generate rooms method)

/******** START HERE!!! *********/
//HOW CAN YOU CHECK IF A BROKEN ITEM WAS NEEDED / IS STILL NEEDED AFTER FAILED USE?