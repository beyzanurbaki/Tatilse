using System.ComponentModel.DataAnnotations;

namespace Tatilse.Data
{
    public class Client
    {
        [Key]
        [Display(Name = "Müşteri Id")]
        public int client_id { get; set; }


        [Display(Name = "Kullanıcı Adı")]
        [MaxLength(15, ErrorMessage = "15 karakterden fazla şifre tanımlayamazsınız.")]

        public string client_username { get; set; }

        [MaxLength(50, ErrorMessage = "HATA!")]

        [Display(Name = "Ad")]
        public string client_name { get; set; }


        //public string NameSurname
        //{
        //    get
        //    {
        //        return this.client_name + " " + this.client_surname;
        //    }
        //}

        [Display(Name = "Soyad")]
        public string client_surname { get; set; }

        [Display(Name = "Doğum Tarihi")]
        public string client_birthdate { get; set; }

        [Display(Name = "Kimlik Numarası")]
        public string client_identity { get; set; }

        [Display(Name = "Telefon Numarası")]
        public string client_phone { get; set; }

        [Display(Name = "Email Adresi")]
        public string client_email { get; set; }
        [Required]

        [MaxLength(12, ErrorMessage = "12 karakterden fazla şifre tanımlayamazsınız.")]
        public string client_passw { get; set; }

        public bool isAdmin { get; set; }
    }
}
