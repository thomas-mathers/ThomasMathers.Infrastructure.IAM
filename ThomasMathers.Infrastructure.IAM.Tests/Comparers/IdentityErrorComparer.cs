using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ThomasMathers.Infrastructure.IAM.Tests.Comparers;

internal class IdentityErrorComparer : IEqualityComparer<IdentityError>
{
    public bool Equals(IdentityError? x, IdentityError? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.Code == y.Code;
    }

    public int GetHashCode(IdentityError obj)
    {
        return obj.GetHashCode();
    }
}