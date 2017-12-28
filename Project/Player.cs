using System;
using System.Collections.Generic;

namespace CastleGrimtol.Project
{
    public class Player : IPlayer
    {
        public string Name { get; set; }
        public int Score { get; set; }

        public List<Item> Inventory { get; set; }

        public Dictionary<string, bool> Powers { get; set; } = new Dictionary<string, bool>();

        public void UseItem(Item item)
        {
            if (!Powers.ContainsKey(item.Power))
            {
                Powers.Add(item.Power, true);
                Console.WriteLine($"You can now use {item.Power}");
            }
            // foreach (var CurrentPower in Powers)
            // {
            //     System.Console.WriteLine(item.Power);
            //     System.Console.WriteLine(CurrentPower.Key);
            //     System.Console.WriteLine(CurrentPower.Value);
            //     if (CurrentPower.Key == item.Power)
            //     {
            //         Powers[item.Power] = true;
            //     }
            // }
        }
        public Player(string name)
        {
            Name = name;
            Score = 0;
            Inventory = new List<Item>();
        }
    }
}