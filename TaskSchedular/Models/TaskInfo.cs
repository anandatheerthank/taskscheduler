using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskSchedular.Models
{
    public class TaskInfo
    {
        //Implement the Code here
        [Key]
        public int TaskID { get; set; }

        [Display(Name = "Task Description")]
        [Required(ErrorMessage = "Please provide the Task Description")]
        public string TaskDescription { get; set; }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Please provide the Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Expected Closure Date")]
        [Required(ErrorMessage = "Please provide the Expected Closure Date")]
        [DataType(DataType.Date)]
        public DateTime ExpectedClosureDate { get; set; }

        [Display(Name = "Assigned To")]
        [Required(ErrorMessage = "Please provide the Assigned To")]
        public string AssignedTo { get; set; }

        [Display(Name = "Completion Status")]
        [Required(ErrorMessage = "Please provide the Completion Status")]
        public string CompletionStatus { get; set; }
    }
}

