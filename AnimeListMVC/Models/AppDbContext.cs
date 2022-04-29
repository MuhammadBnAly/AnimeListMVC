using AnimeListMVC.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace AnimeListMVC.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Anime> Animes { get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}
