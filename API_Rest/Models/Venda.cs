using System;

namespace API_Rest.Models
{
    public class Venda
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public Evento Evento { get; set; }
        public int QtdIngressos { get; set; }
        public decimal Preco { get; set; }
        public Decimal Total { get; set; }
        
    }
}