using Eleva.Server.Mcp;
using Eleva.Services.Data;
using Eleva.Services.Services.Engagement;
using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Engagement;
using Microsoft.EntityFrameworkCore;

namespace Eleva.Server.Mcp.Services;

public class EngagementMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "pulse_create",
            Description = "Cria pesquisa pulse",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "survey", new McpParameter { Type = "object", Description = "Pesquisa", Required = true } },
                { "questions", new McpParameter { Type = "array", Description = "Perguntas", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IPulseService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var survey = McpPayloadBinder.Read<PulseSurveyPO>(args, "survey") ?? new PulseSurveyPO();
                var questions = McpPayloadBinder.Read<PulseQuestionPO[]?>(args, "questions");
                return await service.CreateAsync(instanceId, survey, questions);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "pulse_respond",
            Description = "Responde pesquisa",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "surveyId", new McpParameter { Type = "integer", Description = "Pesquisa", Required = true } },
                { "response", new McpParameter { Type = "object", Description = "Resposta", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IPulseService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var response = McpPayloadBinder.Read<PulseResponsePO>(args, "response") ?? new PulseResponsePO();
                return await service.RespondAsync(instanceId, McpArgs.Int(args, "surveyId", 0), response);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "pulse_results",
            Description = "Resultados pulse",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "surveyId", new McpParameter { Type = "integer", Description = "Pesquisa", Required = true } },
                { "departmentId", new McpParameter { Type = "integer", Description = "Departamento", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IPulseService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.GetResultsAsync(instanceId, McpArgs.Int(args, "surveyId", 0), McpArgs.IntOrNull(args, "departmentId"));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "feedback_create",
            Description = "Cria feedback",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "feedback", new McpParameter { Type = "object", Description = "Feedback", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IFeedbackService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var feedback = McpPayloadBinder.Read<FeedbackPO>(args, "feedback") ?? new FeedbackPO();
                return await service.CreateAsync(instanceId, feedback);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "feedback_list",
            Description = "Lista feedbacks",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Colaborador", Required = false } },
                { "type", new McpParameter { Type = "string", Description = "Tipo", Required = false } },
                { "visibility", new McpParameter { Type = "string", Description = "Visibilidade", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IFeedbackService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var type = Enum.TryParse<FeedbackType>(McpArgs.StrOrNull(args, "type"), true, out var parsedType) ? parsedType : (FeedbackType?)null;
                var visibility = Enum.TryParse<FeedbackVisibility>(McpArgs.StrOrNull(args, "visibility"), true, out var parsedVisibility) ? parsedVisibility : (FeedbackVisibility?)null;
                return await service.ListAsync(instanceId, McpArgs.IntOrNull(args, "employeeId"), type, visibility);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "checkin_create",
            Description = "Cria check-in",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "checkIn", new McpParameter { Type = "object", Description = "Check-in", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<ICheckInService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var checkIn = McpPayloadBinder.Read<CheckInPO>(args, "checkIn") ?? new CheckInPO();
                return await service.CreateAsync(instanceId, checkIn);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "checkin_list",
            Description = "Lista check-ins",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Colaborador", Required = false } },
                { "from", new McpParameter { Type = "string", Description = "Inicio", Required = false } },
                { "to", new McpParameter { Type = "string", Description = "Fim", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<ICheckInService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.ListAsync(instanceId, McpArgs.IntOrNull(args, "employeeId"), McpArgs.DateOrNull(args, "from")?.ToDateTime(TimeOnly.MinValue), McpArgs.DateOrNull(args, "to")?.ToDateTime(TimeOnly.MinValue));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "mood_register",
            Description = "Registra humor",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "entry", new McpParameter { Type = "object", Description = "Entrada", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var db = sp.GetRequiredService<AppDbContext>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var entry = McpPayloadBinder.Read<MoodEntryPO>(args, "entry") ?? new MoodEntryPO();
                entry.InstanceId = instanceId;
                db.MoodEntries.Add(entry);
                await db.SaveChangesAsync();
                return entry;
            }
        });

        registry.Register(new McpFunction
        {
            Name = "mood_team_summary",
            Description = "Resumo de humor do time",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "managerId", new McpParameter { Type = "integer", Description = "Gestor", Required = true } },
                { "period", new McpParameter { Type = "string", Description = "Periodo", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var db = sp.GetRequiredService<AppDbContext>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var managerId = McpArgs.Int(args, "managerId", 0);
                var period = McpArgs.StrOrNull(args, "period");

                var teamIds = await db.Teams.AsNoTracking()
                    .Where(t => t.InstanceId == instanceId && t.LeadId == managerId && t.DeletedAt == null)
                    .Select(t => t.Id)
                    .ToListAsync();

                var memberIds = await db.TeamMembers.AsNoTracking()
                    .Where(m => m.InstanceId == instanceId && teamIds.Contains(m.TeamId))
                    .Select(m => m.EmployeeId)
                    .ToListAsync();

                var entries = await db.MoodEntries.AsNoTracking()
                    .Where(x => x.InstanceId == instanceId && memberIds.Contains(x.EmployeeId))
                    .OrderByDescending(x => x.EntryAt)
                    .Take(20)
                    .ToListAsync();

                return new
                {
                    managerId,
                    period,
                    total = entries.Count,
                    latest = entries
                };
            }
        });
    }
}
