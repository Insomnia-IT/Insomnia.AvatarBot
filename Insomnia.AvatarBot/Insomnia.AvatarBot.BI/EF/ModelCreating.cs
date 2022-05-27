using Insomnia.AvatarBot.Data.Attributes;
using Insomnia.AvatarBot.Data.Entity;
using Insomnia.AvatarBot.General.Expansions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insomnia.AvatarBot.EF
{
    public partial class ServiceDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
