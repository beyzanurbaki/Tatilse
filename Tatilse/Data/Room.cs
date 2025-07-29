using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatilse.Data
{
    public class Room
    {
        [Key]

        [Display(Name = ("Oda id"))]
        public int room_id { get; set; }

        [Display(Name = ("Oda adı"))]
        public string room_name { get; set; }

        [Display(Name = ("Oda fiyatı"))]
        public decimal room_price { get; set; }

        [Display(Name = ("Oda miktarı"))]
        public short room_quantity { get; set; }

        [Display(Name = ("Oda büyüklüğü"))]
        public short room_capacity { get; set; }

        [Display(Name = ("Odada kalacak maksimum kişi sayısı"))]
        public short room_max_people { get; set; }

        [Display(Name = ("Oda görsel"))]
        public string? room_image { get; set; }

        //  [ForeignKey("hotel_id")] // yabancı anahtar old için
        [ForeignKey(nameof(hotel_id))]
        public Hotel hotel { get; set; } = null!;
        [Display(Name = ("Hotel id"))]
        public int hotel_id { get; set; } //bir oda sadece bir otele ait olabilir

        public ICollection<Reservation> reservations { get; set; }
    }
}
