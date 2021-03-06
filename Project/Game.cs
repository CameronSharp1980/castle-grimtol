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

            // Default console colors in case different than Windows default
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;

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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("Do you wish to restart the game?");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Enter \'Y\' for yes or \'N\' for no.");
            Console.ForegroundColor = ConsoleColor.Gray;
            ReallyRestart = Console.ReadLine();
            if (ReallyRestart.ToLower() == "y" || ReallyRestart.ToLower() == "yes")
            {
                Quit = false;
                CurrentPlayer.Dead = false;
                Setup();
                Start();
            }
            else if (ReallyRestart.ToLower() == "n" || ReallyRestart.ToLower() == "no")
            {
                if (CurrentPlayer.Dead)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Better luck next time...");
                    Console.ForegroundColor = ConsoleColor.Gray;
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
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Please enter \'Y\' or'N\'");
                Console.ForegroundColor = ConsoleColor.Gray;
                Reset();
            }
        }

        public void Start()
        {
            string[] Command;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"I would bid you welcome, good {CurrentPlayer.Name}, were it not for the misfortune in which you have found yourself.\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Your journey begins in a haze. You awaken to only darkness.");
            Console.WriteLine("You've no idea how you came to be here, or even where \"here\" is.");
            Console.WriteLine("What you do know however, is that you are in danger, and that you must escape...");
            Console.WriteLine("As your vision clears, you realize that you are in a prison cell built into a cave wall.");
            Console.WriteLine("The door is ajar and you appear to be alone.");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine();
            Console.WriteLine("Press enter to begin your quest...");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ReadLine();

            Look();
            while (!Quit && !CurrentPlayer.Dead)
            {
                Command = PromptUser();
                ParseCommand(Command);
                if (CurrentRoom.Name == "Exit Room")
                {
                    Quit = true;
                    GameGoodEnd();
                }
                if (CurrentRoom.Enemies.Count > 0)
                {
                    CurrentRoom.Enemies[0].DetectPlayerCheck(CurrentPlayer);
                    if (CurrentPlayer.Dead)
                    {
                        Reset();
                    }
                }

            }
        }

        private void DisplayTitleScreen()
        {
            Console.Clear();
            Console.Write("Welcome to \"");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("The Pits of Despair");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\"\n");
            Console.WriteLine("A text adventure.");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Press Enter to start game");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ReadLine();
        }

        public void UseItem(string itemName)
        {
            if (itemName == "empty")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("You must enter an item name with the \"use\" command.");
                Console.ForegroundColor = ConsoleColor.Gray;
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
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine($"After weeks of searching, you collapse in a heap as your strength fades...\nYou realize that the {CurrentPlayer.Inventory[i].Name} you broke weeks ago was essential to your escape... \nThe darkness takes you.");
                                            Console.ForegroundColor = ConsoleColor.Gray;
                                            CurrentPlayer.Dead = true;
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
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine($"You do not have any {itemName} in your inventory.");
            Console.ForegroundColor = ConsoleColor.Gray;

        }

        private void Look()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(CurrentRoom.Description);
            Console.ForegroundColor = ConsoleColor.Gray;
            for (int i = 0; i < CurrentRoom.Enemies.Count; i++)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(CurrentRoom.Enemies[i].InRoomDescription);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            for (int i = 0; i < CurrentRoom.Items.Count; i++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("You see " + CurrentRoom.Items[i].Description + " " + CurrentRoom.Items[i].ItemLocation);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void Peer(string Direction)
        {
            Console.Clear();
            if (Direction == "empty")
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("You must enter a direction with the \"peer\" command.");
                Console.ForegroundColor = ConsoleColor.Gray;
                return;
            }
            else if (CurrentPlayer.Powers.ContainsKey("peer"))
            {
                if (CurrentRoom.Exits.ContainsKey(Direction))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(CurrentRoom.Exits[Direction].PeerDetails);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if (CurrentRoom.Hazards.ContainsKey(Direction))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Mists obscure your farsight... You must use your wits and senses...");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("There is nothing to see in that direction...");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("You lack that ability...");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        private void Help()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("The following commands are available:\n");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Simple commands:\n");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("help");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - Displays this help menu.\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("restart");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - Allows the player to restart the game.\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("yield");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - Allows the player to give up and restart the game.\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("quit");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - Allows the player to quit the game.\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("look");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - Displays description of current room.\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("inventory");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - Displays the player's current inventory\n");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Compound commands:\n");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("go <Exit or Direction name>");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - The player will attempt to move in the specified direction.\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("take <Item Name>");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - The player will attempt to take an item from the current room.\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("use <Item name>");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" -  Uses the specified item\n");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("say/speak <Words to speak>");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - The player will speak the specified phrase aloud.\n");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Commands gained from items:\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("peer <Direction or exit name>");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - Allows the player to magically \"peer\" in the direction specified, sometimes providing a valuable clue.\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("sneak");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - After use, the player will move silently for a limited number of actions.\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("attack <Target>");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" - The player will attempt to attack the specified target with the currenly equipped weapon. Must have a weapon to use.\n");
        }

        private void EndGame()
        {
            string ReallyQuit;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("Are you sure you want to quit?");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Enter \'Y\' for yes or \'N\' for no.");
            Console.ForegroundColor = ConsoleColor.Gray;
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
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Please enter \'Y\' or'N\'");
                Console.ForegroundColor = ConsoleColor.Gray;
                EndGame();
            }
        }

        private void GiveUp()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("As dispair overwhelms you, you consider ending the torment yourself...");
            Console.ForegroundColor = ConsoleColor.Gray;
            EndGame();
        }

        private string[] PromptUser()
        {
            string CommandStringInput;
            string[] CommandArr;
            string Command;
            string CommandArg;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("(Type \"help\" at any time for a list of commands)");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("What would you like to to?: ");
            Console.ForegroundColor = ConsoleColor.Gray;
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
            else if (CommandStringInput.ToLower().Contains("go") || CommandStringInput.ToLower().Contains("take") || CommandStringInput.ToLower().Contains("use") || CommandStringInput.ToLower().Contains("look") || CommandStringInput.ToLower().Contains("help") || CommandStringInput.ToLower().Contains("quit") || CommandStringInput.ToLower().Contains("restart") || CommandStringInput.ToLower().Contains("inventory") || CommandStringInput.ToLower().Contains("yield") || CommandStringInput.ToLower().Contains("say") || CommandStringInput.ToLower().Contains("speak") || CommandStringInput.ToLower().Contains("sneak") || CommandStringInput.ToLower().Contains("attack"))
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
            if (!CurrentPlayer.Dead)
            {
                CurrentPlayer.CheckStatus();
            }

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
                case "attack":
                    CurrentPlayer.Attack(CommandArr[1].ToLower(), CurrentRoom);
                    break;
                case "inventory":
                    ViewInventory();
                    break;
                case "use":
                    UseItem(CommandArr[1].ToLower());
                    break;
                case "say":
                case "speak":
                    CurrentPlayer.Dead = CurrentPlayer.Speak(CommandArr[1].ToLower(), CurrentRoom);
                    if (CurrentPlayer.Dead)
                    {
                        Reset();
                    }
                    break;
                case "look":
                    Look();
                    break;
                case "peer":
                    Peer(CommandArr[1].ToLower());
                    break;
                case "sneak":
                    CurrentPlayer.Sneak();
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
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("You must enter a direction with the \"go\" command.");
                Console.ForegroundColor = ConsoleColor.Gray;
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
                    CurrentPlayer.Dead = true;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(CurrentRoom.Hazards[Direction].DeathMessage);
                    Console.ForegroundColor = ConsoleColor.Gray;
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
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine($"The door is locked... The lock appears to fit a {CurrentRoom.Locks[Direction].Type}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else if (CurrentRoom.Locks[Direction].Name.ToLower().Contains("passphrase"))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine($"You cannot pass this way... An inscription states that you must utter a {CurrentRoom.Locks[Direction].Name}");
                        Console.ForegroundColor = ConsoleColor.Gray;
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
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("You cannot go that way.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        private void Take(string Item)
        {
            bool ItemTaken = false;

            if (Item == "empty")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("You must enter an item name with the \"take\" command.");
                Console.ForegroundColor = ConsoleColor.Gray;
                return;
            }
            if (CurrentPlayer.Status == "sneaking")
            {
                CurrentPlayer.StatusCount = 0;
                CurrentPlayer.CheckStatus();
            }
            for (int i = 0; i < CurrentRoom.Items.Count; i++)
            {
                if (CurrentRoom.Items[i].Name.ToLower() == Item.ToLower())
                {
                    CurrentPlayer.Inventory.Add(CurrentRoom.Items[i]);
                    CurrentRoom.Items.RemoveAt(i);
                    ItemTaken = true;
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"You took the {Item}.");
                    Console.ForegroundColor = ConsoleColor.Gray;
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
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.WriteLine($"There is no {Item} in the room, but you have one in your inventory.");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            return;
                        }
                    }
                }
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"There are no {Item}s in the room for you to take.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }

        }

        private void ViewInventory()
        {
            Console.Clear();
            if (CurrentPlayer.Inventory.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("You rummage through your pack and find the following items:");
                for (int i = 0; i < CurrentPlayer.Inventory.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{i + 1}. {CurrentPlayer.Inventory[i].Name} : {CurrentPlayer.Inventory[i].Description}");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Your pack is empty...");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void GameGoodEnd()
        {
            Console.Clear();
            System.Console.WriteLine(Rooms["Exit Room"].Description);
            Console.WriteLine($"Congratulations {CurrentPlayer.Name}! You have successfully escaped the pits of despair!");
            Console.WriteLine("We hope you enjoyed your quest and will play again!");
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
                                            "You have entered a brightly-lit room with an exit to the northwest.\nTo the east, you see an ornate stone slab affixed into the wall.\nA large engraving on the northern wall states:\n\"To proceed east towards freedom, you must SPEAK the ancient words of \"No-clipping\".\nTo obtain these words, proceed northwest. \"",
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

            Room Hallway = new Room("Hallway",
                                    "You are in a well-lit hall. The stone slab you passed through, now solid once more sits to the west.\nOrnate tapestries line the walls.\nAt first glance the room appears well-kept, but as you peer eastward, you see that the rooms state gradually decays as you vision nears the eastern exit...\nYou hear something... Something large beyond those doors.\nA sense of dread grips you... For that is the only direction you can go...",
                                    "Journey's end... Freedom near... Hush...");

            Room GrueChamber = new Room("Grue Chamber",
                                       "The stench is overwhemling! Something has died in this room... Perhaps many somethings...\nA portcullis has dropped before the western exit... There is no turning back!\nTo the north is an exit... If you can make it that far. Something has been living here!",
                                       "Danger... Hear your steps... Blind...");

            Room ExitRoom = new Room("Exit Room",
                                     "The way opens before you! The cavern walls give way to a sunlit forest and your freedom!",
                                     "Freedom... Take it...");

            Item BronzeKey = new Item("Bronze Key", "An old, Bronze Key", false, false, "You attempted to use the Bronze Key", "On a set of key hooks", "Crossroads North", true, 5, false, "none");
            Item GoldenKey = new Item("Golden Key", "An ornate Golden Key, decorated with Gemstones.", false, false, "You attempted to use the Golden Key", "Hanging near the Exit", "Grue Chamber", true, 5, false, "none");
            Item SilverGoblet = new Item("Silver Goblet", "A shining silver goblet, filled with a viscous yellow fluid.", true, false, "You drank from the Silver Goblet", "On a Crystal dais in the center of the room.", "any", false, 1, false, "peer");
            Item SlickShoes = new Item("Slick Shoes", "A well worn pair of slick shoes with soft pads.", true, false, "You equipped the Slick Shoes", "worn by a dead adventurer next to the east exit...", "any", false, 1, false, "sneak");
            Item IronSword = new Item("Iron Sword", "An Iron Sword", true, true, "You equipped the Iron Sword", "Hanging in a decorative frame on the eastern wall", "any", true, 5000, true, "attack");

            Lock BronzeLock = new Lock("Bronze Lock", "Bronze Key", true);
            Lock GoldenLock = new Lock("Golden Lock", "Golden Key", true);

            Lock KonamiLock = new Lock("Magic Passphrase", "idspispopd", true);

            Hazard Fall = new Hazard("Pit fall", "Pit fall", "Your misstep proves fatal as you fall to your death...", false);
            Hazard BoulderCrush = new Hazard("Boulder Crush", "Boulder Crush", "The walls give way as the timbers lining the cavern fail...\nYou are crushed below the earth...", false);

            Enemy Grue = new Enemy("Grue", "A grue hulks before you!", "sneaking", "The Grue has heard you! Although blind, he zeroes in on your position quickly.\nHis giant fists close around your head and pop your skull like an egg shell...", IronSword.Name);

            Chime Success = new Chime("Success", "Success", 2500);
            Chime Failure = new Chime("Failure", "Failure", 2500);

            Cell.Exits.Add("east", Bridge);

            Bridge.Exits.Add("west", Cell);
            Bridge.Exits.Add("east", Crossroads);

            Bridge.Items.Add(SlickShoes);

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
            KonamiClueRoom.Exits.Add("east", Hallway);

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

            Hallway.Exits.Add("east", GrueChamber);

            Hallway.Items.Add(IronSword);

            GrueChamber.Exits.Add("north", ExitRoom);

            GrueChamber.Locks.Add("north", GoldenLock);

            GrueChamber.Items.Add(GoldenKey);

            GrueChamber.Enemies.Add(Grue);

            Rooms.Add("Cell", Cell);
            Rooms.Add("Bridge", Bridge);
            Rooms.Add("Crossroads", Crossroads);
            Rooms.Add("Room of Peering", GobletRoom);
            Rooms.Add("Crossroads North", CrossroadsNorth);
            Rooms.Add("East Bend Tunnel", EastBendTunnel);
            Rooms.Add("Jailer's Key Room", JailersKeyRoom);
            Rooms.Add("Konami Clue Room", KonamiClueRoom);

            // Add maze rooms to Rooms dictionary? Currently only needed to lock checks and there are no locks there...

            Rooms.Add("Hallway", Hallway);
            Rooms.Add("Grue Chamber", GrueChamber);
            Rooms.Add("Exit Room", ExitRoom);

        }

        private void GeneratePlayer()
        {
            string PlayerName;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Please enter a name for our hero: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            PlayerName = Console.ReadLine();
            CurrentPlayer = new Player(PlayerName);
            CurrentRoom = Rooms["Cell"];
        }

    }
}