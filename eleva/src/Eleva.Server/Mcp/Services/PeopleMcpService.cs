using Eleva.Server.Mcp;
using Eleva.Services.Data;
using Eleva.Services.Services.People;
using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.People;
using Microsoft.EntityFrameworkCore;

namespace Eleva.Server.Mcp.Services;

public class PeopleMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "employee_list",
            Description = "Listar colaboradores com filtros",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "search", new McpParameter { Description = "Busca por nome ou e-mail", Required = false } },
                { "departmentId", new McpParameter { Type = "integer", Description = "Filtrar por departamento", Required = false } },
                { "managerId", new McpParameter { Type = "integer", Description = "Filtrar por gestor", Required = false } },
                { "isActive", new McpParameter { Type = "boolean", Description = "Filtrar ativos", Required = false } },
                { "skip", new McpParameter { Type = "integer", Description = "Itens a ignorar", Required = false } },
                { "take", new McpParameter { Type = "integer", Description = "Itens a retornar", Required = false } }
            },
            Handler = async (args, sp) => await sp.GetRequiredService<IEmployeeService>()
                .ListAsync(sp.GetRequiredService<InstanceContext>().InstanceId, McpArgs.StrOrNull(args, "search"), McpArgs.IntOrNull(args, "departmentId"), McpArgs.IntOrNull(args, "managerId"), args.ContainsKey("isActive") ? McpArgs.Bool(args, "isActive") : (bool?)null, McpArgs.Int(args, "skip", 0), McpArgs.Int(args, "take", 50))
        });

        registry.Register(new McpFunction
        {
            Name = "employee_get",
            Description = "Busca colaborador por id",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Id do colaborador", Required = true } }
            },
            Handler = async (args, sp) => await sp.GetRequiredService<IEmployeeService>()
                .GetAsync(sp.GetRequiredService<InstanceContext>().InstanceId, McpArgs.Int(args, "employeeId", 0))
        });

        registry.Register(new McpFunction
        {
            Name = "employee_create",
            Description = "Cria colaborador",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "name", new McpParameter { Type = "string", Description = "Nome", Required = true } },
                { "email", new McpParameter { Type = "string", Description = "E-mail", Required = true } },
                { "departmentId", new McpParameter { Type = "integer", Description = "Departamento", Required = false } },
                { "positionId", new McpParameter { Type = "integer", Description = "Cargo", Required = false } },
                { "managerId", new McpParameter { Type = "integer", Description = "Gestor", Required = false } },
                { "hireDate", new McpParameter { Type = "string", Description = "Data de admissao", Required = false } },
                { "isActive", new McpParameter { Type = "boolean", Description = "Ativo", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IEmployeeService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var employee = new EmployeePO
                {
                    Name = McpArgs.Str(args, "name", string.Empty),
                    Email = McpArgs.Str(args, "email", string.Empty),
                    DepartmentId = McpArgs.IntOrNull(args, "departmentId"),
                    PositionId = McpArgs.IntOrNull(args, "positionId"),
                    ManagerId = McpArgs.IntOrNull(args, "managerId"),
                    HireDate = McpArgs.DateOrNull(args, "hireDate"),
                    Status = args.ContainsKey("isActive") && !McpArgs.Bool(args, "isActive") ? EmploymentStatus.Terminated : EmploymentStatus.Active
                };

                return await service.CreateAsync(instanceId, employee);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "employee_update",
            Description = "Atualiza colaborador",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Id do colaborador", Required = true } },
                { "name", new McpParameter { Type = "string", Description = "Nome", Required = false } },
                { "email", new McpParameter { Type = "string", Description = "E-mail", Required = false } },
                { "departmentId", new McpParameter { Type = "integer", Description = "Departamento", Required = false } },
                { "positionId", new McpParameter { Type = "integer", Description = "Cargo", Required = false } },
                { "managerId", new McpParameter { Type = "integer", Description = "Gestor", Required = false } },
                { "status", new McpParameter { Type = "string", Description = "Status", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IEmployeeService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var statusValue = McpArgs.StrOrNull(args, "status");
                var employee = new EmployeePO
                {
                    Id = McpArgs.Int(args, "employeeId", 0),
                    Name = McpArgs.Str(args, "name", string.Empty),
                    Email = McpArgs.Str(args, "email", string.Empty),
                    DepartmentId = McpArgs.IntOrNull(args, "departmentId"),
                    PositionId = McpArgs.IntOrNull(args, "positionId"),
                    ManagerId = McpArgs.IntOrNull(args, "managerId"),
                    Status = Enum.TryParse<EmploymentStatus>(statusValue, true, out var status) ? status : EmploymentStatus.Active
                };

                return await service.UpdateAsync(instanceId, employee);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "department_list",
            Description = "Lista departamentos",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "search", new McpParameter { Type = "string", Description = "Busca", Required = false } },
                { "isActive", new McpParameter { Type = "boolean", Description = "Filtrar ativos", Required = false } }
            },
            Handler = async (args, sp) => await sp.GetRequiredService<IDepartmentService>()
                .ListAsync(sp.GetRequiredService<InstanceContext>().InstanceId, McpArgs.StrOrNull(args, "search"), args.ContainsKey("isActive") ? McpArgs.Bool(args, "isActive") : (bool?)null)
        });

        registry.Register(new McpFunction
        {
            Name = "department_get",
            Description = "Busca departamento por id",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "departmentId", new McpParameter { Type = "integer", Description = "Id do departamento", Required = true } }
            },
            Handler = async (args, sp) => await sp.GetRequiredService<IDepartmentService>()
                .GetAsync(sp.GetRequiredService<InstanceContext>().InstanceId, McpArgs.Int(args, "departmentId", 0))
        });

        registry.Register(new McpFunction
        {
            Name = "department_create",
            Description = "Cria departamento",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "name", new McpParameter { Type = "string", Description = "Nome", Required = true } },
                { "code", new McpParameter { Type = "string", Description = "Codigo", Required = true } },
                { "parentDepartmentId", new McpParameter { Type = "integer", Description = "Departamento pai", Required = false } },
                { "managerId", new McpParameter { Type = "integer", Description = "Gestor", Required = false } },
                { "description", new McpParameter { Type = "string", Description = "Descricao", Required = false } },
                { "type", new McpParameter { Type = "string", Description = "Tipo", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IDepartmentService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var department = new DepartmentPO
                {
                    Name = McpArgs.Str(args, "name", string.Empty),
                    Code = McpArgs.Str(args, "code", string.Empty),
                    ParentDepartmentId = McpArgs.IntOrNull(args, "parentDepartmentId"),
                    ManagerId = McpArgs.IntOrNull(args, "managerId"),
                    Description = McpArgs.StrOrNull(args, "description"),
                    Type = Enum.TryParse<OrganizationalUnitType>(McpArgs.StrOrNull(args, "type"), true, out var type) ? type : OrganizationalUnitType.Department
                };

                return await service.CreateAsync(instanceId, department);
            }
        });

        registry.Register(new McpFunction
        {
            Name = "position_list",
            Description = "Lista cargos",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "search", new McpParameter { Type = "string", Description = "Busca", Required = false } },
                { "departmentId", new McpParameter { Type = "integer", Description = "Departamento", Required = false } },
                { "isActive", new McpParameter { Type = "boolean", Description = "Filtrar ativos", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var db = sp.GetRequiredService<AppDbContext>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var search = McpArgs.StrOrNull(args, "search");
                var departmentId = McpArgs.IntOrNull(args, "departmentId");
                var isActive = args.ContainsKey("isActive") ? McpArgs.Bool(args, "isActive") : (bool?)null;

                var query = db.Positions.AsNoTracking().Where(x => x.InstanceId == instanceId && x.DeletedAt == null);
                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(x => x.Name.Contains(search) || x.Code.Contains(search));
                if (departmentId.HasValue)
                    query = query.Where(x => x.DepartmentId == departmentId.Value);
                if (isActive.HasValue)
                    query = query.Where(x => x.IsActive == isActive.Value);

                return await query.ToListAsync();
            }
        });

        registry.Register(new McpFunction
        {
            Name = "team_list",
            Description = "Lista times",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "search", new McpParameter { Type = "string", Description = "Busca", Required = false } },
                { "departmentId", new McpParameter { Type = "integer", Description = "Departamento", Required = false } },
                { "leadId", new McpParameter { Type = "integer", Description = "Lider", Required = false } },
                { "isActive", new McpParameter { Type = "boolean", Description = "Filtrar ativos", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var db = sp.GetRequiredService<AppDbContext>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var search = McpArgs.StrOrNull(args, "search");
                var departmentId = McpArgs.IntOrNull(args, "departmentId");
                var leadId = McpArgs.IntOrNull(args, "leadId");
                var isActive = args.ContainsKey("isActive") ? McpArgs.Bool(args, "isActive") : (bool?)null;

                var query = db.Teams.AsNoTracking().Where(x => x.InstanceId == instanceId && x.DeletedAt == null);
                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(x => x.Name.Contains(search) || x.Code.Contains(search));
                if (departmentId.HasValue)
                    query = query.Where(x => x.DepartmentId == departmentId.Value);
                if (leadId.HasValue)
                    query = query.Where(x => x.LeadId == leadId.Value);
                if (isActive.HasValue)
                    query = query.Where(x => x.IsActive == isActive.Value);

                return await query.ToListAsync();
            }
        });

        registry.Register(new McpFunction
        {
            Name = "position_create",
            Description = "Cria cargo",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "name", new McpParameter { Type = "string", Description = "Nome do cargo", Required = true } },
                { "code", new McpParameter { Type = "string", Description = "Codigo do cargo", Required = false } },
                { "description", new McpParameter { Type = "string", Description = "Descricao", Required = false } },
                { "level", new McpParameter { Type = "string", Description = "Nivel", Required = false } },
                { "departmentId", new McpParameter { Type = "integer", Description = "Departamento", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var db = sp.GetRequiredService<AppDbContext>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var name = McpArgs.Str(args, "name", string.Empty);
                var code = McpArgs.StrOrNull(args, "code")
                    ?? name.ToLowerInvariant().Replace(" ", "-");
                var position = new PositionPO
                {
                    InstanceId = instanceId,
                    Name = name,
                    Code = code,
                    Description = McpArgs.StrOrNull(args, "description"),
                    Level = McpArgs.StrOrNull(args, "level"),
                    DepartmentId = McpArgs.IntOrNull(args, "departmentId"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                db.Positions.Add(position);
                await db.SaveChangesAsync();
                return new { position.Id, position.Name, position.Code, position.Level, position.IsActive };
            }
        });

        registry.Register(new McpFunction
        {
            Name = "team_create",
            Description = "Cria time",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "name", new McpParameter { Type = "string", Description = "Nome do time", Required = true } },
                { "code", new McpParameter { Type = "string", Description = "Codigo do time", Required = false } },
                { "description", new McpParameter { Type = "string", Description = "Descricao", Required = false } },
                { "leadId", new McpParameter { Type = "integer", Description = "Id do lider", Required = false } },
                { "departmentId", new McpParameter { Type = "integer", Description = "Departamento", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var db = sp.GetRequiredService<AppDbContext>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                var name = McpArgs.Str(args, "name", string.Empty);
                var code = McpArgs.StrOrNull(args, "code")
                    ?? name.ToLowerInvariant().Replace(" ", "-");
                var team = new TeamPO
                {
                    InstanceId = instanceId,
                    Name = name,
                    Code = code,
                    Description = McpArgs.StrOrNull(args, "description"),
                    LeadId = McpArgs.IntOrNull(args, "leadId"),
                    DepartmentId = McpArgs.IntOrNull(args, "departmentId"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                db.Teams.Add(team);
                await db.SaveChangesAsync();
                return new { team.Id, team.Name, team.Code, team.LeadId, team.IsActive };
            }
        });

        registry.Register(new McpFunction
        {
            Name = "hierarchy_get",
            Description = "Retorna hierarquia",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "employeeId", new McpParameter { Type = "integer", Description = "Id do colaborador", Required = true } }
            },
            Handler = async (args, sp) => await sp.GetRequiredService<IEmployeeService>()
                .GetHierarchyAsync(sp.GetRequiredService<InstanceContext>().InstanceId, McpArgs.Int(args, "employeeId", 0))
        });
    }
}
