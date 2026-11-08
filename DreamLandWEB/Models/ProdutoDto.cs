using DreamLandWEB.Enums;

namespace DreamLandWEB.Models
{
    public record ProdutoDto(
        int Id,
        string Nome,
        string? Descricao,
        CategoriaProduto Categoria,
        decimal Preco,
        string Tamanho,
        CondicaoProduto Condicao,
        int Estoque,
        string? ImagemUrl,
        string? Marca,
        string? FornecedorNome);
}