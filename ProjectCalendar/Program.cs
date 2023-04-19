using System.Text;
using Microsoft.EntityFrameworkCore;
using ProjectCalendar.Middlewares;
using NLog;
using NLog.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProjectCalendar.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using ProjectCalendar.DB;

namespace ProjectCalendar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

            var builder = WebApplication.CreateBuilder(args);

            //Подключение сервисов 
            builder.Services.AddControllersWithViews();
            builder.Services.AddControllers();
            builder.Services.AddDbContext<ProjectContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));//Добавление контекста БД в качестве сервиса, получение параметров подключения из файла конфигурации
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //JWT-Токен
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // указывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = true,
                    // строка, представляющая издателя
                    ValidIssuer = AuthOptions.ISSUER,
                    // будет ли валидироваться потребитель токена
                    ValidateAudience = true,
                    // установка потребителя токена
                    ValidAudience = AuthOptions.AUDIENCE,
                    // будет ли валидироваться время существования
                    ValidateLifetime = true,
                    // установка ключа безопасности
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = true,
                };
            });
            // NLog
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();
           
            //builder.Configuration.AddJsonFile("person.json");

            var app = builder.Build();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //================= Подключение файлов для авторизации
            app.UseDefaultFiles();
            app.UseStaticFiles();
            //=================
            app.UseMiddleware<ErrorsMiddleware>();
            app.UseMiddleware<LoggerMiddleware>();
            /*
            app.Run(async context =>
            {
                using (ProjectContext db = new ProjectContext())
                {
                    Project project1 = new Project { Name = "a" };
                    Project project2 = new Project { Name = "b" };
                    Person person1 = new Person { First_Name = "Дмитрий", Second_Name = "Копытов", Middle_Name = "Игоревич" };
                    Person person2 = new Person { First_Name = "Виктор", Second_Name = "Поляков", Middle_Name = "Степанович" };

                    Dates dates1 = new Dates { Date = new DateTime(2022, 11, 02), IsWeekend = false, Hours = 2, Project = project1, Person = person1 };
                    Dates dates2 = new Dates { Date = new DateTime(2022, 11, 03), IsWeekend = false, Hours = 5, Project = project2, Person = person1 };
                    Dates dates3 = new Dates { Date = new DateTime(2022, 11, 04), IsWeekend = false, Hours = 4, Project = project1, Person = person2 };
                    Dates dates4 = new Dates { Date = new DateTime(2022, 11, 02), IsWeekend = false, Hours = 6, Project = project2, Person = person2 };

                    db.Projects.AddRange(project1, project2);
                    db.Persons.AddRange(person1, person2);
                    db.Dates.AddRange(dates1, dates2, dates3, dates4);
                    db.SaveChanges();
                }
            });
                /*
                logger.LogInformation("Requested Path: {0}", context.Request.Path);
                using (ProjectContext db = new ProjectContext())
                {
                    
                    var str = new StringBuilder();
                    
                    Project project1 = new Project { Name = "a" };
                    Project project2 = new Project { Name = "b" };

                    Dates dates1 = new Dates { Date = new DateTime(2022, 11, 02), IsWeekend = false, Hours = 2 , Project = project1};
                    Dates dates2 = new Dates { Date = new DateTime(2022, 11, 03), IsWeekend = false, Hours = 5 , Project = project2};
                    Dates dates3 = new Dates { Date = new DateTime(2022, 11, 04), IsWeekend = false, Hours = 4 , Project = project1 };
                    Dates dates4 = new Dates { Date = new DateTime(2022, 11, 02), IsWeekend = false, Hours = 6, Project = project2 };

                    db.Projects.AddRange(project1, project2);
                    db.Dates.AddRange(dates1, dates2, dates3, dates4);
                    db.SaveChanges();
                    
                    /*var dates = db.Dates.Include(u => u.Project).ToList();
                    foreach (var data in dates)
                    {
                        str.Append($"Дата {data.Date.ToShortDateString()} в проекте {data.Project?.Name} кол-во часов {data.Hours} \n");
                    }
                    */
            /*
            var prods = db.Projects.Include(u => u.Dates).ToList();
            foreach( var prod in prods)
            {
                foreach(var date in prod.Dates)
                {
                    str.Append($"Дата: {date.Date.ToShortDateString()}, Проект: {prod.Name}, Часов на проект: {date.Hours} | ");
                    str.Append("<br>");
                }
            }
            context.Response.ContentType = "text/html;charset=utf-8";
            //await context.Response.WriteAsync(str.ToString());
            //await context.Response.WriteAsync(prods.ToString());
        }   
    });*/

            app.UseEndpoints((endpoints) =>
            {
                endpoints.MapControllers(); // подключаем маршрутизацию на контроллеры
            });
            app.Run();
        }
    }
}