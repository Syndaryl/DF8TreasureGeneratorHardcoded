using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syndaryl.Collections
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class WeightedList<ListType> : SortedDictionary<ListType, double>
    {
        SortedDictionary<ListType, double> cumulativeWeights;

        double totalWeight = 0.0;

        public SortedDictionary<ListType, double> CumulativeWeights
        {
            get
            {
                return cumulativeWeights;
            }

            set
            {
                cumulativeWeights = value;
            }
        }

        public double TotalWeight
        {
            get
            {
                return totalWeight;
            }

            set
            {
                totalWeight = value;
            }
        }

        public double DoWeightCumulation()
        {
            CumulativeWeights = new SortedDictionary<ListType, double>();
            TotalWeight = 0.0;
            foreach (var item in this.Keys)
            {
                TotalWeight += this[item];
                CumulativeWeights[item] = TotalWeight;
            }
            return TotalWeight;
        }

        public ListType GetFromWeight(double weight)
        {
            if (weight <= TotalWeight)
            {
                var index = (int) Array.FindIndex(CumulativeWeights.Values.ToArray(), x => weight <= x);
                index = index < 0 ? 0 : index;
                return CumulativeWeights.Keys.ToArray()[index];
            }
            return default(ListType);
        }
    }

}
