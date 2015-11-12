

namespace Syndaryl.RandomTreasure
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    public partial class df8
    {
        public static df8 LoadFile(string FileName)
        {
            df8 myObject;
            // Construct an instance of the XmlSerializer with the type
            // of object that is being deserialized.
            var mySerializer =
                    new XmlSerializer(typeof(df8));
            // To read the file, create a FileStream.
            var myFileStream =
                    new FileStream(FileName, FileMode.Open);
            // Call the Deserialize method and cast to the object type.
            myObject = (df8)mySerializer.Deserialize(myFileStream);

            return myObject;
        }
    }
    public partial class df8CategoryItem : IComparable, ICloneable
    {
        //public int CompareTo(object obj)
        //{
        //    if ( obj.GetType() == typeof (df8CategoryItem))
        //        return string.Compare(name, ((df8CategoryItem)obj).name, StringComparison.CurrentCulture);
        //    return
        //        string.Compare(name, obj.ToString(), StringComparison.CurrentCulture);
        //}
        public int CompareTo(object obj)
        {
            if (obj is string)
            {
                return ToString().CompareTo(obj);
            }
            //if (obj is df8CategoryItem)
            //{
            //    return 
            //}
            return GetHashCode().CompareTo(obj.GetHashCode());

        }

        public override string ToString()
        {
            //List<df8CategoryItemAttribute> attributes = new List<df8CategoryItemAttribute>(GetAttributes());
            //var isValidFor = GetSingleItemByEnum(ItemsChoiceType.ValidForAttribute);

            var str = this.name;
            var notAttributes = GetNotAttributes().Select(x => x.name + ": " + x.value).ToList();
            var attributes = GetAttributes().Select(x => x.name + ": " + x.value).ToList();
            attributes.AddRange(notAttributes);
            var qualities = String.Join("; ", attributes);

            return String.Format("{0} ({1})", str, qualities);
        }

        /// <summary>
        /// Returns the hash code for this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        private List<ItemsChoiceType> itemList;

        public object GetSingleItemByEnum(ItemsChoiceType choiceType)
        {
            if (itemList == null)
                itemList = new List<ItemsChoiceType>(this.ItemsElementName);
            var returnvalue = itemList.FindIndex(x => x == choiceType);
            if (returnvalue > -1)
                return this.Items[returnvalue];
            return null;
        }
        public df8CategoryItemAttribute GetAttribute(string attributeName)
        {
            var attribs = GetAttributes();
            return attribs.FirstOrDefault(x => x.name.Equals(attributeName));
        }
        public IEnumerable<df8CategoryItemAttribute> GetAttributes()
        {
            if (itemList == null && this.ItemsElementName != null)
                itemList = new List<ItemsChoiceType>(this.ItemsElementName);
            var attributeIndexes = getAttributeIndexes();
            foreach (var index in attributeIndexes)
            {
                yield return (df8CategoryItemAttribute)Items[index];
            }
        }
        public IEnumerable<df8CategoryItemAttribute> GetNotAttributes()
        {
            if (itemList == null && this.ItemsElementName != null)
                itemList = new List<ItemsChoiceType>(this.ItemsElementName);
            var notAttributeIndexes = getNotAttributeIndexes();
            foreach (var index in notAttributeIndexes)
            {
                yield return new df8CategoryItemAttribute(ItemsElementName[index].ToString(), Items[index].ToString());
            }
        }

        private IEnumerable<int> getAttributeIndexes()
        {
            for (int i = 0; i < this.ItemsElementName.Length; i++)
            {
                if (ItemsElementName[i] == ItemsChoiceType.Attribute)
                    yield return i;
            }
        }
        private IEnumerable<int> getNotAttributeIndexes()
        {
            for (int i = 0; i < this.ItemsElementName.Length; i++)
            {
                if (ItemsElementName[i] != ItemsChoiceType.Attribute)
                    yield return i;
            }
        }

        internal IEnumerable<df8CategoryItemReroll> GetRerolls()
        {
            foreach (var index in this.getRerollsIndexes())
            {
                yield return (df8CategoryItemReroll)((df8CategoryItemReroll)this.Items[index]).Clone();
            }
        }

        private IEnumerable<int> getRerollsIndexes()
        {
            for (int i = 0; i < this.ItemsElementName.Length; i++)
            {
                if (ItemsElementName[i] == ItemsChoiceType.Reroll)
                    yield return i;
            }
        }


        public object Clone()
        {
            var result = new df8CategoryItem();
            result.name = this.name;
            result.Items = new object[this.Items.Length][];
            for (int i = 0; i < result.Items.Length; i++)
            {
                result.Items = (object[])this.Items.Clone();
            }
            result.ItemsElementName = (Syndaryl.RandomTreasure.ItemsChoiceType[])this.ItemsElementName.Clone();

            return result;
        }
    }


    public partial class df8CategoryItemAttribute : IComparable, ICloneable
    {
        public df8CategoryItemAttribute()
        {
        }

        public df8CategoryItemAttribute(string Name, string Value, string Operation = "")
        {
            this.name = Name;
            this.value = Value;
            this.operation = Operation;
        }

        public object Clone()
        {
            var result = new df8CategoryItemAttribute();
            result.value = (string)value.Clone();
            result.name = name == null ? "" : (string)name.Clone();
            result.operation = operation == null ? "" : (string)operation.Clone();
            return result;
        }

        public int CompareTo(object obj)
        {
            return ToString().CompareTo(obj);
        }

        public override string ToString()
        {
            return string.Format("df8CategoryItemAttribute[Name: {0}; value: {1}; operation: {2}]", name, value, operation);
        }
    }
    public partial class df8CategoryItemReroll : IComparable, ICloneable
    {
        public df8CategoryItemReroll()
        {
            category = "";
        }
        public df8CategoryItemReroll(string reroll)
        {
            category = reroll;
        }

        public override string ToString()
        {
            return "df8CategoryItemReroll [" + category + "]";
        }

        public object Clone()
        {
            var result = new df8CategoryItemReroll();
            result.category = this.category;
            return result;
        }

        public int CompareTo(object obj)
        {
            return category.CompareTo(obj);
        }
    }
}
