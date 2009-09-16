using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Library.Code.V3
{
    //[DebuggerDisplay("(UIName={UIName},Title={Title})")]
    public sealed class SlideBlueprint : IEquatable<SlideBlueprint>
    {
        // Fields
        private DateTime _endDate;
        private int _hashCode;
        private SlideInsertion _insertion;
        private DateTime _startDate;
        private string _title;
        private string _uiName;

        // Methods
        private SlideBlueprint()
        {
        }

        public SlideBlueprint(string uiName, string title, DateTime startDate, DateTime endDate)
            : this(uiName, title, startDate, endDate, SlideInsertion.Add)
        {
        }

        public SlideBlueprint(string uiName, string title, DateTime startDate, DateTime endDate, SlideInsertion insertion)
        {
            if (uiName == null)
            {
                throw new ArgumentNullException("uiName");
            }
            this._uiName = uiName;
            this._hashCode = this._uiName.ToLowerInvariant().GetHashCode();
            this._title = title;
            this._startDate = startDate;
            this._endDate = endDate;
            this._insertion = insertion;
        }

        public SlideBlueprint Clone()
        {
            return new SlideBlueprint { _startDate = this._startDate, _endDate = this._endDate, _insertion = this._insertion, _uiName = this._uiName, _hashCode = this._uiName.ToLowerInvariant().GetHashCode(), _title = this._title };
        }

        public bool Equals(SlideBlueprint other)
        {
            if (other == null)
            {
                return false;
            }
            return (0 == string.Compare(this._uiName, other.UIName, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SlideBlueprint);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "UIName=\"{0}\",Title=\"{1}\",Insertion={2},StartDate={3},EndDate={4}", new object[] { this.UIName, this.Title, this.Insertion, this.StartDate, this.EndDate });
        }

        // Properties
        [MarkupVisible]
        public DateTime EndDate
        {
            get
            {
                return this._endDate;
            }
        }

        public SlideInsertion Insertion
        {
            get
            {
                return this._insertion;
            }
            set
            {
                this._insertion = value;
            }
        }

        [MarkupVisible]
        public bool IsExpired
        {
            get
            {
                return ((DateTime.UtcNow >= this.StartDate) && (DateTime.UtcNow < this.EndDate));
            }
        }

        [MarkupVisible]
        public DateTime StartDate
        {
            get
            {
                return this._startDate;
            }
        }
        [MarkupVisible]
        public string Title
        {
            get
            {
                return this._title;
            }
        }

        [MarkupVisible]
        public string UIName
        {
            get
            {
                return this._uiName;
            }
        }
    }
    public enum SlideInsertion
    {
        Add,
        Remove
    }


}
