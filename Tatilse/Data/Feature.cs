using System.ComponentModel.DataAnnotations;

namespace Tatilse.Data
{
    public class Feature
    {
        [Key]
        [Required]

        [Display(Name = "Özellik ID")]
        public byte feature_id { get; set; }

        [Required]
        [Display(Name = "Özellik Adı")]
        public string feature_name { get; set; }

        [Display(Name = "Özellik Simgesi")]
        public string feature_image { get; set; }
    }
}
