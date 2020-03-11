using System;
using System.Collections.Generic;
using System.Text;
using API_Rest.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Rest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Casa> Casas { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Usuario> Usuarios {get; set;}  
        public DbSet<Venda> Vendas {get; set;}  
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}