using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tatilse.Data;

namespace Tatilse.Models { }
public class Reservation
{
    [Key]
    [Display(Name = "Rezervasyon ID")]
    public int reservation_id { get; set; }

    [Display(Name = "Giriş Tarihi")]
    [DataType(DataType.Date)]
    public DateTime? start_date { get; set; }

    [Display(Name = "Çıkış Tarihi")]
    [DataType(DataType.Date)]
    public DateTime? end_date { get; set; }

    [Display(Name = "Müşteri No")]
    public int client_id { get; set; }

    [ForeignKey("client_id")]
    public Client client { get; set; } = null!;

    [Display(Name = "Oda No")]
    public int room_id { get; set; }

    [ForeignKey("room_id")]
    public Room room { get; set; } = null!;
}

