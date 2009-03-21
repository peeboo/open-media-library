/**************************************************************
 * This code taken from the media browser                     *
 * project at http://code.google.com/p/videobrowser/          *
 * and based on a conversation about IModelItem vs ModelItem  *
 * as a base class for testing purposes                       *
 * ************************************************************/

using System;
using System.Collections.Generic;
using Microsoft.MediaCenter.UI;

namespace Library
{
    public class BaseModelItem : IModelItem, IDisposable
    {
        string description = string.Empty;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        bool selected;
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        Guid uniqueId = Guid.NewGuid();
        public Guid UniqueId
        {
            get { return uniqueId; }
            set { uniqueId = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void FirePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, property);
        }

        List<ModelItem> items = new List<ModelItem>();

        public void RegisterObject(ModelItem modelItem)
        {
            items.Add(modelItem);
        }

        public void UnregisterObject(ModelItem modelItem)
        {
            if (items.Exists((i) => i == modelItem))
            {
                modelItem.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                foreach (var item in items)
                {
                    item.Dispose();
                }
            }
        }

        ~BaseModelItem()
        {
            Dispose(false);
        }
    }
}
