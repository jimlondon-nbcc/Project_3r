using Project_3r.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_3r.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        /// <summary>
        ///     Creates view for All Products (with sorting)
        /// </summary>
        /// <param name="sortBy">0 = ProdCode, 1 = Description, 2 = UnitPrice, 3 = Quant</param>
        /// <param name="isDesc">false = Ascending Order, true = Descending Order</param>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Product> products = context.Products.OrderBy(c => c.ProductCode).ToList();

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                            products = context.Products.OrderByDescending(p => p.Description).ToList();
                        else
                            products = context.Products.OrderBy(p => p.Description).ToList();

                        break;
                    }
                case 2:
                    {
                        if (isDesc)
                            products = context.Products.OrderByDescending(p => p.UnitPrice).ToList();
                        else
                            products = context.Products.OrderBy(p => p.UnitPrice).ToList();

                        break;
                    }
                case 3:
                    {
                        if (isDesc)
                            products = context.Products.OrderByDescending(p => p.OnHandQuantity).ToList();
                        else
                            products = context.Products.OrderBy(p => p.OnHandQuantity).ToList();

                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                            products = context.Products.OrderByDescending(p => p.ProductCode).ToList();
                        else
                            products = context.Products.OrderBy(p => p.ProductCode).ToList();

                        break;
                    }

            }
            //id is used as searchTerm 
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();
                decimal minAmt = 0m;
                if (decimal.TryParse(id, out minAmt)) // if id is decimal
                {
                    products = products.Where(p =>
                        p.UnitPrice <= minAmt ||
                        p.OnHandQuantity <= minAmt
                    ).ToList();
                }
                else // if id doesn't parse as decimal
                {
                    products = products.Where(p =>
                    p.ProductCode.ToLower().Contains(id) ||
                    p.Description.ToLower().Contains(id)
                    ).ToList();
                }
            }
            products = products.Where(p => p.IsDeleted == false).ToList();
            return View(products);
        }
        /// <summary>          
        ///     Upsert GET method to fetch data 
        /// </summary>
        /// <param name="id">The ProductCode of the product to update</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Upsert(string id)
        {
            BooksEntities context = new BooksEntities();
            Product product = context.Products.Where(p => p.ProductCode == id).FirstOrDefault();
            if(product== null)
            {
                product = new Product();
            }

            //Ensure deleted invoices aren't visible
            if (product.IsDeleted)
            {
                return RedirectToAction("All");
            }

            return View(product);
        }
        /// <summary>
        ///     Used to Update or Insert customers
        /// </summary>
        /// <param name="product">The product being posted</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upsert(Product product)
        {
            try
            {
                BooksEntities context = new BooksEntities();

                context.Products.AddOrUpdate(product);

                context.SaveChanges();
            }
            catch (Exception) { }

            return RedirectToAction("All");
        }
        /// <summary>
        ///     HttpGet to delete a product
        /// </summary>
        /// <param name="id">The Id of the product to delete</param>
        /// <returns>Redirect to All Products View</returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                Product product = context.Products.Where(s => s.ProductCode == id).FirstOrDefault();
                product.IsDeleted = true;

                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToAction("All");
        }
    }
}