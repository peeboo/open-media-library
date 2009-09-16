using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.UI;
using System.Reflection;

namespace Library.Code.V3
{
    [MarkupVisible]
    public class PipHelper : BaseModelItem
    {
        private Type myType;
        private PropertyInfo pinfo;
        public void DisablePIP(object PIP)
        {
            this.setValues(PIP);
            //Type myType = PIP.GetType();
            //PropertyInfo pinfo = myType.GetProperty("VisualState");
            pinfo.SetValue(PIP, PipVisualState.Inactive, null);
        }

        public void EnablePIP(object PIP)
        {
            this.setValues(PIP);
            //Type myType = PIP.GetType();
            //PropertyInfo pinfo = myType.GetProperty("VisualState");
            pinfo.SetValue(PIP, PipVisualState.Active, null);
        }

        public void setValues(object PIP)
        {
            if (myType == null)
            {
                myType = PIP.GetType();
                pinfo = myType.GetProperty("VisualState");
            }
        }
    }
}
