using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ThomasMathers.Common.IAM.Tests.Comparers
{
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

        public int GetHashCode([DisallowNull] IdentityError obj)
        {
            return obj.GetHashCode();
        }
    }
}
