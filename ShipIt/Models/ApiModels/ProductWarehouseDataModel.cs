﻿using ShipIt.Models.DataModels;

namespace ShipIt.Models.ApiModels
{
        public class ProductWarehouseDataModel
    {
        public int Id { get; set; }

        public string Gtin { get; set; }

        public string Name { get; set; }

        public double Weight { get; set; }

        public int Held { get; set; }

        public ProductWarehouseDataModel(ProductDataModel apiModel, int held)
        {
            Id = apiModel.Id;
            Gtin = apiModel.Gtin;
            Name = apiModel.Name;
            Weight = apiModel.Weight;
            Held = held;
        }
    }
}