﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopPhuKien.Models
{
    public class MfavoriteProduct
    {
        public Mproduct favoriteProduct{ get; set; }
        public int status { get; set; }

        public string method { get;set;}

    }
}