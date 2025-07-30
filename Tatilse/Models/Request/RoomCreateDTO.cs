using System.ComponentModel.DataAnnotations;

namespace Tatilse.Models.Request
{
    public class RoomCreateDTO
    {


        [Required]
        [Display(Name = "Oda adı")]
        public string room_name { get; set; }

        [Required]
        [Display(Name = "Fiyat")]
        public decimal room_price { get; set; }

        [Required]
        [Display(Name = "Oda sayısı")]
        public short room_quantity { get; set; }


        [Required]
        [Display(Name = "Odanın büyüklüğü")]
        public short room_capacity { get; set; }

        [Required]
        [Display(Name = "Odada kalabilecek kişi sayısı")]
        public short room_max_people { get; set; }

        [Required]
        [Display(Name = "Görsel")]
        public IFormFile? room_image { get; set; }

        public int hotel_id { get; set; }

    }
}

