using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _91APP.Models
{
    public class Order
    {
        public string Id { get; set; }
        public string Item { get; set; }
        public int Price { get; set; }
        public int Cost { get; set; }
        public string Status { get; set; }
    }

    public class ShipOrder
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string Status { get; set; }
        public string CreatedDateTime { get; set; }
    }
}