using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StorageV1.Models;

namespace StorageV1.Data
{
    public class StorageV1Context : DbContext
    {
        public StorageV1Context (DbContextOptions<StorageV1Context> options)
            : base(options)
        {
        }

        public DbSet<StorageV1.Models.Product> Product { get; set; } = default!;
    }
}
