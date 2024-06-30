// Fornecedor.cs
namespace loja.models
{
    public class Fornecedor
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string CNPJ { get; set; }
        public required string Email { get; set; }
    }
}
