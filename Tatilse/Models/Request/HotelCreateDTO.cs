using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tatilse.Models
{
    public class HotelCreateDTO
    {
        [Required]
        [Display(Name = "Hotel Ad")]
        public string hotel_name { get; set; }

        [Required]
        [Display(Name = "Fiyat")]
        public decimal hotel_price { get; set; }

        [Required]
        [Display(Name = "Şehir")]
        public string hotel_city { get; set; }

        [Required]
        [Display(Name = "İlçe")]
        public string hotel_township { get; set; }

        [Required]
        [Display(Name = "Açıklama")]
        public string hotel_description { get; set; }

        [Display(Name = "Hotel Fotoğrafı")]
        public IFormFile? hotel_image { get; set; }

        [Display(Name = "Özellikler")]
        public int[] SelectedFeatureIds { get; set; } = new int[0];
    }
}
