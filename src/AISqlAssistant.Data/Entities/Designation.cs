namespace AISqlAssistant.Data.Entities
{
    public class Designation
    {
        public int DesignationID { get; set; }
        public string Title { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
