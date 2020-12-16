using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MoviesMvcBilgeAdam.Models.DirectorsModels
{
    public class DirectorsModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Director Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Director Surname")]
        public string Surname { get; set; }

        [DisplayName("Is Director Retired?")]
        public string RetiredText { get; set; }
    }
}