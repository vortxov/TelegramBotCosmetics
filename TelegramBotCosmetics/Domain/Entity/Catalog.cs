using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotCosmetics.Domain.Entity
{
    public class Catalog
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Href { get; set; }
        public string People { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
        public TypeCatalog Type { get; set; }
    }
}
