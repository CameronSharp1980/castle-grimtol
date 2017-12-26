using System;
using System.Collections.Generic;

namespace CastleGrimtol.Project
{
    public class Game : IGame
    {
        bool Quit = false;
        public Room CurrentRoom { get; set; }

        public Player CurrentPlayer { get; set; }

        public Dictionary<string, Room> Rooms = new Dictionary<string, Room>();


        public void Setup()
        {
            //re-initialize your Rooms dictionary here? or perhaps in reset?
            //Generate rooms and populate with exits (And items later)
            GenerateRooms();

            // Instantiate player
            GeneratePlayer();


        }

        public void Reset()
        {

        }

        public void Start()
        {
            string[] Command;

            // Console.WriteLine("Current Room: " + CurrentRoom.Name);
            // Console.WriteLine("Current Score: " + CurrentPlayer.Score);
            // CurrentRoom = Rooms["Barracks"];
            // Console.WriteLine("Current Room: " + CurrentRoom.Name);

            DisplayTitleScreen();
            Look();
            while (!Quit)
            {
                Command = PromptUser();
                ParseCommand(Command);

            }
        }

        private void DisplayTitleScreen()
        {
            Console.Clear();
            Console.WriteLine("<Insert Title screen and formatting here>");
            Console.WriteLine("Press Enter to start game");
            Console.ReadLine();
        }

        public void UseItem(string itemName)
        {
            //Use / dispose of item? Use the rooms method?

            // Pass to use function? (maybe the one in game.cs that takes a string?)

            /******** START HERE!!! *********/
            //HOW CAN YOU CHECK IF A BROKEN ITEM WAS NEEDED / IS STILL NEEDED AFTER FAILED USE?

            if (itemName == "empty")
            {
                Console.Clear();
                Console.WriteLine("You must enter an item name with the \"use\" command.");
                return;
            }
            for (int i = 0; i < CurrentPlayer.Inventory.Count; i++)
            {
                if (CurrentPlayer.Inventory[i].Name.ToLower() == itemName && CurrentPlayer.Inventory[i].Uses > 0)
                {
                    CurrentPlayer.Inventory[i].Uses--;
                    CurrentRoom.UseItem(CurrentPlayer.Inventory[i]);
                    if (CurrentPlayer.Inventory[i].Uses <= 0)
                    {
                        Console.WriteLine($"The {itemName} broke apart in your hands after you used it.");
                    }
                    return;
                }
                else if (CurrentPlayer.Inventory[i].Name.ToLower() == itemName && CurrentPlayer.Inventory[i].Uses <= 0)
                {
                    Console.WriteLine($"That {itemName} is broken and can no longer be used.");
                    return;
                }
            }
            Console.Clear();
            Console.WriteLine($"You do not have any {itemName} in your inventory.");

        }

        private void Look()
        {
            Console.Clear();
            Console.WriteLine(CurrentRoom.Description);
            for (int i = 0; i < CurrentRoom.Items.Count; i++)
            {
                Console.WriteLine("You see a " + CurrentRoom.Items[i].Name + " " + CurrentRoom.Items[i].ItemLocation);
            }
        }

        private void Help()
        {
            Console.Clear();
            Console.WriteLine("Implement help function here...");
        }

        private void EndGame()
        {
            string ReallyQuit;
            Console.WriteLine("Are you sure you want to quit?");
            Console.WriteLine("Enter \'Y\' for yes or \'N\' for no.");
            ReallyQuit = Console.ReadLine();
            if (ReallyQuit.ToLower() == "y" || ReallyQuit.ToLower() == "yes")
            {
                Quit = true;
            }
            else if (ReallyQuit.ToLower() == "n" || ReallyQuit.ToLower() == "no")
            {
                Quit = false;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Please enter \'Y\' or'N\'");
                EndGame();
            }
        }
        private string[] PromptUser()
        {
            string CommandStringInput;
            string[] CommandArr;
            string Command;
            string CommandArg;

            Console.Write("What would you like to to?: ");
            CommandStringInput = Console.ReadLine();
            // Parse to switch case later.(?) (Might not be able to?)
            if (CommandStringInput.Contains(" ") && CommandStringInput.IndexOf(" ") != CommandStringInput.Length - 1)
            {
                Command = CommandStringInput.Remove(CommandStringInput.IndexOf(' '), (CommandStringInput.Length - 1) - CommandStringInput.IndexOf(' ') + 1);
                CommandArg = CommandStringInput.Remove(0, CommandStringInput.IndexOf(' ') + 1);
            }
            else if (!CommandStringInput.Contains(" "))
            {
                Command = CommandStringInput.ToLower();
                CommandArg = "empty";
            }
            else if (CommandStringInput.ToLower().Contains("go") || CommandStringInput.ToLower().Contains("take") || CommandStringInput.ToLower().Contains("use") || CommandStringInput.ToLower().Contains("look") || CommandStringInput.ToLower().Contains("help") || CommandStringInput.ToLower().Contains("quit"))
            {
                Command = CommandStringInput.Remove(CommandStringInput.IndexOf(' '));
                CommandArg = "empty";
            }
            else
            {
                // Not used in ParseCommand switch case, but the CommandArr array needs both defined in any case
                // Any time the command is invalid, the default switch behavior will work.
                Command = "invalid";
                CommandArg = "empty";
            }

            // if statement to check for existence of spaces?
            CommandArr = new string[] { Command, CommandArg };
            // System.Console.WriteLine(CommandArr[0]);
            // System.Console.WriteLine(CommandArr[1]);

            return CommandArr;
            // return CommandString.Split(" ");

        }

