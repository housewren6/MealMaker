namespace HouseWrenDevelopment.Models.Contact
{
    public class EmailMessage
    {
        public List<EmailAddress> ToAddresses { get; set; } = new List<EmailAddress>();
        public List<EmailAddress> FromAddresses { get; set; } = new List<EmailAddress>();
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
