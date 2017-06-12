using System.Collections.Generic;

namespace Syndaryl.TreasureGenerator
{
    internal class DF8Result
    {
        private readonly df8CategoryItem item;
        private readonly List<df8CategoryItemAttribute> modifiers = new List<df8CategoryItemAttribute>();

        public df8CategoryItem UnmodifiedItem { get; set; }

        public DF8Result(df8CategoryItem rolledItem)
        {
            this.item = rolledItem;
        }

        public df8CategoryItem Item
        {
            get
            {
                return item;
            }
        }

        public List<df8CategoryItemAttribute> Modifiers
        {
            get
            {
                return modifiers;
            }
        }
    }
}