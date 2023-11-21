namespace DistributedBanking.Domain.Options;

public record TransactionalClockOptions(bool UseTransactionalClock, string TransactionalClockHostUrl);