        private void ParseCommand(string[] CommandArr)
        {
            switch (CommandArr[0].ToLower())
            {
                case "go":
                    Go(CommandArr[1].ToLower());
                    break;
                case "take":
                    Take(CommandArr[1].ToLower());
                    break;
                case "use":
                    UseItem(CommandArr[1].ToLower());
                    break;
                case "look":
                    Look();
                    break;
                case "help":
                    Help();
                    break;
                case "quit":
                    EndGame();
                    break;
                default:
                    Console.WriteLine("Invalid command");
                    break;
            }
        }

        private void Go(string Direction)
        {
            //use switch cases to handle multiple spellings of direction?
            // check for spaces before split?
            Console.Clear();
            if (Direction == "empty")
            {
                Console.WriteLine("You must enter a direction with the \"go\" command.");
                return;
            }

            if (CurrentRoom.Locks.ContainsKey(Direction))
            {
                if (CurrentRoom.Locks[Direction].Locked)
                {
                    Console.WriteLine($"The door is locked... The lock appears to fit a {CurrentRoom.Locks[Direction].Type}");
                }
                else if (CurrentRoom.Exits.ContainsKey(Direction))
                {
                    // System.Console.WriteLine("Test" + CurrentRoom.Exits[Direction]);
                    CurrentRoom = CurrentRoom.Exits[Direction];
                    Look();
                }
            }
            else if (CurrentRoom.Exits.ContainsKey(Direction))
            {
                // System.Console.WriteLine("Test" + CurrentRoom.Exits[Direction]);
                CurrentRoom = CurrentRoom.Exits[Direction];
                Look();
            }
            else
            {
                Console.WriteLine("You cannot go that way.");
            }
        }

        private void Take(string Item)
        {
            bool ItemTaken = false;
            // Console.WriteLine(ItemTaken);
            if (Item == "empty")
            {
                Console.Clear();
                Console.WriteLine("You must enter an item name with the \"take\" command.");
                return;
            }
            for (int i = 0; i < CurrentRoom.Items.Count; i++)
            {
                if (CurrentRoom.Items[i].Name.ToLower() == Item.ToLower())
                {
                    //No spaces in key names?
                    CurrentPlayer.Inventory.Add(CurrentRoom.Items[i]);
                    CurrentRoom.Items.RemoveAt(i);
                    ItemTaken = true;
                    Console.Clear();
                    Console.WriteLine($"You took the {Item}.");
                }
            }
            if (!ItemTaken)
            {
                if (CurrentPlayer.Inventory.Count > 0)
                {
                    for (int i = 0; i < CurrentPlayer.Inventory.Count; i++)
                    {
                        if (CurrentPlayer.Inventory[i].Name.ToLower() == Item.ToLower())
                        {
                            Console.Clear();
                            Console.WriteLine($"There is no {Item} in the room, but you have one in your inventory.");
                            return;
                        }
                    }
                }
                Console.Clear();
                Console.WriteLine($"There are no {Item}s in the room for you to take.");
            }

        }

        private void GenerateRooms()
        {
            // Separate Room generation from exit and item population? (And adding to rooms dictionary?)
            Room Hallway = new Room("Hallway", "A room with an exit to the east");
            Room Barracks = new Room("Barracks", "A room with exits to the east and west");
            Room CastleCourtyard = new Room("Castle Courtyard", "A room with exits to the east and west");
            Room CaptainsQuarters = new Room("Captain's Quarters", "A room with an exit to the west");

            Item BronzeKey = new Item("Bronze Key", "An old, seemingly discarded Bronze Key", "On the Floor", 1);
            Item SilverGoblet = new Item("Silver Goblet", "A shining silver goblet", "On a Dais in the center of the room.", 1);

            Lock BronzeLock = new Lock("Bronze Lock", "Bronze Key", true);

            Hallway.Exits.Add("east", Barracks);

            Barracks.Exits.Add("west", Hallway);
            Barracks.Exits.Add("east", CastleCourtyard);

            CastleCourtyard.Exits.Add("west", Barracks);
            CastleCourtyard.Exits.Add("east", CaptainsQuarters);

            CastleCourtyard.Locks.Add("east", BronzeLock);

            CaptainsQuarters.Exits.Add("west", CastleCourtyard);

            CastleCourtyard.Items.Add(BronzeKey);
            CastleCourtyard.Items.Add(SilverGoblet);

            Rooms.Add("Hallway", Hallway);
            Rooms.Add("Barracks", Barracks);
            Rooms.Add("Castle Courtyard", CastleCourtyard);
            Rooms.Add("Captain's Quarters", CaptainsQuarters);

        }

        private void GeneratePlayer()
        {
            // Welcome user and ask them to create player name? (Add propery to model)
            CurrentPlayer = new Player();
            CurrentRoom = Rooms["Hallway"];
        }

    }
}