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
                this.EquipWeapon(item);
            }
            if (!Powers.ContainsKey(item.Power))
            {
                Powers.Add(item.Power, true);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"You can now use the {item.Power} ability!");
                Console.ForegroundColor = ConsoleColor.Gray;
                item.Uses--;
            }
        }

        public bool Speak(string passphrase, Room currentRoom)
        {
            Console.Clear();
            if (passphrase == "empty")
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("You must enter a phrase with the \"say/speak\" command.");
                Console.ForegroundColor = ConsoleColor.Gray;
                return false;
            }
            if (this.Status == "sneaking")
            {
                this.StatusCount = 0;
                this.CheckStatus();
            }
            foreach (var lockToCheck in currentRoom.Locks)
            {
                if (!lockToCheck.Value.Type.ToLower().Contains("key"))
                {
                    if (lockToCheck.Value.Type == passphrase)
                    {
                        Console.Clear();
                        lockToCheck.Value.Locked = false;
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine("The wall before you becomes semi-transparent and shimmers like the surface of water...\nYou have the sense that you can pass through it...");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        return false;
                    }
                    else
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("A voice thunders in your mind: \"FOOL! To think that you would blaspheme your utterance of the sacred phrase!\"");
                        Console.WriteLine("A bolt arcs from the stone slab striking your chest.\nYour now smoking corpse falls to the floor.");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        return true;
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            System.Console.WriteLine("Your voice echoes off the walls, but nothing happens...");
            Console.ForegroundColor = ConsoleColor.Gray;
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
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("You are now sneaking.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    this.Status = this.Status;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("You lack that ability...");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public void Attack(string enemyName, Room currentRoom)
        {
            Console.Clear();
            if (!this.Powers.ContainsKey("attack"))
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("You lack that ability...");
                Console.ForegroundColor = ConsoleColor.Gray;
                return;
            }
            if (enemyName == "empty")
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("You must enter an enemy name with the \"attack\" command.");
                Console.ForegroundColor = ConsoleColor.Gray;
                return;
            }

            if (this.Status == "sneaking")
            {
                this.StatusCount = 0;
                this.CheckStatus();
            }

            if (currentRoom.Enemies.Count > 0)
            {
                for (int i = 0; i < currentRoom.Enemies.Count; i++)
                {
                    Enemy currentEnemy = currentRoom.Enemies[i];
                    if (currentEnemy.Name.ToLower() == enemyName.ToLower() && currentEnemy.Weakness == this.EquippedWeapon.Name)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"You attack the {currentEnemy.Name} with your {this.EquippedWeapon.Name}!");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        currentEnemy.Death(currentRoom);
                    }
                }
                if (currentRoom.Enemies.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine($"Your attack fails as there is no such enemy nearby... Perhaps you are seeing things in the darkness.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"Your attack fails as there is no such enemy nearby... Perhaps you are seeing things in the darkness.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }

        }

        public void CheckStatus()
        {
            this.StatusCount--;
            if (this.StatusCount <= 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"You are no longer {this.Status}");
                Console.ForegroundColor = ConsoleColor.Gray;
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