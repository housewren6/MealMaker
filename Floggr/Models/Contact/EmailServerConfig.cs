namespace HouseWrenDevelopment.Models.Contact
{
    public class EmailServerConfig
    {
        public EmailServerConfig(int _smtpPort = 587)
        {
            SmtpPort = _smtpPort;
        }

        public string SmtpServer { get; set; }
        public int SmtpPort { get; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
    }
}
