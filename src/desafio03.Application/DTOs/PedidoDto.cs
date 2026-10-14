using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desafio03.Application.DTOs
{
    public class PedidoDto
    {
        public int Id { get; set; }
        public DateTime FechaPedido { get; set; }
        public int Cantidad { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
    }

    public class CrearPedidoDto
    {
        public int Cantidad { get; set; }
        public int ClienteId { get; set; }
        public int ProductoId { get; set; }
    }
}