namespace MoviesMvcBilgeAdam.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MovieDirectors
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public int DirectorId { get; set; }

        public virtual Directors Directors { get; set; }

        public virtual Movies Movies { get; set; }
    }
}
