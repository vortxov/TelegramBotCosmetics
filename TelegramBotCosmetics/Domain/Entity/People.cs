using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotCosmetics.Domain.Entity
{
    public class People
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<TypeCatalog> TypeCatalogs { get; set; }
        public string DataP { get; set; }
    }
}
