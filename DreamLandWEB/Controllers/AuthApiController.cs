using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DreamLandWEB.Data;
using DreamLandWEB.Helpers;
using DreamLandWEB.Models;
using DreamLandWEB.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DreamLandWEB.Controllers
{
    public record AuthResponse(string Token, string Nome, string Email);

    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthApiController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (usuario == null || !PasswordHelper.VerificarSenha(model.Senha, usuario.SenhaHash))
                return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

            return Ok(GerarResposta(usuario));
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroViewModel model)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
                return Conflict(new { mensagem = "Este e-mail já está cadastrado." });

            var usuario = new Usuario
            {
                Nome = model.Nome,
                Email = model.Email,
                Telefone = model.Telefone,
                SenhaHash = PasswordHelper.HashSenha(model.Senha),
                Admin = false
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(GerarResposta(usuario)); // já devolve o token: o usuário entra logado
        }

        private AuthResponse GerarResposta(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: new SigningCredentials(chave, SecurityAlgorithms.HmacSha256));

            return new AuthResponse(
                new JwtSecurityTokenHandler().WriteToken(token), usuario.Nome, usuario.Email);
        }
    }
}