using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJsonApi.Infrastructure
{
    public class IdAndTypeComparer<T> : IEqualityComparer<T>
    {
        private Func<T, object> IdGetter;

        public IdAndTypeComparer(Func<T, object> idGetter)
        {
            IdGetter = idGetter;
        }

        public bool Equals(T x, T y)
        {
            if (x == null ^ y == null)
                return false;
            if (x == null && y == null)
                return true;
            return x.GetType() == y.GetType() && IdGetter(x).Equals(IdGetter(y));
        }

        public int GetHashCode(T obj)
        {
            return obj == null ? 0 : obj.GetType().GetHashCode() * 13 + IdGetter(obj).GetHashCode();
        }
    }
}