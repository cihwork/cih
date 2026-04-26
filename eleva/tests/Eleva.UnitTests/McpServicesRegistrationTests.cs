namespace Eleva.UnitTests;

using Eleva.Server.Mcp;
using Eleva.Server.Mcp.Services;

public class McpServicesRegistrationTests
{
    [Fact]
    public void PeopleMcpService_RegistersExpectedTools()
    {
        var registry = new McpServiceRegistry();
        new PeopleMcpService().RegisterTools(registry);
        var tools = registry.GetAllTools();
        Assert.True(tools.ContainsKey("employee_list"));
        Assert.True(tools.ContainsKey("employee_get"));
        Assert.True(tools.ContainsKey("employee_create"));
        Assert.True(tools.ContainsKey("employee_update"));
        Assert.True(tools.ContainsKey("department_list"));
        Assert.True(tools.ContainsKey("department_create"));
        Assert.True(tools.ContainsKey("position_list"));
        Assert.True(tools.ContainsKey("position_create"));
        Assert.True(tools.ContainsKey("team_list"));
        Assert.True(tools.ContainsKey("team_create"));
        Assert.True(tools.ContainsKey("hierarchy_get"));
    }

    [Fact]
    public void DiscMcpService_RegistersExpectedTools()
    {
        var registry = new McpServiceRegistry();
        new DiscMcpService().RegisterTools(registry);
        var tools = registry.GetAllTools();
        Assert.True(tools.ContainsKey("disc_assessment_create"));
        Assert.True(tools.ContainsKey("disc_assessment_submit"));
        Assert.True(tools.ContainsKey("disc_result_get"));
        Assert.True(tools.ContainsKey("disc_team_map"));
        Assert.True(tools.ContainsKey("disc_matcher"));
    }

    [Fact]
    public void EcMcpService_RegistersExpectedTools()
    {
        var registry = new McpServiceRegistry();
        new EcMcpService().RegisterTools(registry);
        var tools = registry.GetAllTools();
        Assert.True(tools.ContainsKey("ec_assessment_create"));
        Assert.True(tools.ContainsKey("ec_assessment_submit"));
        Assert.True(tools.ContainsKey("ec_result_get"));
        Assert.True(tools.ContainsKey("ec_org_maturity"));
    }

    [Fact]
    public void PdiMcpService_RegistersExpectedTools()
    {
        var registry = new McpServiceRegistry();
        new PdiMcpService().RegisterTools(registry);
        var tools = registry.GetAllTools();
        Assert.True(tools.ContainsKey("pdi_plan_create"));
        Assert.True(tools.ContainsKey("pdi_plan_get"));
        Assert.True(tools.ContainsKey("pdi_plan_list"));
        Assert.True(tools.ContainsKey("pdi_goal_create"));
        Assert.True(tools.ContainsKey("pdi_action_create"));
        Assert.True(tools.ContainsKey("pdi_checkpoint_create"));
        Assert.True(tools.ContainsKey("pdi_evidence_upload"));
        Assert.True(tools.ContainsKey("pdi_cycle_create"));
        Assert.True(tools.ContainsKey("pdi_cycle_list"));
        Assert.True(tools.ContainsKey("pdi_progress_summary"));
    }

    [Fact]
    public void PerformanceMcpService_RegistersExpectedTools()
    {
        var registry = new McpServiceRegistry();
        new PerformanceMcpService().RegisterTools(registry);
        var tools = registry.GetAllTools();
        Assert.True(tools.ContainsKey("bsc_perspective_list"));
        Assert.True(tools.ContainsKey("bsc_goal_create"));
        Assert.True(tools.ContainsKey("bsc_goal_list"));
        Assert.True(tools.ContainsKey("bsc_indicator_create"));
        Assert.True(tools.ContainsKey("bsc_indicator_update"));
        Assert.True(tools.ContainsKey("review_create"));
        Assert.True(tools.ContainsKey("review_submit"));
        Assert.True(tools.ContainsKey("review_360_create"));
        Assert.True(tools.ContainsKey("review_cycle_create"));
    }

    [Fact]
    public void EngagementMcpService_RegistersExpectedTools()
    {
        var registry = new McpServiceRegistry();
        new EngagementMcpService().RegisterTools(registry);
        var tools = registry.GetAllTools();
        Assert.True(tools.ContainsKey("pulse_create"));
        Assert.True(tools.ContainsKey("pulse_respond"));
        Assert.True(tools.ContainsKey("pulse_results"));
        Assert.True(tools.ContainsKey("feedback_create"));
        Assert.True(tools.ContainsKey("feedback_list"));
        Assert.True(tools.ContainsKey("checkin_create"));
        Assert.True(tools.ContainsKey("checkin_list"));
        Assert.True(tools.ContainsKey("mood_register"));
        Assert.True(tools.ContainsKey("mood_team_summary"));
    }

