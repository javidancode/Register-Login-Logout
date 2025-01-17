﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity_Frame_Work_Project.ViewModels
{
    public class BasketDetailVM
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public int Count { get; set; }
    }
}
