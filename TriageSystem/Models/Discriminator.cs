namespace TriageSystem.Models
{
    public class Discriminator
    {
        private Priority _Priority;
    
        public string Name { get; set; }
        public string Description { get; set; }
        public Priority Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                _Priority = value;
                PriorityString = _Priority.ToString();
            }
        }
        public string PriorityString{ get; set; }

    }
}
