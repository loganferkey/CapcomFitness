namespace CapcomFitness.Utility
{
    public class Feedback
    {
        public Feedback()
        {
            SuccessMessages = new List<string>();
            ErrorMessages = new List<string>();
        }

        public List<string> SuccessMessages { get; set; }
        public List<string> ErrorMessages { get; set; }
    }

}
