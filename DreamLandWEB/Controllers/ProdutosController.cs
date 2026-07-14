using DreamLandWEB.Data;
using DreamLandWEB.Enums;
using DreamLandWEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using DreamLandWEB.Models.ViewModels;

namespace DreamLandWEB.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProdutosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Produtos
        [Authorize(Policy = "RequerAdmin")]
        public async Task<IActionResult> Index(CategoriaProduto? categoria, CondicaoProduto? condicao, string? busca)
        {
            var produtos = _context.Produtos.AsQueryable(); // sem filtro de Disponivel/Estoque — é visão admin

            if (categoria.HasValue)
                produtos = produtos.Where(p => p.Categoria == categoria.Value);

            if (condicao.HasValue)
                produtos = produtos.Where(p => p.Condicao == condicao.Value);

            if (!string.IsNullOrWhiteSpace(busca))
                produtos = produtos.Where(p => p.Nome.Contains(busca));

            ViewBag.CategoriaSelecionada = categoria;
            ViewBag.CondicaoSelecionada = condicao;
            ViewBag.Busca = busca;

            return View(await produtos.OrderByDescending(p => p.DataCadastro).ToListAsync());
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Produtos/Create
        [Authorize(Policy = "RequerAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produtos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequerAdmin")]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,Categoria,Preco,Tamanho,Estoque,Disponivel,ImagemUrl,FornecedorId")] Produto produto)
        {
            produto.DataCadastro = DateTime.Now;
            produto.Condicao = CondicaoProduto.Novo;

            if (ModelState.IsValid)
            {
                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // GET: Produtos/Edit/5
        [Authorize(Policy = "RequerAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            return View(produto);
        }

        // POST: Produtos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequerAdmin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,Categoria,Preco,Tamanho,Condicao,Estoque,Disponivel,ImagemUrl")] Produto produto)
        {
            if (id != produto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // GET: Produtos/Delete/5
        [Authorize(Policy = "RequerAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequerAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }

        // GET: /Produtos/Doar
        [Authorize]
        public IActionResult Doar()
        {
            return View();
        }

        // POST: /Produtos/Doar
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Doar(DoacaoViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var produto = new Produto
            {
                Nome = model.Nome,
                Descricao = model.Descricao,
                Categoria = model.Categoria,
                Preco = model.Preco,
                Tamanho = model.Tamanho,
                Condicao = model.Condicao,
                ImagemUrl = model.ImagemUrl,
                Estoque = 1, // itens de doação geralmente são peça única
                Disponivel = false, // pendente de aprovação
                DataCadastro = DateTime.Now,
                FornecedorId = usuarioId
            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Produto enviado! Ele ficará disponível na loja após aprovação.";
            return RedirectToAction("MeusProdutos");
        }

        // GET: /Produtos/MeusProdutos — o usuário acompanha o status do que enviou
        [Authorize]
        public async Task<IActionResult> MeusProdutos()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var produtos = await _context.Produtos
                .Where(p => p.FornecedorId == usuarioId)
                .OrderByDescending(p => p.DataCadastro)
                .ToListAsync();

            return View(produtos);
        }
    }
}
