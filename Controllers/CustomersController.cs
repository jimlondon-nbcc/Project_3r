using Project_3r.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_3r.Controllers
{
    public class CustomersController : Controller
    {
        // GET: Customers
        /// <summary>
        ///     Creates view for All Customers (with sorting)
        /// </summary>
        /// <param name="sortBy">0 = Id, 1 = Name, 2 = City, 3 = State, 4 = Zip</param>
        /// <param name="isDesc">false = Ascending Order, true = Descending Order</param>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Customer> customers = context.Customers.OrderBy(c => c.CustomerID).ToList();

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                            customers = context.Customers.OrderByDescending(c => c.Name).ToList();
                        else
                            customers = context.Customers.OrderBy(c => c.Name).ToList();

                        break;
                    }
                case 2:
                    {
                        if (isDesc)
                            customers = context.Customers.OrderByDescending(c => c.City).ToList();
                        else
                            customers = context.Customers.OrderBy(c => c.City).ToList();

                        break;
                    }
                case 3:
                    {
                        if (isDesc)
                            customers = context.Customers.OrderByDescending(c => c.State).ToList();
                        else
                            customers = context.Customers.OrderBy(c => c.State).ToList();

                        break;
                    }
                case 4:
                    {
                        if (isDesc)
                            customers = context.Customers.OrderByDescending(c => c.ZipCode).ToList();
                        else
                            customers = context.Customers.OrderBy(c => c.ZipCode).ToList();

                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                            customers = context.Customers.OrderByDescending(c => c.CustomerID).ToList();
                        else
                            customers = context.Customers.OrderBy(c => c.CustomerID).ToList();

                        break;
                    }
            }
            //id is used as searchTerm 
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                customers = customers.Where(c =>
                c.Name.ToLower().Contains(id) ||
                c.City.ToLower().Contains(id) ||
                c.State.ToLower().Contains(id) ||
                c.ZipCode.ToLower().Contains(id)
                ).ToList();
            }

            customers = customers.Where(c => c.IsDeleted == false).ToList();
            return View(customers);
        }
    }
}