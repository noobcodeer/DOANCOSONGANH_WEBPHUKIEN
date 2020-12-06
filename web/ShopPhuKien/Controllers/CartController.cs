﻿using ShopPhuKien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ShopPhuKien.Controllers
{
    public class CartController : Controller
    {
        // khởi tạo session:
        private const string SessionCart = "SessionCart";
        // GET: Cart
        ShopPhuKienDbContext db = new ShopPhuKienDbContext();
        public ActionResult Index()
        {
            var cart = Session[SessionCart];
            var list = new List<Cart_item>();
            if (cart != null)
            {
                list = (List<Cart_item>)cart;
            }
            return View(list);
        }
        public ActionResult card_header()
        {
            var cart = Session[SessionCart];
            var list = new List<Cart_item>();
            float priceTotol = 0;
            if (cart != null)
            {
                list = (List<Cart_item>)cart;
                foreach (var item1 in list)
                {
                    if (item1.product.pricesale > 0)
                    {
                        int temp = (((int)item1.product.price) - ((int)item1.product.price / 100 * (int)item1.product.pricesale)) * ((int)item1.quantity);

                        priceTotol += temp;
                    }
                    else
                    {
                        int temp = (int)item1.product.price * (int)item1.quantity;
                        priceTotol += temp;
                    }

                }
            }
            ViewBag.CartTotal = priceTotol;
            return View(list);
        }
        public RedirectToRouteResult updateitem(long P_SanPhamID, int P_quantity)
        {
            var cart = Session[SessionCart];
            var list = (List<Cart_item>)cart;
            Cart_item itemSua = list.FirstOrDefault(m => m.product.ID == P_SanPhamID);
            if (itemSua != null)
            {
                itemSua.quantity = P_quantity;
            }
            return RedirectToAction("Index");
        }
        public RedirectToRouteResult deleteitem(long productID)
        {
            var cart = Session[SessionCart];
            var list = (List<Cart_item>)cart;

            Cart_item itemXoa = list.FirstOrDefault(m => m.product.ID == productID);
            if (itemXoa != null)
            {
                list.Remove(itemXoa);
                if (list.Count == 0)
                {
                    Session["SessionCart"] = null;
                }
            }
            return RedirectToAction("Index");
        }
        public JsonResult Additem(long productID, int quantity)
        {
            var item = new Cart_item();
            Mproduct product = db.Products.Find(productID);
            var cart = Session[SessionCart];
            if (cart != null)
            {
                var list = (List<Cart_item>)cart;
                if (list.Exists(m => m.product.ID == productID))
                {
                    int quantity1 = 0;
                    bool bad = false;
                    foreach (var item1 in list)
                    {
                        if (item1.product.ID == productID)
                        {
                            if ((item1.quantity + quantity) > (item1.product.number - item1.product.sold))
                            {
                                 bad = true;
                            }
                            else {
                                item1.quantity += quantity;
                                quantity1 = item1.quantity;
                            }
                           
                        }
                    }
                    int priceTotol = 0;
                    
                    int price = 0;
                    foreach (var item1 in list)
                    {
                        if (item1.product.pricesale > 0)
                        {
                            int temp = (((int)item1.product.price) - ((int)item1.product.price / 100 * (int)item1.product.pricesale)) * ((int)item1.quantity);
                           
                            priceTotol += temp;
                        }
                        else {
                            int temp = (int)item1.product.price * (int)item1.quantity;
                            priceTotol += temp;
                        }
                     
                    }
                    return Json(new
                    {  
                        ProductPrice = ((int)product.price) - (((int)product.price / 100) * ((int)product.pricesale)),
                        bad = bad,
                        price = product.price,
                        priceSale = product.pricesale,
                        quantity = quantity1,
                        priceTotol = priceTotol,
                        productID = productID,
                        meThod = "updateQuantity"
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    item.product = product;
                    item.quantity = quantity;
                    list.Add(item);
                    item.countCart = list.Count();
                    item.meThod = "cartExist";
                    int priceTotol = 0;
                    foreach (var item1 in list)
                    {
                        if (item1.product.pricesale > 0)
                        {
                            int temp = (((int)item1.product.price) - ((int)item1.product.price / 100 * (int)item1.product.pricesale)) * ((int)item1.quantity);
                            priceTotol += temp;

                        }
                        else
                        {
                            int temp = (int)item1.product.price * (int)item1.quantity;
                            priceTotol += temp;
                        }
                    }
                    item.priceTotal = priceTotol;
                    item.priceSaleee = (int)product.price - (int)product.price / 100 * (int)product.pricesale;
                        return Json(item, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {               
                item.product = product;
                item.quantity = quantity;
                item.meThod = "cartEmpty";              
                item.countCart = 1;
                if (item.product.pricesale > 0)
                {
                    item.priceTotal = (((int)item.product.price) - ((int)item.product.price / 100 * (int)item.product.pricesale)) * ((int)item.quantity);
                }
                else
                {
                    item.priceTotal = (int)product.price;
                }
                item.priceTotal = (((int)item.product.price) - ((int)item.product.price / 100 * (int)item.product.pricesale)) * ((int)item.quantity);
                var list = new List<Cart_item>();
                list.Add(item);
                Session[SessionCart] = list;
               
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }
    }
}