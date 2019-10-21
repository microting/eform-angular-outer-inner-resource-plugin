using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Constants;

namespace OuterInnerResource.Pn.Infrastructure.Data.Seed.Data
{
    public class OuterInnerResourcePermissionsSeedData
    {
        public static PluginPermission[] Data => new[]
        {
            new PluginPermission()
            {
                PermissionName = "Access OuterInnerResource Plugin",
                ClaimName = OuterInnerResourceClaims.AccessOuterInnerResourcePlugin
            },
            new PluginPermission()
            {
                PermissionName = "Create Machines",
                ClaimName = OuterInnerResourceClaims.CreateMachines
            },
        };
    }
}
