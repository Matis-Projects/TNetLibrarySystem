using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tismatis.TNetLibrarySystem.Coding
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NetworkedMethod : Attribute
    {
        public NetworkedMethod() { }
    }
}
