using System;
using System.Collections.Generic;

namespace CastleGrimtol.Project
{
    public class Room : IRoom
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Dictionary<string, Room> Exits { get; set; }

        public Dictionary<string, Lock> Locks { get; set; }

        public List<Item> Items { get; set; }

        public void UseItem(Item item)
        {
            if (item.Name.ToLower().Contains("key"))
            {
                // System.Console.WriteLine("key test");
                foreach (var lockToCheck in Locks)
                {
                    if (lockToCheck.Value.Type == item.Name)
                    {
                        lockToCheck.Value.Locked = false;
                        Console.WriteLine($"You unlocked the {lockToCheck.Value.Name}");
                    }
                }


            }
        }

        public Room(string name, string description)
        {
            Name = name;
            Description = description;
            Exits = new Dictionary<string, Room>();
            Locks = new Dictionary<string, Lock>();
            Items = new List<Item>();
        }
    }
}