using System.Collections.Generic;

namespace CastleGrimtol.Project
{
    public class Item : IItem
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string ItemLocation { get; set; }

        public string ItemCommand { get; set; }

        public int Uses { get; set; }

        public Item(string name, string description, string itemLocation, int uses)
        {
            Name = name;
            Description = description;
            ItemLocation = itemLocation;
            Uses = uses;
        }
    }

}