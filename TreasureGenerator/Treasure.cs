
namespace Syndaryl.TreasureGenerator
{
    using Syndaryl.Collections;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    internal class Treasure
    {
        string libraryName = "";
        private df8 library;

        public Treasure(string LibraryName)
        {
            this.LibraryName = LibraryName;
        }

        public string LibraryName
        {
            get
            {
                return libraryName;
            }

            set
            {
                libraryName = value;
                Library = df8.LoadFile(libraryName);
            }
        }

        public df8 Library
        {
            get
            {
                return library;
            }

            set
            {
                library = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LibraryName">File name/path of the library XML to load. Must conform to UnifiedFile.xsd so it can be deserialized.</param>
        /// <returns></returns>
        internal TreeRoot<DF8Result> Generate(string LibraryName)
        {
            if (!this.LibraryName.Equals(LibraryName) || Library == null)
            {
                this.LibraryName = LibraryName;
            }
            return Generate();
        }

        internal TreeRoot<DF8Result> Generate()
        {
            if (Library == null || this.LibraryName.Equals(string.Empty))
            {
                throw new NoLibraryException("Tried to generate a random item without a library successfully loaded!");
            }
            return FromRootCategory(LoadTables(Library), "gems");
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Converts a df8 library into a dictionary.
        /// </summary>
        /// <param name="library">The library containing all the categories.</param>
        /// <returns>A Dictionary of WeightedList of df8CategoryItem</returns>
        private Dictionary<string, WeightedList<df8CategoryItem>> LoadTables(df8 library)
        {
            var categories = new Dictionary<string, WeightedList<df8CategoryItem>>();
            foreach (var category in library.category)
            {
                categories.Add(category.name, new WeightedList<df8CategoryItem>());
                foreach (var randomItem in category.item)
                {
                    int itemWeight = 1;
                    var choiceIndex = randomItem.ItemsElementName.ToList().FindIndex(x => x == ItemsChoiceType.RandomWeight);
                    if (choiceIndex > -1)
                    {
                        itemWeight = Convert.ToInt16(randomItem.Items[choiceIndex]);
                    }
                    categories[category.name].Add(randomItem, itemWeight);
                }
                categories[category.name].DoWeightCumulation();
            }
            return categories;
        }

        internal TreeRoot<DF8Result> FromRootCategory(Dictionary<string, WeightedList<df8CategoryItem>> categories, string category)
        {
            if (!categories.ContainsKey(category))
                return null;

            var roller = new WeightedListRoller<df8CategoryItem>(categories[category]);
            var randomItem = roller.RandomItem();
            var resolver = new DF8Resolver(categories);
            var resolvedRoller = resolver.ProduceFinalItem(randomItem);

            return resolvedRoller;
        }
    }

    [Serializable]
    internal class NoLibraryException : Exception
    {
        public NoLibraryException()
        {
        }

        public NoLibraryException(string message) : base(message)
        {
        }

        public NoLibraryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoLibraryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}