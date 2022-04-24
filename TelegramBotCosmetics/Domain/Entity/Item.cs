using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotCosmetics.Domain.Entity
{
    public class Item
    {
        public Guid Id { get; set; }
        public string ItemName { get; set; }
        public string IdItemOnPage { get; set; }
        public string? People { get; set; } //TODO:Изменить поиск по людям
        public string? Type { get; set; }
        public string Price { get; set; }
        public string Href { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public string? Brend { get; set; }
        public List<Catalog> Catalogs { get; set; } = new List<Catalog>();
        public List<Formula> Formulas { get; set; } = new List<Formula>();
        public Tag? Tag { get; set; }
    }
}
