﻿using System;
using System.Collections.Generic;
using System.Linq;
 using Microsoft.AspNetCore.Mvc;
 using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Models.DataModels;
using ShipIt.Repositories;

namespace ShipIt.Controllers
{
    [Route("orders/outbound")]
    public class OutboundOrderController : ControllerBase
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly IStockRepository _stockRepository;
        private readonly IProductRepository _productRepository;

        public OutboundOrderController(IStockRepository stockRepository, IProductRepository productRepository)
        {
            _stockRepository = stockRepository;
            _productRepository = productRepository;
        }

        [HttpPost("")]
        public OutboundOrderResponseTruck Post([FromBody] OutboundOrderRequestModel request)
        {
            Log.Info(String.Format("Processing outbound order: {0}", request));

            //get product gtin list, no duplicate.
            var gtins = new List<String>();
            foreach (var orderLine in request.OrderLines)
            {
                if (gtins.Contains(orderLine.gtin))
                {
                    throw new ValidationException(String.Format("Outbound order request contains duplicate product gtin: {0}", orderLine.gtin));
                }
                gtins.Add(orderLine.gtin);
            }

            var productDataModels = _productRepository.GetProductsByGtin(gtins);
            var products = productDataModels.ToDictionary(p => p.Gtin, p => new Product(p));

            var lineItems = new List<StockAlteration>();
            var productIds = new List<int>();
            var errors = new List<string>();

            foreach (var orderLine in request.OrderLines)
            {
                if (!products.ContainsKey(orderLine.gtin))
                {
                    errors.Add(string.Format("Unknown product gtin: {0}", orderLine.gtin));
                }
                else
                {
                    var product = products[orderLine.gtin];
                    lineItems.Add(new StockAlteration(product.Id, orderLine.quantity, product.Weight));
                    productIds.Add(product.Id);
                }
            }

            if (errors.Count > 0)
            {
                throw new NoSuchEntityException(string.Join("; ", errors));
            }

            var stock = _stockRepository.GetStockByWarehouseAndProductIds(request.WarehouseId, productIds);

            var orderLines = request.OrderLines.ToList();
            errors = new List<string>();

            for (int i = 0; i < lineItems.Count; i++)
            {
                var lineItem = lineItems[i];
                var orderLine = orderLines[i];

                if (!stock.ContainsKey(lineItem.ProductId))
                {
                    errors.Add(string.Format("Product: {0}, no stock held", orderLine.gtin));
                    continue;
                }

                var item = stock[lineItem.ProductId];
                if (lineItem.Quantity > item.held)
                {
                    errors.Add(
                        string.Format("Product: {0}, stock held: {1}, stock to remove: {2}", orderLine.gtin, item.held,
                            lineItem.Quantity));
                }
            }

            if (errors.Count > 0)
            {
                throw new InsufficientStockException(string.Join("; ", errors));
            }
            
            // _stockRepository.RemoveStock(request.WarehouseId, lineItems);
            OutboundOrderResponse response = new OutboundOrderResponse(request.WarehouseId, lineItems);
            

            List<Truck> trucks = new List<Truck>();
            List<StockAlteration> stockNeedToSend = new List<StockAlteration>();

//stockNeedToSend is reference types??

            stockNeedToSend = response.StockAlteration.ToList();

            //get total Weight need to send
            int maxCapicityOfTruck = 2000;
            float totalWeight = 0;
            foreach (var item in response.StockAlteration)
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
                foreach (var item in stockNeedToSend)
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
                Console.WriteLine("Total Weight{0}:", truck.totalWeight);
                Console.WriteLine("This is trunk{0}", truckNumber);
                
                foreach(var S in truck.Items)
                {
                    Console.WriteLine("Item:");
                    Console.WriteLine(S.ProductId);
                    Console.WriteLine(S.Quantity);
                    Console.WriteLine(S.Weight);
                }
                truckNumber ++;
            }
            return new OutboundOrderResponseTruck(trucks, stockNeedToSend, response.StockAlteration.ToList());
            // return response;
        }
    }
}