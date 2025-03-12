using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Domain.Comm
{
    public static class CurrentObj<T>
    {
        public static T SetValue<KT>(Action<KT> setter, KT value)
        {
            setter(value);
            return default!;
        }
    }
}
