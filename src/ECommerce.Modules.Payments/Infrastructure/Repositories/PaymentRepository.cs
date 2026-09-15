using Dapper;
using ECommerce.Modules.Payments.Application.Interfaces;
using ECommerce.Modules.Payments.Domain.Entities;
using ECommerce.Modules.Payments.Infrastructure.Database;
using Microsoft.Data.SqlClient;

namespace ECommerce.Modules.Payments.Infrastructure.Repositories;

public sealed class PaymentRepository(IDbConnectionFactory connectionFactory) : IPaymentRepository
{
    public async Task<Payment?> GetByIdAsync(Guid paymentId, Guid userId)
    {
        const string sql = @"
            SELECT
                Id,
                OrderId,
                UserId,
                Amount,
                Currency,
                Status,
                IdempotencyKey,
                StripePaymentIntentId,
                StripeRefundId,
                FailureReason,
                CreatedAt,
                UpdatedAt
            FROM Payments
            WHERE Id = @PaymentId
              AND UserId = @UserId;
        ";

        using var connection = connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Payment>(sql, 
            new
            {
                PaymentId = paymentId,
                UserId = userId
            });
    }

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId, Guid userId)
    {
        const string sql = @"
            SELECT
                Id,
                OrderId,
                UserId,
                Amount,
                Currency,
                Status,
                IdempotencyKey,
                StripePaymentIntentId,
                StripeRefundId,
                FailureReason,
                CreatedAt,
                UpdatedAt
            FROM Payments
            WHERE OrderId = @OrderId
              AND UserId = @UserId;
        ";

        using var connection = connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Payment>(sql,
            new
            {
                OrderId = orderId,
                UserId = userId
            });
    }

    public async Task<Payment?> GetByIdempotencyKeyAsync(Guid userId, string idempotencyKey)
    {
        const string sql = @"
            SELECT
                Id,
                OrderId,
                UserId,
                Amount,
                Currency,
                Status,
                IdempotencyKey,
                StripePaymentIntentId,
                StripeRefundId,
                FailureReason,
                CreatedAt,
                UpdatedAt
            FROM Payments
            WHERE UserId = @UserId
              AND IdempotencyKey = @IdempotencyKey;
        ";

        using var connection = connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Payment>(sql,
            new
            {
                UserId = userId,
                IdempotencyKey = idempotencyKey
            });
    }

    public async Task CreatePendingAsync(Payment payment)
    {
        const string paymentSql = @"
            INSERT INTO Payments
            (
                Id,
                OrderId,
                UserId,
                Amount,
                Currency,
                Status,
                IdempotencyKey,
                StripePaymentIntentId,
                StripeRefundId,
                FailureReason,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @Id,
                @OrderId,
                @UserId,
                @Amount,
                @Currency,
                @Status,
                @IdempotencyKey,
                @StripePaymentIntentId,
                @StripeRefundId,
                @FailureReason,
                @CreatedAt,
                @UpdatedAt
            );
        ";

        const string historySql = @"
            INSERT INTO PaymentStatusHistory
            (
                Id,
                PaymentId,
                Status,
                Reason,
                CreatedAt
            )
            VALUES
            (
                @Id,
                @PaymentId,
                @Status,
                @Reason,
                @CreatedAt
            );
        ";

        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(paymentSql,
                new
                {
                    payment.Id,
                    payment.OrderId,
                    payment.UserId,
                    payment.Amount,
                    payment.Currency,
                    Status = (int)payment.Status,
                    payment.IdempotencyKey,
                    payment.StripePaymentIntentId,
                    payment.StripeRefundId,
                    payment.FailureReason,
                    payment.CreatedAt,
                    payment.UpdatedAt
                },
                transaction);

            await connection.ExecuteAsync(historySql,
                new
                {
                    Id = Guid.NewGuid(),
                    PaymentId = payment.Id,
                    Status = (int)payment.Status,
                    Reason = "Payment created.",
                    CreatedAt = payment.CreatedAt
                },
                transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task AttachStripePaymentIntentAsync(
        Guid paymentId,
        string stripePaymentIntentId)
    {
        const string sql = @"
            UPDATE Payments
            SET
                StripePaymentIntentId = @StripePaymentIntentId,
                UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @PaymentId
              AND Status = @PendingStatus
              AND StripePaymentIntentId IS NULL;
            ";

        using var connection = connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(sql,
            new
            {
                PaymentId = paymentId,
                StripePaymentIntentId = stripePaymentIntentId,
                PendingStatus = (int)PaymentStatus.Pending
            });

        if (affectedRows == 0)
            throw new InvalidOperationException("The Stripe PaymentIntent could not be attached to the payment.");
    }

    public async Task MarkRefundedAsync(Guid paymentId, string stripeRefundId)
    {
        const string updateSql = @"
            UPDATE Payments
            SET
                Status = @RefundedStatus,
                StripeRefundId = @StripeRefundId,
                FailureReason = NULL,
                UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @PaymentId
              AND Status = @SucceededStatus;
            ";

        const string historySql = @"
            INSERT INTO PaymentStatusHistory
            (
                Id,
                PaymentId,
                Status,
                Reason,
                CreatedAt
            )
            VALUES
            (
                @Id,
                @PaymentId,
                @Status,
                @Reason,
                SYSUTCDATETIME()
            );
            ";

        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            var affectedRows = await connection.ExecuteAsync(updateSql,
                new
                {
                    PaymentId = paymentId,
                    RefundedStatus = (int)PaymentStatus.Refunded,
                    SucceededStatus = (int)PaymentStatus.Succeeded,
                    StripeRefundId = stripeRefundId
                },
                transaction);

            if (affectedRows == 0)
                throw new InvalidOperationException("Only succeeded payments can be refunded.");

            await connection.ExecuteAsync(historySql,
                new
                {
                    Id = Guid.NewGuid(),
                    PaymentId = paymentId,
                    Status = (int)PaymentStatus.Refunded,
                    Reason = "Payment refunded."
                },
                transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> ApplyWebhookAsync(
        string eventId,
        string eventType,
        string stripePaymentIntentId,
        PaymentStatus status,
        string? failureReason,
        string? stripeRefundId)
    {
        const string insertEventSql = @"
            INSERT INTO StripeWebhookEvents
            (
                EventId,
                EventType,
                ProcessedAt
            )
            VALUES
            (
                @EventId,
                @EventType,
                SYSUTCDATETIME()
            );
            ";

        const string getPaymentSql = @"
            SELECT
                Id,
                OrderId,
                UserId,
                Amount,
                Currency,
                Status,
                IdempotencyKey,
                StripePaymentIntentId,
                StripeRefundId,
                FailureReason,
                CreatedAt,
                UpdatedAt
            FROM Payments
            WHERE StripePaymentIntentId = @StripePaymentIntentId;
            ";

        const string updatePaymentSql = @"
            UPDATE Payments
            SET
                Status = @Status,
                FailureReason = @FailureReason,
                StripeRefundId =
                    CASE
                        WHEN @Status = @RefundedStatus
                            THEN @StripeRefundId
                        ELSE StripeRefundId
                    END,
                UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @PaymentId;
            ";

        const string historySql = @"
            INSERT INTO PaymentStatusHistory
            (
                Id,
                PaymentId,
                Status,
                Reason,
                CreatedAt
            )
            VALUES
            (
                @Id,
                @PaymentId,
                @Status,
                @Reason,
                SYSUTCDATETIME()
            );
            ";

        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            try
            {
                await connection.ExecuteAsync(insertEventSql,
                    new
                    {
                        EventId = eventId,
                        EventType = eventType
                    },
                    transaction);
            }
            catch (SqlException ex) when (ex.Number is 2601 or 2627)
            {
                transaction.Rollback();
                return false;
            }

            var payment = await connection.QuerySingleOrDefaultAsync<Payment>(getPaymentSql,
                new
                {
                    StripePaymentIntentId = stripePaymentIntentId
                },
                transaction);

            if (payment is null)
            {
                transaction.Commit();
                return false;
            }

            if (payment.Status == status)
            {
                transaction.Commit();
                return true;
            }

            if (!IsValidTransition(payment.Status, status))
            {
                transaction.Commit();
                return false;
            }

            await connection.ExecuteAsync(updatePaymentSql,
                new
                {
                    PaymentId = payment.Id,
                    Status = (int)status,
                    FailureReason = failureReason,
                    RefundedStatus = (int)PaymentStatus.Refunded,
                    StripeRefundId = stripeRefundId
                },
                transaction);

            await connection.ExecuteAsync(historySql,
                new
                {
                    Id = Guid.NewGuid(),
                    PaymentId = payment.Id,
                    Status = (int)status,
                    Reason = failureReason
                        ?? $"Stripe webhook: {eventType}"
                },
                transaction);

            transaction.Commit();

            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static bool IsValidTransition(PaymentStatus current, PaymentStatus next)
    {
        return current switch
        {
            PaymentStatus.Pending =>
                next is PaymentStatus.Succeeded or PaymentStatus.Failed,

            PaymentStatus.Failed =>
                next == PaymentStatus.Succeeded,

            PaymentStatus.Succeeded =>
                next == PaymentStatus.Refunded,

            PaymentStatus.Refunded =>
                false,

            _ => false
        };
    }
}