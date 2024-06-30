// Services/UsuarioService.cs

using loja.data;
using loja.models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace loja.services
{
    public class UsuarioService
    {
        private readonly LojaDbContext _dbContext;

        public UsuarioService(LojaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Usuario>> GetAllUsuariosAsync()
        {
            return await _dbContext.Usuarios.ToListAsync();
        }

        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            return await _dbContext.Usuarios.FindAsync(id);
        }

        public async Task AddUsuarioAsync(Usuario usuario)
        {
            _dbContext.Usuarios.Add(usuario);
            await _dbContext.SaveChangesAsync();
        }

        internal async Task UpdateUsuarioAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        internal async Task DeleteUsuarioAsync(int id)
        {
            throw new NotImplementedException();
        }

        // Métodos adicionais conforme necessário (Update, Delete, etc.)
    }
}



