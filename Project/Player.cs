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
                Console.WriteLine($"You can now use the {item.Power} ability!");
                item.Uses--;
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

        public bool Speak(string passphrase, Room currentRoom)
        {
            Console.Clear();
            if (passphrase == "empty")
            {
                Console.WriteLine("You must enter a phrase with the \"say/speak\" command.");
                return false;
            }
            foreach (var lockToCheck in currentRoom.Locks)
            {
                if (lockToCheck.Value.Type == passphrase)
                {
                    Console.Clear();
                    lockToCheck.Value.Locked = false;
                    Console.WriteLine("The wall before you becomes semi-transparent and shimmers like the surface of water...\nYou have the sense that you can pass through it...");
                    return false;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("A voice thunders in your mind: \"FOOL! To think that you would blaspheme your utterance of the sacred phrase!\"");
                    Console.WriteLine("A bolt arcs from the stone slab striking your chest.\nYour now smoking corpse falls to the floor.");
                    return true;
                }
            }
            return false;
        }

        public Player(string name)
        {
            Name = name;
            Score = 0;
            Inventory = new List<Item>();
        }
    }
}