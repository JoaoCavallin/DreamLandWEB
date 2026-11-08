using DreamLandWEB.Data;
using DreamLandWEB.Enums;
using DreamLandWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamLandWEB.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProdutosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Listar(CategoriaProduto? categoria, CondicaoProduto? condicao, string? marca, string? busca)
        {
            var query = _context.Produtos.Where(p => p.Disponivel && p.Estoque > 0);

            if (categoria.HasValue) query = query.Where(p => p.Categoria == categoria.Value);
            if (condicao.HasValue) query = query.Where(p => p.Condicao == condicao.Value);
            if (!string.IsNullOrWhiteSpace(marca)) query = query.Where(p => p.Marca == marca);
            if (!string.IsNullOrWhiteSpace(busca)) query = query.Where(p => p.Nome.Contains(busca));
            if (categoria.HasValue) query = query.Where(p => p.Categoria == categoria.Value);
            if (condicao.HasValue) query = query.Where(p => p.Condicao == condicao.Value);
            if (!string.IsNullOrWhiteSpace(marca)) query = query.Where(p => p.Marca == marca);

            var lista = await query
                .OrderByDescending(p => p.DataCadastro)
                .Select(p => new ProdutoDto(
                    p.Id, p.Nome, p.Descricao, p.Categoria, p.Preco, p.Tamanho,
                    p.Condicao, p.Estoque, p.ImagemUrl, p.Marca,
                    p.Fornecedor != null ? p.Fornecedor.Nome : null))
                .ToListAsync();

            return Ok(lista);
        }

        [HttpGet("marcas")]
        public async Task<IActionResult> Marcas()
        {
            var marcas = await _context.Produtos
                .Where(p => p.Disponivel && p.Estoque > 0 && !string.IsNullOrEmpty(p.Marca))
                .Select(p => p.Marca!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();

            return Ok(marcas);
        }
    }
}