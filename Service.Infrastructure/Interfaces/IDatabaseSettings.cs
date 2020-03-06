namespace Service.Infrastructure.Interfaces
{
    public interface IDatabaseSettings
    {
        string DatabaseName { get; set; }
        string ServerSecret { get; set; }
        string ServerUrl { get; set; }
    }
}