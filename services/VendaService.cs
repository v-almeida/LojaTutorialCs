// Services/VendaService.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using loja.data;
using loja.models;

namespace loja.services
{
    public class VendaService
    {
        private readonly LojaDbContext _context;

        public VendaService(LojaDbContext context)
        {
            _context = context;
        }

        // Gravar uma venda
        public async Task<bool> GravarVendaAsync(Venda venda)
        {
            // Verificar se o cliente existe
            var cliente = await _context.Clientes.FindAsync(venda.ClienteId);
            if (cliente == null)
                throw new ArgumentException("Cliente não encontrado.");

            // Verificar se o produto existe
            var produto = await _context.Produtos.FindAsync(venda.ProdutoId);
            if (produto == null)
                throw new ArgumentException("Produto não encontrado.");

            // Adicionar a venda ao contexto e salvar no banco de dados
            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync();

            return true;
        }

        // Consultar vendas por produto (detalhada)
        public async Task<List<VendaDetalhadaDTO>> ConsultarVendasPorProdutoDetalhadaAsync(int produtoId)
        {
            var vendasDetalhadas = await _context.Vendas
                .Where(v => v.ProdutoId == produtoId)
                .Select(v => new VendaDetalhadaDTO
                {
                    ProdutoNome = v.Produto.Nome,
                    DataVenda = v.DataVenda,
                    VendaId = v.Id,
                    ClienteNome = v.Cliente.Nome,
                    QuantidadeVendida = v.QuantidadeVendida,
                    PrecoVenda = v.PrecoUnitario
                })
                .ToListAsync();

            return vendasDetalhadas;
        }

        // Consultar vendas por produto (sumarizada)
        public async Task<List<VendaSumarizadaDTO>> ConsultarVendasPorProdutoSumarizadaAsync(int produtoId)
        {
            var vendasSumarizadas = await _context.Vendas
                .Where(v => v.ProdutoId == produtoId)
                .GroupBy(v => v.ProdutoId)
                .Select(g => new VendaSumarizadaDTO
                {
                    ProdutoNome = g.FirstOrDefault().Produto.Nome,
                    TotalQuantidadeVendida = g.Sum(v => v.QuantidadeVendida),
                    TotalPrecoVendido = g.Sum(v => v.QuantidadeVendida * v.PrecoUnitario)
                })
                .ToListAsync();

            return vendasSumarizadas;
        }

        // Consultar vendas por cliente (detalhada)
        public async Task<List<VendaDetalhadaDTO>> ConsultarVendasPorClienteDetalhadaAsync(int clienteId)
        {
            var vendasDetalhadas = await _context.Vendas
                .Where(v => v.ClienteId == clienteId)
                .Select(v => new VendaDetalhadaDTO
                {
                    ProdutoNome = v.Produto.Nome,
                    DataVenda = v.DataVenda,
                    VendaId = v.Id,
                    ClienteNome = v.Cliente.Nome,
                    QuantidadeVendida = v.QuantidadeVendida,
                    PrecoVenda = v.PrecoUnitario
                })
                .ToListAsync();

            return vendasDetalhadas;
        }

        // Consultar vendas por cliente (sumarizada)
        public async Task<List<VendaSumarizadaDTO>> ConsultarVendasPorClienteSumarizadaAsync(int clienteId)
        {
            var vendasSumarizadas = await _context.Vendas
                .Where(v => v.ClienteId == clienteId)
                .GroupBy(v => v.ClienteId)
                .Select(g => new VendaSumarizadaDTO
                {
                    ClienteNome = g.FirstOrDefault().Cliente.Nome,
                    TotalPrecoVendido = g.Sum(v => v.QuantidadeVendida * v.PrecoUnitario),
                    ProdutosVendidos = g.Select(v => new ProdutoVendidoDTO
                    {
                        NomeProduto = v.Produto.Nome,
                        QuantidadeVendida = v.QuantidadeVendida
                    }).ToList()
                })
                .ToListAsync();

            return vendasSumarizadas;
        }
    }

    // DTOs para representar os dados de venda
    public class VendaDetalhadaDTO
    {
        public string ProdutoNome { get; set; }
        public DateTime DataVenda { get; set; }
        public int VendaId { get; set; }
        public string ClienteNome { get; set; }
        public int QuantidadeVendida { get; set; }
        public decimal PrecoVenda { get; set; }
    }

    public class VendaSumarizadaDTO
    {
        public string ProdutoNome { get; set; }
        public int TotalQuantidadeVendida { get; set; }
        public decimal TotalPrecoVendido { get; set; }
        public List<ProdutoVendidoDTO> ProdutosVendidos { get; internal set; }
        public string ClienteNome { get; internal set; }
    }

    public class ProdutoVendidoDTO
    {
        public string NomeProduto { get; set; }
        public int QuantidadeVendida { get; set; }
    }
}
