using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class TransactionTypeEnum : SmartEnum<TransactionTypeEnum>
{
    public static readonly TransactionTypeEnum Bonus = new(1, "پاداش", "Bonus");
    public static readonly TransactionTypeEnum Charge = new(2, "شارژ حساب", "Charge");
    public static readonly TransactionTypeEnum Deposit = new(3, "واریز به حساب", "Deposit");
    public static readonly TransactionTypeEnum Withdraw = new(4, "برداشت از حساب", "Withdraw");

    public string PersianName { get; }

    public string EnglishName { get; }

    private TransactionTypeEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}