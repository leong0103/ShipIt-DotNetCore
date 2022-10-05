﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipIt.Models.ApiModels
{
    public class OutboundOrderResponseTruck : Response
    {
        public List<Truck> Trucks { get; set; }
        public List<StockAlteration> StockNeedToSend { get; set;}
        public IEnumerable<StockAlteration> StockRemaining { get; set; }
        public OutboundOrderResponseTruck(List<Truck> trucks, List<StockAlteration> stockNeedToSend, IEnumerable<StockAlteration> stockRemaining)
        {
            Trucks = trucks;
            StockNeedToSend = stockNeedToSend;
            StockRemaining = stockRemaining;
            Success = true;
        }

        public OutboundOrderResponseTruck ()
        {
            
        }
    }
}