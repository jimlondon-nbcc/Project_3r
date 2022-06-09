using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_3r.Models
{
    public class InvoiceDTO
    {
        public Invoice Invoice { get; set; }
        public List<Customer> Customers { get; set; }
        public List<InvoiceLineItem> InvoiceLineItems { get; set; }
    }
}