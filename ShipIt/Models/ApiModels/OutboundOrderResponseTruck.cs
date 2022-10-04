﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipIt.Models.ApiModels
{
    public class OutboundOrderResponseTruck : Response
    {
        public List<Truck> Trucks { get; set; }
        public List<StockAlteration> StockRemaining { get; set; }
        public List<StockAlteration> StockNeedToSend { get; set;}
        public OutboundOrderResponseTruck(List<Truck> trucks, List<StockAlteration> stockRemaining, List<StockAlteration> stockNeedToSend)
        {
            Trucks = trucks;
            StockRemaining = stockRemaining;
            StockNeedToSend = stockNeedToSend;
            Success = true;
        }
    }
}