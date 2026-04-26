using Eleva.Server.Mcp;
using Eleva.Services.Services.Questions;
using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Questions;

namespace Eleva.Server.Mcp.Services;

public class QuestionsMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "question_bank_list",
            Description = "Lista bancos de perguntas",
            Annotation = ToolAnnotation.ReadOnly,
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IQuestionBankService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.ListBanksAsync(instanceId);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "question_list",
            Description = "Lista perguntas",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "bankId", new McpParameter { Type = "integer", Description = "Banco", Required = false } },
                { "search", new McpParameter { Type = "string", Description = "Busca", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IQuestionBankService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.ListQuestionsAsync(instanceId, McpArgs.IntOrNull(args, "bankId"), McpArgs.StrOrNull(args, "search"));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "question_create",
            Description = "Cria pergunta",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "question", new McpParameter { Type = "object", Description = "Pergunta", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IQuestionBankService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var question = McpPayloadBinder.Read<QuestionPO>(args, "question") ?? new QuestionPO();
                return await service.CreateQuestionAsync(instanceId, question);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "question_select_smart",
            Description = "Seleciona perguntas com IA",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "contextType", new McpParameter { Type = "string", Description = "Tipo de contexto", Required = true } },
                { "contextId", new McpParameter { Type = "integer", Description = "Contexto", Required = false } },
                { "take", new McpParameter { Type = "integer", Description = "Quantidade", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IQuestionBankService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var contextType = Enum.TryParse<AiContextType>(McpArgs.StrOrNull(args, "contextType"), true, out var parsed) ? parsed : AiContextType.Pdi;
                return await service.SelectSmartAsync(instanceId, contextType, McpArgs.IntOrNull(args, "contextId"), McpArgs.Int(args, "take", 10));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "question_validate_cross",
            Description = "Valida cruzamento",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "questionId", new McpParameter { Type = "integer", Description = "Pergunta", Required = true } },
                { "targetBankId", new McpParameter { Type = "integer", Description = "Banco alvo", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IQuestionBankService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.ValidateCrossAsync(instanceId, McpArgs.Int(args, "questionId", 0), McpArgs.IntOrNull(args, "targetBankId"));
            }
        });
    }
}
