using Project_3r.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_3r.Controllers
{
    public class InvoicesController : Controller
    {
        // GET: Invoices
        /// <summary>
        ///     Creates view for All Invoices (with sorting)
        /// </summary>
        /// <param name="sortBy">0 = Invoices_Id, 1 = customerId, 2 = Date, 3 = Total</param>
        /// <param name="isDesc">false = Ascending Order, true = Descending Order</param>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Invoice> invoices = context.Invoices.OrderBy(s => s.InvoiceID).ToList();

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                            invoices = context.Invoices.OrderByDescending(s => s.CustomerID).ToList();
                        else
                            invoices = context.Invoices.OrderBy(s => s.CustomerID).ToList();

                        break;

                    }
                case 2:
                    {
                        if (isDesc)
                            invoices = context.Invoices.OrderByDescending(s => s.InvoiceDate.ToShortDateString()).ToList();
                        else
                            invoices = context.Invoices.OrderBy(s => s.InvoiceDate.ToShortDateString()).ToList();

                        break;
                    }
                case 3:
                    {
                        if (isDesc)
                            invoices = context.Invoices.OrderByDescending(s => s.InvoiceTotal).ToList();
                        else
                            invoices = context.Invoices.OrderBy(s => s.InvoiceTotal).ToList();

                        break;
                    }

                default:
                    {
                        if (isDesc)
                            invoices = context.Invoices.OrderByDescending(s => s.InvoiceID).ToList();
                        else
                            invoices = context.Invoices.OrderBy(s => s.InvoiceID).ToList();

                        break;
                    }

            }
            //id is used as searchTerm 
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();
                int idLookup = 0;
                if (int.TryParse(id, out idLookup)) // if id is an int
                {
                    invoices = invoices.Where(s =>
                        s.CustomerID == idLookup ||
                        s.InvoiceID == idLookup
                    ).ToList();
                }
                else // if id doesn't parse as an int
                {
                    invoices = invoices.Where(s =>
                    s.InvoiceDate.ToShortDateString().ToLower().Contains(id)
                    || s.Customer.Name.ToLower().Contains(id)
                    ).ToList();
                }

            }

            return View(invoices);
        }

        /// <summary>
        ///     Used to retrieve an Invoice and view its details
        /// </summary>
        /// <param name="id">Id of the Invoice to view/edit</param>
        /// <returns>An Invoice object</returns>
        [HttpGet]
        public ActionResult Upsert(int id = 0)
        {
            BooksEntities context = new BooksEntities();
            Invoice invoice = context.Invoices.Where(e => e.InvoiceID == id).FirstOrDefault();

            if (invoice == null) { invoice = new Invoice(); }

            InvoiceDTO dto = new InvoiceDTO()
            {
                Invoice = invoice,
                Customers = context.Customers.ToList(),
                InvoiceLineItems = context.InvoiceLineItems.ToList()
            };            

            return View(dto);

        }

        /// <summary>
        ///     Used to Update or Insert an invoice base on the object's ObjectDTO.Invoice.InvoiceID
        /// </summary>
        /// <param name="invoiceDTO">The InvoiceDTO being posted</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upsert(InvoiceDTO invoiceDTO, string customerId)
        {
            try
            {
                int iCustomerId = Convert.ToInt32(customerId.Split('-')[0].Trim());
                invoiceDTO.Invoice.CustomerID = iCustomerId;

                
                BooksEntities context = new BooksEntities();
                Invoice invoiceToUpdate = context.Invoices.Where(e => e.InvoiceID == invoiceDTO.Invoice.InvoiceID).FirstOrDefault();

                context.Invoices.AddOrUpdate(invoiceDTO.Invoice);

                return RedirectToAction("All");
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}