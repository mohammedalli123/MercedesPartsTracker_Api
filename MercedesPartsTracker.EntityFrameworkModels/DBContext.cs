using MercedesPartsTracker.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using System;


namespace MercedesPartsTracker.EntityFramework
{
    public class DBContext : DbContext
    {
        public virtual DbSet<Part> Parts { get; set; }

        public DBContext(DbContextOptions<DBContext> options)
       : base(options) { }
    }
}
