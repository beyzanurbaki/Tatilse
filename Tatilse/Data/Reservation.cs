using System.ComponentModel.DataAnnotations;

namespace Tatilse.Data
{
    public class Reservation
    {
        [Key]
        public int reservation_id{ get; set; }
        
        [Display(Name = "Giriş Tarihi")]
        public string start_date{ get; set; }

        [Display(Name = "Çıkış Tarihi")]
        public string end_date { get; set; }

        public int client_id { get; set; }

        public int room_id { get; set; }
    } 
}
