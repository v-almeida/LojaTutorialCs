using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using loja.data;
using loja.models;

namespace loja.services
{
    public class ClientService
    {
        private readonly LojaDbContext _dbContext;

        public ClientService(LojaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Método para consultar todos os clientes
        public async Task<List<Cliente>> GetAllClientsAsync()
        {
            return await _dbContext.Clientes.ToListAsync();
        }

        // Método para consultar um cliente a partir do seu Id
        public async Task<Cliente?> GetClientByIdAsync(int id)
        {
            return await _dbContext.Clientes.FindAsync(id);
        }

        // Método para gravar um novo cliente
        public async Task AddClientAsync(Cliente cliente)
        {
            _dbContext.Clientes.Add(cliente);
            await _dbContext.SaveChangesAsync();
        }

        // Método para atualizar os dados de um cliente
        public async Task UpdateClientAsync(Cliente cliente)
        {
            _dbContext.Entry(cliente).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        // Método para excluir um cliente
        public async Task DeleteClientAsync(int id)
        {
            var cliente = await _dbContext.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _dbContext.Clientes.Remove(cliente);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}

