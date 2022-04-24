using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotCosmetics.Domain.Entity
{
    public class TypeCatalog
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<People> Peoples { get; set; }
    }
}
