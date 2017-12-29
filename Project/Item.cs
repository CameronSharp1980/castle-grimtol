using System.Collections.Generic;

namespace CastleGrimtol.Project
{
    public class Item : IItem
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public bool PlayerItem { get; set; }

        public string UseText { get; set; }

        public string ItemLocation { get; set; }

        public string RequiredLocation { get; set; }

        public bool RequiredToProceed { get; set; }

        public string ItemCommand { get; set; }

        public bool Sturdy { get; set; }

        public int Uses { get; set; }

        public string Power { get; set; }

        public Item(string name, string description, bool playerItem, string useText, string itemLocation, string requiredLocation, bool requiredToProceed, int uses, bool sturdy, string power)
        {
            Name = name;
            Description = description;
            PlayerItem = playerItem;
            UseText = useText;
            ItemLocation = itemLocation;
            RequiredLocation = requiredLocation;
            RequiredToProceed = requiredToProceed;
            Uses = uses;
            Sturdy = sturdy;
            Power = power;
        }
    }

}