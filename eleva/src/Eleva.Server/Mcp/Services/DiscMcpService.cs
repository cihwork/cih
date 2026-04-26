using Eleva.Server.Mcp;
using Eleva.Services.Data;
using Eleva.Services.Services.People;
using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Assessments;
using Microsoft.EntityFrameworkCore;

namespace Eleva.Server.Mcp.Services;

public class DiscMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "disc_assessment_create",
            Description = "Cria assessment DISC",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Colaborador", Required = true } },
                { "assessmentType", new McpParameter { Type = "string", Description = "Tipo de assessment", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IDiscService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.CreateAssessmentAsync(instanceId, McpArgs.Int(args, "employeeId", 0), McpArgs.Str(args, "assessmentType", string.Empty));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "disc_assessment_submit",
            Description = "Envia respostas do DISC",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "assessmentId", new McpParameter { Type = "integer", Description = "Id do assessment", Required = true } },
                { "responses", new McpParameter { Type = "array", Description = "Respostas", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IDiscService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var responses = McpPayloadBinder.Read<DiscResponsePO[]?>(args, "responses") ?? Array.Empty<DiscResponsePO>();
                return await service.SubmitAssessmentAsync(instanceId, McpArgs.Int(args, "assessmentId", 0), responses);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "disc_result_get",
            Description = "Retorna resultado do DISC",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "assessmentId", new McpParameter { Type = "integer", Description = "Id do assessment", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IDiscService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.GetResultAsync(instanceId, McpArgs.Int(args, "assessmentId", 0));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "disc_team_map",
            Description = "Mapa DISC do time",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "managerId", new McpParameter { Type = "integer", Description = "Gestor", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IDiscService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.GetTeamMapAsync(instanceId, McpArgs.Int(args, "managerId", 0));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "disc_assessment_list",
            Description = "Lista assessments DISC com filtros opcionais",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Filtrar por colaborador", Required = false } },
                { "status", new McpParameter { Type = "string", Description = "Filtrar por status", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var db = sp.GetRequiredService<AppDbContext>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var employeeId = McpArgs.IntOrNull(args, "employeeId");
                var statusStr = McpArgs.StrOrNull(args, "status");

                var query = db.DiscAssessments.AsNoTracking().Where(a => a.InstanceId == instanceId);

                if (employeeId.HasValue)
                    query = query.Where(a => a.EmployeeId == employeeId.Value);

                if (!string.IsNullOrEmpty(statusStr) && Enum.TryParse<AssessmentStatus>(statusStr, true, out var status))
                    query = query.Where(a => a.Status == status);

                return await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
            }
        });

        registry.Register(new McpFunction
        {
            Name = "disc_matcher",
            Description = "Sugere match comportamental",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Colaborador", Required = true } },
                { "targetEmployeeId", new McpParameter { Type = "integer", Description = "Alvo", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IDiscService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.MatchAsync(instanceId, McpArgs.Int(args, "employeeId", 0), McpArgs.Int(args, "targetEmployeeId", 0));
            }
        });
    }
}
