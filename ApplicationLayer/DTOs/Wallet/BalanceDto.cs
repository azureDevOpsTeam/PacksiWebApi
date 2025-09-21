namespace ApplicationLayer.DTOs.Wallet;

public class BalanceDto
{
    public int UserAccountId { get; set; }

    public decimal USDT { get; set; }

    public decimal IRR { get; set; }
}