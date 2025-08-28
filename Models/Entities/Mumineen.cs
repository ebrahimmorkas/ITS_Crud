namespace ITSAssignment.Web.Models.Entities
{
    public class Mumineen
    {
        public Guid Id { get; set; }
        public int Its { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
        public int Mobile_number { get; set; }
        public string? Email_address { get; set; }
        public string? Marital_status { get; set; }
        public string? Address { get; set; }
        public string? Role { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
