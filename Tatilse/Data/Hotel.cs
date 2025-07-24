using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatilse.Data
{
    public class Hotel
    {

        public Hotel() { }
        public Hotel(int hotel_id)
        {
            this.hotel_id = hotel_id;
        }

        [Key]
        public int hotel_id { get; set; }

        [Display(Name = "Hotel Ad")]
        public string hotel_name { get; set; }

        [Display(Name = "Fiyat")]
        public decimal hotel_price { get; set; }

        [Display(Name = "Açıklama")]
        public string hotel_description { get; set; }

        [Display(Name = "İl")]
        public string hotel_city { get; set; }

        [Display(Name ="İlçe")]
        public string hotel_township { get; set; }
        [Display(Name = "Görsel")]
        public string hotel_image { get; set; }

        public ICollection<Room> rooms { get; set; } = new List<Room>();  //bir otelde birden fazla oda bulunabilir
        public ICollection<Feature> features { get; set; } = new List<Feature>();

        [NotMapped]
        public List<int> SelectedFeatureIds { get; set; }


    }
}
