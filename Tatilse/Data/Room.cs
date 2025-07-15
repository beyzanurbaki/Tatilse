using System.ComponentModel.DataAnnotations;

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

        public string room_image { get; set; }

        public int hotel_id { get; set; }
    }
}
