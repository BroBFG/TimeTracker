using System.ComponentModel.DataAnnotations;

namespace ProjectCalendar.Models
{
    public class Person //Класс сотрудников
    {
        [Key]
        public int PersonId { get; set; } //Номер сотрудника
        public string? First_Name { get; set; } // Имя сотрудника
        public string? Second_Name { get; set; } // Фамилия сотрудника
        public string? Middle_Name { get; set; } // Отчество сотрудника
        public List<Dates>? Dates { get; set; } // Даты с затраченным временем

    }
}
