using System.Security.Claims;
using DreamLandWEB.Data;
using DreamLandWEB.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamLandWEB.Controllers
{
    public record ItemPedidoRequest(int ProdutoId, int Quantidade);
    public record PedidoRequest(List<ItemPedidoRequest> Itens);
    public record PedidoResponse(int Id, decimal Total, string Status);

    [ApiController]
    [Route("api/pedidos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PedidosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PedidosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] PedidoRequest request)
        {
            var idTexto = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("nameid");
            if (!int.TryParse(idTexto, out var usuarioId)) return Unauthorized();

            if (request.Itens == null || request.Itens.Count == 0)
                return BadRequest(new { mensagem = "O carrinho está vazio." });

            // Junta linhas repetidas do mesmo produto
            var itens = request.Itens
                .GroupBy(i => i.ProdutoId)
                .Select(g => new { ProdutoId = g.Key, Quantidade = g.Sum(x => x.Quantidade) })
                .ToList();

            if (itens.Any(i => i.Quantidade <= 0))
                return BadRequest(new { mensagem = "Quantidade inválida." });

            var ids = itens.Select(i => i.ProdutoId).ToList();
            var produtos = await _context.Produtos
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                Data = DateTime.Now,
                Status = "Pendente",
                Itens = new List<ItemPedido>()
            };

            foreach (var item in itens)
            {
                if (!produtos.TryGetValue(item.ProdutoId, out var produto))
                    return Conflict(new { mensagem = "Um dos produtos não existe mais." });

                if (!produto.Disponivel || produto.Estoque < item.Quantidade)
                    return Conflict(new { mensagem = $"O produto '{produto.Nome}' não está mais disponível na quantidade desejada." });

                pedido.Itens.Add(new ItemPedido
                {
                    ProdutoId = produto.Id,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = produto.Preco // preço real, vindo do banco
                });

                produto.Estoque -= item.Quantidade;
                if (produto.Estoque == 0) produto.Disponivel = false;
            }

            pedido.Total = pedido.Itens.Sum(i => i.PrecoUnitario * i.Quantidade);

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return Ok(new PedidoResponse(pedido.Id, pedido.Total, pedido.Status));
        }
    }
}