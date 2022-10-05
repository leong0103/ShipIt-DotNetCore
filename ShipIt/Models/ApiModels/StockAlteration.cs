﻿using ShipIt.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipIt.Models.ApiModels
{
    public class StockAlteration
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public float Weight { get; set; }

        public StockAlteration(int productId, int quantity)
        {
            this.ProductId = productId;
            this.Quantity = quantity;

            if (quantity < 0)
            {
                throw new MalformedRequestException("Alteration must be positive");
            }
        }

        public StockAlteration(int productId, int quantity, float weight)
        {
            this.ProductId = productId;
            this.Quantity = quantity;
            this.Weight = weight;

            if (quantity < 0 || weight < 0)
            {
                throw new MalformedRequestException("Alteration must be positive");
            }
        }

        public StockAlteration(int productId, string productName, int quantity, float weight)
        {
            this.ProductId = productId;
            this.ProductName = productName;
            this.Quantity = quantity;
            this.Weight = weight;

            if (quantity < 0 || weight < 0)
            {
                throw new MalformedRequestException("Alteration must be positive");
            }
        }
    }
}