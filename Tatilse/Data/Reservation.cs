using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatilse.Data
{
    public class Reservation
    {
        [Key]
        [Display(Name = "Rezervasyon ID")]
        public int reservation_id{ get; set; }
        
        [Display(Name = "Giriş Tarihi")]
        public DateOnly start_date { get; set; } 

        [Display(Name = "Çıkış Tarihi")]
        public DateOnly end_date { get; set; }

        //public Client client { get; set; } = null!;

        [Display(Name = "Müşteri No")]
        public int client_id { get; set; }

        [ForeignKey(nameof(room_id))]
        public Room room { get; set; } = null!;

        [Display(Name = "Giriş Tarihi")]
        public int room_id { get; set; } 
    } 
}
