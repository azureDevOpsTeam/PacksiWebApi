using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.Wallet;
using ApplicationLayer.Extensions.SmartEnums;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface IWalletService
{
    Task<decimal> GetBalanceByCurrencyAsync(long userAccountId, int currency, CancellationToken ct = default);

    Task<Result> CreditAsync(int userAccountId, int currency, decimal amount, TransactionTypeEnum transactionType, string related = null, int? operatorUserId = null);

    Task<Result> DebitAsync(int userAccountId, int currency, decimal amount, TransactionTypeEnum transactionType, string related = null, int? operatorUserId = null);

    Task<Result<BalanceDto>> GetBalanceAsync(long userAccountId);
}