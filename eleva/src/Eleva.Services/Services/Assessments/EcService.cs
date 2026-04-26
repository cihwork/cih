namespace Eleva.Services.Services.Assessments;

using System.Text.Json;
using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.Assessments;
using Microsoft.EntityFrameworkCore;

public class EcService : IEcService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public EcService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public async Task<EcAssessmentPO> CreateAssessmentAsync(int instanceId, EcAssessmentPO assessment)
    {
        _db.EcAssessments.Add(assessment);
        await _db.SaveChangesAsync();
        return assessment;
    }

    public async Task<EcAssessmentPO> SubmitAssessmentAsync(int instanceId, int assessmentId, EcResponsePO[] responses)
    {
        var assessment = await _db.EcAssessments
            .FirstOrDefaultAsync(a => a.Id == assessmentId && a.InstanceId == instanceId && a.DeletedAt == null)
            ?? throw new InvalidOperationException("Assessment not found.");

        var now = DateTime.UtcNow;
        foreach (var r in responses)
        {
            r.EcAssessmentId = assessmentId;
            r.ResponseDate = now;
        }
        _db.EcResponses.AddRange(responses);

        var energyLevels = responses.Where(r => r.EnergyLevel.HasValue).Select(r => r.EnergyLevel!.Value).ToList();
        var overallScore = energyLevels.Count > 0 ? (decimal)energyLevels.Average() : 0m;

        var maturityLevel = overallScore switch
        {
            <= 1m => EcLevel.Level1,
            <= 2m => EcLevel.Level2,
            <= 3m => EcLevel.Level3,
            <= 4m => EcLevel.Level4,
            <= 5m => EcLevel.Level5,
            <= 6m => EcLevel.Level6,
            _ => EcLevel.Level7
        };

        string[] strengths, risks, recommendations;

        if (maturityLevel <= EcLevel.Level2)
        {
            strengths = ["Abertura ao diálogo"];
            risks = ["Comunicação fragmentada", "Baixa confiança"];
            recommendations = ["Investir em rituais de escuta", "Criar canais seguros"];
        }
        else if (maturityLevel <= EcLevel.Level4)
        {
            strengths = ["Cultura de feedback emergente"];
            risks = ["Consistência", "Liderança ainda reativa"];
            recommendations = ["Estruturar ciclos de feedback", "Treinar lideranças"];
        }
        else
        {
            strengths = ["Cultura consolidada de escuta", "Alta confiança psicológica"];
            risks = ["Manter consistência em escala"];
            recommendations = ["Automatizar pulsos", "Desenvolver multiplicadores"];
        }

        var result = new EcResultPO
        {
            EcAssessmentId = assessmentId,
            OverallScore = overallScore,
            MaturityLevel = maturityLevel,
            StrengthsJson = JsonSerializer.Serialize(strengths),
            RisksJson = JsonSerializer.Serialize(risks),
            RecommendationsJson = JsonSerializer.Serialize(recommendations),
        };
        _db.EcResults.Add(result);

        assessment.Status = AssessmentStatus.Completed;
        assessment.OverallScore = overallScore;
        assessment.UpdatedAt = now;

        await _db.SaveChangesAsync();
        return assessment;
    }

    public async Task<EcResultPO?> GetResultAsync(int instanceId, int assessmentId)
    {
        return await _db.EcResults
            .FirstOrDefaultAsync(r => r.EcAssessmentId == assessmentId && r.InstanceId == instanceId);
    }

    public async Task<object> GetOrgMaturityAsync(int instanceId, int? departmentId = null)
    {
        var assessmentsQuery = _db.EcAssessments
            .Where(a => a.InstanceId == instanceId && a.DeletedAt == null && a.Status == AssessmentStatus.Completed);

        if (departmentId.HasValue)
            assessmentsQuery = assessmentsQuery.Where(a => a.OrganizationalUnitId == departmentId);

        var assessmentIds = await assessmentsQuery.Select(a => a.Id).ToListAsync();

        var results = await _db.EcResults
            .Where(r => r.InstanceId == instanceId && assessmentIds.Contains(r.EcAssessmentId))
            .ToListAsync();

        var grouped = results
            .Join(
                await _db.EcAssessments
                    .Where(a => a.InstanceId == instanceId && a.DeletedAt == null)
                    .ToListAsync(),
                r => r.EcAssessmentId,
                a => a.Id,
                (r, a) => new { r.MaturityLevel, a.OrganizationalUnitId })
            .GroupBy(x => x.OrganizationalUnitId)
            .Select(g => new
            {
                DepartmentId = g.Key,
                AverageMaturityLevel = g.Where(x => x.MaturityLevel.HasValue)
                                        .Select(x => (int)x.MaturityLevel!)
                                        .DefaultIfEmpty(0)
                                        .Average()
            })
            .ToList();

        return new { Departments = grouped };
    }
}
