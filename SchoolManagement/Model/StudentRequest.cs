using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace SchoolManagement.Model
{
    public class StudentRequest
    {
        [Key]
        public int studentRequestId { get; set; }

        public string StudentName { get; set; } 
         
        public string StudentEmail { get; set; }
        public  long Contect { get; set; }
         
        public string standard { get; set; }


    }
}
