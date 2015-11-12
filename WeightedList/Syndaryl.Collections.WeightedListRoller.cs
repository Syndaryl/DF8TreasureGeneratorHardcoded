namespace Syndaryl.Collections
{
    using System;
    class WeightedListRoller<T>
    {
        private WeightedList<T> weightedList;
        private readonly Random rng;

        public WeightedListRoller(WeightedList<T> weightedList)
        {
            this.WeightedList = weightedList;
            rng = new Random();
        }

        public WeightedList<T> WeightedList
        {
            get
            {
                return weightedList;
            }

            set
            {
                weightedList = value;
            }
        }

        public T RandomItem()
        {
            var indexToLookup = Math.Truncate(1 + ( rng.NextDouble() * (WeightedList.TotalWeight-1) ));
            return WeightedList.GetFromWeight( indexToLookup);
        }
    }
}
