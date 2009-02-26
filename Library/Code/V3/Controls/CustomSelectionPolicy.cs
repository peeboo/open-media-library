using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.UI;

namespace Library.Code.V3
{
    public class CustomSelectionPolicy : ModelItem, ISelectionPolicy
    {
        // Fields
        private bool m_fClearSelectionOnLostFocus;
        private bool m_fRequestFocusOnSelect;
        private bool m_fSelectOnGainFocus;
        private int m_nSelectionIndex;
        private ISelectable m_selected;

        // Methods
        public CustomSelectionPolicy()
            : this(null)
        {
        }

        public CustomSelectionPolicy(IModelItemOwner owner)
            : base(owner)
        {
        }

        public bool AllowSetSelection(ISelectable item, bool fSelect)
        {
            return true;
        }

        private void OnCurrentSelectionChanged()
        {
            base.FirePropertyChanged("CurrentSelection");
        }

        public void OnItemGainFocus(ISelectable item)
        {
            if (this.SelectOnGainFocus)
            {
                item.Selected = true;
            }
        }

        public void OnItemLostFocus(ISelectable item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Cannot specify a null ISelectable.");
            }
            if ((item == this.m_selected) && this.ClearSelectionOnLostFocus)
            {
                this.m_selected.Selected = false;
                this.m_selected = null;
                this.OnCurrentSelectionChanged();
            }
        }

        public void OnSetSelection(ISelectable item, bool fSelect)
        {
            if (fSelect)
            {
                if (this.m_selected != null)
                {
                    this.m_selected.Selected = false;
                }
                this.m_selected = item;
                if (this.RequestFocusOnSelect)
                {
                    item.RequestFocus();
                }
                this.OnCurrentSelectionChanged();
            }
            else if (this.m_selected == item)
            {
                this.m_selected = null;
                this.OnCurrentSelectionChanged();
            }
        }

        // Properties
        public bool ClearSelectionOnLostFocus
        {
            get
            {
                return this.m_fClearSelectionOnLostFocus;
            }
            set
            {
                this.m_fClearSelectionOnLostFocus = value;
            }
        }

        public ISelectable CurrentSelection
        {
            get
            {
                return this.m_selected;
            }
        }

        public bool RequestFocusOnSelect
        {
            get
            {
                return this.m_fRequestFocusOnSelect;
            }
            set
            {
                this.m_fRequestFocusOnSelect = value;
            }
        }

        public int SelectionIndex
        {
            get
            {
                return this.m_nSelectionIndex;
            }
            set
            {
                if (this.m_nSelectionIndex != value)
                {
                    this.m_nSelectionIndex = value;
                    base.FirePropertyChanged("SelectionIndex");
                }
            }
        }

        public bool SelectOnGainFocus
        {
            get
            {
                return this.m_fSelectOnGainFocus;
            }
            set
            {
                this.m_fSelectOnGainFocus = value;
            }
        }
    }
}
