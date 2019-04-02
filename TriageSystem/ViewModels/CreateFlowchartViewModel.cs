using System.ComponentModel.DataAnnotations;

namespace TriageSystem.ViewModels
{
    public class CreateFlowchartViewModel
    {
        public string Name { get; set; }

        [Required(ErrorMessage = "You need to provide the discrimiantors for this priority level!")]
        public string Red { get; set; }

        [Required(ErrorMessage = "You need to provide the discrimiantors for this priority level!")]
        public string Orange { get; set; }

        [Required(ErrorMessage = "You need to provide the discrimiantors for this priority level!")]
        public string Yellow { get; set; }

        [Required(ErrorMessage = "You need to provide the discrimiantors for this priority level!")]
        public string Green { get; set; }

        //public IEnumerable<Discriminator> Discriminators { get; set; }

        public bool Edit { get; set; }

        public string SeeAlso { get; set; }
        public string Notes { get; set; }
    }
}
