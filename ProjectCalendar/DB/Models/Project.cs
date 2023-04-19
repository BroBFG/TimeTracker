using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectCalendar.Models
{
    public class Project // Таблица проекты
    {
        public int Id { get; set; } // id проекта
        public string? Name { get; set; } // Имя проекта
        [JsonIgnore]
        public List<Dates>? Dates { get; set; } // Даты с затраченным временем
    }
}
