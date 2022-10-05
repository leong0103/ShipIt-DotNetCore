using System;
using System.Collections.Generic;
using System.Linq;
using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;

namespace ShipIt.Helper
{
    public static class TruckCalculationHelper
    {
        public static OutboundOrderResponseTruck GetTrucksNumber(OutboundOrderResponse response)
        {
            List<Truck> trucks = new List<Truck>();
            IEnumerable<StockAlteration> stockRemaining = response.StockAlteration;
            List<StockAlteration> stockNeedToSend = response.StockAlteration.ToList();
            var errors = new List<string>();

//stockRemaining is reference types??

            //get total Weight need to send
            int maxCapicityOfTruck = 2000;
            float totalWeight = 0;
            foreach (var item in stockRemaining)
            {
                totalWeight += item.Weight * item.Quantity;
            }
            
            //inital setup
            bool isSuitableItemFind = false;
            int counter = 0;
            bool needNewTruck = true;

            //If totalWeight > 0, infinity loop
            while (totalWeight > 1)
            {   
                //loop the list one more time to pick items, 
                if(counter > 1)
                {
                    needNewTruck = true;
                }
                if(needNewTruck)
                {
                    trucks.Add(new Truck());
                    
                    needNewTruck = false;

                }
                foreach (var item in stockRemaining)
                {   
                    //get last truck weight and compare with maxCapicity
                    if(trucks.LastOrDefault().totalWeight < maxCapicityOfTruck)
                    {
                        for (int i = item.Quantity; i > 0; i--)
                        {
                            float weight = i * item.Weight;
                            
                            if((!(weight >= maxCapicityOfTruck - trucks.LastOrDefault().totalWeight)) && !isSuitableItemFind && trucks.LastOrDefault() != null)
                            {
                                item.Quantity -= i;
                                
                                //No this part, will got error
                                if (trucks.LastOrDefault().Items == null)
                                {
                                    throw new InsufficientStockException(string.Join("; ", errors));
                                    
                                }
                                trucks.LastOrDefault().Items.Add(new StockAlteration(item.ProductId, i, weight));
                                trucks.LastOrDefault().totalWeight += weight;
                                totalWeight -= weight;
                                isSuitableItemFind = true;
                            }
                        }
                    }
                    else
                    {
                        trucks.Add(new Truck());
                        needNewTruck = false;
                    }
                    isSuitableItemFind = false;
                }
                
                counter++;
            }

            //setup for print
            int truckNumber = 0;
            foreach(var truck in trucks)
            {  
                Console.WriteLine("=======Total Weight {0}:======", truck.totalWeight);
                Console.WriteLine("This is trunk{0}", truckNumber);
                Console.WriteLine("");
                
                foreach(var S in truck.Items)
                {
                    Console.WriteLine("Item: {0}", S.ProductId);
                    Console.WriteLine("ProductName: {0}", S.ProductName);
                    Console.WriteLine("Quantity: {0}", S.Quantity);
                    Console.WriteLine("Weight: {0}", S.Weight);
                    Console.WriteLine("");
                }
                truckNumber ++;
            }
            return new OutboundOrderResponseTruck(trucks, stockNeedToSend, stockRemaining);
        }
    }
}