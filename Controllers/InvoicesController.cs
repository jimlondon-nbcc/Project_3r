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
        //Constants to use during calculations
        const decimal SALES_TAX_RATE = 0.075m;
        const decimal FIRST_BOOK_SHIP_RATE = 3.75m;
        const decimal ADDITIONAL_BOOK_SHIP_RATE = 1.25m;
        
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
                            invoices = context.Invoices.OrderByDescending(s => s.InvoiceDate).ToList();
                        else
                            invoices = context.Invoices.OrderBy(s => s.InvoiceDate).ToList();

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
                case 4:
                    {
                        if (isDesc)
                            invoices = context.Invoices.OrderByDescending(s => s.InvoiceLineItems.Count).ToList();
                        else
                            invoices = context.Invoices.OrderBy(s => s.InvoiceLineItems.Count).ToList();

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
                        s.InvoiceID == idLookup ||
                        s.InvoiceDate.ToShortDateString().ToLower().Contains(id)
                    ).ToList();
                }
                else // if id doesn't parse as an int
                {
                    invoices = invoices.Where(s =>
                    s.Customer.Name.ToLower().Contains(id)
                    ).ToList();
                }

            }
            invoices = invoices.Where(s => s.IsDeleted == false).ToList();
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

            if (invoice == null) 
            {
                invoice = new Invoice();
                invoice.Customer = new Customer();
                invoice.InvoiceDate = DateTime.Now;
            }

            //Ensure deleted invoices aren't visible
            if (invoice.IsDeleted)
            {
                return RedirectToAction("All");
            }

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
        public ActionResult Upsert(InvoiceDTO invoiceDTO, string customerId)        {
            
            try
            {
                int iCustomerId = Convert.ToInt32(customerId.Split('-')[0].Trim());
                
                //Match invoice from db with customerId
                invoiceDTO.Invoice.CustomerID = iCustomerId;                

                //Calculate totals, tax, shipping
                Invoice invoiceToUpdate = CalculateInvoiceTotals(invoiceDTO.Invoice);
                
                BooksEntities context = new BooksEntities();

                //Already have invoice match, so we don't need this:
                //invoiceToUpdate = context.Invoices.Where(e => e.InvoiceID == invoiceDTO.Invoice.InvoiceID).FirstOrDefault();

                context.Invoices.AddOrUpdate(invoiceToUpdate);
                context.SaveChanges();

                return Redirect("/Invoices/Upsert/" + invoiceToUpdate.InvoiceID.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        ///     HttpGet to delete an invoice
        /// </summary>
        /// <param name="id">The Id of the invoice to delete</param>
        /// <returns>Redirect to All Invoices View</returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {
            BooksEntities context = new BooksEntities();

            if (int.TryParse(id, out int invoiceId))
            {
                try
                {
                    Invoice invoice = context.Invoices.Where(s => s.InvoiceID == invoiceId).FirstOrDefault();
                    invoice.IsDeleted = true;

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
        /// Called by upsert to calculate/recalculate the totals of the line items + tax, shipping
        /// </summary>
        /// <param name="invoice">The invoice object being updated</param>
        /// <returns>Invoice object</returns>
        public Invoice CalculateInvoiceTotals(Invoice invoice)
        {
            BooksEntities context = new BooksEntities();

            List<InvoiceLineItem> lineItems = context.InvoiceLineItems.Where(i => i.InvoiceID == invoice.InvoiceID).ToList();

            invoice.InvoiceTotal = 0;
            foreach(var lineItem in lineItems)
            {
                invoice.ProductTotal += lineItem.ItemTotal;
            }
            //Calculate shipping and tax based on number of line items
            if(lineItems.Count > 0)
            {
                invoice.Shipping = FIRST_BOOK_SHIP_RATE + ((lineItems.Count - 1) * ADDITIONAL_BOOK_SHIP_RATE);
            } else
            {
                invoice.Shipping = 0;
            }
            
            invoice.SalesTax = (invoice.ProductTotal + invoice.Shipping) * SALES_TAX_RATE;
            
            //Update the total
            invoice.InvoiceTotal = invoice.ProductTotal + invoice.Shipping + invoice.SalesTax;

            return invoice;
        }
    }
}