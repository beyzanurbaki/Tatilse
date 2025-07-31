using System.ComponentModel.DataAnnotations;

namespace Tatilse.Models.Request
{
    public class ReservationCreateDTO
    {

        [Required]
        public DateTime start_date { get; set; }

        [Required]
        public DateTime end_date { get; set; }

        [Required]
        public int room_id { get; set; }
    }

    public class ReservationCreatePageParametersDTO
    {
        public int guestCount { get; set; }

        public DateTime startdate { get; set; }

        public DateTime enddate { get; set; }

        public int roomid { get; set; }
    }
}
