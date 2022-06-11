using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_3r.Models
{
    public class InvoiceLineItemDTO
    {
        public InvoiceLineItem InvoiceLineItem { get; set; }
        public List<Product> Products { get; set; }
    }
}