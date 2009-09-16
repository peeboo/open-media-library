using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Library.Code.V3
{
    //[DebuggerDisplay("(Default {_defaultSlideName}, {_slides} )")]
    public sealed class SlideDeckBlueprint : IEquatable<SlideDeckBlueprint>
    {
        // Fields
        private SlideBlueprint _defaultSlide;
        private DateTime _endDate;
        private ReadOnlyCollection<SlideBlueprint> _slides;
        private DateTime _startDate;
        private string _uiName;

        // Methods
        public SlideDeckBlueprint()
        {
        }

        public SlideDeckBlueprint(SlideBlueprint defaultSlide, string uiName, DateTime startDate, DateTime endDate, params SlideBlueprint[] slides)
            : this(defaultSlide, uiName, startDate, endDate, null, slides)
        {
        }

        public SlideDeckBlueprint(SlideBlueprint defaultSlide, string uiName, DateTime startDate, DateTime endDate, SlideDeckBlueprint baseBlueprint, params SlideBlueprint[] slides)
        {
            if (slides == null)
            {
                throw new ArgumentNullException("slides");
            }
            List<SlideBlueprint> list = new List<SlideBlueprint>();
            SlideBlueprint blueprint = defaultSlide;
            if (baseBlueprint != null)
            {
                if (blueprint == null)
                {
                    blueprint = baseBlueprint.DefaultSlide;
                }
                list.AddRange(baseBlueprint.Slides);
                foreach (SlideBlueprint blueprint2 in slides)
                {
                    if (blueprint2 != null)
                    {
                        if (blueprint2.Insertion == SlideInsertion.Add)
                        {
                            int index = list.IndexOf(blueprint2);
                            if (-1 < index)
                            {
                                list[index] = blueprint2;
                            }
                            else
                            {
                                list.Add(blueprint2);
                            }
                        }
                        else if (blueprint2.Insertion == SlideInsertion.Remove)
                        {
                            int num2 = list.IndexOf(blueprint2);
                            if (-1 < num2)
                            {
                                list.RemoveAt(num2);
                            }
                        }
                    }
                }
                this._slides = new ReadOnlyCollection<SlideBlueprint>(list);
            }
            else
            {
                this._slides = new ReadOnlyCollection<SlideBlueprint>(slides);
            }
            this._defaultSlide = blueprint;
            this._uiName = uiName;
            this._startDate = startDate;
            this._endDate = endDate;
        }

        public SlideDeckBlueprint Clone()
        {
            SlideDeckBlueprint blueprint = new SlideDeckBlueprint
            {
                _defaultSlide = this._defaultSlide,
                _uiName = this._uiName,
                _startDate = this._startDate,
                _endDate = this._endDate
            };
            SlideBlueprint[] list = new SlideBlueprint[this._slides.Count];
            for (int i = 0; i < list.Length; i++)
            {
                if (this._slides[i] == null)
                {
                    throw new InvalidOperationException("null slide encountered during Clone");
                }
                list[i] = this._slides[i].Clone();
            }
            blueprint._slides = new ReadOnlyCollection<SlideBlueprint>(list);
            return blueprint;
        }

        public bool Equals(SlideDeckBlueprint other)
        {
            if (other == null)
            {
                return false;
            }
            if (((this._slides == null) && (other._slides != null)) || ((this._slides != null) && (other._slides == null)))
            {
                return false;
            }
            if (!this.DefaultSlide.Equals(other.DefaultSlide))
            {
                return false;
            }
            if (((this._slides != null) && (other._slides != null)) && (this._slides.Count != other._slides.Count))
            {
                return false;
            }
            if (((this._slides == null) && (other._slides != null)) || ((this._slides != null) && (other._slides == null)))
            {
                return false;
            }
            if ((this._slides != null) || (other._slides != null))
            {
                for (int i = 0; i < this._slides.Count; i++)
                {
                    if ((((this._slides[i] == null) && (other._slides[i] != null)) || ((this._slides[i] != null) && (other._slides[i] == null))) || !this._slides[i].Equals(other._slides[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SlideDeckBlueprint);
        }

        public override int GetHashCode()
        {
            return this._uiName.GetHashCode();
        }

        // Properties
        public SlideBlueprint DefaultSlide
        {
            get
            {
                return this._defaultSlide;
            }
            set
            {
                this._defaultSlide = value;
            }
        }

        [MarkupVisible]
        public DateTime EndDate
        {
            get
            {
                return this._endDate;
            }
        }

        public bool IsExpired
        {
            get
            {
                return ((DateTime.UtcNow >= this.StartDate) && (DateTime.UtcNow < this.EndDate));
            }
        }

        public ReadOnlyCollection<SlideBlueprint> Slides
        {
            get
            {
                return this._slides;
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
        public string UIName
        {
            get
            {
                //return this._uiName;
                return "UIName";
            }
            set
            {
            }
        }

        [MarkupVisible]
        public string Title
        {
            get
            {
                //return this._uiName;
                return "Title";
            }
            set
            {
            }
        }
    }
}
