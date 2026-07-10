namespace DreamLandWEB.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime Data { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } // Pendente, Pago, Enviado, Entregue
        public ICollection<ItemPedido> Itens { get; set; }
    }
}
