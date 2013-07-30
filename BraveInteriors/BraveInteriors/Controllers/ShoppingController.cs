using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BraveInteriors.Models;
using System.Net;
using System.Text;
using System.IO;

namespace BraveInteriors.Controllers
{
    public class ShoppingController : Controller
    {
        //
        // GET: /Shopping/

        private List<SelectListItem> GetHowDidYouHearUs()
        {
            List<SelectListItem> options = new List<SelectListItem>();
            options.Add(new SelectListItem { Text = "How Did You Hear About Us?", Value = "" });
            options.Add(new SelectListItem { Text = "Search Engine", Value = "Search Engine" });
            options.Add(new SelectListItem { Text = "Referred by a friend", Value = "Referred by a friend" });
            options.Add(new SelectListItem { Text = "Print advertising", Value = "Print advertising" });
            options.Add(new SelectListItem { Text = "From another web site", Value = "From another web site" });
            options.Add(new SelectListItem { Text = "Other", Value = "Other" });


            return options;
        }

        private List<SelectListItem> GetCountries()
        {
            List<SelectListItem> _countries = new List<SelectListItem>();
            _countries.Add(new SelectListItem { Text = "United Kigndom", Value = "GB", Selected = true });
            _countries.Add(new SelectListItem { Text = "Andorra", Value = "AD" });
            _countries.Add(new SelectListItem { Text = "Austria", Value = "AT" });
            _countries.Add(new SelectListItem { Text = "Belgium", Value = "BE" });
            _countries.Add(new SelectListItem { Text = "Cyprus", Value = "CY" });
            _countries.Add(new SelectListItem { Text = "Czech Republic", Value = "CZ" });
            _countries.Add(new SelectListItem { Text = "Denmark", Value = "DK" });
            _countries.Add(new SelectListItem { Text = "Finland", Value = "FI" });
            _countries.Add(new SelectListItem { Text = "France", Value = "FR" });
            _countries.Add(new SelectListItem { Text = "Germany", Value = "DE" });
            _countries.Add(new SelectListItem { Text = "Gibraltar", Value = "GI" });
            _countries.Add(new SelectListItem { Text = "Greece", Value = "GR" });
            _countries.Add(new SelectListItem { Text = "Hungary", Value = "HU" });
            _countries.Add(new SelectListItem { Text = "Iceland", Value = "IS" });
            _countries.Add(new SelectListItem { Text = "Ireland", Value = "IE" });
            _countries.Add(new SelectListItem { Text = "Italy", Value = "IT" });
            _countries.Add(new SelectListItem { Text = "Latvia", Value = "LV" });
            _countries.Add(new SelectListItem { Text = "Liechtenstein", Value = "LI" });
            _countries.Add(new SelectListItem { Text = "Lithuania", Value = "LT" });
            _countries.Add(new SelectListItem { Text = "Luxembourg", Value = "LU" });
            _countries.Add(new SelectListItem { Text = "Malta", Value = "MT" });
            _countries.Add(new SelectListItem { Text = "Monaco", Value = "MC" });
            _countries.Add(new SelectListItem { Text = "Netherlands", Value = "NL" });
            _countries.Add(new SelectListItem { Text = "Norway", Value = "NO" });
            _countries.Add(new SelectListItem { Text = "Poland", Value = "PL" });
            _countries.Add(new SelectListItem { Text = "Portugal", Value = "PT" });
            _countries.Add(new SelectListItem { Text = "Slovakia", Value = "SK" });
            _countries.Add(new SelectListItem { Text = "Slovenia", Value = "SI" });
            _countries.Add(new SelectListItem { Text = "Spain", Value = "ES" });
            _countries.Add(new SelectListItem { Text = "Sweden", Value = "SE" });
            _countries.Add(new SelectListItem { Text = "Switzerland", Value = "CH" });
            _countries.Add(new SelectListItem { Text = "Ukraine", Value = "UA" });
            
            return _countries;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cart()
        {
            var _cart = Session["cart"] as Dictionary<string, CartItem>;

            foreach (var _product in _cart)
                update(_product.Key, _product.Value.Qty, "GB", "");

            ViewBag.SubTotal = GetSubTotal();
            ViewBag.Total = GetTotal();
            ViewBag.Shipping = GetShipping();
            
            
            ViewBag.Countries = GetCountries();
            ViewBag.how_did_you = GetHowDidYouHearUs();

            return View(_cart);
        }

        public ActionResult Checkout()
        {
            
           
            var _cart = Session["cart"] as Dictionary<string, CartItem>;
            if(_cart.Count == 0)
                return RedirectToAction("Index", "Home");
            BIContext _context = new BIContext();

            var _customer = new Customer(); ;
            if (User.Identity.IsAuthenticated)
                _customer = _context.Customers.FirstOrDefault(c => c.Email.Equals(User.Identity.Name));
            
            ViewBag.Customer = _customer;
            ViewBag.Cart = _cart;

            List<SelectListItem> _countries = GetCountries();

            object _country = Session["ShippingCountry"];
            if (_country == null)
            {
                _countries.First(c => c.Value.Equals("GB")).Selected = true;
                Session["ShippingCountry"] = "GB";
            }
            else
                _countries.First(c => c.Value.Equals(Session["ShippingCountry"])).Selected = true;

            ViewBag.Countries = _countries;

            return View();
        }

        [HttpPost]
        public ActionResult ProccessTransaction(FormCollection data)
        {
            


             return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cart(FormCollection data)
        {
            var _cart = Session["cart"] as Dictionary<string, CartItem>;
            if (_cart == null)
            {
                _cart = new Dictionary<string, CartItem>();
                var _item = GetCartItem(data);
                _cart.Add(_item.Id, _item);
            }
            else
            {
                var _item = GetCartItem(data);
                if (!_cart.ContainsKey(_item.Id))
                {
                    _cart.Add(_item.Id, _item);
                }

                foreach (var _product in _cart)
                    update(_product.Key, _product.Value.Qty, "GB", (data.AllKeys.Contains("couponCode") ? data["couponCode"] : ""));
                        
            }

            Session["cart"] = _cart;

            ViewBag.SubTotal = GetSubTotal();
            ViewBag.Total = GetTotal();
            ViewBag.Shipping = GetShipping();

            Session["HeardUsFrom"] = data["how_did_you"];
            
            ViewBag.Countries = GetCountries();
            ViewBag.how_did_you = GetHowDidYouHearUs();

            return View(_cart);
        }

        public ActionResult CartUpdate(FormCollection data)
        {
            var _cart = Session["cart"] as Dictionary<string, CartItem>;
            foreach (var _item in _cart)
            {
                update(_item.Key, int.Parse(data["qty_" + _item.Key]), data["country"], (data.AllKeys.Contains("couponCode") ? data["couponCode"]: ""));
            }

            ViewBag.SubTotal = GetSubTotal();
            ViewBag.Total = GetTotal();
            ViewBag.Shipping = GetShipping();
            ViewBag.ShippingCountry = data["country"];
            Session["ShippingCountry"] = data["country"];
            Session["HeardUsFrom"] = data["how_did_you"];

            List<SelectListItem> _countries = GetCountries();
            
            _countries.First(c => c.Value.Equals(data["country"])).Selected = true;

            ViewBag.Countries = _countries;


            List<SelectListItem> _options = ViewBag.how_did_you = GetHowDidYouHearUs();

            _options.First(c => c.Value.Equals(data["how_did_you"])).Selected = true;


            ViewBag.how_did_you = _options;

            return View("Cart", (Dictionary<string, CartItem>)Session["cart"]);

        }

        private CartItem GetCartItem(FormCollection data)
        {
            string _id = string.Empty;

            _id = data["product"].Split(new char[] { '|' })[0];
            _id += data["size"].Split(new char[] { '|' })[0];


            CartItem _item = new CartItem { Type = _id, Description = data["product"].Split(new char[] { '|' })[1] };
            _item.Price = double.Parse(data["size"].Split(new char[] { '|' })[0]);
            _item.Description = string.Format("{0} ({1})", _item.Description, data["size"].Split(new char[] { '|' })[1]);

            if (_item.Description.Contains("Primi"))
            {
                _item.Shipping = 7;
                _item.EUShipping = 21;
            }
            else if (_item.Type.StartsWith("04") || _item.Type.StartsWith("06"))
            {
                _item.Shipping = 0;
                _item.EUShipping = 0;
            }
            else if (_item.Type.StartsWith("05"))
            {
                _item.Shipping = 11;
                _item.EUShipping = 11;
            }
            else if (_item.Type.StartsWith("08"))
            {
                _item.Shipping = 4;
                _item.EUShipping = 4;
            }
            else if (_item.Type.StartsWith("07"))
            {
                _item.Shipping = 20;
                _item.EUShipping = 20;
            }
            else
            {
                _item.Shipping = 10;
                _item.EUShipping = 25;
            }

            if (_item.CouponCode == null && data.AllKeys.Contains("couponCode") && data["couponCode"].Equals("BR001"))
            {
                if (!_item.Type.StartsWith("05") && !_item.Type.StartsWith("06"))
                {
                    _item.Price = _item.Price - 10;
                    if (_item.Type.StartsWith("04"))
                    {
                        _item.Price = _item.Price - 10;
                    }

                    _item.CouponCode = data["couponCode"];
                }

            }
            
            if (data.AllKeys.Contains("color"))
            {
                _id += data["color"].Split(new char[] { '|' })[1];
                _item.Description = string.Format("{0} + {1}", _item.Description, data["color"].Split(new char[] { '|' })[1]);
            }

            if (data.AllKeys.Contains("addon") && !data["addon"].Equals(""))
            {
                _id += data["addon"].Split(new char[] { '|' })[0]; 
                _item.Price += double.Parse(data["addon"].Split(new char[] { '|' })[0]);
                _item.Description = string.Format("{0} + {1}", _item.Description, data["addon"].Split(new char[] { '|' })[1]);
            }

            if (data.AllKeys.Contains("addon2") && !data["addon2"].Equals(""))
            {
                _id += data["addon2"].Split(new char[] { '|' })[0];
                _item.Price += double.Parse(data["addon2"].Split(new char[] { '|' })[0]);
                _item.Description = string.Format("{0} + {1}", _item.Description, data["addon2"].Split(new char[] { '|' })[1]);

                if (_item.Description.Contains("Top Shelf"))
                {
                    _item.Shipping = 14;
                    _item.EUShipping = 30;
                }
            }

            if (data.AllKeys.Contains("addon3") && !data["addon3"].Equals(""))
            {
                _id += data["addon3"].Split(new char[] { '|' })[0];
                _item.Price += double.Parse(data["addon3"].Split(new char[] { '|' })[0]);
                _item.Description = string.Format("{0} + {1}", _item.Description, data["addon3"].Split(new char[] { '|' })[1]);
            }

            if (data.AllKeys.Contains("param1") && !data["param1"].Equals(""))
            {
                _id += data["param1"].Split(new char[] { '|' })[0]; 
                _item.Price += double.Parse(data["param1"].Split(new char[] { '|' })[0]);
                _item.Description = string.Format("{0} + {1}", _item.Description, data["param1"].Split(new char[] { '|' })[1]);


                if (_item.Description.Contains("2 Side Shelves"))
                {
                    _item.Shipping += 4;
                    _item.EUShipping += 4;
                }

                if (_item.Description.Contains("3 Side Shelves"))
                {
                    _item.Shipping += 8;
                    _item.EUShipping += 8;
                }
                
            }

            if (data.AllKeys.Contains("param2") && !data["param2"].Equals(""))
            {
                _id += data["param2"].Split(new char[] { '|' })[0]; 
                _item.Price += double.Parse(data["param2"].Split(new char[] { '|' })[0]);
                _item.Description = string.Format("{0} + {1}", _item.Description, data["param2"].Split(new char[] { '|' })[1]);
            }

            if (data.AllKeys.Contains("param3") && !data["param3"].Equals(""))
            {
                _id += data["param3"].Split(new char[] { '|' })[0];
                _item.Price += double.Parse(data["param3"].Split(new char[] { '|' })[0]);
                _item.Description = string.Format("{0} + {1}", _item.Description, data["param3"].Split(new char[] { '|' })[1]);
            }

            if (data.AllKeys.Contains("items1") && !data["items1"].Equals(""))
            {
                _id += data["items1"];
                _item.Price += double.Parse(data["items1"]) * 30;
                _item.Shipping += double.Parse(data["items1"]) * 4;
                _item.EUShipping += double.Parse(data["items1"]) * 5;
                _item.Description = string.Format("{0} + {1}", _item.Description,  data["items1"] + " Side Shelves");
            }

            if (data.AllKeys.Contains("qty") && !data["qty"].Equals(""))
            {
                _item.Qty = int.Parse(data["qty"]);
            }

            _item.Total = _item.Price * _item.Qty;
            _item.ShippingTotal = _item.Shipping * _item.Qty;
            //_item.ShippingTotal += _item.Shipping * _item.Qty;

            _item.Id = _id;

            return _item;
        }

        public ActionResult Delete(string id)
        {
            var _cart = Session["cart"] as Dictionary<string, CartItem>;

            _cart.Remove(id);

            Session["cart"] = _cart;
            if(_cart.Count == 0)
                return RedirectToAction("Index", "Home");
            return RedirectToAction("Cart");
        }

        private double GetShipping()
        {
            var _cart = Session["cart"] as Dictionary<string, CartItem>;
            double _total = 0;
            foreach (var _item in _cart)
                _total += _item.Value.ShippingTotal;
            return _total;
        }

        private double GetSubTotal()
        {
            var _cart = Session["cart"] as Dictionary<string, CartItem>;
            double _total = 0;
            foreach (var _item in _cart)
                _total += _item.Value.Total;
            return _total;
        }

        private double GetTotal()
        {
            var _cart = Session["cart"] as Dictionary<string, CartItem>;
            double _total = 0;
            foreach (var _item in _cart)
                _total += _item.Value.Total + _item.Value.ShippingTotal;
            return _total;
        }

        private void update(string item, int qty, string country,string couponCode)
        {
            var _cart = Session["cart"] as Dictionary<string, CartItem>;

            var _item = _cart[item];
            _item.Qty = qty;

            if (couponCode.Equals("BR001") && _item.CouponCode == null)
            {
                if (!_item.Type.StartsWith("05") && !_item.Type.StartsWith("06"))
                {
                    if (_item.Type.StartsWith("04"))
                    {
                        _item.Price = _item.Price - 20;

                    }
                    else
                    {
                        _item.Price = _item.Price - 10;
                    }

                    _item.CouponCode = couponCode;
                }
                
            }
           
             
            _item.Total = _item.Price * qty;
            if (country.Equals("GB"))
            {
                _item.ShippingTotal = _item.Shipping * qty;
            }
            else
            {
                _item.ShippingTotal = _item.EUShipping * qty;
            }
            Session["cart"] = _cart;
        }

        

        public ActionResult Notification(FormCollection data)
        {

            return View();
        }

        public JsonResult UpdateHearAboutUs(string from)
        {
            Session["HeardUsFrom"] = from;
            return new JsonResult();
        }




    }
}
