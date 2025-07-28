using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Tatilse.Models
{
    public class HotelEditDTO
    {
        public int hotel_id { get; set; }

        [Display(Name = "Hotel Ad")]
        [Required]
        public string hotel_name { get; set; }

        [Display(Name = "Fiyat")]
        [Required]
        public decimal hotel_price { get; set; }

        [Display(Name = "Şehir")]
        [Required]
        public string hotel_city { get; set; }

        [Display(Name = "İlçe")]
        [Required]
        public string hotel_township { get; set; }

        [Display(Name = "Açıklama")]
        public string hotel_description { get; set; }

        [Display(Name = "Görsel")]
        public IFormFile? hotel_image { get; set; }

        [Display(Name = "Özellikler")]
        public string[] SelectedFeatureIds { get; set; } = new string[0];
    }
}
