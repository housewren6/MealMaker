namespace HouseWrenDevelopment.Models.Contact
{
    public interface IEmailService
    {
        void Send(EmailMessage message);
    }
}
