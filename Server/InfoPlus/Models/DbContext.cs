using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InfoPlus.Models
{
    public class DBContext : DbContext
    {
        private readonly IWebHostEnvironment environment;

        public DbSet<Koatuu> Koatuu { get; set; }
        public DBContext(DbContextOptions<DBContext> option, IWebHostEnvironment _environment) : base(option)
        {
            environment = _environment;
            // Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {

                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool) || property.ClrType == typeof(bool))
                        property.SetValueConverter(new BoolToZeroOneConverter<Int16>());
                    else if (property.ClrType == typeof(Nullable<bool>) || property.ClrType == typeof(Nullable<Boolean>))
                        property.SetValueConverter(new BoolToZeroOneConverter<Nullable<Int16>>());
                }
            }
            //
            // Too much data for seeding
            //
          /*  List<Koatuu> items = new List<Koatuu>();
            var path = Path.Combine(environment.WebRootPath, "data.json");
              using (StreamReader r = new StreamReader(path))
              {
                  string json = r.ReadToEnd();
                  items = JsonConvert.DeserializeObject<List<Koatuu>>(json);
              }
              modelBuilder.Entity<Koatuu>().HasData(items);
           

            base.OnModelCreating(modelBuilder);*/
        }


    }
}
