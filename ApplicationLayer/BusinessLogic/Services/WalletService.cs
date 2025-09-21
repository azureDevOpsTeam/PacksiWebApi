using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.Wallet;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.BusinessLogic.Services;

[InjectAsScoped]
public class WalletService(IUnitOfWork unitOfWork, IRepository<Wallet> walletRepository, IRepository<WalletTransaction> walletTransactionRepository) : IWalletService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IRepository<Wallet> _walletRepository = walletRepository;
    private readonly IRepository<WalletTransaction> _walletTransactionRepository = walletTransactionRepository;

    public async Task<Result<BalanceDto>> GetBalanceAsync(long userAccountId)
    {
        var wallet = await _walletRepository.Query()
            .Where(w => w.UserAccountId == userAccountId)
            .GroupBy(w => w.UserAccountId)
            .Select(g => new BalanceDto
            {
                UserAccountId = g.Key,
                USDT = g.Where(x => x.Currency == CurrencyEnum.USDT).Select(x => x.Balance).FirstOrDefault(),
                IRR = g.Where(x => x.Currency == CurrencyEnum.IRR).Select(x => x.Balance).FirstOrDefault(),
            }).FirstOrDefaultAsync();

        if (wallet == null)
            return Result<BalanceDto>.NotFound();

        return Result<BalanceDto>.Success(wallet);
    }

    public async Task<decimal> GetBalanceByCurrencyAsync(long userAccountId, int currency, CancellationToken ct = default)
    {
        var wallet = await _walletRepository.Query()
            .FirstOrDefaultAsync(w => w.UserAccountId == userAccountId && w.Currency == currency, ct);
        return wallet?.Balance ?? 0m;
    }

    public async Task<Result> CreditAsync(int userAccountId, int currency, decimal amount, TransactionTypeEnum transactionType, string related = null, int? operatorUserId = null)
    {
        if (amount <= 0) return Result.GeneralFailure("amount must be > 0");

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var wallet = await _walletRepository.Query()
                .FirstOrDefaultAsync(w => w.UserAccountId == userAccountId && w.Currency == currency);

            if (wallet == null)
            {
                wallet = new Wallet
                {
                    UserAccountId = userAccountId,
                    Currency = currency,
                    Balance = 0
                };
                await _walletRepository.AddAsync(wallet);
            }

            wallet.Balance += amount;

            var walletTransaction = new WalletTransaction
            {
                Wallet = wallet,
                Amount = amount,
                TransactionType = transactionType,
                RelatedEntity = related
            };

            await _walletTransactionRepository.AddAsync(walletTransaction);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result.Success();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            return Result.GeneralFailure("خطا در ثبت تراکنش");
        }
    }

    public async Task<Result> DebitAsync(int userAccountId, int currency, decimal amount, TransactionTypeEnum transactionType, string related = null, int? operatorUserId = null)
    {
        if (amount <= 0) return Result.Failure("مبلغ نمیتواند صفر یا منفی باشد");

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var wallet = await _walletRepository.Query()
                .FirstOrDefaultAsync(w => w.UserAccountId == userAccountId && w.Currency == currency);

            if (wallet == null) return Result.NotFound();

            if (wallet.Balance < amount) return Result.Failure("موجودی شما کافی نیست");

            wallet.Balance -= amount;

            var walletTransaction = new WalletTransaction
            {
                Wallet = wallet,
                TransactionType = transactionType,
                Amount = amount,
                BalanceAfter = wallet.Balance,
                RelatedEntity = related
            };

            await _walletTransactionRepository.AddAsync(walletTransaction);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result.Success();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            return Result.GeneralFailure("خطا در برداشت موجودی");
        }
    }

}