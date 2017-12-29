using System;
using System.Collections.Generic;

namespace CastleGrimtol.Project
{
    public class Game : IGame
    {
        bool Quit = false;
        bool Dead = false;
        public Room CurrentRoom { get; set; }

        public Player CurrentPlayer { get; set; }

        public Dictionary<string, Room> Rooms = new Dictionary<string, Room>();


        public void Setup()
        {
            // Generate rooms and populate with exits, items etc.
            GenerateRooms();

            // Display Title Screen
            DisplayTitleScreen();

            // Instantiate player
            GeneratePlayer();


        }

        public void Reset()
        {
            string ReallyRestart;
            Console.WriteLine("Do you wish to restart the game?");
            Console.WriteLine("Enter \'Y\' for yes or \'N\' for no.");
            ReallyRestart = Console.ReadLine();
            if (ReallyRestart.ToLower() == "y" || ReallyRestart.ToLower() == "yes")
            {
                Setup();
                Start();
            }
            else if (ReallyRestart.ToLower() == "n" || ReallyRestart.ToLower() == "no")
            {
                if (Dead)
                {
                    Console.WriteLine("Better luck next time...");
                    return;
                }
                else
                {
                    return;
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Please enter \'Y\' or'N\'");
                Reset();
            }
        }

        public void Start()
        {
            string[] Command;

            Console.WriteLine($"Welcome {CurrentPlayer.Name} <insert greeting text here>");
            Console.WriteLine("Press enter to begin your quest...");
            Console.ReadLine();

            Look();
            while (!Quit && !Dead)
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
                    Console.WriteLine(CurrentPlayer.Inventory[i].UseText);
                    if (!CurrentPlayer.Inventory[i].PlayerItem)
                    {
                        CurrentRoom.UseItem(CurrentPlayer.Inventory[i]);
                    }
                    else
                    {
                        CurrentPlayer.UseItem(CurrentPlayer.Inventory[i]);
                    }
                    if (CurrentPlayer.Inventory[i].Uses <= 0)
                    {
                        CurrentPlayer.Inventory.Remove(CurrentPlayer.Inventory[i]);
                    }
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

        private void Peer(string Direction)
        {
            Console.Clear();
            if (Direction == "empty")
            {
                Console.WriteLine("You must enter a direction with the \"peer\" command.");
                return;
            }
            else if (CurrentPlayer.Powers.ContainsKey("peer"))
            {
                Console.WriteLine(CurrentRoom.Exits[Direction].PeerDetails);
            }
            else
            {
                Console.WriteLine("You lack that ability...");
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

        private void GiveUp()
        {
            Console.WriteLine("As dispair overwhelms you, you consider ending the torment yourself...");
            Reset();
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
            else if (CommandStringInput.ToLower().Contains("go") || CommandStringInput.ToLower().Contains("take") || CommandStringInput.ToLower().Contains("use") || CommandStringInput.ToLower().Contains("look") || CommandStringInput.ToLower().Contains("help") || CommandStringInput.ToLower().Contains("quit") || CommandStringInput.ToLower().Contains("restart") || CommandStringInput.ToLower().Contains("inventory") || CommandStringInput.ToLower().Contains("yield"))
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

            CommandArr = new string[] { Command, CommandArg };

            return CommandArr;

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
                case "inventory":
                    ViewInventory();
                    break;
                case "use":
                    UseItem(CommandArr[1].ToLower());
                    break;
                case "look":
                    Look();
                    break;
                case "peer":
                    Peer(CommandArr[1].ToLower());
                    break;
                case "help":
                    Help();
                    break;
                case "yield":
                    GiveUp();
                    break;
                case "quit":
                    EndGame();
                    break;
                case "restart":
                    Reset();
                    break;
                default:
                    Console.WriteLine("Invalid command");
                    break;
            }
        }

        private void Go(string Direction)
        {
            //use switch cases to handle multiple spellings of direction?
            Console.Clear();
            if (Direction == "empty")
            {
                Console.WriteLine("You must enter a direction with the \"go\" command.");
                return;
            }

            if (CurrentRoom.Hazards.ContainsKey(Direction))
            {
                if (!CurrentRoom.Hazards[Direction].Nullified)
                {
                    Dead = true;
                    Console.WriteLine(CurrentRoom.Hazards[Direction].DeathMessage);
                    Reset();
                    return;
                }
            }




            if (CurrentRoom.Locks.ContainsKey(Direction))
            {
                if (CurrentRoom.Locks[Direction].Locked)
                {
                    Console.WriteLine($"The door is locked... The lock appears to fit a {CurrentRoom.Locks[Direction].Type}");
                }
                else if (CurrentRoom.Exits.ContainsKey(Direction))
                {
                    CurrentRoom = CurrentRoom.Exits[Direction];
                    Look();
                }
            }
            else if (CurrentRoom.Exits.ContainsKey(Direction))
            {
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

        private void ViewInventory()
        {
            Console.Clear();
            if (CurrentPlayer.Inventory.Count > 0)
            {

                Console.WriteLine("You rummage through your pack and find the following items:");
                for (int i = 0; i < CurrentPlayer.Inventory.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {CurrentPlayer.Inventory[i].Name} : {CurrentPlayer.Inventory[i].Description}");
                }
            }
            else
            {
                Console.WriteLine("Your pack is empty...");
            }
        }

        private void GenerateRooms()
        {

            Rooms = new Dictionary<string, Room>();

            // Separate Room generation from exit and item population? (And adding to rooms dictionary?)
            Room Hallway = new Room("Hallway", "A room with an exit to the east", "PeerText for Hallway");
            Room Bridge = new Room("Bridge", "A rickety wooden bridge running east to west with doors at each end.", "Peertext for Bridge");
            Room Barracks = new Room("Barracks", "A room with exits to the east and west", "PeerText for Barracks");
            Room CastleCourtyard = new Room("Castle Courtyard", "A room with exits to the east and west", "PeerText for Castle Courtyard");
            Room CaptainsQuarters = new Room("Captain's Quarters", "A room with an exit to the west", "PeerText for Captain's Quarters");

            Item BronzeKey = new Item("Bronze Key", "An old, seemingly discarded Bronze Key", false, "You attempted to use the Bronze Key", "On the Floor", 5, false, "none");
            Item SilverGoblet = new Item("Silver Goblet", "A shining silver goblet", true, "You drank from the Silver Goblet", "On a Dais in the center of the room.", 1, false, "peer");
            Item IronSword = new Item("Iron Sword", "An Iron Sword", true, "You swung the Iron sword with all your might", "Hanging in a decorative frame on the wall", 5000, true, "attack");

            Lock BronzeLock = new Lock("Bronze Lock", "Bronze Key", true);

            Hazard Fall = new Hazard("Pit fall", "Pit fall", "Your misstep proves fatal as you fall to your death...", false);

            Hallway.Exits.Add("east", Bridge);

            Bridge.Exits.Add("west", Hallway);
            Bridge.Exits.Add("east", Barracks);

            Bridge.Hazards.Add("north", Fall);
            Bridge.Hazards.Add("south", Fall);

            Barracks.Exits.Add("west", Bridge);
            Barracks.Exits.Add("east", CastleCourtyard);

            Barracks.Items.Add(IronSword);

            CastleCourtyard.Exits.Add("west", Barracks);
            CastleCourtyard.Exits.Add("east", CaptainsQuarters);

            CastleCourtyard.Locks.Add("east", BronzeLock);

            CastleCourtyard.Items.Add(BronzeKey);
            CastleCourtyard.Items.Add(SilverGoblet);

            CaptainsQuarters.Exits.Add("west", CastleCourtyard);


            Rooms.Add("Hallway", Hallway);
            Rooms.Add("Barracks", Barracks);
            Rooms.Add("Castle Courtyard", CastleCourtyard);
            Rooms.Add("Captain's Quarters", CaptainsQuarters);

        }

        private void GeneratePlayer()
        {
            string PlayerName;
            Console.Write("Please enter a name for our hero:");
            PlayerName = Console.ReadLine();
            CurrentPlayer = new Player(PlayerName);
            CurrentRoom = Rooms["Hallway"];
        }

    }
}