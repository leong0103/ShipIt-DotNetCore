﻿using ShipIt.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipIt.Models.ApiModels
{
    public class OutboundOrderResponse
    {
        public int WarehouseId { get; set; }
        public int TotalTruckNeeded { get; set;}
        public IEnumerable<StockAlteration> StockAlteration { get; set; }

        public OutboundOrderResponse(int warehouseId, IEnumerable<StockAlteration> stockAlteration)
        {
            this.WarehouseId = warehouseId;
            this.StockAlteration = stockAlteration;
        }
    }
}