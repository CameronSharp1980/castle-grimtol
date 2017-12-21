using System;
using System.Collections.Generic;

namespace CastleGrimtol.Project
{
    public class Game : IGame
    {
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
            Console.WriteLine("Current Room: " + CurrentRoom.Name);
            Console.WriteLine("Current Score: " + CurrentPlayer.Score);

        }

        public void UseItem(string itemName)
        {

        }


        private void GenerateRooms()
        {
            // Separate Room generation from exit and item population? (And adding to rooms dictionary?)
            Room Hallway = new Room("Hallway", "Description comes later");
            Room Barracks = new Room("Barracks", "Description comes later");
            Room CastleCourtyard = new Room("Castle Courtyard", "Description comes later");
            Room CaptainsQuarters = new Room("Captain's Quarters", "Description comes later");

            Hallway.Exits.Add("East", Barracks);

            Barracks.Exits.Add("West", Hallway);
            Barracks.Exits.Add("East", CastleCourtyard);

            CastleCourtyard.Exits.Add("West", Barracks);
            CastleCourtyard.Exits.Add("East", CaptainsQuarters);

            CaptainsQuarters.Exits.Add("West", CastleCourtyard);

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