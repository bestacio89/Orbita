using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Franz.Common.Business.Domain;
using Franz.Common.DependencyInjection;
using Franz.Common.Mediator.Messages;
using Franz.Common.Business.Repositories;
using System.Reflection;
using Interface = System.Reflection.TypeInfo;
using Xunit;

namespace Orbita.Testing.ArchitectureTests
{
  public class ContractsArchitecture : BaseArchitectureTest
  {
    [Fact]
    public void Contracts_Assembly_Should_Exist()
    {
      Assert.NotNull(ContractsLayer);
    }

    [Fact]
    public void QueryNameConventionIsCorrect()
    {
      var commandobjects = ContractsLayer.GetObjects(BaseArchitecture)
        .Where(t => t.Name.EndsWith("Query", StringComparison.OrdinalIgnoreCase))
        .ToList();

      if (!commandobjects.Any())
      {
        Console.WriteLine("🟡 No Queries found in Contract layer — skipping rule.");
        return;
      }

      ArchRuleDefinition.Classes().That().AreAssignableTo(typeof(IQuery<>))
        .Should().HaveNameEndingWith("Query")
        .Check(BaseArchitecture);
    }

    [Fact]
    public void CommandNameConventionIsCorrect()
    {
      var commandobjects = ContractsLayer.GetObjects(BaseArchitecture)
        .Where(t => t.Name.EndsWith("Command", StringComparison.OrdinalIgnoreCase))
        .ToList();

      if (!commandobjects.Any())
      {
        Console.WriteLine("🟡 No Commands found in Contract layer — skipping rule.");
        return;
      }

      ArchRuleDefinition.Classes().That().AreAssignableTo(typeof(ICommand<>))
        .Should().HaveNameEndingWith("Command")
        .Check(BaseArchitecture);
    }

    [Fact]
    public void DtosNameConvetionIsCorrect()
    {
      var dtoObjects = ContractsLayer.GetObjects(BaseArchitecture)
        .Where(type => string.Equals(type.Namespace.FullName, "Orbita.Contracts.DTOS", StringComparison.Ordinal))
        .ToList();

      if (dtoObjects.Count == 0)
      {
        Console.WriteLine("🟡 No DTOs found in Orbita.Contracts.DTOS — skipping naming rule.");
        return;
      }

      ArchRuleDefinition.Classes().That()
        .ResideInNamespace("Orbita.Contracts.DTOS")
        .Should().HaveNameEndingWith("Dto")
        .Because("DTOs should use the Dto suffix")
        .WithoutRequiringPositiveResults()
        .Check(BaseArchitecture);
    }

    [Fact]
    public void Infraestructure_Interfaces_Follow_Rules()
    {
      var contractObjects = ContractsLayer.GetObjects(BaseArchitecture)
        .Where(type => type.Name.StartsWith("I") &&
                       !type.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase))
        .ToList();

      if (contractObjects.Count == 0)
      {
        Console.WriteLine("🟡 No custom service interfaces found — skipping lifetime rule.");
        return;
      }

      ArchRuleDefinition.Classes()
        .That().ResideInNamespace("Orbita.Contracts.Infrastructure.*")
        .Should().BeAssignableTo(typeof(IScopedDependency))
        .OrShould().BeAssignableTo(typeof(ISingletonDependency))
        .Because("Every infrastructure interface requires an explicit lifetime")
        .WithoutRequiringPositiveResults()
        .Check(BaseArchitecture);
    }

    [Fact]
    public void Persistence_Interfaces_Follow_Rules()
    {
      var contractObjects = ContractsLayer.GetObjects(BaseArchitecture)
        .Where(type => type.Name.StartsWith("I") &&
                       type.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase))
        .ToList();

      if (contractObjects.Count == 0)
      {
        Console.WriteLine("🟡 No repository interfaces found — skipping persistence contract enforcement.");
        return;
      }

      ArchRuleDefinition.Classes().That().Are(contractObjects)
        .Should().BeAssignableTo(typeof(IScopedDependency))
        .AndShould().NotBeAssignableTo(typeof(IEntityRepository<,>))
        .OrShould().NotBeAssignableTo(typeof(IAggregateRootRepository<,,>))
        .AndShould().HaveNameEndingWith("Repository")
        .Because("Repositories are scoped and custom repositories must not inherit generic framework implementations")
        .WithoutRequiringPositiveResults()
        .Check(BaseArchitecture);

      Console.WriteLine($"✅ Verified {contractObjects.Count} repository interface(s) follow the Orbita persistence contract model.");
    }

    [Fact]
    public void CustomPersistenceInterfaces_Follow_Rules()
    {
      var contractObjects = ContractsLayer.GetObjects(BaseArchitecture)
        .Where(type => type.Name.StartsWith("I") &&
                       type.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase) &&
                       !type.Name.Contains("Aggregate") &&
                       !type.Name.Contains("Read") &&
                       !type.NameContains("Entity"))
        .ToList();

      if (contractObjects.Count == 0)
      {
        Console.WriteLine("🟡 No custom repository interfaces found — skipping persistence contract enforcement.");
        return;
      }

      ArchRuleDefinition.Classes()
        .That().AreNot(typeof(IEntityRepository<,>))
        .And().AreNot(typeof(IAggregateRepository<,>))
        .And().HaveNameEndingWith("Repository")
        .Should().ImplementAnyInterfacesThat()
        .HaveFullNameMatching(typeof(IScopedDependency).FullName!)
        .Because("Custom persistence interfaces should use scoped lifetime management")
        .WithoutRequiringPositiveResults()
        .Check(BaseArchitecture);
    }

    [Fact]
    public void Dtos_Must_Be_Immutable_Or_Records()
    {
      var dtoObjects = ContractsLayer.GetObjects(BaseArchitecture)
        .Where(type => type.Name.EndsWith("Dto", StringComparison.OrdinalIgnoreCase) &&
                       type.Namespace.FullName.Contains("DTOs", StringComparison.OrdinalIgnoreCase))
        .Where(type => !type.Name.StartsWith("Create", StringComparison.OrdinalIgnoreCase) &&
                       !type.Name.StartsWith("Update", StringComparison.OrdinalIgnoreCase))
        .ToList();

      if (dtoObjects.Count == 0)
      {
        Console.WriteLine("🟡 No immutable DTOs found — skipping immutability rule.");
        return;
      }

      ArchRuleDefinition.Classes().That().Are(dtoObjects)
        .Should().BeImmutable()
        .Because("Read DTOs should be immutable records or immutable classes")
        .WithoutRequiringPositiveResults()
        .Check(BaseArchitecture);
    }
  }
}
