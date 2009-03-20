using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.UI;
using System.Reflection;

namespace Library.Code.V3
{
    [MarkupVisible]
    public class EnvironmentHelper : ModelItem
    {
        private Type myType;
        private object environment;
        private PropertyInfo pinfo;
        public void GetEnvironment(object Environment)
        {
            this.environment = Environment;
            //this.setValues(PIP);
            Type myType = Environment.GetType();
            EventInfo evClick = myType.GetEvent("PropertyChanged");
            Type tDelegate = evClick.EventHandlerType;

            MethodInfo miHandler =
            typeof(EnvironmentHelper).GetMethod("Environment_PropertyChanged",
                BindingFlags.NonPublic|BindingFlags.Instance);

            Delegate d = Delegate.CreateDelegate(tDelegate, this, miHandler);
            //Delegate d = Delegate.CreateDelegate(tDelegate, miHandler, true);
            //Delegate d = Delegate.CreateDelegate(tDelegate, miHandler, true);
            MethodInfo addHandler = evClick.GetAddMethod();
            Object[] addHandlerArgs = { d };
            addHandler.Invoke(Environment, addHandlerArgs);

            //pinfo = myType.GetProperty("IsMouseActive");
            
            ////pinfo.SetValue(PIP, PipVisualState.Inactive, null);
            ////Microsoft.MediaCenter.Hosting.AddInHost.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);
            //Microsoft.MediaCenter.UI.Timer t = new Timer();
            //t.AutoRepeat = true;
            //t.Tick += new EventHandler(t_Tick);
            //t.Interval = 1000;
            //t.Enabled = true;
            //t.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            object o = pinfo.GetValue(environment, null);
            bool oBool = (bool)o;
            if(oBool==true)
                Console.WriteLine("PROP::"+o.ToString());
        }

        void Environment_PropertyChanged(IPropertyObject sender, string property)
        {
            
            Console.WriteLine("PROP:::"+property);
        }
        //public void EnablePIP(object PIP)
        //{
        //    this.setValues(PIP);
        //    //Type myType = PIP.GetType();
        //    //PropertyInfo pinfo = myType.GetProperty("VisualState");
        //    pinfo.SetValue(PIP, PipVisualState.Active, null);
        //}

        //public void setValues(object PIP)
        //{
        //    if (myType == null)
        //    {
        //        myType = PIP.GetType();
        //        pinfo = myType.GetProperty("VisualState");
        //    }
        //}
    }
}
