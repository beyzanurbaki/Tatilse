using System.ComponentModel.DataAnnotations;

namespace Tatilse.Models.Request {
    public class ReservationEditDTO
    {
        public int reservation_id { get; set; }

        [Required]
        public DateTime? start_date { get; set; }

        [Required]
        public DateTime? end_date { get; set; }

        [Required]
        public int client_id { get; set; }

        [Required]
        public int room_id { get; set; }
    }
}
