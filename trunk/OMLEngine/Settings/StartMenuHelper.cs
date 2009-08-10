using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace OMLEngine.Settings
{
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    public class StartMenuHelper
    {
        public event ChangedEventHandler Changed;

        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        private static string EntryPointRegKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Media Center\Extensibility\Entry Points";
        private static string OMLCategoryRegKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Media Center\Extensibility\Categories\OML\Open Media Library";
        private static string OMLApplicationId = "{3f0850a7-0fd7-4cbf-b8dc-c7f7ea31534e}";
        private static string OMLAddIn = "Library.MyAddIn, Library,Culture=Neutral,Version=3.0.0.0,PublicKeyToken=74d3b407d6cf16f1";

        public List<StartMenuItem> StartMenuItems { get; set; }

        public StartMenuHelper()
        {
            this.GetStartMenuItems();
        }

        private void GetStartMenuItems()
        {
            this.StartMenuItems = new List<StartMenuItem>();
            RegistryKey subKey = Registry.CurrentUser;
            subKey = subKey.OpenSubKey(OMLCategoryRegKey, false);
            if (subKey != null)
            {
                foreach (string sub in subKey.GetSubKeyNames())
                {
                    RegistryKey entryPoint = Registry.CurrentUser;
                    entryPoint = subKey.OpenSubKey(sub, true);
                    double dblSortTimeStamp = GetTimeStampFromDateTime(DateTime.Now);
                    double.TryParse(entryPoint.GetValue("TimeStamp", string.Empty).ToString(), out dblSortTimeStamp);
                    entryPoint = Registry.CurrentUser;
                    entryPoint = entryPoint.OpenSubKey(string.Format(@"{0}\{1}", EntryPointRegKey, sub), true);
                    if (entryPoint != null)
                    {
                        StartMenuItem entryPointItem = this.GetEntryPointFromRegistry(entryPoint);
                        entryPointItem.ItemId = sub;
                        entryPointItem.SortTimeStamp = dblSortTimeStamp;
                        StartMenuItems.Add(entryPointItem);
                    }
                }
            }
            this.StartMenuItems = this.SortStartMenu(StartMenuItems) as List<StartMenuItem>;
        }

        public void MoveItemUp(StartMenuItem item)
        {
            int idx = this.StartMenuItems.IndexOf(item);
            if (idx > 0)
            {
                StartMenuItems.Remove(item);
                StartMenuItems.Insert(idx - 1, item);
            }
            this.SaveStartMenu();
        }

        public void MoveItemDown(StartMenuItem item)
        {
            int idx = this.StartMenuItems.IndexOf(item);
            if (idx < 3)
            {
                StartMenuItems.Remove(item);
                StartMenuItems.Insert(idx + 1, item);
            }
            this.SaveStartMenu();
        }

        public void SaveStartMenu()
        {
            //clear the categories
            RegistryKey category = Registry.CurrentUser;
            try
            {
                category.DeleteSubKeyTree(OMLCategoryRegKey);
            }
            catch { }
            category = category.CreateSubKey(OMLCategoryRegKey);
            category.Close();
            //gotta seed this... otherwise MC won't honor our sort
            DateTime sortTimeStamp = DateTime.Now;
            foreach (StartMenuItem item in StartMenuItems)
            {
                saveStartMenuItem(item, sortTimeStamp);
                sortTimeStamp = sortTimeStamp.AddDays(1);
            }

            //refresh from registry
            this.GetStartMenuItems();
            this.OnChanged(EventArgs.Empty);
        }

        public void AddStartMenuItem(StartMenuItem item)
        {
            item.AddIn = OMLAddIn;
            item.AppId = OMLApplicationId;
            item.ItemId = GenerateNewAppId();
            this.StartMenuItems.Add(item);
            this.SaveStartMenu();
        }

        public void DeleteStartMenuItem(StartMenuItem item)
        {
            int idx = this.StartMenuItems.IndexOf(item);
            StartMenuItems.Remove(item);
            this.SaveStartMenu();

        }

        private void saveStartMenuItem(StartMenuItem item, DateTime sortTimeStamp)
        {
            //set the entrypoint
            RegistryKey entryPoint = Registry.CurrentUser;
            entryPoint = entryPoint.CreateSubKey(string.Format(@"{0}\{1}", EntryPointRegKey, item.ItemId));
            entryPoint.SetValue("AddIn", item.AddIn, RegistryValueKind.ExpandString);
            entryPoint.SetValue("AppId", item.AppId, RegistryValueKind.String);
            entryPoint.SetValue("Context", item.Context, RegistryValueKind.ExpandString);
            entryPoint.SetValue("Description", item.Description, RegistryValueKind.ExpandString);
            entryPoint.SetValue("ImageUrl", item.ImageUrl, RegistryValueKind.ExpandString);
            if (!string.IsNullOrEmpty(item.InactiveImageUrl))
                entryPoint.SetValue("InactiveImageUrl", item.InactiveImageUrl, RegistryValueKind.ExpandString);
            entryPoint.SetValue("Title", item.Title, RegistryValueKind.ExpandString);
            entryPoint.SetValue("TimeStamp", GetTimeStampFromDateTime(sortTimeStamp), RegistryValueKind.DWord);
            entryPoint.Close();
            //set the category
            RegistryKey categoryKey = Registry.CurrentUser;
            categoryKey = categoryKey.CreateSubKey(string.Format(@"{0}\{1}", OMLCategoryRegKey, item.ItemId));
            categoryKey.SetValue("AppId", item.AppId, RegistryValueKind.String);
            categoryKey.SetValue("TimeStamp", GetTimeStampFromDateTime(sortTimeStamp), RegistryValueKind.DWord);
            categoryKey.Close();

            switch (item.Context)
            {
                case Context.Custom1:
                    Properties.Settings.Default.StartMenuCustom1 = item.ExtendedContext;
                    Properties.Settings.Default.Save();
                    break;

                case Context.Custom2:
                    Properties.Settings.Default.StartMenuCustom2 = item.ExtendedContext;
                    Properties.Settings.Default.Save();
                    break;

                case Context.Custom3:
                    Properties.Settings.Default.StartMenuCustom3 = item.ExtendedContext;
                    Properties.Settings.Default.Save();
                    break;
                case Context.Custom4:
                    Properties.Settings.Default.StartMenuCustom4 = item.ExtendedContext;
                    Properties.Settings.Default.Save();
                    break;
                case Context.Custom5:
                    Properties.Settings.Default.StartMenuCustom5 = item.ExtendedContext;
                    Properties.Settings.Default.Save();
                    break;
            }
        }

        private IEnumerable<StartMenuItem> SortStartMenu(IEnumerable<StartMenuItem> items)
        {
            //we are sorting based on the seconds from 1/1/2000
            List<StartMenuItem> sortedList = new List<StartMenuItem>(items);
            sortedList.Sort(delegate(StartMenuItem x, StartMenuItem y) { return x.SortTimeStamp.CompareTo(y.SortTimeStamp); });

            //maintaining this a bit simpler if a user wants to move up/down in their start menu
            //when we save we will regen the date + the added seed
            int sortSeed = 0;
            foreach (StartMenuItem item in sortedList)
            {
                item.SortOrder = sortSeed;
                sortSeed++;
            }
            return sortedList;
        }

        //public List<StartMenuItem> GetStartMenuItemsOld()
        //{
        //    List<StartMenuItem> startMenuItems =new List<StartMenuItem>();
        //    RegistryKey subKey = Registry.CurrentUser;
        //    subKey = subKey.OpenSubKey(EntryPointRegKey, false);
        //    foreach (string sub in subKey.GetSubKeyNames())
        //    {
        //        RegistryKey entryPoint = Registry.CurrentUser;
        //        entryPoint = subKey.OpenSubKey(sub, true);
        //        if (entryPoint.GetValue("AppId", string.Empty).ToString() == OMLApplicationId)
        //        {
        //            startMenuItems.Add(this.GetEntryPointFromRegistry(entryPoint));
        //        }
        //    }
        //    return startMenuItems;
        //}

        private StartMenuItem GetEntryPointFromRegistry(RegistryKey key)
        {
            //http://blogs.msdn.com/astebner/archive/2006/08/22/713468.aspx
            StartMenuItem entryPoint = new StartMenuItem();
            entryPoint.AddIn = OMLAddIn;
            entryPoint.AppId = OMLApplicationId;
            entryPoint.Context = GetContextFromString(key.GetValue("Context", string.Empty).ToString());
            entryPoint.Description = key.GetValue("Description", string.Empty).ToString();
            entryPoint.ImageUrl = key.GetValue("ImageUrl", string.Empty).ToString();
            entryPoint.InactiveImageUrl = key.GetValue("InactiveImageUrl", string.Empty).ToString();

            double dblTimeStamp = GetTimeStampFromDateTime(DateTime.Now);
            double.TryParse(key.GetValue("TimeStamp", string.Empty).ToString(), out dblTimeStamp);
            entryPoint.TimeStamp = dblTimeStamp;
            entryPoint.Title = key.GetValue("Title", string.Empty).ToString();

            switch (entryPoint.Context)
            {
                case Context.Custom1:
                    entryPoint.ExtendedContext = Properties.Settings.Default.StartMenuCustom1;
                    break;

                case Context.Custom2:
                    entryPoint.ExtendedContext = Properties.Settings.Default.StartMenuCustom2;
                    break;

                case Context.Custom3:
                    entryPoint.ExtendedContext = Properties.Settings.Default.StartMenuCustom3;
                    break;
                case Context.Custom4:
                    entryPoint.ExtendedContext = Properties.Settings.Default.StartMenuCustom4;
                    break;
                case Context.Custom5:
                    entryPoint.ExtendedContext = Properties.Settings.Default.StartMenuCustom5;
                    break;
            }

            return entryPoint;
        }

        private static Context GetContextFromString(string ctx)
        {
            switch (ctx)
            {
                case "Home":
                    return Context.Home;

                case "Trailers":
                    return Context.Trailers;

                case "Movies":
                    return Context.Movies;

                case "TV":
                    return Context.TV;

                case "Search":
                    return Context.Search;

                case "Settings":
                    return Context.Settings;

                case "Custom1":
                    return Context.Custom1;

                case "Custom2":
                    return Context.Custom2;

                case "Custom3":
                    return Context.Custom3;

                case "Custom4":
                    return Context.Custom4;

                case "Custom5":
                    return Context.Custom5;

                default:
                    return Context.Home;
            }
        }

        private static double GetTimeStampFromDateTime(DateTime entryTime)
        {
            //The number of seconds that have elapsed since midnight on January 1, 2000 C.E.
            DateTime centuryBegin = new DateTime(2000, 1, 1);

            long elapsedTicks = entryTime.Ticks - centuryBegin.Ticks;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            return elapsedSpan.TotalSeconds;
        }

        private static string GenerateNewAppId()
        {
            return System.Guid.NewGuid().ToString("B");
        }
    }
}
