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

            while (!Quit)
            {
                Look();
                Command = PromptUser();
                ParseCommand(Command);

            }
        }

        public void UseItem(string itemName)
        {
            System.Console.WriteLine($"You used the {itemName}");
            //Use / dispose of item? Use the rooms method?
        }

        private void Look()
        {
            Console.WriteLine(CurrentRoom.Description);
            for (int i = 0; i < CurrentRoom.Items.Count; i++)
            {
                Console.WriteLine("You see a " + CurrentRoom.Items[i].Name + " " + CurrentRoom.Items[i].ItemLocation);
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

            Command = CommandStringInput.Remove(CommandStringInput.IndexOf(' '), (CommandStringInput.Length - 1) - CommandStringInput.IndexOf(' ') + 1);
            CommandArg = CommandStringInput.Remove(0, CommandStringInput.IndexOf(' ') + 1);

            // if statement to check for existence of spaces?
            CommandArr = new string[] { Command, CommandArg };
            // System.Console.WriteLine(CommandArr[0]);
            // System.Console.WriteLine(CommandArr[1]);

            return CommandArr;
            // return CommandString.Split(" ");

        }

        private void ParseCommand(string[] CommandArr)
        {
            if (CommandArr[0].ToLower() == "go")
            {
                Go(CommandArr[1].ToLower());
            }
            else if (CommandArr[0].ToLower() == "take")
            {
                Take(CommandArr[1].ToLower());
            }
            else if (CommandArr[0].ToLower() == "use")
            {
                for (int i = 0; i < CurrentPlayer.Inventory.Count; i++)
                {
                    Console.WriteLine(CurrentPlayer.Inventory[i].Name);
                    if (CurrentPlayer.Inventory[i].Name.ToLower() == CommandArr[1].ToLower())
                    {
                        CurrentRoom.UseItem(CurrentPlayer.Inventory[i]);
                    }
                }
            }
        }

        private void Go(string Direction)
        {
            //use switch cases to handle multiple spellings of direction?
            // check for spaces before split?
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
                }
            }
            else if (CurrentRoom.Exits.ContainsKey(Direction))
            {
                // System.Console.WriteLine("Test" + CurrentRoom.Exits[Direction]);
                CurrentRoom = CurrentRoom.Exits[Direction];
            }
        }

        private void Take(string Item)
        {
            for (int i = 0; i < CurrentRoom.Items.Count; i++)
            {
                if (CurrentRoom.Items[i].Name.ToLower() == Item.ToLower())
                {
                    //No spaces in key names?
                    CurrentPlayer.Inventory.Add(CurrentRoom.Items[i]);
                    CurrentRoom.Items.RemoveAt(i);
                }
            }
        }

        private void GenerateRooms()
        {
            // Separate Room generation from exit and item population? (And adding to rooms dictionary?)
            Room Hallway = new Room("Hallway", "A room with an exit to the east");
            Room Barracks = new Room("Barracks", "A room with exits to the east and west");
            Room CastleCourtyard = new Room("Castle Courtyard", "A room with exits to the east and west");
            Room CaptainsQuarters = new Room("Captain's Quarters", "A room with an exit to the west");

            Item BronzeKey = new Item("Bronze Key", "An old, seemingly discarded Bronze Key", "On the Floor");
            Item SilverGoblet = new Item("Silver Goblet", "A shining silver goblet", "On a Dais in the center of the room.");

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