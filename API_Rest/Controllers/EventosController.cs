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
            if (database.Eventos.Count() == 0)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Não há eventos cadastrados!" });
            }
            var casas = database.Casas.ToList();
            var eventos = database.Eventos.ToList();
            return Ok(eventos);
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
            catch (Exception e)
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
            else
            {
                if (eTemp.Nome != null && eTemp.Capacidade != 0 && eTemp.Ingressos != 0 && eTemp.Data != null && eTemp.Preco != 0 && eTemp.Genero != null && eTemp.CasaId != 0)
                {

                    var casa = database.Casas.Where(c => c.Id == eTemp.CasaId).FirstOrDefault();
                    if (casa != null)
                    {
                        Evento e = new Evento();
                        e.Nome = eTemp.Nome;
                        e.Capacidade = eTemp.Capacidade;
                        e.Ingressos = eTemp.Ingressos;
                        e.Data = eTemp.Data;
                        e.Preco = eTemp.Preco;
                        e.Genero = eTemp.Genero;
                        e.Casa = database.Casas.First(nomecasa => nomecasa.Id == eTemp.CasaId);
                        if (eTemp.Data < DateTime.Now)
                        {
                            Response.StatusCode = 406;
                            return new ObjectResult(new { msg = "A data não pode ser anterior a data atual!" });
                        }
                        database.Eventos.Add(e);
                        database.SaveChanges();
                        Response.StatusCode = 201;
                        return new ObjectResult(new { msg = "Evento cadastrado com sucesso!" });
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new { msg = "A casa selecionada não existe!" });
                    }
                }
                else
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Todos os dados são de preenchimento obrigatório!" });
                }
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody]EventoTemp evento)
        {
            evento.Id = id;
            if (evento.Id > 0)
            {
                try
                {
                    var e = database.Eventos.First(etemp => etemp.Id == evento.Id);
                    if (e != null)
                    {
                        if (database.Casas.Any(etemp => etemp.Id == evento.CasaId))
                        {
                            var casa = database.Casas.Where(etemp => etemp.Id == evento.CasaId).FirstOrDefault();
                            e.Nome = evento.Nome != null ? evento.Nome : e.Nome;
                            e.Capacidade = evento.Capacidade != 0 ? evento.Capacidade : e.Capacidade;
                            e.Ingressos = evento.Ingressos != 0 ? evento.Ingressos : e.Ingressos;
                            e.Data = evento.Data != null ? evento.Data : e.Data;
                            e.Preco = evento.Preco != 0 ? evento.Preco : e.Preco;
                            e.Genero = evento.Genero != null ? evento.Genero : e.Genero;
                            e.Casa = evento.CasaId != 0 ? casa : e.Casa;
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
                            return new ObjectResult(new { msg = "A casa selecionada não existe!" });
                        }
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
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O Id do evento é inválido!" });

            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]EventoTemp evento)
        {
            evento.Id = id;
            if (evento.Id > 0)
            {
                try
                {
                    var e = database.Eventos.First(etemp => etemp.Id == evento.Id);
                    if (evento.Nome != null && evento.Capacidade != 0 && evento.Ingressos != 0 && evento.Data != null && evento.Preco != 0 && evento.Genero != null)
                    {
                        var casa = database.Casas.Where(etemp => etemp.Id == evento.CasaId).FirstOrDefault();
                        if (database.Casas.Any(etemp => etemp.Id == evento.CasaId))
                        {
                            e.Nome = evento.Nome;
                            e.Capacidade = evento.Capacidade;
                            e.Ingressos = evento.Ingressos;
                            e.Data = evento.Data;
                            e.Preco = evento.Preco;
                            e.Genero = evento.Genero;
                            e.Casa = database.Casas.First(c => c.Id == evento.CasaId);
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
                            Response.StatusCode = 400;
                            return new ObjectResult(new { msg = "A casa selecionada não existe!" });
                        }
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new { msg = "Todos os campos são de preenchimento obrigatório!" });
                    }

                }
                catch
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Evento não encontrada!" });
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O Id do evento é inválido!" });

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
            catch (Exception e)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Evento não encontrado!" });
            }
        }


        [HttpGet("capacidade/asc")]
        public IActionResult Listar_Capacidade_Asc()
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

        [HttpGet("capacidade/desc")]
        public IActionResult Listar_Capacidade_Desc()
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

        [HttpGet("data/asc")]
        public IActionResult Listar_Data_Asc()
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

        [HttpGet("data/desc")]
        public IActionResult Listar_Data_Desc()
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

        [HttpGet("nome/asc")]
        public IActionResult Listar_Nome_Asc()
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

        [HttpGet("nome/desc")]
        public IActionResult Listar_Nome_Desc()
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

        [HttpGet("preco/asc")]
        public IActionResult Listar_Preco_Asc()
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

        [HttpGet("preco/desc")]
        public IActionResult Listar_Preco_Desc()
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
        public class EventoTemp
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public int Capacidade { get; set; }
            public int Ingressos { get; set; }
            public DateTime Data { get; set; }
            public Decimal Preco { get; set; }
            public string Genero { get; set; }
            public int CasaId { get; set; }

        }
    }
}