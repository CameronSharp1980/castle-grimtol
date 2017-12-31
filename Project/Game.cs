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
                Quit = false;
                Dead = false;
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

            Console.Clear();
            Console.WriteLine($"Welcome {CurrentPlayer.Name}!");
            Console.WriteLine("Your journey begins in a haze. You awaken to only darkness.");
            Console.WriteLine("You've no idea how you came to be here, or even where \"here\" is.");
            Console.WriteLine("What you do know however, is that you are in danger, and that you must escape...");
            Console.WriteLine("As your vision clears, you realize that you are in a prison cell built into a cave wall.");
            Console.WriteLine("The door is ajar and you appear to be alone.");
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
            Console.WriteLine("Welcome to \"The Pits of Despair\"");
            Console.WriteLine("A text adventure.");
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
                        if (CurrentPlayer.Inventory[i].RequiredToProceed)
                        {
                            foreach (var room in Rooms)
                            {
                                if (CurrentPlayer.Inventory[i].RequiredLocation == room.Value.Name)
                                {
                                    foreach (var lockToCheck in room.Value.Locks)
                                    {
                                        // Long shoe-horned way to check if the lock corresponding to the key that broke is still locked. (Only keys are "required to proceed" items)
                                        if (lockToCheck.Value.Type == CurrentPlayer.Inventory[i].Name && lockToCheck.Value.Locked)
                                        {
                                            Console.Clear();
                                            Console.WriteLine($"After weeks of searching, you collapse in a heap as your strength fades...\nYou realize that the {CurrentPlayer.Inventory[i].Name} you broke weeks ago was essential to your escape... \nThe darkness takes you.");
                                            Dead = true;
                                            Reset();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            CurrentPlayer.Inventory.Remove(CurrentPlayer.Inventory[i]);
                        }
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
                if (CurrentRoom.Exits.ContainsKey(Direction))
                {
                    Console.WriteLine(CurrentRoom.Exits[Direction].PeerDetails);
                }
                else if (CurrentRoom.Hazards.ContainsKey(Direction))
                {
                    Console.WriteLine("Mists obscure your farsight... You must use your wits and senses...");
                }
                else
                {
                    Console.WriteLine("There is nothing to see in that direction...");
                }
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

            if (CurrentRoom.Chimes.ContainsKey(Direction))
            {
                CurrentRoom.Chimes[Direction].PlayChime();
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
                    if (CurrentRoom.Locks[Direction].Type.ToLower().Contains("key"))
                    {
                        Console.WriteLine($"The door is locked... The lock appears to fit a {CurrentRoom.Locks[Direction].Type}");
                    }
                    else if (CurrentRoom.Locks[Direction].Type.ToLower().Contains("passphrase"))
                    {
                        Console.WriteLine($"You cannot pass this way... An inscription states that you must utter a {CurrentRoom.Locks[Direction].Type}");
                    }
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
            Room Cell = new Room("Cell",
                                "You are in what appears to be a prison cell that was built into a dead end tunnel of a cave.\nThe cell door is slightly ajar, the lock long since rusted and broken.\nThe only direction open to you is a tunnel to the east.",
                                 "A darkened room... A feeling of claustrophobia grips you... An image of iron bars...");
            Room Bridge = new Room("Bridge",
                                    "You see A rickety wooden bridge running east to west with doors at each end.\nFrom below you hear only silence.\nYou are certain a fall from here would be fatal.",
                                    "A narrow path... A feeling of vertigo... Watch your step...");
            Room Crossroads = new Room("Crossroads",
                                        "Your path opens to a large cavern.\nTorches line the cavern walls gifting you with much needed illumination.\nYou can see paths worn into the floor leading north, south, east, and west.\nTo the east the sound of falling stones and creaking wood...\nTo the south, a warm light welcomes you...",
                                        "The crossing of roads... A feeling of indecision... Choose your path...");
            Room GobletRoom = new Room("Room of Peering",
                                        "You are in a room nearly empty, save for the crystal dais in the center of the room...\nAn unnatural light fills the room, its source unknown to you. The only exit returning you north whence you came...",
                                        "Much needed aid... A sense of hope and light... Farsight...");
            Room CrossroadsNorth = new Room("Crossroads North",
                                            "A measure of light can be seen from a tunnel to the south.\nYou can also see exits leading north and east...",
                                            "More tunnels... Empty walls... Alone...");

            Room EastBendTunnel = new Room("East Bend Tunnel",
                                            "You are now in a narrow tunnel.\nIt bends at nearly a right angle with exits to the west and south...",
                                            "A tunnel... A bend... Alone...");
            Room JailersKeyRoom = new Room("Jailer's Key Room",
                                        "In contrast to the rest of the cave, this room looks to have once known some comfort.\nYou see the remnants of a desk and chair.\nIt would appear this room was once occupied by the jailer.\nThe exit to the north is open. The exit to the west looks unstable.",
                                        "The confiners lair... Long abandoned... Something needed...");
            Room KonamiClueRoom = new Room("Konami Clue Room",
                                            "You have entered a brightly-lit room with an exit to the northwest.\nTo the east, you see an ornate stone slab affixed into the wall.\nA large engraving on the northern wall states: \"To proceed east towards freedom, you must SPEAK the ancient words of \"No-clipping\".\nTo obtain these words, proceed northwest. \"",
                                            "A maze... An enigma... Hearken to the chimes...");
            #region Konami Maze Rooms
            // Collection of nearly identical rooms comprising a maze whos exits conform to the "Konami code". 
            // The final room has a pass key needed to proceed and any wrong turn returns you to the start of the maze.
            Room Maze1 = new Room("Maze1",
                                "As you enter, a voice echoes in your mind:\n\"There is but one path to your goal. You must move in step with the code of lives to succeed!\nAny misstep shall rob you of your progress.\nFailure shall ensure your eternal entombment within this place... The only way out is through...\"\nThe room is empty and the walls undecorated.\nThere are but four exits labelled: \"Up\", \"Down\", \"Left\", and \"Right\"",
                                "The Chimes will guide you...");
            Room Maze2 = new Room("Maze2",
                                "Move in step to the sacred code of lives!\n(The room is empty and the walls undecorated. There are but four exits labelled: \"Up\", \"Down\", \"Left\", and \"Right\")",
                                "A maze... An enigma... Hearken to the chimes...");
            Room Maze3 = new Room("Maze3",
                                "Move in step to the sacred code of lives!\n(The room is empty and the walls undecorated. There are but four exits labelled: \"Up\", \"Down\", \"Left\", and \"Right\")",
                                "A maze... An enigma... Hearken to the chimes...");
            Room Maze4 = new Room("Maze4",
                                "Move in step to the sacred code of lives!\n(The room is empty and the walls undecorated. There are but four exits labelled: \"Up\", \"Down\", \"Left\", and \"Right\")",
                                "A maze... An enigma... Hearken to the chimes...");
            Room Maze5 = new Room("Maze5",
                                "Move in step to the sacred code of lives!\n(The room is empty and the walls undecorated. There are but four exits labelled: \"Up\", \"Down\", \"Left\", and \"Right\")",
                                "A maze... An enigma... Hearken to the chimes...");
            Room Maze6 = new Room("Maze6",
                                "Move in step to the sacred code of lives!\n(The room is empty and the walls undecorated. There are but four exits labelled: \"Up\", \"Down\", \"Left\", and \"Right\")",
                                "A maze... An enigma... Hearken to the chimes...");
            Room Maze7 = new Room("Maze7",
                                "Move in step to the sacred code of lives!\n(The room is empty and the walls undecorated. There are but four exits labelled: \"Up\", \"Down\", \"Left\", and \"Right\")",
                                "A maze... An enigma... Hearken to the chimes...");
            Room Maze8 = new Room("Maze8",
                                "Move in step to the sacred code of lives!\n(The room is empty and the walls undecorated. There are but four exits labelled: \"Up\", \"Down\", \"Left\", and \"Right\")",
                                "A maze... An enigma... Hearken to the chimes...");
            Room Maze9 = new Room("Maze9",
                                "Move in step to the sacred code of lives!\n(The room is empty and the walls undecorated. There are but two exits labelled: \"B\", and \"A\")",
                                "A maze... An enigma... Hearken to the chimes...");
            Room Maze10 = new Room("Maze10",
                                "Move in step to the sacred code of lives!\n(The room is empty and the walls undecorated. There are but two exits labelled: \"B\", and \"A\")",
                                "A maze... An enigma... Hearken to the chimes...");
            Room Maze11 = new Room("Maze11",
                                "Move in step to the sacred code of...Konami!\n(The room is empty and the walls undecorated. There is but a single exit labelled: \"Start\")",
                                "A maze... An enigma... Hearken to the chimes...");
            Room Maze12 = new Room("Maze12",
                                "You have entered a room with no exits, save for the southern entrance you came through.\nCarved into the wall is a seemingly random series of letters:\n\"idspispopd\"... Could this be the sacred word you were searching for?\nLooking south through the doorway you came in, you see that it leads to the start of the maze...",
                                "A maze... An enigma... Hearken to the chimes...");
            #endregion


            Item BronzeKey = new Item("Bronze Key", "An old, Bronze Key", false, "You attempted to use the Bronze Key", "On a set of key hooks", "Crossroads North", true, 5, false, "none");
            Item SilverGoblet = new Item("Silver Goblet", "A shining silver goblet, filled with a viscous yellow fluid.", true, "You drank from the Silver Goblet", "On a Crystal dais in the center of the room.", "any", false, 1, false, "peer");
            Item IronSword = new Item("Iron Sword", "An Iron Sword", true, "You swung the Iron sword with all your might", "Hanging in a decorative frame on the wall", "any", false, 5000, true, "attack");

            Lock BronzeLock = new Lock("Bronze Lock", "Bronze Key", true);

            Lock KonamiLock = new Lock("Konami Lock", "Magic Passphrase", true);

            Hazard Fall = new Hazard("Pit fall", "Pit fall", "Your misstep proves fatal as you fall to your death...", false);
            Hazard BoulderCrush = new Hazard("Boulder Crush", "Boulder Crush", "The walls give way as the timbers lining the cavern fail...\nYou are crushed below the earth...", false);

            Chime Success = new Chime("Success", "Success", 2500);
            Chime Failure = new Chime("Failure", "Failure", 2500);

            Cell.Exits.Add("east", Bridge);

            Bridge.Exits.Add("west", Cell);
            Bridge.Exits.Add("east", Crossroads);

            Bridge.Hazards.Add("north", Fall);
            Bridge.Hazards.Add("south", Fall);

            Crossroads.Exits.Add("north", CrossroadsNorth);
            Crossroads.Exits.Add("south", GobletRoom);
            Crossroads.Exits.Add("west", Bridge);

            Crossroads.Hazards.Add("east", BoulderCrush);

            GobletRoom.Exits.Add("north", Crossroads);

            GobletRoom.Items.Add(SilverGoblet);

            CrossroadsNorth.Exits.Add("south", Crossroads);
            CrossroadsNorth.Exits.Add("north", KonamiClueRoom);
            CrossroadsNorth.Exits.Add("east", EastBendTunnel);

            CrossroadsNorth.Locks.Add("north", BronzeLock);

            EastBendTunnel.Exits.Add("west", CrossroadsNorth);
            EastBendTunnel.Exits.Add("south", JailersKeyRoom);

            JailersKeyRoom.Exits.Add("north", EastBendTunnel);

            JailersKeyRoom.Hazards.Add("west", BoulderCrush);

            JailersKeyRoom.Items.Add(BronzeKey);

            KonamiClueRoom.Exits.Add("northwest", Maze1);
            KonamiClueRoom.Exits.Add("east", Crossroads);

            KonamiClueRoom.Locks.Add("east", KonamiLock);

            #region Konami Maze Data
            // Collection of nearly identical rooms comprising a maze whos exits conform to the "Konami code". 
            // The final room has a pass key needed to proceed and any wrong turn returns you to the start of the maze.

            Maze1.Exits.Add("up", Maze2);
            Maze1.Exits.Add("down", KonamiClueRoom);
            Maze1.Exits.Add("left", KonamiClueRoom);
            Maze1.Exits.Add("right", KonamiClueRoom);

            Maze1.Chimes.Add("up", Success);
            Maze1.Chimes.Add("down", Failure);
            Maze1.Chimes.Add("left", Failure);
            Maze1.Chimes.Add("right", Failure);

            Maze2.Exits.Add("up", Maze3);
            Maze2.Exits.Add("down", KonamiClueRoom);
            Maze2.Exits.Add("left", KonamiClueRoom);
            Maze2.Exits.Add("right", KonamiClueRoom);

            Maze2.Chimes.Add("up", Success);
            Maze2.Chimes.Add("down", Failure);
            Maze2.Chimes.Add("left", Failure);
            Maze2.Chimes.Add("right", Failure);

            Maze3.Exits.Add("up", KonamiClueRoom);
            Maze3.Exits.Add("down", Maze4);
            Maze3.Exits.Add("left", KonamiClueRoom);
            Maze3.Exits.Add("right", KonamiClueRoom);

            Maze3.Chimes.Add("up", Failure);
            Maze3.Chimes.Add("down", Success);
            Maze3.Chimes.Add("left", Failure);
            Maze3.Chimes.Add("right", Failure);

            Maze4.Exits.Add("up", KonamiClueRoom);
            Maze4.Exits.Add("down", Maze5);
            Maze4.Exits.Add("left", KonamiClueRoom);
            Maze4.Exits.Add("right", KonamiClueRoom);

            Maze4.Chimes.Add("up", Failure);
            Maze4.Chimes.Add("down", Success);
            Maze4.Chimes.Add("left", Failure);
            Maze4.Chimes.Add("right", Failure);

            Maze5.Exits.Add("up", KonamiClueRoom);
            Maze5.Exits.Add("down", KonamiClueRoom);
            Maze5.Exits.Add("left", Maze6);
            Maze5.Exits.Add("right", KonamiClueRoom);

            Maze5.Chimes.Add("up", Failure);
            Maze5.Chimes.Add("down", Failure);
            Maze5.Chimes.Add("left", Success);
            Maze5.Chimes.Add("right", Failure);

            Maze6.Exits.Add("up", KonamiClueRoom);
            Maze6.Exits.Add("down", KonamiClueRoom);
            Maze6.Exits.Add("left", KonamiClueRoom);
            Maze6.Exits.Add("right", Maze7);

            Maze6.Chimes.Add("up", Failure);
            Maze6.Chimes.Add("down", Failure);
            Maze6.Chimes.Add("left", Failure);
            Maze6.Chimes.Add("right", Success);

            Maze7.Exits.Add("up", KonamiClueRoom);
            Maze7.Exits.Add("down", KonamiClueRoom);
            Maze7.Exits.Add("left", Maze8);
            Maze7.Exits.Add("right", KonamiClueRoom);

            Maze7.Chimes.Add("up", Failure);
            Maze7.Chimes.Add("down", Failure);
            Maze7.Chimes.Add("left", Success);
            Maze7.Chimes.Add("right", Failure);

            Maze8.Exits.Add("up", KonamiClueRoom);
            Maze8.Exits.Add("down", KonamiClueRoom);
            Maze8.Exits.Add("left", KonamiClueRoom);
            Maze8.Exits.Add("right", Maze9);

            Maze8.Chimes.Add("up", Failure);
            Maze8.Chimes.Add("down", Failure);
            Maze8.Chimes.Add("left", Failure);
            Maze8.Chimes.Add("right", Success);

            Maze9.Exits.Add("b", Maze10);
            Maze9.Exits.Add("a", KonamiClueRoom);

            Maze9.Chimes.Add("b", Success);
            Maze9.Chimes.Add("a", Failure);

            Maze10.Exits.Add("b", KonamiClueRoom);
            Maze10.Exits.Add("a", Maze11);

            Maze10.Chimes.Add("b", Failure);
            Maze10.Chimes.Add("a", Success);

            Maze11.Exits.Add("start", Maze12);

            Maze11.Chimes.Add("start", Success);

            Maze12.Exits.Add("south", KonamiClueRoom);

            Maze12.Chimes.Add("south", Success);
            #endregion

            // Crossroads.Items.Add(IronSword); <- Add later? Attack function?

            Rooms.Add("Cell", Cell);
            Rooms.Add("Bridge", Bridge);
            Rooms.Add("Crossroads", Crossroads);
            Rooms.Add("Room of Peering", GobletRoom);
            Rooms.Add("Crossroads North", CrossroadsNorth);
            Rooms.Add("East Bend Tunnel", EastBendTunnel);
            Rooms.Add("Jailer's Key Room", JailersKeyRoom);
            Rooms.Add("Konami Clue Room", KonamiClueRoom);

        }

        private void GeneratePlayer()
        {
            string PlayerName;
            Console.Write("Please enter a name for our hero:");
            PlayerName = Console.ReadLine();
            CurrentPlayer = new Player(PlayerName);
            CurrentRoom = Rooms["Cell"];
        }

    }
}