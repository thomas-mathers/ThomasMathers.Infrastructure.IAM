using System.Collections.Generic;
using System.Security.Claims;

namespace ThomasMathers.Infrastructure.IAM.Tests.Comparers;

internal class ClaimComparer : IEqualityComparer<Claim>
{
    public bool Equals(Claim? x, Claim? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.Type == y.Type && x.Value == y.Value;
    }

    public int GetHashCode(Claim obj)
    {
        return obj.GetHashCode();
    }
}