using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookWebApi.Utilities
{

    public class AlwaysFailRequirement :
        AuthorizationHandler<AlwaysFailRequirement>, IAuthorizationRequirement
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AlwaysFailRequirement requirement)
        {
            context.Fail();
            return Task.FromResult(0);
        }

    }

}
