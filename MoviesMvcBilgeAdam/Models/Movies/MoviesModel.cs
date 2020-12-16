using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MoviesMvcBilgeAdam.Models.Movies
{
    public class MoviesModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(4)]
        [DisplayName("Movie Production Year")]
        public string ProductionYear { get; set; }

        [DisplayName("Movie Box Office Return")]
        public double? BoxOfficeReturn { get; set; }

        [DisplayName("Movie Directors Names")]
        public  string DirectorNames { get; set; }
    }
}