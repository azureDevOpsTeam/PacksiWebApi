namespace ApplicationLayer.DTOs.MiniApp;

public abstract class PaymentDto
{
    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; }
}

public class TetherPaymentViewModel : PaymentDto
{
    public string WalletAddress { get; set; }

    public string Network { get; set; }
}

public class RialPaymentViewModel : PaymentDto
{
    public string PaymentUrl { get; set; }
}