    [Fact]
    public void AiCopilotMcpService_RegistersExpectedTools()
    {
        var registry = new McpServiceRegistry();
        new AiCopilotMcpService().RegisterTools(registry);
        var tools = registry.GetAllTools();
        Assert.True(tools.ContainsKey("ai_pdi_recommend"));
        Assert.True(tools.ContainsKey("ai_disc_insights"));
        Assert.True(tools.ContainsKey("ai_team_analysis"));
        Assert.True(tools.ContainsKey("ai_leadership_tips"));
        Assert.True(tools.ContainsKey("ai_question_suggest"));
    }

    [Fact]
    public void QuestionsMcpService_RegistersExpectedTools()
    {
        var registry = new McpServiceRegistry();
        new QuestionsMcpService().RegisterTools(registry);
        var tools = registry.GetAllTools();
        Assert.True(tools.ContainsKey("question_bank_list"));
        Assert.True(tools.ContainsKey("question_list"));
        Assert.True(tools.ContainsKey("question_create"));
        Assert.True(tools.ContainsKey("question_select_smart"));
        Assert.True(tools.ContainsKey("question_validate_cross"));
    }

    [Fact]
    public void AuthMcpService_RegistersExpectedTools()
    {
        var registry = new McpServiceRegistry();
        new AuthMcpService().RegisterTools(registry);
        var tools = registry.GetAllTools();
        Assert.True(tools.ContainsKey("auth_login"));
        Assert.True(tools.ContainsKey("auth_refresh"));
        Assert.True(tools.ContainsKey("instance_create"));
        Assert.True(tools.ContainsKey("instance_list"));
    }

    [Fact]
    public void AuditMcpService_RegistersExpectedTools()
    {
        var registry = new McpServiceRegistry();
        new AuditMcpService().RegisterTools(registry);
        var tools = registry.GetAllTools();
        Assert.True(tools.ContainsKey("audit_log_list"));
        Assert.True(tools.ContainsKey("audit_log_search"));
    }

    [Fact]
    public void ConfigMcpService_RegistersExpectedTools()
    {
        var registry = new McpServiceRegistry();
        new ConfigMcpService().RegisterTools(registry);
        var tools = registry.GetAllTools();
        Assert.True(tools.ContainsKey("config_get"));
        Assert.True(tools.ContainsKey("config_set"));
        Assert.True(tools.ContainsKey("config_list"));
    }

    [Fact]
    public void SystemMcpService_RegistersExpectedTools()
    {
        var registry = new McpServiceRegistry();
        new SystemMcpService().RegisterTools(registry);
        var tools = registry.GetAllTools();
        Assert.True(tools.ContainsKey("system_version"));
        Assert.True(tools.ContainsKey("system_health"));
    }

    [Fact]
    public void AllMcpServices_Register67Tools()
    {
        var registry = new McpServiceRegistry();
        IMcpService[] services =
        [
            new PeopleMcpService(), new DiscMcpService(), new EcMcpService(),
            new PdiMcpService(), new PerformanceMcpService(), new EngagementMcpService(),
            new AiCopilotMcpService(), new QuestionsMcpService(), new AuthMcpService(),
            new AuditMcpService(), new ConfigMcpService(), new SystemMcpService(),
        ];
        foreach (var svc in services)
            svc.RegisterTools(registry);
        Assert.Equal(74, registry.GetAllTools().Count);
    }

    [Fact]
    public void McpServiceRegistry_GetToolsByAnnotation_FiltersCorrectly()
    {
        var registry = new McpServiceRegistry();
        new PeopleMcpService().RegisterTools(registry);
        new SystemMcpService().RegisterTools(registry);

        var readOnly = registry.GetToolsByAnnotation(ToolAnnotation.ReadOnly).ToList();
        var mutating = registry.GetToolsByAnnotation(ToolAnnotation.Mutating).ToList();

        Assert.NotEmpty(readOnly);
        Assert.NotEmpty(mutating);
        Assert.True(readOnly.All(t => t.Annotation == ToolAnnotation.ReadOnly));
        Assert.True(mutating.All(t => t.Annotation == ToolAnnotation.Mutating));
    }
}
