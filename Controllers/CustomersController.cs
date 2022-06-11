using Project_3r.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        /// <summary>
        ///     Returns a customer for update, or creates a new customer to add
        /// </summary>
        /// <param name="id">Id of the customer to create/edit</param>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult Upsert(int id = 0)
        {
            BooksEntities context = new BooksEntities();
            //If no customer in db, create new instance of customer

            Customer customer = context.Customers.Where(c => c.CustomerID == id).FirstOrDefault();
            List<State> states = context.States.ToList();
            
            if(customer == null) 
            {
                customer = new Customer();
            }

            CustomerDTO dto = new CustomerDTO()
            {
                Customer = customer,
                States = states
            };

            //make sure that data for deleted customers is not visible
            if (customer.IsDeleted)
            {
                return RedirectToAction("All");
            }

            return View(dto);
        }
        /// <summary>
        ///     HttpPost to submit data to db for new customer or update
        /// </summary>
        /// <param name="customer">The customer to add/update</param>
        /// <returns>Redirect to All Customers View</returns>
        [HttpPost]
        public ActionResult Upsert(CustomerDTO customerDTO)
        {
            Customer customer = customerDTO.Customer;            
            BooksEntities context = new BooksEntities();

            try
            {
                context.Customers.AddOrUpdate(customer);
                context.SaveChanges(); 
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToAction("All");
        }
        /// <summary>
        ///     HttpGet to delete a customer
        /// </summary>
        /// <param name="id">The Id of the customer to delete</param>
        /// <returns>Redirect to All Customers View</returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {
            BooksEntities context = new BooksEntities();

            if (int.TryParse(id, out int customerId))
            {
                try
                {
                    Customer customer = context.Customers.Where(c => c.CustomerID == customerId).FirstOrDefault();
                    customer.IsDeleted = true;

                    context.SaveChanges();
                }
                catch (Exception)
                {
                    throw;
                }

            }

            return RedirectToAction("All");
        }
        /// <summary>
        ///     Function called by Upsert to send a welcome Email to new customers
        /// </summary>
        /// <param name="newCustomer">The customer to send an Email to</param>

    }
}