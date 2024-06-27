using loja.models;
using Microsoft.EntityFrameworkCore;

namespace loja.data
{
    public class LojaDbContext : DbContext
    {
        public LojaDbContext(DbContextOptions<LojaDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; } // Correct DbSet<> declaration

        public DbSet<Cliente>Clientes{get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add any additional configuration here if needed
            base.OnModelCreating(modelBuilder);
        }
    }
}

