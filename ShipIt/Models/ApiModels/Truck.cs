using System.Collections.Generic;
using ShipIt.Models.DataModels;

namespace ShipIt.Models.ApiModels
{
        public class Truck
    {
        public List<StockAlteration> Items;
        public int totalCapicaityForEachTruck = 2000;
        public float totalWeight = 0;

        public Truck()
        {
            this.Items = new List<StockAlteration>();
        }
    
    }
}