using System.ComponentModel.DataAnnotations;

namespace Tatilse.Models
{
    public class HotelEditDTO
    {
        public int hotel_id { get; set; }

        [Display(Name = "Hotel Ad")]
        public string hotel_name { get; set; }

        [Display(Name = "Fiyat")]
        public decimal hotel_price { get; set; }


        [Display(Name = "Şehir")]
        public string hotel_city { get; set; }

        [Display(Name = "İlçe")]
        public string hotel_township { get; set; }

        [Display(Name = "Açıklama")]
        public string hotel_description{ get; set; }



        [Display(Name = "Görsel")]
        public IFormFile? hotel_image { get; set; }
    }
}
