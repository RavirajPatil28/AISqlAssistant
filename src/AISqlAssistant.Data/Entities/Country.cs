namespace AISqlAssistant.Data.Entities
{
    public class Country
    {
        public int CountryID { get; set; }
        public string Name { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
