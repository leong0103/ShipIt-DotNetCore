using System.Collections.Generic;
using ShipIt.Models.DataModels;

namespace ShipIt.Models.ApiModels
{
        public class Truck
    {
        public IEnumerable<StockAlteration> Items;
        public int totalCapicaityForEachTruck = 2000000;
        public float totalWeight = 0;

        public Truck()
        {}
    
    }
}