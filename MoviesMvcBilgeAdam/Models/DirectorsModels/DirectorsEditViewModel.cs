using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoviesMvcBilgeAdam.Entities;

namespace MoviesMvcBilgeAdam.Models.DirectorsModels
{
    public class DirectorsEditViewModel
    {
        public Directors Directors { get; set; }

        [DisplayName("Movies")]
        public MultiSelectList MoviesMultiSelectList { get; set; }
    }
}