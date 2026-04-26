using Eleva.Server.Mcp;
using Eleva.Services.Data;
using Eleva.Services.Services.Performance;
using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Performance;

namespace Eleva.Server.Mcp.Services;

public class PerformanceMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "bsc_perspective_list",
            Description = "Lista perspectivas",
            Annotation = ToolAnnotation.ReadOnly,
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IBscService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var perspectives = await service.ListPerspectivesAsync(instanceId);
                if (perspectives.Count == 0)
                {
                    var db = sp.GetRequiredService<AppDbContext>();
                    var seeds = new[]
                    {
                        new BscPerspectivePO { InstanceId = instanceId, Code = "FIN", Name = "Financial", Description = "Perspectiva Financeira", SortOrder = 1, CreatedAt = DateTime.UtcNow },
                        new BscPerspectivePO { InstanceId = instanceId, Code = "CUS", Name = "Customer", Description = "Perspectiva do Cliente", SortOrder = 2, CreatedAt = DateTime.UtcNow },
                        new BscPerspectivePO { InstanceId = instanceId, Code = "INT", Name = "Internal Processes", Description = "Perspectiva de Processos Internos", SortOrder = 3, CreatedAt = DateTime.UtcNow },
                        new BscPerspectivePO { InstanceId = instanceId, Code = "LRN", Name = "Learning & Growth", Description = "Perspectiva de Aprendizado e Crescimento", SortOrder = 4, CreatedAt = DateTime.UtcNow }
                    };
                    db.BscPerspectives.AddRange(seeds);
                    await db.SaveChangesAsync();
                    return seeds;
                }
                return perspectives;
            }
        });

        registry.Register(new McpFunction
        {
            Name = "bsc_goal_create",
            Description = "Cria meta BSC",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "goal", new McpParameter { Type = "object", Description = "Meta", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IBscService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var goal = McpPayloadBinder.Read<BscGoalPO>(args, "goal") ?? new BscGoalPO();
                return await service.CreateGoalAsync(instanceId, goal);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "bsc_goal_list",
            Description = "Lista metas BSC",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "ownerId", new McpParameter { Type = "integer", Description = "Responsavel", Required = false } },
                { "status", new McpParameter { Type = "string", Description = "Status", Required = false } },
                { "perspectiveId", new McpParameter { Type = "integer", Description = "Perspectiva", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IBscService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var statusText = McpArgs.StrOrNull(args, "status");
                var status = Enum.TryParse<BscStatus>(statusText, true, out var parsed) ? parsed : (BscStatus?)null;
                return await service.ListGoalsAsync(instanceId, McpArgs.IntOrNull(args, "ownerId"), status, McpArgs.IntOrNull(args, "perspectiveId"));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "bsc_indicator_create",
            Description = "Cria indicador",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "indicator", new McpParameter { Type = "object", Description = "Indicador", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IBscService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var indicator = McpPayloadBinder.Read<BscIndicatorPO>(args, "indicator") ?? new BscIndicatorPO();
                return await service.CreateIndicatorAsync(instanceId, indicator);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "bsc_indicator_update",
            Description = "Atualiza indicador",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "indicator", new McpParameter { Type = "object", Description = "Indicador", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IBscService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var indicator = McpPayloadBinder.Read<BscIndicatorPO>(args, "indicator") ?? new BscIndicatorPO();
                return await service.UpdateIndicatorAsync(instanceId, indicator);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "review_create",
            Description = "Cria avaliacao de performance",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "review", new McpParameter { Type = "object", Description = "Avaliacao", Required = true } },
                { "responses", new McpParameter { Type = "array", Description = "Respostas", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IReviewService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var review = McpPayloadBinder.Read<PerformanceReviewPO>(args, "review") ?? new PerformanceReviewPO();
                var responses = McpPayloadBinder.Read<ReviewResponsePO[]?>(args, "responses");
                return await service.CreateAsync(instanceId, review, responses);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "review_submit",
            Description = "Submete avaliacao",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "reviewId", new McpParameter { Type = "integer", Description = "Id da avaliacao", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IReviewService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.SubmitAsync(instanceId, McpArgs.Int(args, "reviewId", 0));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "review_360_create",
            Description = "Cria ciclo 360",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Colaborador", Required = true } },
                { "cycleId", new McpParameter { Type = "integer", Description = "Ciclo", Required = true } },
                { "reviewerId", new McpParameter { Type = "integer", Description = "Id do revisor responsável", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IReviewService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.Create360Async(instanceId, McpArgs.Int(args, "employeeId", 0), McpArgs.Int(args, "cycleId", 0), McpArgs.Int(args, "reviewerId", 0));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "review_list",
            Description = "Lista avaliacoes de performance",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Colaborador", Required = false } },
                { "status", new McpParameter { Type = "string", Description = "Status", Required = false } },
                { "cycleId", new McpParameter { Type = "integer", Description = "Ciclo", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IReviewService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var statusText = McpArgs.StrOrNull(args, "status");
                var status = Enum.TryParse<ReviewStatus>(statusText, true, out var parsed) ? parsed : (ReviewStatus?)null;
                return await service.ListAsync(instanceId, McpArgs.IntOrNull(args, "employeeId"), status, McpArgs.IntOrNull(args, "cycleId"));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "review_cycle_create",
            Description = "Cria ciclo de avaliacao",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "cycle", new McpParameter { Type = "object", Description = "Ciclo", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IReviewService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var cycle = McpPayloadBinder.Read<PerformanceCyclePO>(args, "cycle") ?? new PerformanceCyclePO();
                return await service.CreateCycleAsync(instanceId, cycle);
            }
        });
    }
}
