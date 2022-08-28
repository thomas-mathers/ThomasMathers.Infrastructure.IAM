using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;

namespace ThomasMathers.Infrastructure.IAM.Tests.Comparers;

internal class IdentityErrorComparer : IEqualityComparer<IdentityError>
{
    public bool Equals(IdentityError? x, IdentityError? y) => (x == null && y == null) || (x != null && y != null && x.Code == y.Code);

    public int GetHashCode(IdentityError obj) => obj.GetHashCode();
}