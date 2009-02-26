using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.UI;
using System.Reflection;

namespace Library.Code.V3
{
    [MarkupVisible]
    public class PipHelper : ModelItem
    {

        public void DisablePIP(object PIP)
        {
            Type myType = PIP.GetType();
            PropertyInfo pinfo = myType.GetProperty("VisualState");
            pinfo.SetValue(PIP, PipVisualState.Inactive, null);
        }

        public void EnablePIP(object PIP)
        {
            Type myType = PIP.GetType();
            PropertyInfo pinfo = myType.GetProperty("VisualState");
            pinfo.SetValue(PIP, PipVisualState.Active, null);
        }

    }
}
