using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotCosmetics.Domain.Entity
{
    public class WhiteFormula
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool V { get; set; }
        public bool L { get; set; }
    }

}
