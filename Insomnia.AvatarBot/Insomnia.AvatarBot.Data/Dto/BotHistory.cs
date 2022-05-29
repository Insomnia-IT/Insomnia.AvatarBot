using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insomnia.AvatarBot.Data.Dto
{
    public class BotHistory
    {
        public string UserId { get; set; }

        public int Number { get; set; }

        public string Url { get; set; }

        public DateTime TimeLastCreate { get; set; }
    }
}
