using System.Text.Json.Serialization;

namespace ProjectCalendar.Models
{
    public class ContractModel
    {
        public Project Project { get; set; }
        public List<Dates> Dates { get; set; }

        public ContractModel(Project project, List<Dates> dates)
        {
            Project = project;
            Dates = dates;
        }
    }

}
