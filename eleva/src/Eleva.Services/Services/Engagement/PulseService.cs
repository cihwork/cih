namespace Eleva.Services.Services.Engagement;

using Eleva.Services.Data;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.Engagement;
using Microsoft.EntityFrameworkCore;

public class PulseService : IPulseService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public PulseService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public async Task<PulseSurveyPO> CreateAsync(int instanceId, PulseSurveyPO survey, IEnumerable<PulseQuestionPO>? questions = null)
    {
        await _db.PulseSurveys.AddAsync(survey);
        await _db.SaveChangesAsync();

        if (questions != null)
        {
            foreach (var question in questions)
            {
                question.PulseSurveyId = survey.Id;
            }
            await _db.PulseQuestions.AddRangeAsync(questions);
            await _db.SaveChangesAsync();
        }

        return survey;
    }

    public async Task<PulseResponsePO> RespondAsync(int instanceId, int surveyId, PulseResponsePO response)
    {
        response.PulseSurveyId = surveyId;

        if (response.EmployeeId.HasValue)
        {
            var alreadyResponded = await _db.PulseResponses
                .AnyAsync(r => r.PulseSurveyId == surveyId && r.EmployeeId == response.EmployeeId.Value);
            if (alreadyResponded)
                throw new InvalidOperationException("Funcionário já respondeu este survey.");
        }

        if (response.ResponseDate == default)
            response.ResponseDate = DateTime.UtcNow;

        await _db.PulseResponses.AddAsync(response);
        await _db.SaveChangesAsync();

        return response;
    }

    public async Task<object> GetResultsAsync(int instanceId, int surveyId, int? departmentId = null)
    {
        var survey = await _db.PulseSurveys
            .FirstOrDefaultAsync(s => s.Id == surveyId && s.InstanceId == instanceId && s.DeletedAt == null);

        var responsesQuery = _db.PulseResponses
            .Where(r => r.PulseSurveyId == surveyId && r.InstanceId == instanceId);

        if (departmentId.HasValue)
        {
            var employeeIdsInDept = await _db.Employees
                .Where(e => e.InstanceId == instanceId && e.DepartmentId == departmentId.Value)
                .Select(e => e.Id)
                .ToListAsync();
            responsesQuery = responsesQuery.Where(r => r.EmployeeId.HasValue && employeeIdsInDept.Contains(r.EmployeeId.Value));
        }

        var responses = await responsesQuery.ToListAsync();

        var totalResponses = responses.Count;

        var avgEnergy = responses.Any(r => r.EnergyLevel.HasValue)
            ? responses.Where(r => r.EnergyLevel.HasValue).Average(r => r.EnergyLevel!.Value)
            : (double?)null;

        var sentimentDistribution = responses
            .Where(r => r.OverallSentiment.HasValue)
            .GroupBy(r => r.OverallSentiment!.Value)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        return new
        {
            SurveyId = surveyId,
            Survey = survey,
            DepartmentId = departmentId,
            TotalResponses = totalResponses,
            AverageEnergyLevel = avgEnergy,
            SentimentDistribution = sentimentDistribution
        };
    }
}
