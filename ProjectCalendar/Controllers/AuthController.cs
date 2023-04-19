using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectCalendar.DB;
using ProjectCalendar.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProjectCalendar.Controllers
{
    public class AuthController : Controller
    {
        private readonly ProjectContext db;
        public AuthController(ProjectContext context)
        {
            db = context;
        }

        // POST: /login
        [HttpPost("/login")]
        public ActionResult Login([FromBody] Auth id, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                throw new Exception("Задача прервана");
            else
            {
                // находим пользователя 
                Person? person = db.Persons.FirstOrDefault(p => p.PersonId == id.id);
                // если пользователь не найден, отправляем статусный код 401
                if (person is null) return (Unauthorized());

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.First_Name), new Claim("id", Convert.ToString(person.PersonId)) };
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                // формируем ответ
                var response = new
                {
                    access_token = encodedJwt,
                    username = person.First_Name

                };
                //logger.Debug("Выход из post");
                return (Json(response));
            }
        }

        public record class Auth(int id);
    }
}
