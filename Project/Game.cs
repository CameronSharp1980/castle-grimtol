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
            string CommandString;
            Console.Write("What would you like to to?: ");
            CommandString = Console.ReadLine();
            return CommandString.Split(" ");

        }

        private void ParseCommand(string[] CommandArr)
        {
            if (CommandArr[0].ToLower() == "go")
            {
                Go(CommandArr[1].ToLower());
            }
        }

        private void Go(string Direction)
        {
            if (CurrentRoom.Exits.ContainsKey(Direction))
            {
                // System.Console.WriteLine("Test" + CurrentRoom.Exits[Direction]);
                CurrentRoom = CurrentRoom.Exits[Direction];
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

            Hallway.Exits.Add("east", Barracks);

            Barracks.Exits.Add("west", Hallway);
            Barracks.Exits.Add("east", CastleCourtyard);

            CastleCourtyard.Exits.Add("west", Barracks);
            CastleCourtyard.Exits.Add("east", CaptainsQuarters);

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