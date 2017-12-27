using System.Collections.Generic;

namespace CastleGrimtol.Project
{
    public class Item : IItem
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string UseText { get; set; }

        public string ItemLocation { get; set; }

        public string ItemCommand { get; set; }

        public bool Sturdy { get; set; }

        public int Uses { get; set; }

        public Item(string name, string description, string useText, string itemLocation, int uses, bool sturdy)
        {
            Name = name;
            Description = description;
            UseText = useText;
            ItemLocation = itemLocation;
            Uses = uses;
            Sturdy = sturdy;
        }
    }

}