

using System;

namespace CastleGrimtol.Project
{

    public class Enemy
    {
        public string Name { get; set; }

        public string InRoomDescription { get; set; }

        public string DetectionCheck { get; set; }

        public string KillMessage { get; set; }

        public string Weakness { get; set; }

        public bool EnemyDead { get; set; }

        private void AttackPlayer(Player currentPlayer)
        {
            currentPlayer.Dead = true;
            Console.WriteLine(this.KillMessage);
        }

        public void DetectPlayerCheck(Player currentPlayer)
        {
            if (currentPlayer.Status != DetectionCheck && !this.EnemyDead)
            {
                AttackPlayer(currentPlayer);
            }
        }

        public void Death(Room currentRoom)
        {
            this.EnemyDead = true;
            Console.WriteLine($"You have slain the {this.Name}!\nIt's corpse tumbles to the floor and evaporates... A summoned monster?!");
            for (int i = 0; i < currentRoom.Enemies.Count; i++)
            {
                Enemy currentEnemy = currentRoom.Enemies[i];
                currentRoom.Enemies.Remove(currentEnemy);
            }
        }

        public Enemy(string name, string inRoomDescription, string detectionCheck, string killMessage, string weakness)
        {
            Name = name;
            InRoomDescription = inRoomDescription;
            DetectionCheck = detectionCheck;
            KillMessage = killMessage;
            Weakness = weakness;
            EnemyDead = false;
        }


    }
}