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

            //����������� �������� 
            builder.Services.AddControllersWithViews();
            builder.Services.AddControllers();
            builder.Services.AddDbContext<ProjectContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));//���������� ��������� �� � �������� �������, ��������� ���������� ����������� �� ����� ������������
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //JWT-�����
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // ���������, ����� �� �������������� �������� ��� ��������� ������
                    ValidateIssuer = true,
                    // ������, �������������� ��������
                    ValidIssuer = AuthOptions.ISSUER,
                    // ����� �� �������������� ����������� ������
                    ValidateAudience = true,
                    // ��������� ����������� ������
                    ValidAudience = AuthOptions.AUDIENCE,
                    // ����� �� �������������� ����� �������������
                    ValidateLifetime = true,
                    // ��������� ����� ������������
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    // ��������� ����� ������������
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
            //================= ����������� ������ ��� �����������
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
                    Person person1 = new Person { First_Name = "�������", Second_Name = "�������", Middle_Name = "��������" };
                    Person person2 = new Person { First_Name = "������", Second_Name = "�������", Middle_Name = "����������" };

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
                        str.Append($"���� {data.Date.ToShortDateString()} � ������� {data.Project?.Name} ���-�� ����� {data.Hours} \n");
                    }
                    */
            /*
            var prods = db.Projects.Include(u => u.Dates).ToList();
            foreach( var prod in prods)
            {
                foreach(var date in prod.Dates)
                {
                    str.Append($"����: {date.Date.ToShortDateString()}, ������: {prod.Name}, ����� �� ������: {date.Hours} | ");
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
                endpoints.MapControllers(); // ���������� ������������� �� �����������
            });
            app.Run();
        }
    }
}