using System;
using System.Linq;
using API_Rest.Data;
using API_Rest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace API_Rest.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize]
    public class VendasController : ControllerBase
    {

        private readonly ApplicationDbContext database;
        public VendasController(ApplicationDbContext database)
        {
            this.database = database;
        }


        [HttpGet]
        public IActionResult Get()
        {
            if (database.Eventos.Count() == 0)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Não há vendas cadastrados!" });
            }
            var vendas = database.Vendas.ToList();
            var eventos = database.Eventos.ToList();
            var casas = database.Casas.ToList();
            return Ok(vendas);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Venda venda = database.Vendas.First(c => c.Id == id);
                var eventos = database.Eventos.ToList();
                var casas = database.Casas.ToList();
                return Ok(venda);
            }
            catch (Exception e)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Venda não encontrada!" });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] VendaTemp vTemp)
        {
            if (vTemp.QtdIngressos > 0)
            {
                var evento = database.Eventos.Where(v => v.Id == vTemp.EventoId).SingleOrDefault();

                if (evento != null)
                {
                    Venda v = new Venda();
                    v.Data = DateTime.Now;
                    v.Evento = evento;
                    v.QtdIngressos = vTemp.QtdIngressos;
                    v.Preco = evento.Preco;
                    v.Total = evento.Preco * vTemp.QtdIngressos;
                    evento.Ingressos = evento.Ingressos - v.QtdIngressos;
                    database.Vendas.Add(v);
                    database.SaveChanges();
                    Response.StatusCode = 201;
                    return new ObjectResult(new { msg = "Venda cadastrada com sucesso!" });
                }
                else
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Evento não encontrado!" });
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "É necessário informar uma quantidade de ingressos maior que zero!" });
            }
        }


        public class VendaTemp
        {
            public int EventoId { get; set; }
            public int QtdIngressos { get; set; }

        }
    }
}