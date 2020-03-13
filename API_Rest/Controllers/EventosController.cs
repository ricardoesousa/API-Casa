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
    [Authorize]
    public class EventosController : ControllerBase
    {

        private readonly ApplicationDbContext database;
        public EventosController(ApplicationDbContext database)
        {
            this.database = database;
        }


        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                if (database.Eventos.Count() == 0)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Não há eventos cadastrados!" });
                }
                var casas = database.Casas.ToList();
                var eventos = database.Eventos.ToList();
                return Ok(eventos);
            }
            catch
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Requisição Inválida!" });
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var casas = database.Casas.ToList();
                Evento evento = database.Eventos.First(c => c.Id == id);
                return Ok(evento);
            }
            catch
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Evento não encontrado!" });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] EventoTemp eTemp)
        {
            if (database.Casas.Count() == 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Não é possível cadastrar um evento sem uma casa cadastrada" });
            }
            if (eTemp.Nome == null || eTemp.Nome.Length < 1 || eTemp.Capacidade == 0 || eTemp.Data == null || eTemp.Preco == 0 || eTemp.Genero == null || eTemp.Genero.Length < 1 || eTemp.CasaId == 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Todos os dados são de preenchimento obrigatório!" });
            }
            if (eTemp.Data < DateTime.Now)
            {
                Response.StatusCode = 406;
                return new ObjectResult(new { msg = "A data não pode ser anterior a data atual!" });
            }
            var casa = database.Casas.Where(c => c.Id == eTemp.CasaId).FirstOrDefault();
            if (casa == null)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "A casa selecionada não existe!" });
            }
            Evento e = new Evento();
            e.Nome = eTemp.Nome;
            e.Capacidade = eTemp.Capacidade;
            e.Data = eTemp.Data;
            e.Preco = eTemp.Preco;
            e.Genero = eTemp.Genero;
            e.Casa = database.Casas.First(c => c.Id == eTemp.CasaId);
            database.Eventos.Add(e);
            database.SaveChanges();
            Response.StatusCode = 201;
            return new ObjectResult(new { msg = "Evento cadastrado com sucesso!" });

        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody]EventoTemp evento)
        {
            evento.Id = id;
            try
            {
                var e = database.Eventos.First(etemp => etemp.Id == evento.Id);
                if (e != null)
                {
                        var casa = database.Casas.Where(etemp => etemp.Id == evento.CasaId).FirstOrDefault();
                        e.Nome = evento.Nome != null ? evento.Nome : e.Nome;
                        e.Capacidade = evento.Capacidade != 0 ? evento.Capacidade : e.Capacidade;
                        e.Data = evento.Data != null ? evento.Data : e.Data;
                        e.Preco = evento.Preco != 0 ? evento.Preco : e.Preco;
                        e.Genero = evento.Genero != null ? evento.Genero : e.Genero;
                        e.Casa = casa != null ? casa : e.Casa;
                        if (evento.Data < DateTime.Now)
                        {
                            Response.StatusCode = 406;
                            return new ObjectResult(new { msg = "A data não pode ser anterior a data atual!" });
                        }
                        database.SaveChanges();
                        Response.StatusCode = 200;
                        return new ObjectResult(new { msg = "Evento alterado com sucesso!" });
                }
                else
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Evento não encontrado!" });
                }
            }
            catch
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Evento não encontrado!" });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]EventoTemp evento)
        {
            evento.Id = id;
            try
            {
                var e = database.Eventos.First(etemp => etemp.Id == evento.Id);
                if (evento.Nome == null || evento.Nome.Length < 1 || evento.Capacidade == 0 || evento.Data == null || evento.Preco == 0 || evento.Genero == null || evento.Genero.Length < 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Todos os campos são de preenchimento obrigatório!" });
                }
                var casa = database.Casas.Where(etemp => etemp.Id == evento.CasaId).FirstOrDefault();
                if (!database.Casas.Any(etemp => etemp.Id == evento.CasaId))
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "A casa selecionada não existe!" });
                }
                if (evento.Data < DateTime.Now)
                {
                    Response.StatusCode = 406;
                    return new ObjectResult(new { msg = "A data não pode ser anterior a data atual!" });
                }
                e.Nome = evento.Nome;
                e.Capacidade = evento.Capacidade;
                e.Data = evento.Data;
                e.Preco = evento.Preco;
                e.Genero = evento.Genero;
                e.Casa = database.Casas.First(c => c.Id == evento.CasaId);
                database.SaveChanges();
                Response.StatusCode = 200;
                return new ObjectResult(new { msg = "Evento alterado com sucesso!" });
            }
            catch
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Evento não encontrado!" });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Evento evento = database.Eventos.First(e => e.Id == id);
                database.Eventos.Remove(evento);
                database.SaveChanges();
                return Ok(new { msg = "Evento deletado com sucesso!" });
            }
            catch
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Evento não encontrado!" });
            }
        }


        [HttpGet("capacidade/asc")]
        public IActionResult Listar_Capacidade_Asc()
        {
            try
            {
                if (database.Eventos.Count() == 0)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Não há eventos cadastrados!" });
                }
                var casas = database.Casas.ToList();
                var eventos = database.Eventos.OrderBy(s => s.Capacidade).ToList();
                return Ok(eventos);
            }
            catch
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Requisição Inválida!" });
            }
        }

        [HttpGet("capacidade/desc")]
        public IActionResult Listar_Capacidade_Desc()
        {
            try
            {
                if (database.Eventos.Count() == 0)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Não há eventos cadastrados!" });
                }
                var casas = database.Casas.ToList();
                var eventos = database.Eventos.OrderByDescending(s => s.Capacidade).ToList();
                return Ok(eventos);
            }
            catch
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Requisição Inválida!" });
            }
        }

        [HttpGet("data/asc")]
        public IActionResult Listar_Data_Asc()
        {
            try
            {
                if (database.Eventos.Count() == 0)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Não há eventos cadastrados!" });
                }
                var casas = database.Casas.ToList();
                var eventos = database.Eventos.OrderBy(s => s.Data).ToList();
                return Ok(eventos);
            }
            catch
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Requisição Inválida!" });
            }
        }

        [HttpGet("data/desc")]
        public IActionResult Listar_Data_Desc()
        {
            try
            {
                if (database.Eventos.Count() == 0)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Não há eventos cadastrados!" });
                }
                var casas = database.Casas.ToList();
                var eventos = database.Eventos.OrderByDescending(s => s.Data).ToList();
                return Ok(eventos);
            }
            catch
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Requisição Inválida!" });
            }
        }

        [HttpGet("nome/asc")]
        public IActionResult Listar_Nome_Asc()
        {
            try
            {
                if (database.Eventos.Count() == 0)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Não há eventos cadastrados!" });
                }
                var casas = database.Casas.ToList();
                var eventos = database.Eventos.OrderBy(s => s.Nome).ToList();
                return Ok(eventos);
            }
            catch
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Requisição Inválida!" });
            }
        }

        [HttpGet("nome/desc")]
        public IActionResult Listar_Nome_Desc()
        {
            try
            {
                if (database.Eventos.Count() == 0)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Não há eventos cadastrados!" });
                }
                var casas = database.Casas.ToList();
                var eventos = database.Eventos.OrderByDescending(s => s.Nome).ToList();
                return Ok(eventos);
            }
            catch
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Requisição Inválida!" });
            }
        }

        [HttpGet("preco/asc")]
        public IActionResult Listar_Preco_Asc()
        {
            try
            {
                if (database.Eventos.Count() == 0)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Não há eventos cadastrados!" });
                }
                var casas = database.Casas.ToList();
                var eventos = database.Eventos.OrderBy(s => s.Preco).ToList();
                return Ok(eventos);
            }
            catch
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Requisição Inválida!" });
            }
        }

        [HttpGet("preco/desc")]
        public IActionResult Listar_Preco_Desc()
        {
            try
            {
                if (database.Eventos.Count() == 0)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Não há eventos cadastrados!" });
                }
                var casas = database.Casas.ToList();
                var eventos = database.Eventos.OrderByDescending(s => s.Preco).ToList();
                return Ok(eventos);
            }
            catch
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Requisição Inválida!" });
            }
        }
        public class EventoTemp
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public DateTime Data { get; set; }
            public int Capacidade { get; set; }
            public Decimal Preco { get; set; }
            public string Genero { get; set; }
            public int CasaId { get; set; }

        }
    }
}