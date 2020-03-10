using System;

namespace API_Rest.Models
{
    public class Evento
    {
        public int Id { get; set; } 
        public string Nome { get; set; }
        public int Capacidade { get; set; }
        public int Ingressos { get; set; }
        public DateTime Data { get; set; }
        public Decimal Preco { get; set; }
        public string Genero { get; set; }
        public Casa Casa { get; set; }
        
    }
}