namespace TestCarPhoneNumbers.Services
{
    public interface INotificationService
    {
        Task SendAsync(string phoneNumber, string message);
        bool IsBypassCode(string code);
    }

    public class DummyNotificationService : INotificationService
    {
        public Task SendAsync(string phoneNumber, string message)
        {
            Console.WriteLine($"[DUMMY SMS to {phoneNumber}]: {message}");
            return Task.CompletedTask;
        }
        public bool IsBypassCode(string code) => code == "999999";
    }
}
