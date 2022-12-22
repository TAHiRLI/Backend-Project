﻿using Microsoft.EntityFrameworkCore;
using Quarter.Models;

namespace Quarter.DAL
{
    public class QuarterDbContext:DbContext
    {
        public QuarterDbContext(DbContextOptions<QuarterDbContext> options):base(options)
        {

        }
        public DbSet<City> Cities { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Owner>Owners { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<HouseImage> HousesImages { get; set; }
        public DbSet<HomeSlider> HomeSliders { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<HouseAmenity> HouseAmenities { get; set; }
        public DbSet<Service> Services { get; set; } 


    }
}
