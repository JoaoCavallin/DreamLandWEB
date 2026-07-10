namespace DreamLandWEB.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; } // hash, claro
        public string? Telefone { get; set; }
        public bool Admin { get; set; } = false;

        public ICollection<Pedido>? Pedidos { get; set; }
    }
}
