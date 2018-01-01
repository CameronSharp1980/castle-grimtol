using System;
using System.Collections.Generic;

namespace CastleGrimtol.Project
{
    public class Player : IPlayer
    {
        public string Name { get; set; }
        public int Score { get; set; }

        public bool Dead { get; set; }

        public string Status { get; set; }

        public int StatusCount { get; set; }

        public Item EquippedWeapon { get; set; }

        public List<Item> Inventory { get; set; }

        public Dictionary<string, bool> Powers { get; set; } = new Dictionary<string, bool>();

        public void UseItem(Item item)
        {
            if (item.Weapon)
            {
                this.EquippedWeapon = item;
                System.Console.WriteLine("Currently equipped: " + this.EquippedWeapon.Name);
            }
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
            if (this.Status == "sneaking")
            {
                this.StatusCount = 0;
                this.CheckStatus();
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
            System.Console.WriteLine("Your voice echoes off the walls, but nothing happens...");
            return false;
        }

        public void EquipWeapon(Item weapon)
        {
            this.EquippedWeapon = weapon;
        }

        public void Sneak()
        {
            Console.Clear();
            if (this.Powers.ContainsKey("sneak"))
            {
                if (this.Status != "sneaking")
                {
                    this.Status = "sneaking";
                    this.StatusCount = 3;
                    Console.WriteLine("You are now sneaking.");
                }
                else
                {
                    this.Status = this.Status;
                }
            }
            else
            {
                Console.WriteLine("You lack that ability...");
            }
        }

        public void CheckStatus()
        {
            this.StatusCount--;
            if (this.StatusCount <= 0)
            {
                Console.WriteLine($"You are no longer {this.Status}");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                this.Status = "walking";
                this.StatusCount = 5000000;
            }
        }

        public Player(string name)
        {
            Name = name;
            Score = 0;
            Dead = false;
            Status = "walking";
            this.StatusCount = 5000000;
            EquippedWeapon = new Item("Fists", "Fists", true, false, "You swing your fists and punch air.", "At the end of your arms", "any", false, 100000, true, "Punch");
            Inventory = new List<Item>();
        }
    }
}