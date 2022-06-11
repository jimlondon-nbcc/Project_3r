using Project_3r.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;


namespace Project_3r.Controllers
{
    public class InvoiceLineItemsController : Controller
    {
        // GET: InvoiceLineItems
       [HttpGet]
        public ActionResult Upsert(int invoiceId, string productCode)
        {
            BooksEntities context = new BooksEntities();

            InvoiceLineItem lineItem = context.InvoiceLineItems.Where(i => i.InvoiceID == invoiceId
            && i.ProductCode == productCode).FirstOrDefault();

            if (lineItem == null)
            {
                lineItem = new InvoiceLineItem();
                lineItem.InvoiceID = invoiceId;
            }

            //Ensure deleted items aren't visible
            if (lineItem.IsDeleted)
            {
                return RedirectToAction("All");
            }

            InvoiceLineItemDTO dto = new InvoiceLineItemDTO()
            {
                InvoiceLineItem = lineItem,
                Products = context.Products.ToList()
            };

            return View(dto);
        }
        [HttpPost]
        public ActionResult Upsert(InvoiceLineItemDTO lineItemDTO)       
        {
            BooksEntities context = new BooksEntities();
            InvoiceLineItem lineItem = lineItemDTO.InvoiceLineItem;

            try
            {
                string prodCode = lineItem.ProductCode.Trim();

                Product product = context.Products.Where
                    (p => p.ProductCode == prodCode).FirstOrDefault();

                if(lineItem.UnitPrice == 0)
                {
                    lineItem.UnitPrice = product.UnitPrice;
                }
                
                lineItem.ItemTotal = lineItem.Quantity * lineItem.UnitPrice;
                
                context.InvoiceLineItems.AddOrUpdate(lineItem);
                context.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }

            return Redirect("/Invoices/Upsert/" + lineItem.InvoiceID.ToString());
        }

        /// <summary>
        ///     HttpGet to delete an invoice line item
        /// </summary>
        /// <param name="id">The Id of the item to delete</param>
        /// <returns>Redirect to originating invoice</returns>
        [HttpGet]
        public ActionResult Delete(int invoiceId, string productCode)
        {
            BooksEntities context = new BooksEntities();

            InvoiceLineItem lineItem = new InvoiceLineItem();

            try
            {
                 lineItem = context.InvoiceLineItems.Where(
                    s => s.InvoiceID == invoiceId && s.ProductCode == productCode)
                    .FirstOrDefault();

                context.InvoiceLineItems.Remove(lineItem);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return Redirect("/Invoices/Upsert/" + lineItem.InvoiceID.ToString());
        }
    }
}