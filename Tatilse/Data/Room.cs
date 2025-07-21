using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatilse.Data
{
    public class Room
    {
        [Key]
        public int room_id { get; set; }

        public string room_name { get; set; }

        public decimal room_price { get; set; }
        public short room_quantity { get; set; }
        public short room_capacity { get; set; }

        public short room_max_people { get; set; }

        public string room_image { get; set; }

      //  [ForeignKey("hotel_id")] // yabancı anahtar old için
        [ForeignKey(nameof(hotel_id))]
        public Hotel hotel { get; set; } = null!;
        public int hotel_id { get; set; } //bir oda sadece bir otele ait olabilir

        public ICollection<Reservation> reservations { get; set; }
    }
}
