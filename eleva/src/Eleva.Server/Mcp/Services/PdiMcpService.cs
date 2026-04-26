using Eleva.Server.Mcp;
using Eleva.Services.Data;
using Eleva.Services.Services.Pdi;
using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Pdi;
using Microsoft.EntityFrameworkCore;

namespace Eleva.Server.Mcp.Services;

public class PdiMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "pdi_plan_create",
            Description = "Cria plano PDI",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "plan", new McpParameter { Type = "object", Description = "Plano", Required = true } },
                { "goals", new McpParameter { Type = "array", Description = "Metas", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IPdiService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var plan = McpPayloadBinder.Read<PdiPlanPO>(args, "plan") ?? new PdiPlanPO();
                var goals = McpPayloadBinder.Read<PdiGoalPO[]?>(args, "goals");
                return await service.CreatePlanAsync(instanceId, plan, goals);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "pdi_plan_get",
            Description = "Busca plano PDI",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "pdiPlanId", new McpParameter { Type = "integer", Description = "Id do plano", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IPdiService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.GetAsync(instanceId, McpArgs.Int(args, "pdiPlanId", 0));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "pdi_plan_list",
            Description = "Lista planos PDI",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Colaborador", Required = false } },
                { "status", new McpParameter { Type = "string", Description = "Status", Required = false } },
                { "cycle", new McpParameter { Type = "string", Description = "Ciclo", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IPdiService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var statusText = McpArgs.StrOrNull(args, "status");
                var status = Enum.TryParse<PdiStatus>(statusText, true, out var parsed) ? parsed : (PdiStatus?)null;
                return await service.ListAsync(instanceId, McpArgs.IntOrNull(args, "employeeId"), status, McpArgs.StrOrNull(args, "cycle"));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "pdi_goal_create",
            Description = "Cria meta PDI",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "pdiPlanId", new McpParameter { Type = "integer", Description = "Plano", Required = true } },
                { "goal", new McpParameter { Type = "object", Description = "Meta", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IPdiService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var goal = McpPayloadBinder.Read<PdiGoalPO>(args, "goal") ?? new PdiGoalPO();
                return await service.CreateGoalAsync(instanceId, McpArgs.Int(args, "pdiPlanId", 0), goal);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "pdi_action_create",
            Description = "Cria acao PDI",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "pdiGoalId", new McpParameter { Type = "integer", Description = "Meta", Required = true } },
                { "action", new McpParameter { Type = "object", Description = "Acao", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IPdiService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var action = McpPayloadBinder.Read<PdiActionPO>(args, "action") ?? new PdiActionPO();
                return await service.CreateActionAsync(instanceId, McpArgs.Int(args, "pdiGoalId", 0), action);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "pdi_checkpoint_create",
            Description = "Cria checkpoint",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "pdiPlanId", new McpParameter { Type = "integer", Description = "Plano", Required = true } },
                { "pdiGoalId", new McpParameter { Type = "integer", Description = "Meta", Required = false } },
                { "checkpoint", new McpParameter { Type = "object", Description = "Checkpoint", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IPdiService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var checkpoint = McpPayloadBinder.Read<PdiCheckpointPO>(args, "checkpoint") ?? new PdiCheckpointPO();
                return await service.CreateCheckpointAsync(instanceId, McpArgs.Int(args, "pdiPlanId", 0), McpArgs.IntOrNull(args, "pdiGoalId") ?? 0, checkpoint);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "pdi_evidence_upload",
            Description = "Anexa evidencias",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "pdiActionId", new McpParameter { Type = "integer", Description = "Acao", Required = true } },
                { "evidence", new McpParameter { Type = "object", Description = "Evidencia", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IPdiService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var evidence = McpPayloadBinder.Read<PdiEvidencePO>(args, "evidence") ?? new PdiEvidencePO();
                return await service.UploadEvidenceAsync(instanceId, McpArgs.Int(args, "pdiActionId", 0), evidence);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "pdi_cycle_create",
            Description = "Cria ciclo PDI",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "cycle", new McpParameter { Type = "object", Description = "Ciclo", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IPdiCycleService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var cycle = McpPayloadBinder.Read<PdiCyclePO>(args, "cycle") ?? new PdiCyclePO();
                return await service.CreateAsync(instanceId, cycle);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "pdi_cycle_list",
            Description = "Lista ciclos PDI",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Colaborador", Required = false } },
                { "status", new McpParameter { Type = "string", Description = "Status", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IPdiCycleService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var statusText = McpArgs.StrOrNull(args, "status");
                var status = Enum.TryParse<CycleStatus>(statusText, true, out var parsed) ? parsed : (CycleStatus?)null;
                return await service.ListAsync(instanceId, McpArgs.IntOrNull(args, "employeeId"), status);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "pdi_plan_update",
            Description = "Atualiza status do plano PDI",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "planId", new McpParameter { Type = "integer", Description = "Id do plano", Required = true } },
                { "status", new McpParameter { Type = "string", Description = "Novo status", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var db = sp.GetRequiredService<AppDbContext>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var planId = McpArgs.Int(args, "planId", 0);
                var statusText = McpArgs.Str(args, "status", string.Empty);
                var plan = await db.PdiPlans.FirstOrDefaultAsync(p => p.Id == planId && p.InstanceId == instanceId && p.DeletedAt == null)
                    ?? throw new InvalidOperationException("Plano não encontrado.");
                if (Enum.TryParse<PdiStatus>(statusText, true, out var parsed))
                    plan.Status = parsed;
                plan.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
                return plan;
            }
        });

        registry.Register(new McpFunction
        {
            Name = "pdi_progress_summary",
            Description = "Resumo de progresso",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "pdiPlanId", new McpParameter { Type = "integer", Description = "Plano", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IPdiService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.GetProgressSummaryAsync(instanceId, McpArgs.Int(args, "pdiPlanId", 0));
            }
        });
    }
}
