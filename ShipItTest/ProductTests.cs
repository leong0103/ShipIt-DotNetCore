﻿using System;
using System.Collections.Generic;
 using NUnit.Framework;
 using ShipIt.Controllers;
using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Models.DataModels;
using ShipIt.Repositories;
using ShipItTest.Builders;

namespace ShipItTest
{
    public class ProductControllerTests : AbstractBaseTest
    {
        ProductController productController = new ProductController(new ProductRepository(), new StockRepository());
        ProductRepository productRepository = new ProductRepository();

        private const int WAREHOUSE_ID = 1;

        // private static readonly Employee EMPLOYEE = new EmployeeBuilder().setWarehouseId(WAREHOUSE_ID).CreateEmployee();
        private const string GTIN = "0000346374230";

        [Test]
        public void TestRoundtripProductRepository()
        {
            onSetUp();
            var product = new ProductBuilder().CreateProductDatabaseModel();
            productRepository.AddProducts(new List<ProductDataModel>() {product});
            Assert.AreEqual(productRepository.GetProductByGtin(product.Gtin).Name, product.Name);
            Assert.AreEqual(productRepository.GetProductByGtin(product.Gtin).Gtin, product.Gtin);
        }

        [Test]
        public void TestGetProduct()
        {
            onSetUp();
            var productBuilder = new ProductBuilder().setGtin(GTIN);
            productRepository.AddProducts(new List<ProductDataModel>() {productBuilder.CreateProductDatabaseModel()});
            var result = productController.Get(GTIN);

            var correctProduct = productBuilder.CreateProduct();
            Assert.IsTrue(ProductsAreEqual(correctProduct, result.Product));
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void TestGetNonexistentProduct()
        {
            onSetUp();
            try
            {
                productController.Get(GTIN);
            }
            catch (NoSuchEntityException e)
            {
                Assert.IsTrue(e.Message.Contains(GTIN));
            }
        }

        [Test]
        public void TestAddProducts()
        {
            onSetUp();
            var productBuilder = new ProductBuilder().setGtin(GTIN);
            var productRequest = productBuilder.CreateProductRequest();

            var response = productController.Post(productRequest);
            var databaseProduct = productRepository.GetProductByGtin(GTIN);
            var correctDatabaseProduct = productBuilder.CreateProductDatabaseModel();

            Assert.IsTrue(response.Success);
            ProductsAreEqual(new Product(databaseProduct), new Product(correctDatabaseProduct));
        }

        [Test]
        public void TestAddPreexistingProduct()
        {
            onSetUp();
            var productBuilder = new ProductBuilder().setGtin(GTIN);
            productRepository.AddProducts(new List<ProductDataModel>() {productBuilder.CreateProductDatabaseModel()});
            var productRequest = productBuilder.CreateProductRequest();

            try
            {
                productController.Post(productRequest);
                Assert.Fail();
            }
            catch (MalformedRequestException e)
            {
                Assert.IsTrue(e.Message.Contains(GTIN));
            }
        }

        [Test]
        public void TestAddDuplicateProduct()
        {
            onSetUp();
            var productBuilder = new ProductBuilder().setGtin(GTIN);
            var productRequest = productBuilder.CreateDuplicateProductRequest();

            try
            {
                productController.Post(productRequest);
                Assert.Fail("Expected exception to be thrown.");
            }
            catch (MalformedRequestException e)
            {
                Assert.IsTrue(e.Message.Contains(GTIN));
            }
        }

        [Test]
        public void TestDiscontinueProduct()
        {
            onSetUp();
            var productBuilder = new ProductBuilder().setGtin(GTIN);
            productRepository.AddProducts(new List<ProductDataModel>() { productBuilder.CreateProductDatabaseModel() });

            productController.Discontinue(GTIN);
            var result = productController.Get(GTIN);

            Assert.IsTrue(result.Product.Discontinued);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void TestDiscontinueNonexistentProduct()
        {
            onSetUp();
            try
            {
                productController.Discontinue(GTIN);
                Assert.Fail("Expected exception to be thrown.");
            }
            catch (NoSuchEntityException e)
            {
                Assert.IsTrue(e.Message.Contains(GTIN));
            }
        }

        [Test]
        public void TestDiscontinueNonexistantProduct()
        {
            onSetUp();
            var nonExistantGtin = "12345678";
            try
            {
                productController.Discontinue(nonExistantGtin);
                Assert.Fail("Expected exception to be thrown.");
            }
            catch (NoSuchEntityException e)
            {
                Assert.IsTrue(e.Message.Contains(nonExistantGtin));
            }
        }

        private bool ProductsAreEqual(Product A, Product B)
        {
            const double floatingPointTolerance = 10 * float.Epsilon;
            return A.Discontinued == B.Discontinued
                   && A.Gcp == B.Gcp
                   && A.Gtin == B.Gtin
                   && A.LowerThreshold == B.LowerThreshold
                   && A.MinimumOrderQuantity == B.MinimumOrderQuantity
                   && A.Name == B.Name
                   && Math.Abs(A.Weight - B.Weight) < floatingPointTolerance;
        }
    }
}
