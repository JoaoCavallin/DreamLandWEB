using DreamLandWEB.Data;
using DreamLandWEB.Helpers;
using DreamLandWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamLandWEB.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CarrinhoSessionKey = "Carrinho";

        public CarrinhoController(ApplicationDbContext context)
        {
            _context = context;
        }

        private List<CarrinhoItem> ObterCarrinho()
        {
            return HttpContext.Session.GetObject<List<CarrinhoItem>>(CarrinhoSessionKey)
                   ?? new List<CarrinhoItem>();
        }

        private void SalvarCarrinho(List<CarrinhoItem> carrinho)
        {
            HttpContext.Session.SetObject(CarrinhoSessionKey, carrinho);
        }

        // GET: /Carrinho
        public IActionResult Index()
        {
            var carrinho = ObterCarrinho();
            ViewBag.Total = carrinho.Sum(i => i.Subtotal);
            return View(carrinho);
        }

        // POST: /Carrinho/Adicionar
        [HttpPost]
        public async Task<IActionResult> Adicionar(int produtoId, int quantidade = 1)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);
            if (produto == null || !produto.Disponivel || produto.Estoque < quantidade)
            {
                TempData["Erro"] = "Produto indisponível na quantidade solicitada.";
                return RedirectToAction("Details", "Produtos", new { id = produtoId });
            }

            var carrinho = ObterCarrinho();
            var itemExistente = carrinho.FirstOrDefault(i => i.ProdutoId == produtoId);

            if (itemExistente != null)
            {
                itemExistente.Quantidade += quantidade;
            }
            else
            {
                carrinho.Add(new CarrinhoItem
                {
                    ProdutoId = produto.Id,
                    Nome = produto.Nome,
                    Preco = produto.Preco,
                    ImagemUrl = produto.ImagemUrl,
                    Tamanho = produto.Tamanho,
                    Quantidade = quantidade
                });
            }

            SalvarCarrinho(carrinho);
            TempData["Sucesso"] = "Produto adicionado ao carrinho!";
            return RedirectToAction("Index");
        }

        // POST: /Carrinho/Remover
        [HttpPost]
        public IActionResult Remover(int produtoId)
        {
            var carrinho = ObterCarrinho();
            var item = carrinho.FirstOrDefault(i => i.ProdutoId == produtoId);
            if (item != null)
            {
                carrinho.Remove(item);
                SalvarCarrinho(carrinho);
            }
            return RedirectToAction("Index");
        }

        // POST: /Carrinho/AtualizarQuantidade
        [HttpPost]
        public IActionResult AtualizarQuantidade(int produtoId, int quantidade)
        {
            var carrinho = ObterCarrinho();
            var item = carrinho.FirstOrDefault(i => i.ProdutoId == produtoId);
            if (item != null && quantidade > 0)
            {
                item.Quantidade = quantidade;
                SalvarCarrinho(carrinho);
            }
            return RedirectToAction("Index");
        }
    }
}