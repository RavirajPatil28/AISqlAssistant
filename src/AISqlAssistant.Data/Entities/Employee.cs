namespace AISqlAssistant.Data.Entities
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }

        public int CountryID { get; set; }
        public Country Country { get; set; }

        public int DesignationID { get; set; }
        public Designation Designation { get; set; }
    }
}
