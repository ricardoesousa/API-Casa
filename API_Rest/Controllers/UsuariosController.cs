using System;
using System.Linq;
using System.Text;
using API_Rest.Data;
using API_Rest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;

namespace API_Rest.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly ApplicationDbContext database;
        public UsuariosController(ApplicationDbContext database)
        {
            this.database = database;

        }

        [HttpPost("registro")]
        public IActionResult Registro([FromBody] Usuario usuario)
        {
            database.Add(usuario);
            database.SaveChanges();
            return Ok(new { msg = "Usuário cadastrado com sucesso" });
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] Usuario credenciais)
        {

            try
            {
                Usuario usuario = database.Usuarios.First(User => User.Email.Equals(credenciais.Email));
                if (usuario != null)
                {
                    if (usuario.Senha.Equals(credenciais.Senha))
                    {
                        string chaveDeSeguranca = "school_of_net_manda_muito_bem!";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

                        var claims = new List<Claim>();
                        claims.Add(new Claim("id",usuario.Id.ToString()));
                        claims.Add(new Claim("email",usuario.Email));
                        claims.Add(new Claim(ClaimTypes.Role,"Admin"));

                        var JWT = new JwtSecurityToken(
                            issuer: "ricardo",
                            expires: DateTime.Now.AddHours(1),
                            audience: "usuario_comum",
                            signingCredentials: credenciaisDeAcesso,
                            claims: claims
                        );
                        return Ok (new JwtSecurityTokenHandler().WriteToken(JWT));

                    }
                    else
                    {
                        Response.StatusCode = 401;
                        return new ObjectResult("Dados Incorretos");
                    }
                }
                else
                {
                    Response.StatusCode = 401;
                    return new ObjectResult("Usuário não autorizado");
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = 401;
                return new ObjectResult("Usuário não autorizado");
            }

        }

                [HttpGet]
        public IActionResult Get()
        {
                if (database.Usuarios.Count() == 0)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Não há usuários cadastrados!" });
            }
            List<UsuarioTemp> usuarios = database.Usuarios.Select(usuario => new UsuarioTemp(usuario.Email)).ToList();

        
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Usuario usuario = database.Usuarios.First(c => c.Id == id);
                UsuarioTemp a = new UsuarioTemp(usuario.Email);
                return Ok(a);
            }
            catch (Exception e)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Usuário não encontrado!" });
            }
        }

        public class UsuarioTemp
        {
            public UsuarioTemp(string email)
            {
                Email = email;
            }

            public string Email { get; set; }

        }
    }
}