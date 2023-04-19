using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectCalendar.Models
{
    public class Dates // Таблица с датами проекта
    {
        [Key]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; } // Дата
        public bool IsWeekend { get; set; } // Свойство: является ли день выходным (false - нет; true - да)
        public double? Hours { get; set; } // Часы работы

        [JsonIgnore]
        public int ProjectId { get; set; } // Внешний ключ на проекты
        [JsonIgnore]
        public Project? Project { get; set; } // Навигационное свойство на проекты
        [JsonIgnore]
        public int PersonId { get; set; } // Внешний ключ на персонал
        [JsonIgnore]
        public Person? Person { get; set; } // Навигационное свойство на персонал
    }
}
