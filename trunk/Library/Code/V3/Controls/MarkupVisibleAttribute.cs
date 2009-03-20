using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Code.V3
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Event | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class)]
    public sealed class MarkupVisibleAttribute : Attribute
    {
    }
}
