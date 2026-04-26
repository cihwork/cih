namespace Eleva.Services.Services.Performance;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.Performance;
using Microsoft.EntityFrameworkCore;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public ReviewService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public async Task<PerformanceReviewPO> CreateAsync(int instanceId, PerformanceReviewPO review, IEnumerable<ReviewResponsePO>? responses = null)
    {
        review.Status = ReviewStatus.Draft;
        await _db.PerformanceReviews.AddAsync(review);
        await _db.SaveChangesAsync();

        if (responses != null)
        {
            foreach (var response in responses)
            {
                response.PerformanceReviewId = review.Id;
                await _db.ReviewResponses.AddAsync(response);
            }
            await _db.SaveChangesAsync();
        }

        return review;
    }

    public async Task<PerformanceReviewPO> SubmitAsync(int instanceId, int reviewId)
    {
        var review = await _db.PerformanceReviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.InstanceId == instanceId && r.DeletedAt == null)
            ?? throw new KeyNotFoundException($"Review {reviewId} not found for instance {instanceId}.");

        var responses = await _db.ReviewResponses
            .Where(r => r.PerformanceReviewId == reviewId && r.InstanceId == instanceId)
            .ToListAsync();

        var scores = responses.Where(r => r.Score.HasValue).Select(r => r.Score!.Value).ToList();
        if (scores.Count > 0)
            review.OverallScore = scores.Average();

        review.Status = ReviewStatus.Submitted;
        review.SubmittedAt = DateTime.UtcNow;
        review.ClosedAt = DateTime.UtcNow;
        review.CalibrationScore = review.OverallScore;
        review.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return review;
    }

    public async Task<PerformanceReviewPO> Create360Async(int instanceId, int employeeId, int cycleId, int reviewerId)
    {
        var review = new PerformanceReviewPO
        {
            EmployeeId = employeeId,
            ReviewerId = reviewerId,
            PerformanceCycleId = cycleId,
            ReviewType = "Review360",
            Status = ReviewStatus.Draft,
        };
        await _db.PerformanceReviews.AddAsync(review);
        await _db.SaveChangesAsync();
        return review;
    }

    public async Task<PerformanceCyclePO> CreateCycleAsync(int instanceId, PerformanceCyclePO cycle)
    {
        await _db.PerformanceCycles.AddAsync(cycle);
        await _db.SaveChangesAsync();
        return cycle;
    }

    public async Task<IReadOnlyList<PerformanceReviewPO>> ListAsync(int instanceId, int? employeeId = null, ReviewStatus? status = null, int? cycleId = null)
    {
        var query = _db.PerformanceReviews.Where(r => r.InstanceId == instanceId && r.DeletedAt == null);

        if (employeeId.HasValue)
            query = query.Where(r => r.EmployeeId == employeeId);
        if (status.HasValue)
            query = query.Where(r => r.Status == status);
        if (cycleId.HasValue)
            query = query.Where(r => r.PerformanceCycleId == cycleId);

        return await query.ToListAsync();
    }
}
