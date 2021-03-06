using System;
using System.Collections.Generic;

namespace CastleGrimtol.Project
{
    public class Room : IRoom
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string PeerDetails { get; set; }

        public Dictionary<string, Room> Exits { get; set; }

        public Dictionary<string, Lock> Locks { get; set; }

        public Dictionary<string, Chime> Chimes { get; set; }

        public Dictionary<string, Hazard> Hazards { get; set; }

        public List<Enemy> Enemies { get; set; }

        public List<Item> Items { get; set; }

        public void UseItem(Item item)
        {
            if (item.Name.ToLower().Contains("key"))
            {
                foreach (var lockToCheck in Locks)
                {
                    if (lockToCheck.Value.Type == item.Name)
                    {
                        lockToCheck.Value.Locked = false;
                        item.RequiredToProceed = false;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine($"You unlocked the {lockToCheck.Value.Name} with the {item.Name}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                }
            }

            item.Uses--;
            if (item.Uses <= 20)
            {
                item.Sturdy = false;
            }

            if (item.Uses <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine($"The {item.Name} broke apart in your hands after you used it.");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else if (!item.Sturdy)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.WriteLine($"The {item.Name} appears old is noticably more worn after you used it...You should be careful lest it should break...");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
            }

        }

        public Room(string name, string description, string peerDetails)
        {
            Name = name;
            Description = description;
            PeerDetails = peerDetails;
            Exits = new Dictionary<string, Room>();
            Locks = new Dictionary<string, Lock>();
            Chimes = new Dictionary<string, Chime>();
            Hazards = new Dictionary<string, Hazard>();
            Enemies = new List<Enemy>();
            Items = new List<Item>();
        }
    }
}