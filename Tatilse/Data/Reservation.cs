using System.ComponentModel.DataAnnotations;

namespace Tatilse.Data
{
    public class Reservation
    {
        [Key]
        [Display(Name = "Rezervasyon ID")]
        public int reservation_id{ get; set; }
        
        [Display(Name = "Giriş Tarihi")]
        public DateOnly start_date { get; set; }  //bunları sonrasında dateonly olarak değiştir

        [Display(Name = "Çıkış Tarihi")]
        public DateOnly end_date { get; set; }

        [Display(Name = "Müşteri No")]
        public int client_id { get; set; }
        [Display(Name = "Giriş Tarihi")]
        public int room_id { get; set; } 
    } 
}
