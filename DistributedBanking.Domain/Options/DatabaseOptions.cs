namespace DistributedBanking.Domain.Options;

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = null!;

    public string ReplicaSetConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;
}