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

        private PrivateObjectWeasel weasel;
        public void SetLayoutInput(object Panel)
        {
            //Type myType = Panel.GetType();
            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            //MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { oType2, oType });
            //theMethod.Invoke(Panel, new object[] { ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject });

            Type myType = Panel.GetType();
            weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.FlowSizeMemoryLayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[]{});

            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { weasel.Instance.GetType() });
            theMethod.Invoke(Panel, new object[] { weasel.Instance });


        }

        private PrivateObjectWeasel weasel2;
        public void SetScrollingLayout(object Panel)
        {
            //Type myType = Panel.GetType();
            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            //MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { oType2, oType });
            //theMethod.Invoke(Panel, new object[] { ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject });

            Type myType = Panel.GetType();
            weasel2 = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.ScrollingLayout, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] { });
            //PropertyInfo inf = weasel2.GetType().GetProperty("Orientation");
            //inf.SetValue(weasel2, (object)"Horizontal", null);

            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            //MethodInfo theMethod = myType.GetMethod("SetLayout", new Type[] { weasel2.Instance.GetType() });
            //theMethod.Invoke(Panel, new object[] { weasel2.Instance });

            PropertyInfo inf = myType.GetProperty("Layout", BindingFlags.Public | BindingFlags.Instance);
            inf.SetValue(Panel, (object)weasel2.Instance, null);
        }

        public void SetFlowLayout(object Panel)
        {
            //Type myType = Panel.GetType();
            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            //MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { oType2, oType });
            //theMethod.Invoke(Panel, new object[] { ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject });

            Type myType = Panel.GetType();
            //weasel2 = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.FlowSizeMemoryLayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] { });
            PropertyInfo inf = myType.GetProperty("StopOnEmptyItem");
            inf.SetValue(Panel, (object)true, null);

            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            //MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { weasel.Instance.GetType() });
            //theMethod.Invoke(Panel, new object[] { weasel2.Instance });


        }

        private object scroller;
        public object Scroller
        {
            get { return this.scroller; }
            set
            {
                if (this.scroller != value)
                {
                    this.scroller = value;
                    FirePropertyChanged("Scroller");
                }
            }
        }

        private object environment;
        public object Environment
        {
            get { return this.environment; }
            set
            {
                if (this.environment != value)
                {
                    this.environment = value;
                    this.BindIsMouseActive();
                    FirePropertyChanged("Environment");
                }
            }
        }

        private bool m_fMouseActive = false;
        public bool IsMouseActive
        {
            get
            {
                return this.m_fMouseActive;
            }
            set
            {
                if (this.m_fMouseActive != value)
                {
                    this.m_fMouseActive = value;
                    FirePropertyChanged("IsMouseActive");
                }
            }
        }

        private void BindIsMouseActive()
        {
            IPropertyObject env = (IPropertyObject)this.environment;
            //ModelItem scroller = (ModelItem)SourceScroller;
            try
            {
                this.BindFromSource(env, "IsMouseActive", "IsMouseActive");
            }
            catch{ }
        }

        public void BindTargetViewItem(object SourceScroller, object TargetScrollData)
        {
            ModelItem scrollData=(ModelItem)TargetScrollData;
            //ModelItem scroller = (ModelItem)SourceScroller;
            try
            {
                this.BindToTarget(scrollData, "TargetViewItem", "Scroller");
            }
            catch (Exception ex) { }
        }
    }
}
