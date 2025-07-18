using System.ComponentModel.DataAnnotations;

namespace Tatilse.Data
{
    public class Client
    {
        [Key]
        [Display(Name = "Müşteri Id")]
        public int client_id { get; set; }

        [MaxLength(50, ErrorMessage = "HATA!")]

        [Display(Name = "Ad")]
        public string client_name { get; set; }

        [Display(Name = "Soyad")]

        public string NameSurname
        {
            get
            {
                return this.client_name + " " + this.client_surname;
            }
        }
        public string client_surname { get; set; }

        [Display(Name = "Doğum Tarihi")]
        public string client_birthdate { get; set; }

        [Display(Name = "Kimlik Numarası")]
        public string client_identity { get; set; }

        [Display(Name = "Telefon Numarası")]
        public string client_phone { get; set; }

        [Display(Name = "Email Adresi")]
        public string client_email { get; set; }
    }
}
