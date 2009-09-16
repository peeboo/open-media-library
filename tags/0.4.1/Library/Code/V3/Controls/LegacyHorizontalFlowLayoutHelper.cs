using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Collections;

namespace Library.Code.V3
{

    [MarkupVisible]
    public class LegacyHorizontalFlowLayoutHelper : BaseModelItem
    {
        public void SetLayoutInput(object Panel, object oNewValue)
        {
            return;
            //if (this.weasel != null)
            //{
            //if (this.weasel != null)
            //{
            //    this.weasel.SetProperty("Id", "notFocus");
            //    this.weasel = null;
            //}
            //Type myType = Panel.GetType();
            //MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { oNewValue.GetType() });
            //theMethod.Invoke(Panel, new object[] { oNewValue });
            //if (this.weasel != null)
            //    this.weasel = null;
            return;
    //        //    Type zmyType = Panel.GetType();
    //        //    //PrivateObjectWeasel zweasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.AreaOfInterestLayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

    //        //    MethodInfo ztheMethod = zmyType.GetMethod("MarkLayoutInvalid");
    //        //    ztheMethod.Invoke(Panel, null);
    //        //}

    //        //this.weasel.SetProperty("Id", "notFocus");
    //        //}
            
    //        //return;
    ////            Hashtable PropertiesOfMyObject = new Hashtable();
    ////            Type t = weasel.Instance.GetType();
    ////            PropertyInfo[] pis = t.GetProperties(BindingFlags.FlattenHierarchy |
    ////BindingFlags.IgnoreCase |
    ////BindingFlags.Instance |
    ////BindingFlags.NonPublic |
    ////BindingFlags.Public);
    ////            for (int i = 0; i < pis.Length; i++)
    ////            {
    ////                PropertyInfo pi = (PropertyInfo)pis.GetValue(i);
    ////                PropertiesOfMyObject.Add(pi.Name, pi.GetValue(weasel.Instance, new object[] { }));
    ////            }
    ////        }


    ////        return;
    //        ////////////////////
    //        if (this.weasel != null)
    //        {

    //        }
    //        //Type myType = Panel.GetType();
    //        //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
    //        //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
    //        //MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { oType2, oType });
    //        //theMethod.Invoke(Panel, new object[] { ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject });

    //        Type myType = Panel.GetType();
    ////        MethodInfo mi = weasel.Instance.GetType().GetMethod("get_ILayoutData.Data",
    ////BindingFlags.FlattenHierarchy |
    ////BindingFlags.IgnoreCase |
    ////BindingFlags.Instance |
    ////BindingFlags.NonPublic |
    ////BindingFlags.Public);
    ////        mi.Invoke(weasel.Instance, null);
    //        object o = weasel.Instance.GetType().GetProperty("Microsoft.MediaCenter.UI.ILayoutData.Data", BindingFlags.FlattenHierarchy |
    //BindingFlags.IgnoreCase |
    //BindingFlags.Instance |
    //BindingFlags.NonPublic |
    //BindingFlags.Public|BindingFlags.Static);

    //        object o2 = o.GetType().GetProperty("m_value", BindingFlags.FlattenHierarchy |
    //BindingFlags.IgnoreCase |
    //BindingFlags.Instance |
    //BindingFlags.NonPublic |
    //BindingFlags.Public | BindingFlags.Static);
    //        //ValueType v = (ValueType)o;
    //        //System.ComponentModel.PropertyDescriptorCollection pdc = System.ComponentModel.TypeDescriptor.GetProperties(weasel.Instance);
    //        //foreach (System.ComponentModel.PropertyDescriptor pdcI in pdc)
    //        //{

    //        //}
    //        //PrivateObjectWeasel weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.AreaOfInterestLayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] { "Focus" });

    //        //PrivateObjectWeasel weasel2 = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.ViewItem, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
    //        ////Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            
    //        ////Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            
    //        MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { Type.GetType("Microsoft.MediaCenter.UI.Utility.DataCookie, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), Type.GetType("Microsoft.MediaCenter.UI.ILayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35") });
    //        theMethod.Invoke(Panel, new object[] { o, null });


        }

        private PrivateObjectWeasel weasel;

        public void SetLayoutInput(object Panel)
        {
            //Type myType = Panel.GetType();
            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            //MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { oType2, oType });
            //theMethod.Invoke(Panel, new object[] { ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject });

            Type myType = Panel.GetType();
            weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.LegacyHorizontalFlowLayout, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] {  });
            Panel = weasel.Instance;
            //PropertyInfo pi = myType.GetProperty("Layout", BindingFlags.NonPublic | BindingFlags.Instance);
            //pi.SetValue(Panel, weasel.Instance, null);
    //        Hashtable PropertiesOfMyObject = new Hashtable();
    //        PropertyInfo[] pis = myType.GetProperties(BindingFlags.FlattenHierarchy |
    //BindingFlags.IgnoreCase |
    //BindingFlags.Instance |
    //BindingFlags.NonPublic |
    //BindingFlags.Public);
    //        for (int i = 0; i < pis.Length; i++)
    //        {
    //            PropertyInfo pi = (PropertyInfo)pis.GetValue(i);
    //            PropertiesOfMyObject.Add(pi.Name, pi.GetValue(weasel.Instance, new object[] { }));
    //        }
            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            //MethodInfo theMethod = myType.GetMethod("SetLayout", new Type[] { weasel.Instance.GetType() });
            //theMethod.Invoke(Panel, new object[] { weasel.Instance });


        }

        public void RequestFocus(object Panel)
        {
            //Type myType = Panel.GetType();
            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            //MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { oType2, oType });
            //theMethod.Invoke(Panel, new object[] { ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject });
            Microsoft.MediaCenter.UI.ModelItem mi = (Microsoft.MediaCenter.UI.ModelItem)Panel;
            Type myType = mi.GetType();
            //weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.AreaOfInterestLayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",new object[]{"Focus"});

            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            MethodInfo theMethod = myType.GetMethod("AggressivelyRequestFocus",BindingFlags.Instance|BindingFlags.NonPublic);
            theMethod.Invoke(mi, new object[] {});


        }
    }
}
