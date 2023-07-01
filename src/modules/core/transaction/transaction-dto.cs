public record OrderDetails(
    string? senders_wallet_tag,
    int amount,
    string recievers_wallet_tag,
    string purpose
);