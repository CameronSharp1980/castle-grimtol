

using System;

namespace CastleGrimtol.Project
{

    public class Enemy
    {
        public string Name { get; set; }

        public string DetectionCheck { get; set; }

        public string KillMessage { get; set; }

        private void AttackPlayer(Player currentPlayer)
        {
            currentPlayer.Dead = true;
            Console.WriteLine(this.KillMessage);
        }

        public void DetectPlayerCheck(Player currentPlayer)
        {
            if (currentPlayer.Status != DetectionCheck)
            {
                AttackPlayer(currentPlayer);
            }
        }

        public Enemy(string name, string detectionCheck, string killMessage)
        {
            Name = name;
            DetectionCheck = detectionCheck;
            KillMessage = killMessage;
        }


    }
}