using System.Collections.Generic;
using System.Security.Claims;

namespace ThomasMathers.Infrastructure.IAM.Tests.Comparers;

internal class ClaimComparer : IEqualityComparer<Claim>
{
    public bool Equals(Claim? x, Claim? y) => (x == null && y == null) || (x != null && y != null && x.Type == y.Type && x.Value == y.Value);

    public int GetHashCode(Claim obj) => obj.GetHashCode();
}