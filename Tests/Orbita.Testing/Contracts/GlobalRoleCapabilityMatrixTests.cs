using Orbita.Contracts.Authorization;
using Xunit;

namespace Orbita.Testing.Contracts;

public sealed class GlobalRoleCapabilityMatrixTests
{
    [Fact]
    public void Manager_receives_financial_export_capability()
    {
        var capabilities = GlobalRoleCapabilityMatrix.Resolve([GlobalRoleNames.Manager]);

        Assert.Contains(CapabilityNames.ProjectFinancialExport, capabilities);
    }

    [Fact]
    public void Admin_does_not_inherit_manager_financial_export_capability()
    {
        var capabilities = GlobalRoleCapabilityMatrix.Resolve([GlobalRoleNames.Admin]);

        Assert.Contains(CapabilityNames.ProjectFinancialRead, capabilities);
        Assert.DoesNotContain(CapabilityNames.ProjectFinancialExport, capabilities);
    }

    [Fact]
    public void Combined_roles_receive_the_deterministic_union_of_capabilities()
    {
        var first = GlobalRoleCapabilityMatrix.Resolve([GlobalRoleNames.Admin, GlobalRoleNames.Manager]);
        var second = GlobalRoleCapabilityMatrix.Resolve([GlobalRoleNames.Manager, GlobalRoleNames.Admin]);

        Assert.Equal(first, second);
        Assert.Equal(first.Order(StringComparer.Ordinal), first);
    }

    [Fact]
    public void Unknown_roles_do_not_grant_capabilities()
    {
        Assert.Empty(GlobalRoleCapabilityMatrix.Resolve(["UnknownRole"]));
    }

    [Fact]
    public void Null_roles_do_not_grant_capabilities()
    {
        Assert.Empty(GlobalRoleCapabilityMatrix.Resolve(null));
    }
}
