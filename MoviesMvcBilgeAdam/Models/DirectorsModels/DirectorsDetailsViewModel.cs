using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoviesMvcBilgeAdam.Models.DirectorsModels
{
    public class DirectorsDetailsViewModel
    {
        public DirectorsModel Director { get; set; }
        public string Title { get; set; } // const kullanımı için örnek
    }
}