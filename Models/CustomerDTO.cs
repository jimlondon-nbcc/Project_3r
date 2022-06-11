using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_3r.Models
{
    public class CustomerDTO
    {
        public Customer Customer { get; set; }

        public List<State> States { get; set; }
    }
}