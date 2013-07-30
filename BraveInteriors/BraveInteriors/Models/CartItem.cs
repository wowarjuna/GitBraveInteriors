using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace BraveInteriors.Models
{
    public class CartItem
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        public int Qty { get; set; }
        public double Total { get; set; }

        public string Type { get; set; }

        public double Shipping { get; set; }

        public double EUShipping { get; set; }

        public double ShippingTotal { get; set; }

        public string CouponCode { get; set; }

        public CartItem()
        {
            Qty = 1;
        }
    }
}