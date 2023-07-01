public enum UserRole
{
    User = 1,
    Admin
}

public enum OrderType
{
    Deposit = 1,
    Withdrawal,
    Transfer
}

public enum TransactionStatus
{
    Failed = 99,
    Successfull = 00
}

public enum OrderStatus
{
    logged = 05,
    Pending = 04,
    Reprocess = 03,
    Failed = 99,
    Successfull = 00
}