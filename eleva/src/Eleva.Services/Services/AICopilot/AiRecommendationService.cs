namespace Eleva.Services.Services.AICopilot;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.AICopilot;
using Eleva.Shared.PersistenceObjects.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class AiRecommendationService : IAiRecommendationService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _aiRouterUrl;
    private readonly string _aiRouterKey;
    private readonly string _aiModel;

    public AiRecommendationService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor, IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
        _httpClientFactory = httpClientFactory;
        _aiRouterUrl = config["AI_ROUTER_URL"] ?? config["AiRouter:Url"] ?? "https://airouter.escaladaonline.com.br";
        _aiRouterKey = config["AI_ROUTER_KEY"] ?? config["AiRouter:Key"] ?? "";
        _aiModel = config["AI_ROUTER_MODEL"] ?? config["AiRouter:Model"] ?? "eleva";
    }

    private async Task<string> CallAiAsync(string systemPrompt, string userPrompt)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AiRouter");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _aiRouterKey);

            var body = JsonSerializer.Serialize(new
            {
                model = _aiModel,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                }
            });

            var response = await client.PostAsync(
                $"{_aiRouterUrl}/v1/chat/completions",
                new StringContent(body, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
                return "";

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";
        }
        catch
        {
            return "";
        }
    }

    public async Task<IReadOnlyList<AiRecommendationPO>> RecommendPdiAsync(int instanceId, int employeeId, int? competencyId = null)
    {
        var existing = _db.AiRecommendations
            .Where(r => r.InstanceId == instanceId && r.EmployeeId == employeeId && r.ContextType == AiContextType.Pdi);

        if (competencyId.HasValue)
            existing = existing.Where(r => r.ContextId == competencyId.Value);

        var results = await existing.OrderByDescending(r => r.CreatedAt).ToListAsync();

        var employee = await _db.Employees.FindAsync(employeeId);
        var employeeName = employee?.Name ?? $"Colaborador {employeeId}";

        var aiPrompt = $"Sugira 3 ações de desenvolvimento profissional para {employeeName}. Retorne JSON: [{{\"titulo\":\"...\",\"descricao\":\"...\",\"tipo\":\"skill|behavior|knowledge\"}}]";
        var aiSystem = "Você é um especialista em RH e desenvolvimento de pessoas. Retorne apenas JSON válido.";
        var aiResponse = await CallAiAsync(aiSystem, aiPrompt);

        if (!string.IsNullOrWhiteSpace(aiResponse))
        {
            try
            {
                using var doc = JsonDocument.Parse(aiResponse.Trim());
                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    var rec = new AiRecommendationPO
                    {
                        InstanceId = instanceId,
                        EmployeeId = employeeId,
                        ContextType = AiContextType.Pdi,
                        ContextId = competencyId,
                        Title = item.GetProperty("titulo").GetString() ?? "Recomendação PDI",
                        Description = item.GetProperty("descricao").GetString() ?? "",
                        RecommendationType = AiRecommendationType.LearningPath,
                        ConfidenceScore = 0.85m,
                        Status = AiRecommendationStatus.Pending
                    };
                    _db.AiRecommendations.Add(rec);
                    results = results.Prepend(rec).ToList();
                }
            }
            catch { }
        }

        _db.AiInteractions.Add(new AiInteractionPO
        {
            EmployeeId = employeeId,
            ContextType = AiContextType.Pdi,
            Prompt = aiPrompt,
            ResponseSummary = aiResponse.Length > 500 ? aiResponse[..500] : aiResponse
        });

        await _db.SaveChangesAsync();
        return results;
    }

    public async Task<IReadOnlyList<AiRecommendationPO>> DiscInsightsAsync(int instanceId, int employeeId)
    {
        var existing = await _db.AiRecommendations
            .Where(r => r.InstanceId == instanceId && r.EmployeeId == employeeId && r.ContextType == AiContextType.Disc)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var discResult = await _db.DiscResults
            .Join(_db.DiscAssessments, r => r.DiscAssessmentId, a => a.Id, (r, a) => new { Result = r, Assessment = a })
            .Where(x => x.Assessment.InstanceId == instanceId && x.Assessment.EmployeeId == employeeId)
            .OrderByDescending(x => x.Result.CreatedAt)
            .Select(x => x.Result)
            .FirstOrDefaultAsync();

        var profileInfo = discResult != null
            ? $"Perfil DISC: D={discResult.DScore} I={discResult.IScore} S={discResult.SScore} C={discResult.CScore}, Estilo principal: {discResult.PrimaryStyle}"
            : "Perfil DISC não disponível";

        var aiPrompt = $"Com base no perfil comportamental do colaborador ({profileInfo}), forneça 2 insights para desenvolvimento. Retorne JSON: [{{\"titulo\":\"...\",\"descricao\":\"...\",\"confianca\":0.0-1.0}}]";
        var aiSystem = "Você é especialista em comportamento organizacional e metodologia DISC. Retorne apenas JSON válido.";
        var aiResponse = await CallAiAsync(aiSystem, aiPrompt);

        var results = existing.ToList();

        if (!string.IsNullOrWhiteSpace(aiResponse))
        {
            try
            {
                using var doc = JsonDocument.Parse(aiResponse.Trim());
                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    var rec = new AiRecommendationPO
                    {
                        InstanceId = instanceId,
                        EmployeeId = employeeId,
                        ContextType = AiContextType.Disc,
                        Title = item.GetProperty("titulo").GetString() ?? "Insight DISC",
                        Description = item.GetProperty("descricao").GetString() ?? "",
                        RecommendationType = AiRecommendationType.ConversationTopic,
                        ConfidenceScore = item.TryGetProperty("confianca", out var conf) ? (decimal)conf.GetDouble() : 0.80m,
                        Status = AiRecommendationStatus.Pending
                    };
                    _db.AiRecommendations.Add(rec);
                    results = results.Prepend(rec).ToList();
                }
            }
            catch { }
        }

        _db.AiInteractions.Add(new AiInteractionPO
        {
            EmployeeId = employeeId,
            ContextType = AiContextType.Disc,
            Prompt = aiPrompt,
            ResponseSummary = aiResponse.Length > 500 ? aiResponse[..500] : aiResponse
        });

        await _db.SaveChangesAsync();
        return results;
    }

    public async Task<object> TeamAnalysisAsync(int instanceId, int managerId, string? period = null)
    {
        var teamIds = await _db.Teams
            .Where(t => t.InstanceId == instanceId && t.LeadId == managerId && t.DeletedAt == null)
            .Select(t => t.Id)
            .ToListAsync();

        var employeeIds = await _db.TeamMembers
            .Where(m => m.InstanceId == instanceId && teamIds.Contains(m.TeamId) && m.LeftAt == null)
            .Select(m => m.EmployeeId)
            .Distinct()
            .ToListAsync();

        var recommendations = await _db.AiRecommendations
            .Where(r => r.InstanceId == instanceId && r.EmployeeId.HasValue && employeeIds.Contains(r.EmployeeId.Value))
            .ToListAsync();

        var aiPrompt = $"Analise um time de {employeeIds.Count} colaboradores com {recommendations.Count} recomendações ativas. Período: {period ?? "atual"}. Identifique 2 oportunidades de melhoria para o gestor. Retorne JSON: {{\"analise\":\"...\",\"oportunidades\":[\"...\"]}}";
        var aiSystem = "Você é consultor sênior de RH e gestão de equipes. Retorne apenas JSON válido.";
        var aiResponse = await CallAiAsync(aiSystem, aiPrompt);

        string teamAnalysis = "";
        List<string> opportunities = new();
        if (!string.IsNullOrWhiteSpace(aiResponse))
        {
            try
            {
                using var doc = JsonDocument.Parse(aiResponse.Trim());
                teamAnalysis = doc.RootElement.TryGetProperty("analise", out var a) ? a.GetString() ?? "" : "";
                if (doc.RootElement.TryGetProperty("oportunidades", out var ops))
                    opportunities = ops.EnumerateArray().Select(o => o.GetString() ?? "").ToList();
            }
            catch { }
        }

        return new
        {
            ManagerId = managerId,
            Period = period,
            TeamSize = employeeIds.Count,
            TotalRecommendations = recommendations.Count,
            PendingRecommendations = recommendations.Count(r => r.Status == AiRecommendationStatus.Pending),
            AcceptedRecommendations = recommendations.Count(r => r.Status == AiRecommendationStatus.Accepted),
            ImplementedRecommendations = recommendations.Count(r => r.Status == AiRecommendationStatus.Implemented),
            ByType = recommendations
                .GroupBy(r => r.RecommendationType.ToString())
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToList(),
            AiAnalysis = teamAnalysis,
            AiOpportunities = opportunities
        };
    }

    public async Task<object> LeadershipTipsAsync(int instanceId, int leaderId, string? context = null)
    {
        var recommendations = await _db.AiRecommendations
            .Where(r => r.InstanceId == instanceId && r.EmployeeId == leaderId)
            .OrderByDescending(r => r.CreatedAt)
            .Take(10)
            .ToListAsync();

        var interactions = await _db.AiInteractions
            .Where(i => i.InstanceId == instanceId && i.EmployeeId == leaderId)
            .OrderByDescending(i => i.CreatedAt)
            .Take(5)
            .ToListAsync();

        var aiPrompt = $"Gere 3 dicas de liderança práticas para um líder. Contexto: {context ?? "liderança geral"}. Retorne JSON: [{{\"dica\":\"...\",\"categoria\":\"comunicacao|delegacao|feedback|motivacao|desenvolvimento\"}}]";
        var aiSystem = "Você é especialista em liderança e desenvolvimento de líderes. Retorne apenas JSON válido.";
        var aiResponse = await CallAiAsync(aiSystem, aiPrompt);

        List<object> tips = new();
        if (!string.IsNullOrWhiteSpace(aiResponse))
        {
            try
            {
                using var doc = JsonDocument.Parse(aiResponse.Trim());
                tips = doc.RootElement.EnumerateArray().Select(t => (object)new
                {
                    Tip = t.TryGetProperty("dica", out var d) ? d.GetString() : "",
                    Category = t.TryGetProperty("categoria", out var c) ? c.GetString() : ""
                }).ToList();
            }
            catch { }
        }

        return new
        {
            LeaderId = leaderId,
            Context = context,
            AiTips = tips,
            RecentRecommendations = recommendations.Select(r => new
            {
                r.Title,
                r.Description,
                r.RecommendationType,
                r.ConfidenceScore,
                r.Status,
                r.CreatedAt
            }).ToList(),
            RecentInteractions = interactions.Select(i => new
            {
                i.ContextType,
                i.ResponseSummary,
                i.CreatedAt
            }).ToList()
        };
    }

    public async Task<IReadOnlyList<QuestionPO>> QuestionSuggestAsync(int instanceId, int? employeeId = null, int? contextId = null)
    {
        var bankQuery = _db.QuestionBanks
            .Where(b => b.InstanceId == instanceId && b.IsActive && b.DeletedAt == null);

        if (contextId.HasValue)
            bankQuery = bankQuery.Where(b => b.Id == contextId.Value);

        var activeBankIds = await bankQuery.Select(b => b.Id).ToListAsync();

        if (activeBankIds.Count == 0)
            activeBankIds = await _db.QuestionBanks
                .Where(b => b.InstanceId == instanceId && b.IsActive && b.DeletedAt == null)
                .Select(b => b.Id)
                .ToListAsync();

        var questionQuery = _db.Questions
            .Where(q => q.InstanceId == instanceId && q.DeletedAt == null && activeBankIds.Contains(q.QuestionBankId));

        if (employeeId.HasValue)
        {
            var answeredIds = await _db.EcResponses
                .Where(r => r.EmployeeId == employeeId.Value)
                .Select(r => r.QuestionId)
                .Distinct()
                .ToListAsync();

            if (answeredIds.Count > 0)
                questionQuery = questionQuery.Where(q => !answeredIds.Contains(q.Id));
        }

        return await questionQuery
            .Include(q => q.Answers)
            .OrderBy(q => q.SortOrder)
            .Take(20)
            .ToListAsync();
    }
}
