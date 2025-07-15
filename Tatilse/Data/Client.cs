using System.ComponentModel.DataAnnotations;

namespace Tatilse.Data
{
    public class Client
    {
        [Key]
        public int client_id { get; set; }

        public string client_name { get; set; }

        public string client_surname { get; set; }

        public string client_birthdate { get; set; }

        public string client_identity { get; set; } 

        public string client_phone { get; set; }

        public string client_email { get; set; }
    }
}
