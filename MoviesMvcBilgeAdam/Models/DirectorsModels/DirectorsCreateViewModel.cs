using MoviesMvcBilgeAdam.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoviesMvcBilgeAdam.Models.DirectorsModels
{
    public class DirectorsCreateViewModel
    {
        public Directors DirectorEntity { get; set; }

        [DisplayName ("Movie Name")]
        public MultiSelectList MoviesMultiSelectList { get; set; }
        public List<int> MovieIds { get; set; }
    }
}