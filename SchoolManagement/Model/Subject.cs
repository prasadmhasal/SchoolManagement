using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Model
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        public string SubjectName { get; set; }
    }
}
