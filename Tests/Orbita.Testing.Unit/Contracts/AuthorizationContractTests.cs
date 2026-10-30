using Orbita.Contracts.Authorization;
using Xunit;

namespace Orbita.Testing.Contracts;

public sealed class AuthorizationContractTests
{
    [Fact]
    public void Global_roles_are_stable_and_distinct()
    {
        Assert.Equal("Manager", GlobalRoleNames.Manager);
        Assert.Equal("Admin", GlobalRoleNames.Admin);
        Assert.NotEqual(GlobalRoleNames.Manager, GlobalRoleNames.Admin);
    }

    [Fact]
    public void Financial_export_is_a_distinct_capability()
    {
        Assert.Equal("projects.financial.export", CapabilityNames.ProjectFinancialExport);
        Assert.NotEqual(CapabilityNames.ProjectFinancialRead, CapabilityNames.ProjectFinancialExport);
    }
}
