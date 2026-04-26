namespace Eleva.Services.Services.People;

using System.Text.Json;
using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.Assessments;
using Eleva.Shared.PersistenceObjects.People;
using Microsoft.EntityFrameworkCore;

public class DiscService : IDiscService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public DiscService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public async Task<DiscAssessmentPO> CreateAssessmentAsync(int instanceId, int employeeId, string assessmentType)
    {
        var assessment = new DiscAssessmentPO
        {
            EmployeeId = employeeId,
            Status = AssessmentStatus.Pending,
            StartedAt = DateTime.UtcNow,
        };
        _db.DiscAssessments.Add(assessment);
        await _db.SaveChangesAsync();
        return assessment;
    }

    public async Task<DiscAssessmentPO> SubmitAssessmentAsync(int instanceId, int assessmentId, DiscResponsePO[] responses)
    {
        var assessment = await _db.DiscAssessments
            .FirstOrDefaultAsync(a => a.Id == assessmentId && a.InstanceId == instanceId)
            ?? throw new InvalidOperationException("Assessment not found.");

        var now = DateTime.UtcNow;
        foreach (var r in responses)
        {
            r.DiscAssessmentId = assessmentId;
            r.AnsweredAt = now;
        }
        _db.DiscResponses.AddRange(responses);

        static decimal WeightOf(DiscResponsePO r) =>
            decimal.TryParse(r.AnswerValue, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : 1m;

        var dScore = responses.Where(r => r.Axis == "D").Sum(WeightOf);
        var iScore = responses.Where(r => r.Axis == "I").Sum(WeightOf);
        var sScore = responses.Where(r => r.Axis == "S").Sum(WeightOf);
        var cScore = responses.Where(r => r.Axis == "C").Sum(WeightOf);

        var scores = new[] { ("D", dScore), ("I", iScore), ("S", sScore), ("C", cScore) };
        var primary = scores.OrderByDescending(x => x.Item2).First().Item1;
        var secondary = scores.OrderByDescending(x => x.Item2).Skip(1).First().Item1;

        var primaryStyle = Enum.Parse<DiscProfile>(primary);
        var secondaryStyle = Enum.Parse<DiscProfile>(secondary);

        var (interpretation, strengths, development, commTips, stressIndicators) = primaryStyle switch
        {
            DiscProfile.D => (
                "Perfil orientado a resultados e desafios",
                new[] { "Decisivo", "Orientado a metas", "Direto" },
                new[] { "Escuta ativa", "Paciência", "Empatia" },
                new[] { "Seja direto", "Foque em resultados", "Evite detalhes excessivos" },
                new[] { "Perda de controle", "Ineficiência", "Dependência" }
            ),
            DiscProfile.I => (
                "Perfil comunicativo e entusiasta",
                new[] { "Comunicação", "Persuasão", "Criatividade" },
                new[] { "Organização", "Follow-through", "Análise" },
                new[] { "Seja entusiasta", "Reconheça publicamente", "Permita socialização" },
                new[] { "Rejeição", "Rotina", "Isolamento" }
            ),
            DiscProfile.S => (
                "Perfil colaborativo e consistente",
                new[] { "Lealdade", "Paciência", "Trabalho em equipe" },
                new[] { "Assertividade", "Adaptação a mudanças", "Velocidade" },
                new[] { "Seja caloroso", "Dê tempo para adaptar", "Evite mudanças bruscas" },
                new[] { "Conflitos", "Instabilidade", "Pressão por velocidade" }
            ),
            _ => (
                "Perfil analítico e preciso",
                new[] { "Precisão", "Análise", "Qualidade" },
                new[] { "Decisão com dados incompletos", "Flexibilidade", "Comunicação" },
                new[] { "Apresente dados", "Seja sistemático", "Dê tempo para análise" },
                new[] { "Críticas", "Improvisação", "Falta de padrões" }
            )
        };

        var result = new DiscResultPO
        {
            DiscAssessmentId = assessmentId,
            DScore = dScore,
            IScore = iScore,
            SScore = sScore,
            CScore = cScore,
            PrimaryStyle = primaryStyle,
            SecondaryStyle = secondaryStyle,
            ProfileCombination = $"{primary}{secondary}",
            Interpretation = interpretation,
            StrengthsJson = JsonSerializer.Serialize(strengths),
            DevelopmentAreasJson = JsonSerializer.Serialize(development),
            CommunicationTipsJson = JsonSerializer.Serialize(commTips),
            StressIndicatorsJson = JsonSerializer.Serialize(stressIndicators),
        };
        _db.DiscResults.Add(result);

        assessment.Status = AssessmentStatus.Completed;
        assessment.PrimaryStyle = primaryStyle;
        assessment.SecondaryStyle = secondaryStyle;
        assessment.CompletedAt = now;
        assessment.UpdatedAt = now;

        await _db.SaveChangesAsync();
        return assessment;
    }

    public async Task<DiscResultPO?> GetResultAsync(int instanceId, int assessmentId)
    {
        return await _db.DiscResults
            .FirstOrDefaultAsync(r => r.DiscAssessmentId == assessmentId && r.InstanceId == instanceId);
    }

    public async Task<object> GetTeamMapAsync(int instanceId, int managerId)
    {
        var managerTeams = await _db.Teams
            .Where(t => t.InstanceId == instanceId && t.LeadId == managerId && t.DeletedAt == null)
            .Select(t => t.Id)
            .ToListAsync();

        var memberEmployeeIds = await _db.TeamMembers
            .Where(m => m.InstanceId == instanceId && managerTeams.Contains(m.TeamId) && m.LeftAt == null)
            .Select(m => m.EmployeeId)
            .Distinct()
            .ToListAsync();

        var results = await _db.DiscAssessments
            .Where(a => a.InstanceId == instanceId && memberEmployeeIds.Contains(a.EmployeeId) && a.Status == AssessmentStatus.Completed)
            .Select(a => new { a.EmployeeId, a.PrimaryStyle })
            .ToListAsync();

        return results;
    }

    public async Task<object> MatchAsync(int instanceId, int employeeId, int targetEmployeeId)
    {
        var result1 = await _db.DiscResults
            .Join(_db.DiscAssessments,
                r => r.DiscAssessmentId,
                a => a.Id,
                (r, a) => new { Result = r, Assessment = a })
            .Where(x => x.Assessment.InstanceId == instanceId && x.Assessment.EmployeeId == employeeId && x.Assessment.Status == AssessmentStatus.Completed)
            .OrderByDescending(x => x.Result.CreatedAt)
            .Select(x => x.Result)
            .FirstOrDefaultAsync();

        var result2 = await _db.DiscResults
            .Join(_db.DiscAssessments,
                r => r.DiscAssessmentId,
                a => a.Id,
                (r, a) => new { Result = r, Assessment = a })
            .Where(x => x.Assessment.InstanceId == instanceId && x.Assessment.EmployeeId == targetEmployeeId && x.Assessment.Status == AssessmentStatus.Completed)
            .OrderByDescending(x => x.Result.CreatedAt)
            .Select(x => x.Result)
            .FirstOrDefaultAsync();

        return new
        {
            Employee1 = new { EmployeeId = employeeId, result1?.PrimaryStyle, result1?.SecondaryStyle, result1?.ProfileCombination },
            Employee2 = new { EmployeeId = targetEmployeeId, result2?.PrimaryStyle, result2?.SecondaryStyle, result2?.ProfileCombination }
        };
    }
}
