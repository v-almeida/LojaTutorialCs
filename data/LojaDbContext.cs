using Microsoft.EntityFrameworkCore;

namespace loja.data{

    public class LojaDbContext : DbContext{

        public LojaDbContext(DbContextOptions<LojaDbContext> options) : base(options){}

        publicDbSet<ProducesResponseTypeMetadata> Produtos {get; set;}
    }

    internal class publicDbSet<T>
    {
    }
}
