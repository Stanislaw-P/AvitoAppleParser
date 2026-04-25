using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvitoAppleParser.Db.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = string.Empty; // ID объявления с Avito
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string City { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty; // Ссылка на само объявление
        public DateTime CreatedAt { get; set; } // Когда добавлено в парсер
        public bool IsActive { get; set; }
    }
}
