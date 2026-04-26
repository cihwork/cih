using Eleva.Server.Mcp;
using Eleva.Services.Services.AICopilot;

namespace Eleva.Server.Mcp.Services;

public class AiCopilotMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "ai_pdi_recommend",
            Description = "Recomenda acoes de PDI",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Colaborador", Required = true } },
                { "competencyId", new McpParameter { Type = "integer", Description = "Competencia", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IAiRecommendationService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.RecommendPdiAsync(instanceId, McpArgs.Int(args, "employeeId", 0), McpArgs.IntOrNull(args, "competencyId"));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "ai_disc_insights",
            Description = "Insights DISC",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Colaborador", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IAiRecommendationService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.DiscInsightsAsync(instanceId, McpArgs.Int(args, "employeeId", 0));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "ai_team_analysis",
            Description = "Analise do time",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "managerId", new McpParameter { Type = "integer", Description = "Gestor", Required = true } },
                { "period", new McpParameter { Type = "string", Description = "Periodo", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IAiRecommendationService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.TeamAnalysisAsync(instanceId, McpArgs.Int(args, "managerId", 0), McpArgs.StrOrNull(args, "period"));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "ai_leadership_tips",
            Description = "Dicas de lideranca",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "leaderId", new McpParameter { Type = "integer", Description = "Lider", Required = true } },
                { "context", new McpParameter { Type = "string", Description = "Contexto", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IAiRecommendationService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.LeadershipTipsAsync(instanceId, McpArgs.Int(args, "leaderId", 0), McpArgs.StrOrNull(args, "context"));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "ai_question_suggest",
            Description = "Sugere perguntas",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Colaborador", Required = false } },
                { "contextId", new McpParameter { Type = "integer", Description = "Contexto", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IAiRecommendationService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.QuestionSuggestAsync(instanceId, McpArgs.IntOrNull(args, "employeeId"), McpArgs.IntOrNull(args, "contextId"));
            }
        });
    }
}
