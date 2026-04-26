using Eleva.Server.Mcp;
using Eleva.Services.Data;
using Eleva.Services.Services.Assessments;
using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Assessments;
using Microsoft.EntityFrameworkCore;

namespace Eleva.Server.Mcp.Services;

public class EcMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "ec_assessment_create",
            Description = "Cria avaliacao EC",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "assessment", new McpParameter { Type = "object", Description = "Assessment EC", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IEcService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var assessment = McpPayloadBinder.Read<EcAssessmentPO>(args, "assessment") ?? new EcAssessmentPO();
                return await service.CreateAssessmentAsync(instanceId, assessment);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "ec_assessment_submit",
            Description = "Envia respostas EC",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "assessmentId", new McpParameter { Type = "integer", Description = "Id do assessment", Required = true } },
                { "responses", new McpParameter { Type = "array", Description = "Respostas", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IEcService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var responses = McpPayloadBinder.Read<EcResponsePO[]?>(args, "responses") ?? Array.Empty<EcResponsePO>();
                return await service.SubmitAssessmentAsync(instanceId, McpArgs.Int(args, "assessmentId", 0), responses);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "ec_assessment_list",
            Description = "Lista assessments EC com filtros opcionais",
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

                var query = db.EcAssessments.AsNoTracking()
                    .Where(a => a.InstanceId == instanceId && a.DeletedAt == null);

                if (employeeId.HasValue)
                {
                    var assessmentIds = db.EcResponses
                        .Where(r => r.EmployeeId == employeeId.Value)
                        .Select(r => r.EcAssessmentId)
                        .Distinct();
                    query = query.Where(a => assessmentIds.Contains(a.Id));
                }

                if (!string.IsNullOrEmpty(statusStr) && Enum.TryParse<AssessmentStatus>(statusStr, true, out var status))
                    query = query.Where(a => a.Status == status);

                return await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
            }
        });

        registry.Register(new McpFunction
        {
            Name = "ec_result_get",
            Description = "Retorna resultado EC",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "assessmentId", new McpParameter { Type = "integer", Description = "Id do assessment", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IEcService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.GetResultAsync(instanceId, McpArgs.Int(args, "assessmentId", 0));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "ec_org_maturity",
            Description = "Maturidade organizacional",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "departmentId", new McpParameter { Type = "integer", Description = "Departamento", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IEcService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.GetOrgMaturityAsync(instanceId, McpArgs.IntOrNull(args, "departmentId"));
            }
        });
    }
}
