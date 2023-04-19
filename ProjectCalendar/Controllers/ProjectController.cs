using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCalendar.DB;
using ProjectCalendar.Models;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;



namespace ProjectCalendar.Controllers
{
    /// <summary>
    /// Контроллер API, для работы с БД. Используется для изменения и получения нформации по проектам и соответсвующим им датам, используется авторизация для работы с данными по пользователю.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectContext db;
        public ProjectController(ProjectContext context)
        {
            db = context;
        }

        // GET
        /// <summary>
        /// Получение списка проектов и их дат, соотвутсвующих авторизованному пользователю
        /// </summary>
        /// <returns>
        /// Возвращает JSON Списка Проектов с датами.
        /// Поля project: 
        ///     id:int; - Номер проекта
        ///     name:string; - Название проекта
        /// Поля dates: - Список
        ///     date:date; - Дата
        ///     isWeekend:bool; - Является ли день выходным
        ///     hours:double; - Число отработанных часов
        /// </returns>
        [Authorize]
        [HttpGet]
        public Task<List<ContractModel>> GetAsync(CancellationToken token)
        {
            //return db.Projects.Include(u => u.Dates.Where(p => p.PersonId == Get_User_Id())).AsNoTracking().ToListAsync(token); //Вывод данных на основе id пользователя      
            List<ContractModel> model = new();
            if(token.IsCancellationRequested)
                throw new Exception("Задача прервана");
            else
            {
                foreach (var project in db.Projects.AsNoTracking().Include(u => u.Dates.Where(p => p.PersonId == Get_User_Id())))
                {
                    model.Add(new(project, project.Dates));
                }
                return Task.FromResult(model);
            }
        }

        // GET /id
        /// <summary>
        /// Получение соответсвующего проекта, по его номеру, и его дат, соотвутсвующих авторизованному пользователю
        /// </summary>
        /// <param name="id">Номер проекта</param>
        /// <returns>
        /// Возвращает JSON Проекта с датами.
        /// Поля project: 
        ///     id:int; - Номер проекта
        ///     name:string; - Название проекта
        ///     dates:[]; - Даты
        /// Поля dates: 
        ///     date:date; - Дата
        ///     isWeekend:bool; - Является ли день выходным
        ///     hours:double; - Число отработанных часов
        /// </returns>
        [Authorize]
        [HttpGet("{id}")]
        public Task<ContractModel> GetAsync(int id,CancellationToken token)
        {
            if (token.IsCancellationRequested)
                throw new Exception("Задача прервана");
            else
            {
                //return db.Projects.Include(u => u.Dates.Where(p => p.PersonId == Get_User_Id())).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, token);
                return Task.FromResult(new ContractModel(db.Projects.AsNoTracking().FirstOrDefault(p => p.Id == id), db.Dates.AsNoTracking().Where(p => p.PersonId == Get_User_Id() && p.ProjectId == id).ToList()));
            }
        }

        // POST 
        /// <summary>
        /// Добавление новых проектов, с датами. Даты привязываются к авторизованному пользователю.
        /// </summary>
        /// <param name="income_data">Список проектов в формате Json.
        /// Формат:
        /// Поля project: 
        ///     id:int; - Номер проекта
        ///     name:string; - Название проекта
        ///     dates:[]; - Даты
        /// Поля dates: 
        ///     date:date; - Дата
        ///     isWeekend:bool; - Является ли день выходным
        ///     hours:double; - Число отработанных часов
        /// </param>
        /// <returns>Код 200, в случае успеха, Код 400, в случае неудачи</returns>
        [Authorize]
        [HttpPost]
        public Task PostAsync([FromBody] List<ContractModel> income_data, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                throw new Exception("Задача прервана");
            else
            {
                Person? person = db.Persons.FirstOrDefault(p => p.PersonId == Get_User_Id());
                if (income_data != null)
                {
                    //_logger.LogDebug("Список принят");
                    foreach (var prod in income_data)
                    {
                        foreach (var date in prod.Dates)
                        {
                            date.Person = person;
                        }
                        Project? project = new() { Name = prod.Project.Name, Dates = prod.Dates };
                        db.Add(project);
                    }
                    //db.Projects.AddRange(value);
                    db.SaveChangesAsync();
                    return Task.FromResult(Results.Ok());
                }
                return Task.FromResult(Results.BadRequest());
            }
        }

        // PUT 
        /// <summary>
        /// Изменение информации по отдельному проекту
        /// </summary>
        /// <param name="id">Номер изменяемого проекта</param>
        /// <param name="income_data">
        /// Json с изменениями
        /// Формат:
        /// Поля project: 
        ///     id:int; - Номер проекта
        ///     name:string; - Название проекта
        ///     dates:[]; - Даты
        /// Поля dates: 
        ///     date:date; - Дата
        ///     isWeekend:bool; - Является ли день выходным
        ///     hours:double; - Число отработанных часов
        ///     </param>
        /// <returns>Код 200, в случае успеха, Код 400, в случае неудачи</returns>
        [Authorize]
        [HttpPut("{id}")]
        public Task PutAsync(int id, [FromBody] ContractModel income_data, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                throw new Exception("Задача прервана");
            else
            {
                var db_dates = db.Dates.Where(p => p.ProjectId == id && p.PersonId == Get_User_Id()).ToList(); //Даты пользователя по заданному проекту
                var db_project = db.Projects.FirstOrDefault(p => p.Id == id);
                if (db_project != null) //Существует ли проект
                {
                    foreach (var date in income_data.Dates) //Связка новых дат с текупщим пользователем
                    {
                        Person? person = db.Persons.FirstOrDefault(p => p.PersonId == Get_User_Id());
                        date.Person = person;
                    }
                    for (int i = 0; i < income_data.Dates.Count; i++)
                    {
                        if (db_dates.Exists(e => e.Date == income_data.Dates[i].Date)) //Проверка на существование в базе данных дат, в случае существования они изменяются, иначе создаеются
                        {
                            Dates date = db_dates.FirstOrDefault(e => e.Date == income_data.Dates[i].Date); //Выделяем отдельную дату
                            date.Hours = income_data.Dates[i].Hours; //Меняем часы
                        }
                        else
                        {
                            db_dates.Add(income_data.Dates[i]); // добавляем дату
                        }
                    }
                    db.SaveChangesAsync();
                    return Task.FromResult(Results.Ok());
                }
                return Task.FromResult(Results.NotFound());
            }
            

        }

        // DELETE 
        /// <summary>
        /// Удаление проекта
        /// </summary>
        /// <param name="id">Номер удаляемого проекта</param>
        /// <returns>Код 200, в случае успеха, Код 400, в случае неудачи</returns>
        [Authorize]
        [HttpDelete("{id}")]
        public Task DeleteAsync(int id, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                throw new Exception();
            else
            {
                if (db.Projects.Any(p => p.Id == id))
                {
                    db.Projects.Remove(db.Projects.FirstOrDefault(p => p.Id == id));
                    db.SaveChangesAsync();
                    return Task.FromResult(Results.Ok());
                }
                return Task.FromResult(Results.NotFound());
            }
        }

        /// <summary>
        /// Получение номера авторизованного пользователя
        /// </summary>
        /// <returns>Возвращает целое число, соответсвующее id пользователя</returns>
        private int Get_User_Id()
        {
            return Int32.Parse((User.Claims.FirstOrDefault(p => p.Type == "id").Value)); //Поиск id пользователя
        }

    }
}