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
            try
            {
                if (usuario.Email == null || usuario.Senha == null || usuario.Email.Length < 1 || usuario.Senha.Length < 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Favor informar todos os dados para cadastro do usuário!" });
                }
                if (database.Usuarios.Any(u => u.Email == usuario.Email))
                {
                    Response.StatusCode = 406;
                    return new ObjectResult(new { msg = "Email já cadastrado, favor informar sua senha na área de login!" });
                }
                database.Add(usuario);
                database.SaveChanges();
                return Ok(new { msg = "Usuário cadastrado com sucesso" });
            }
            catch
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Requisição Inválida!" });
            }
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] Usuario credenciais)
        {
            try
            {
                Usuario usuario = database.Usuarios.First(User => User.Email.Equals(credenciais.Email));
                if (usuario.Email == null || usuario.Senha == null || usuario.Email.Length < 1 || usuario.Senha.Length < 1)
                {
                    Response.StatusCode = 401;
                    return new ObjectResult(new { msg = "Favor informar todos os dados do usuario para acessar o sistema!" });
                }
                if (!usuario.Senha.Equals(credenciais.Senha))
                {
                    Response.StatusCode = 401;
                    return new ObjectResult(new { msg = "Dados Incorretos" });
                }
                string chaveDeSeguranca = "school_of_net_manda_muito_bem!";
                var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

                var claims = new List<Claim>();
                claims.Add(new Claim("id", usuario.Id.ToString()));
                claims.Add(new Claim("email", usuario.Email));
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                var JWT = new JwtSecurityToken(
                    issuer: "ricardo",
                    expires: DateTime.Now.AddHours(1),
                    audience: "usuario_comum",
                    signingCredentials: credenciaisDeAcesso,
                    claims: claims
                );
                return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));
            }
            catch
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Usuário não encontrado!" });
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                if (database.Usuarios.Count() == 0)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Não há usuários cadastrados!" });
                }
                List<UsuarioTemp> usuarios = database.Usuarios.Select(usuario => new UsuarioTemp(usuario.Email)).ToList();
                return Ok(usuarios);
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
                Usuario usuario = database.Usuarios.First(c => c.Id == id);
                UsuarioTemp a = new UsuarioTemp(usuario.Email);
                return Ok(a);
            }
            catch
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