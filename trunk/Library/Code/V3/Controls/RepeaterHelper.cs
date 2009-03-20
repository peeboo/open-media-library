using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.UI;
using System.Reflection;

namespace Library.Code.V3
{
    [MarkupVisible]
    public class RepeaterHelper : ModelItem
    {
        public void ForceRefresh(object Repeater)
        {
            Type myType = Repeater.GetType();
            MethodInfo theMethod = myType.GetMethod("ForceRefresh");
            theMethod.Invoke(Repeater, null);
        }
    }
}
