using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Library.Code.V3
{
    public sealed class SlideMarkupFinder : ISlideMarkupFinder
    {
        // Fields
        private SlideDeckBlueprint _blueprint;
        private UIContextTree _contextTree = UIContextTree.Instance;
        private Denial _denial;
        private Exception _lastException;
        private ContextObjectState _state;
        private const string k_DenialAborted = "res://ehres!denial/MarkupFinder/TODO_Aborted.mcml";
        private const string k_DenialContextTreeNotReady = "res://ehres!TODO_DENIAL_CONTEXT_TREE_NOT_READY.mcml";
        private const string k_DenialInvalidContext = "res://ehres!denial/MarkupFinder/TODO_InvalidContext.mcml";
        private const string k_DenialQueueUserWorkItemFailed = "res://ehres!denial/MarkupFinder/TODO_QueueUserWorkItemFailed.mcml";
        private const int kMaxTimeout = 0x5dc;

        // Events
        public event SlideStateEventHandler StateChanged;

        // Methods
        public SlideMarkupFinder()
        {
            this.State = this._contextTree.State;
        }

        public void AddContextTreeSource(string source)
        {
            this._contextTree.AddContextToList(source);
        }

        public void ClearContextTreeSourceList()
        {
            this._contextTree.ClearContextList();
        }

        public IList<string> GetContextTreeSourceList()
        {
            return this._contextTree.GetContextList();
        }

        public void LoadContextTree()
        {
            this._contextTree.LoadXml(null);
        }

        public SlideDeckBlueprint Lookup(string context)
        {
            this.Denial = null;
            SlideDeckBlueprint blueprint = null;
            if (null == this.Denial)
            {
                this.State = ContextObjectState.Loading;
                if (ContextObjectState.Ready != this._contextTree.State)
                {
                    this._contextTree.LoadXml(null);
                }
                if (!this._contextTree.TryGetValue(context, out blueprint))
                {
                    this.Denial = new Denial("res://ehres!denial/MarkupFinder/TODO_InvalidContext.mcml", 1);
                }
            }
            if (blueprint != null)
            {
                this.SlideDeckBlueprint = blueprint;
                this.State = ContextObjectState.Ready;
            }
            return blueprint;
        }

        // Properties
        private Collection<IDenialSource> ChildDenialSources
        {
            get
            {
                return new Collection<IDenialSource>(new IDenialSource[] { this._contextTree });
            }
        }

        public Denial Denial
        {
            get
            {
                Denial other = this._denial;
                foreach (IDenialSource source in this.ChildDenialSources)
                {
                    if (((source != null) && (null != source.Denial)) && ((null == other) || (source.Denial.CompareTo(other) > 0)))
                    {
                        other = source.Denial;
                    }
                }
                return other;
            }
            private set
            {
                Denial denial = this.Denial;
                this._denial = value;
                if (this.Denial != denial)
                {
                    if (null == this.Denial)
                    {
                        this.State = ContextObjectState.DenialWithdrawn;
                        this.LastException = null;
                    }
                    else
                    {
                        this.State = ContextObjectState.Denial;
                    }
                }
            }
        }

        public Exception LastException
        {
            get
            {
                return this._lastException;
            }
            private set
            {
                this._lastException = value;
            }
        }

        public SlideDeckBlueprint SlideDeckBlueprint
        {
            get
            {
                return this._blueprint;
            }
            private set
            {
                this._blueprint = value;
            }
        }

        public ContextObjectState State
        {
            get
            {
                return this._state;
            }
            private set
            {
                this._state = value;
                if (this.StateChanged != null)
                {
                    this.StateChanged(this, new ContextObjectStateEventArgs(value));
                }
            }
        }

        // Nested Types
        private enum DenialPriority
        {
            Critical = 0x3e8,
            High = 100,
            Low = 1,
            Medium = 10
        }

        internal class UIContextTree : IDictionary<string, SlideDeckBlueprint>, ICollection<KeyValuePair<string, SlideDeckBlueprint>>, IEnumerable<KeyValuePair<string, SlideDeckBlueprint>>, IEnumerable, IContextListObject, IDenialSource
        {
            // Fields
            private Dictionary<string, string> _aliases = new Dictionary<string, string>();
            private List<string> _contextList = new List<string>();
            private int _count;
            private Denial _denial;
            private static SlideMarkupFinder.UIContextTree _instance;
            private Exception _lastException;
            private bool _loadFromApplicationThread = true;
            private ContextObjectState _state;
            private const string k_DenialInvalidContext = "res://ehres!TODO_DENIAL_INVALID_CONTEXT.mcml";
            private const string k_DenialInvalidXml = "res://ehres!TODO_DENIAL_INVALID_XML.mcml";
            private const string k_DenialLoadError = "res://ehres!TODO_DENIAL_LOAD_ERROR.mcml";
            private const string k_SchemaNamespace = "http://schemas.microsoft.com/2007/MceMarkupContextTree";
            private const string k_SchemaResourcePath = "res://ehres!MceMarkupContextTree.xsd";
            private Node Root = new Node(null, string.Empty, null);

            // Events
            public event SlideStateEventHandler StateChanged;

            // Methods
            private UIContextTree()
            {
                this._contextList.Add("res://ehres!MceMarkupContextTree.xml");
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Media Center\Settings"))
                {
                    if (key != null)
                    {
                        string item = key.GetValue("MceMarkupContextTree", null) as string;
                        if (item != null)
                        {
                            int startIndex = item.IndexOf("://", StringComparison.Ordinal) + 3;
                            int length = item.IndexOf("!", StringComparison.Ordinal) - startIndex;
                            if ((startIndex != 0) && (length > 0))
                            {
                                //MarkupSystem.RegisterUnrestrictedHost(item.Substring(startIndex, length));
                            }
                            this._contextList.Clear();
                            this._contextList.Add(item);
                        }
                    }
                }
            }

            public void Add(KeyValuePair<string, SlideDeckBlueprint> item)
            {
                this.Add(item.Key, item.Value);
            }

            public void Add(string key, SlideDeckBlueprint value)
            {
                lock (this.Root)
                {
                    Node root = this.Root;
                    string[] strArray = key.Split(new char[] { '/' });
                    bool flag = false;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        Node node2 = null;
                        if (!root.Nodes.TryGetValue(strArray[i], out node2))
                        {
                            node2 = new Node(root, string.Join("/", strArray, i, strArray.Length - i), value);
                            this._count++;
                            root.Nodes.Add(strArray[i], node2);
                            flag = true;
                            root = node2;
                            break;
                        }
                        root = node2;
                    }
                    if (!flag)
                    {
                        root.Blueprint = value;
                    }
                }
            }

            public void AddAlias(string alias, string refersTo)
            {
                this._aliases.Add(alias, refersTo);
            }

            public void AddContextToList(string source)
            {
                if (!string.IsNullOrEmpty(source) && !this._contextList.Contains(source))
                {
                    this._contextList.Add(source);
                    if (this.State == ContextObjectState.Ready)
                    {
                        this.State = ContextObjectState.Loading;
                        this.LoadXmlSource(source, false);
                        if (this.Denial == null)
                        {
                            this.State = ContextObjectState.Ready;
                        }
                    }
                    else
                    {
                        this.State = ContextObjectState.NotInitialized;
                    }
                }
            }

            public void Clear()
            {
                this.Root = null;
                this.Root = new Node(null, string.Empty, null);
            }

            public void ClearContextList()
            {
                if (this._contextList != null)
                {
                    this._contextList.Clear();
                }
                this.State = ContextObjectState.NotInitialized;
            }

            public bool Contains(KeyValuePair<string, SlideDeckBlueprint> item)
            {
                throw new NotImplementedException();
            }

            public bool ContainsKey(string key)
            {
                SlideDeckBlueprint blueprint;
                return this.TryGetValue(key, out blueprint);
            }

            public void CopyTo(KeyValuePair<string, SlideDeckBlueprint>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IList<string> GetContextList()
            {
                List<string> list = null;
                if (this._contextList != null)
                {
                    list = new List<string>(this._contextList.Count);
                    list.AddRange(this._contextList);
                }
                return list;
            }

            private Node GetDeepestNodeWithBlueprint(string key)
            {
                Node root = this.Root;
                Node node2 = null;
                string[] strArray = key.Split(new char[] { '/' });
                for (int i = 0; i < strArray.Length; i++)
                {
                    Node node3 = null;
                    if (!root.Nodes.TryGetValue(strArray[i], out node3))
                    {
                        return node2;
                    }
                    root = node3;
                    if (root.Blueprint != null)
                    {
                        node2 = root;
                    }
                }
                return node2;
            }

            public IEnumerator<KeyValuePair<string, SlideDeckBlueprint>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            private string GetXmlAttribute(XmlNode node, string name)
            {
                return this.GetXmlAttribute(node, name, (string)null);
            }

            private DateTime GetXmlAttribute(XmlNode node, string name, DateTime defaultValue)
            {
                DateTime time;
                XmlAttribute attribute = node.Attributes[name];
                if ((attribute != null) && DateTime.TryParse(attribute.Value, out time))
                {
                    return time;
                }
                return defaultValue;
            }

            private string GetXmlAttribute(XmlNode node, string name, string defaultValue)
            {
                XmlAttribute attribute = node.Attributes[name];
                if (attribute != null)
                {
                    return attribute.Value;
                }
                return defaultValue;
            }

            public void LoadXml(object unusedState)
            {
                this.Denial = null;
                if ((this._contextList == null) || (this._contextList.Count == 0))
                {
                    this.Denial = new Denial("res://ehres!TODO_DENIAL_INVALID_CONTEXT.mcml", 100);
                }
                else
                {
                    this.State = ContextObjectState.Loading;
                    for (int i = 0; i < this._contextList.Count; i++)
                    {
                        this.LoadXmlSource(this._contextList[i], i == 0);
                        if (this.Denial != null)
                        {
                            break;
                        }
                    }
                    if (this.Denial == null)
                    {
                        this.State = ContextObjectState.Ready;
                    }
                }
            }

            private void LoadXmlSource(string source, bool clear)
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
                    nsmgr.AddNamespace("m", "http://schemas.microsoft.com/2007/MceMarkupContextTree");
                    Resource resource = ResourceManager.Instance.GetResource("res://ehres!MceMarkupContextTree.xsd");
                    if (resource.Open() != ResourceStatus.Available)
                    {
                        try
                        {
                            resource.Close();
                            this.LastException = new NotImplementedException("res://ehres!MceMarkupContextTree.xsd could not be opened");
                        }
                        catch (Exception exception)
                        {
                            this.LastException = exception;
                        }
                        this.Denial = new Denial("res://ehres!TODO_DENIAL_INVALID_CONTEXT.mcml", 100);
                    }
                    else
                    {
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            IgnoreComments = true,
                            IgnoreWhitespace = true
                        };
                        settings.Schemas.Add(XmlSchema.Read(resource.Stream, null));
                        settings.ValidationType = ValidationType.Schema;
                        settings.ConformanceLevel = ConformanceLevel.Document;
                        settings.ValidationEventHandler += new ValidationEventHandler(this.OnXmlValidation);
                        resource.Close();
                        Resource resource2 = ResourceManager.Instance.GetResource(source);
                        if (resource2.Open() != ResourceStatus.Available)
                        {
                            this.LastException = null;
                            try
                            {
                                resource2.Close();
                                this.LastException = new NotImplementedException(source + " could not be opened");
                            }
                            catch (Exception exception2)
                            {
                                this.LastException = exception2;
                            }
                            this._denial = new Denial("res://ehres!TODO_DENIAL_INVALID_CONTEXT.mcml", 100);
                        }
                        else
                        {
                            XmlReader reader = XmlReader.Create(resource2.Stream, settings);
                            document.Load(reader);
                            reader.Close();
                            resource2.Close();
                            if (clear)
                            {
                                this.Clear();
                            }
                            Queue<KeyValuePair<string, XmlNode>> queue = new Queue<KeyValuePair<string, XmlNode>>();
                            foreach (XmlNode node in document.DocumentElement.SelectNodes("m:Node", nsmgr))
                            {
                                queue.Enqueue(new KeyValuePair<string, XmlNode>(node.Attributes["Name"].Value, node));
                            }
                            Dictionary<string, SlideBlueprint> dictionary = new Dictionary<string, SlideBlueprint>();
                            Queue<KeyValuePair<string, XmlNode>> queue2 = new Queue<KeyValuePair<string, XmlNode>>();
                            while (0 < queue.Count)
                            {
                                KeyValuePair<string, XmlNode> pair = queue.Dequeue();
                                string key = pair.Key;
                                XmlNode node2 = pair.Value;
                                foreach (XmlNode node3 in node2.SelectNodes("m:Node", nsmgr))
                                {
                                    queue.Enqueue(new KeyValuePair<string, XmlNode>(key + "/" + node3.Attributes["Name"].Value, node3));
                                }
                                XmlNode node4 = node2.SelectSingleNode("m:SlideDeck", nsmgr);
                                if (node4 != null)
                                {
                                    queue2.Enqueue(new KeyValuePair<string, XmlNode>(key, node4));
                                }
                            }
                            int num = 0;
                            while (0 < queue2.Count)
                            {
                                if (num == queue2.Count)
                                {
                                    this.LastException = new ArgumentException("Unresolvable (circular) reference while processing SlideDeck blueprints");
                                    this.Denial = new Denial("res://ehres!TODO_DENIAL_INVALID_XML.mcml", 100);
                                    return;
                                }
                                KeyValuePair<string, XmlNode> item = queue2.Dequeue();
                                string str2 = item.Key;
                                XmlNode node5 = item.Value;
                                SlideDeckBlueprint blueprint = null;
                                if ((node5.Attributes["Base"] != null) && (!this.TryGetValue(node5.Attributes["Base"].Value, out blueprint) || (blueprint == null)))
                                {
                                    num++;
                                    queue2.Enqueue(item);
                                }
                                else
                                {
                                    List<SlideBlueprint> list = new List<SlideBlueprint>();
                                    foreach (XmlNode node6 in node5.SelectSingleNode("m:Slides", nsmgr).ChildNodes)
                                    {
                                        SlideInsertion insertion = (SlideInsertion)Enum.Parse(typeof(SlideInsertion), node6.Name);
                                        SlideBlueprint blueprint2 = new SlideBlueprint(this.GetXmlAttribute(node6, "UIName"), this.GetXmlAttribute(node6, "Title"), this.GetXmlAttribute(node6, "StartDate", DateTime.MinValue), this.GetXmlAttribute(node6, "EndDate", DateTime.MaxValue), insertion);
                                        list.Add(blueprint2);
                                        if (!dictionary.ContainsKey(blueprint2.UIName))
                                        {
                                            dictionary.Add(blueprint2.UIName, blueprint2);
                                        }
                                    }
                                    SlideDeckBlueprint blueprint3 = new SlideDeckBlueprint(dictionary[this.GetXmlAttribute(node5, "DefaultSlide")], this.GetXmlAttribute(node5, "UIName"), this.GetXmlAttribute(node5, "StartDate", DateTime.MinValue), this.GetXmlAttribute(node5, "EndDate", DateTime.MaxValue), blueprint, list.ToArray());
                                    this.Add(str2, blueprint3);
                                    num = 0;
                                }
                            }
                        }
                    }
                }
                catch (Exception exception3)
                {
                    this.LastException = exception3;
                    this.Denial = new Denial("res://ehres!TODO_DENIAL_LOAD_ERROR.mcml", 100);
                }
            }

            private void OnXmlValidation(object sender, ValidationEventArgs e)
            {
                if (e.Severity == XmlSeverityType.Error)
                {
                    this.Denial = new Denial("res://ehres!TODO_DENIAL_INVALID_XML.mcml", 0x3e8);
                }
            }

            public bool Remove(KeyValuePair<string, SlideDeckBlueprint> item)
            {
                throw new NotImplementedException();
            }

            public bool Remove(string key)
            {
                throw new NotImplementedException();
            }

            public void SafeLoadXml()
            {
                if (Microsoft.MediaCenter.UI.Application.IsApplicationThread || !this.LoadFromApplicationThread)
                {
                    this.LoadXml(null);
                }
                else
                {
                    Microsoft.MediaCenter.UI.Application.DeferredInvoke(new Microsoft.MediaCenter.UI.DeferredHandler(this.LoadXml));
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(string key, out SlideDeckBlueprint blueprint)
            {
                string str;
                if (!this._aliases.TryGetValue(key, out str))
                {
                    str = key;
                }
                return this.TryGetValueInternal(str, out blueprint);
            }

            private bool TryGetValueInternal(string key, out SlideDeckBlueprint blueprint)
            {
                Node deepestNodeWithBlueprint = this.GetDeepestNodeWithBlueprint(key);
                if (deepestNodeWithBlueprint != null)
                {
                    blueprint = deepestNodeWithBlueprint.Blueprint;
                    return true;
                }
                blueprint = null;
                return false;
            }

            // Properties
            public int Count
            {
                get
                {
                    return this._count;
                }
            }

            public Denial Denial
            {
                get
                {
                    return this._denial;
                }
                private set
                {
                    if ((null == value) && (null != this._denial))
                    {
                        this._denial = value;
                        this.State = ContextObjectState.DenialWithdrawn;
                        this.LastException = null;
                    }
                    else if (null != value)
                    {
                        this._denial = value;
                        this.State = ContextObjectState.Denial;
                    }
                }
            }

            internal static SlideMarkupFinder.UIContextTree Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new SlideMarkupFinder.UIContextTree();
                        _instance.SafeLoadXml();
                    }
                    return _instance;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public SlideDeckBlueprint this[string key]
            {
                get
                {
                    SlideDeckBlueprint blueprint;
                    if (!this.TryGetValue(key, out blueprint))
                    {
                        return null;
                    }
                    return blueprint;
                }
                set
                {
                    Node deepestNodeWithBlueprint = this.GetDeepestNodeWithBlueprint(key);
                    if (deepestNodeWithBlueprint != null)
                    {
                        deepestNodeWithBlueprint.Blueprint = value;
                    }
                    else
                    {
                        this.Add(key, value);
                    }
                }
            }

            public ICollection<string> Keys
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public Exception LastException
            {
                get
                {
                    return this._lastException;
                }
                private set
                {
                    this._lastException = value;
                }
            }

            public bool LoadFromApplicationThread
            {
                get
                {
                    return this._loadFromApplicationThread;
                }
                set
                {
                    this._loadFromApplicationThread = value;
                }
            }

            public ContextObjectState State
            {
                get
                {
                    return this._state;
                }
                private set
                {
                    this._state = value;
                    if (this.StateChanged != null)
                    {
                        this.StateChanged(this, new ContextObjectStateEventArgs(value));
                    }
                }
            }

            public ICollection<SlideDeckBlueprint> Values
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            // Nested Types
            //[DebuggerDisplay("Name={Name}, Path={FullPath}, Nodes={Nodes.Count}, SlideDeck={SlideDeck}")]
            private sealed class Node
            {
                // Fields
                private SlideMarkupFinder.UIContextTree.Node _parent;
                public SlideDeckBlueprint Blueprint;
                public string Name;
                public SortedList<string, SlideMarkupFinder.UIContextTree.Node> Nodes;

                // Methods
                public Node()
                {
                    this.Nodes = new SortedList<string, SlideMarkupFinder.UIContextTree.Node>(StringComparer.OrdinalIgnoreCase);
                }

                public Node(SlideMarkupFinder.UIContextTree.Node parent, string key, SlideDeckBlueprint value)
                    : this()
                {
                    string[] strArray = key.Split(new char[] { '/' });
                    this.Name = strArray[0];
                    this._parent = parent;
                    if (1 == strArray.Length)
                    {
                        this.Blueprint = value;
                    }
                    else
                    {
                        SlideMarkupFinder.UIContextTree.Node node = this;
                        for (int i = 1; i < strArray.Length; i++)
                        {
                            SlideMarkupFinder.UIContextTree.Node node2 = new SlideMarkupFinder.UIContextTree.Node
                            {
                                Name = strArray[i]
                            };
                            node.Nodes.Add(node2.Name, node2);
                            node2._parent = node;
                            node = node2;
                            if (i == (strArray.Length - 1))
                            {
                                node2.Blueprint = value;
                            }
                        }
                    }
                }

                // Properties
                public string FullPath
                {
                    get
                    {
                        if (this._parent == null)
                        {
                            return this.Name;
                        }
                        return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[] { this._parent.FullPath, this.Name });
                    }
                }
            }
        }
    }

    public interface ISlideMarkupFinder
    {
        // Methods
        void AddContextTreeSource(string source);
        void ClearContextTreeSourceList();
        IList<string> GetContextTreeSourceList();
        SlideDeckBlueprint Lookup(string context);
    }

    public interface IContextListObject : IDenialSource
    {
        // Events
        event SlideStateEventHandler StateChanged;

        // Methods
        void AddContextToList(string context);
        void ClearContextList();
        IList<string> GetContextList();

        // Properties
        ContextObjectState State { get; }
    }

    public interface IDenialSource
    {
        // Properties
        Denial Denial { get; }
        Exception LastException { get; }
    }

    public class Denial : Announcement
    {
        // Methods
        public Denial(string uiName, int priority)
            : base(uiName, priority)
        {
        }
    }

    public class Announcement : IComparable<Announcement>
    {
        // Fields
        private string _message;
        private int _priority;
        private string _uiName;

        // Methods
        public Announcement(string uiName, int priority)
        {
            this._uiName = uiName;
            this._priority = priority;
        }

        public virtual int CompareTo(Announcement other)
        {
            if (other == null)
            {
                return 1;
            }
            return -this.Priority.CompareTo(other.Priority);
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override bool Equals(object other)
        {
            return (((other != null) && (base.GetType() == other.GetType())) && ((bool)((this.CompareTo(other as Announcement) == 0))));
        }

        public static Announcement First(Announcement left, Announcement right)
        {
            return ((left.CompareTo(right) <= 0) ? right : left);
        }

        public override int GetHashCode()
        {
            if (this._uiName == null)
            {
                return 0;
            }
            return this._uiName.GetHashCode();
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool operator ==(Announcement left, Announcement right)
        {
            //if (left == null)
            //{
            //    return (bool)((right == null));
            //}
            //return (bool)((left.CompareTo(right) == 0));
            return true;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool operator >(Announcement left, Announcement right)
        {
            //if (left == null)
            //{
            //    return false;
            //}
            //return (bool)((left.CompareTo(right) > 0));
            return true;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool operator !=(Announcement left, Announcement right)
        {
            //if (left == null)
            //{
            //    return ((Convert.ToInt32(right)!= null) ? 1 : 0);
            //}
            //return ((left.CompareTo(right) != 0));// ? ((bool)(1)) : ((bool)(0)));
            return true;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool operator <(Announcement left, Announcement right)
        {
            //if (left == null)
            //{
            //    return ((right != null) ? ((bool)(1)) : ((bool)(0)));
            //}
            //return (bool)(left.CompareTo(right) < 0);
            return true;
        }

        // Properties
        public virtual string Message
        {
            get
            {
                return this._message;
            }
        }

        public virtual int Priority
        {
            get
            {
                return this._priority;
            }
        }

        public virtual string UIName
        {
            get
            {
                return this._uiName;
            }
        }
    }

    public delegate void SlideStateEventHandler(object sender, ContextObjectStateEventArgs e);

    [TypeConverter(typeof(ResourceTypeConverter))]
    public abstract class Resource
    {
        // Events
        public event AcquireCompleteHandler AcquireComplete;

        // Methods
        protected Resource()
        {
        }

        public virtual bool Acquire()
        {
            return false;
        }

        protected static byte[] ByteArrayFromIntPtr(IntPtr pData, int nLength)
        {
            byte[] destination = new byte[nLength];
            Marshal.Copy(pData, destination, 0, nLength);
            return destination;
        }

        protected static byte[] ByteArrayFromStream(Stream stream)
        {
            stream.Position = 0L;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);
            return buffer;
        }

        public virtual void CancelAcquire()
        {
        }

        public abstract void Close();
        public void FireAcquireComplete()
        {
            if (this.AcquireComplete != null)
            {
                this.AcquireComplete();
            }
        }

        protected static IntPtrFromManaged IntPtrFromByteArray(byte[] arByteArray)
        {
            return new IntPtrFromManaged(arByteArray);
        }

        protected static IntPtrFromManaged IntPtrFromStream(Stream stream)
        {
            return new IntPtrFromManaged(stream);
        }

        public abstract ResourceStatus Open();
        protected static Stream StreamFromByteArray(byte[] arByteArray)
        {
            return new MemoryStream(arByteArray);
        }

        protected static Stream StreamFromIntPtr(IntPtr buffer, long nLength)
        {
            return new StreamFromNative(buffer, nLength);
        }

        // Properties
        public abstract IntPtr Buffer { get; }

        public abstract byte[] ByteArray { get; }

        public abstract string Host { get; }

        public abstract string Identifier { get; }

        public virtual bool IsRemote
        {
            get
            {
                return false;
            }
        }

        public abstract int Length { get; }

        public abstract Stream Stream { get; }

        public virtual Exception UnderlyingErrorFromOpen
        {
            get
            {
                return null;
            }
        }
    }

    public class ResourceTypeConverter : TypeConverter
    {
        // Methods
        public override bool CanConvertFrom(ITypeDescriptorContext tdc, Type typeSource)
        {
            return ((typeSource == typeof(string)) || base.CanConvertFrom(tdc, typeSource));
        }

        public override object ConvertFrom(ITypeDescriptorContext tdc, CultureInfo ci, object value)
        {
            string stUri = value as string;
            if (stUri == null)
            {
                return base.ConvertFrom(tdc, ci, value);
            }
            Resource resource = ResourceManager.Instance.GetResource(stUri);
            if (resource == null)
            {
                throw new ArgumentException(InvariantString.Format("Invalid Resource value '{0}'", new object[] { stUri }));
            }
            return resource;
        }
    }

    public delegate void AcquireCompleteHandler();

    public class IntPtrFromManaged : IDisposable
    {
        // Fields
        private GCHandle m_gcData;
        private int m_nLength;
        private IntPtr m_pBuffer;

        // Methods
        public IntPtrFromManaged(byte[] arData)
        {
            this.LockBuffer(arData);
        }

        public IntPtrFromManaged(Stream stream)
        {
            stream.Position = 0L;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);
            this.LockBuffer(buffer);
        }

        public void Dispose()
        {
            if (this.m_gcData.IsAllocated)
            {
                this.m_gcData.Free();
            }
        }

        private void LockBuffer(byte[] arData)
        {
            this.m_nLength = arData.Length;
            this.m_gcData = GCHandle.Alloc(arData, GCHandleType.Pinned);
            this.m_pBuffer = this.m_gcData.AddrOfPinnedObject();
        }

        // Properties
        public IntPtr Buffer
        {
            get
            {
                return this.m_pBuffer;
            }
        }

        public int Length
        {
            get
            {
                return this.m_nLength;
            }
        }
    }

    public enum ResourceStatus
    {
        Acquiring,
        Available,
        Error
    }

    public sealed class ResourceManager
    {
        // Fields
        private HybridDictionary m_dictSources = new HybridDictionary();
        internal const string ProtocolSeparator = "://";
        private static ResourceManager s_instance = new ResourceManager();

        // Methods
        private ResourceManager()
        {
        }

        public Resource GetResource(string stUri)
        {
            return this.GetResource(stUri, null);
        }

        public Resource GetResource(string stUri, IDictionary dictUriData)
        {
            Resource resource = null;
            string str;
            string str2;
            ParseUri(stUri, out str, out str2);
            if (str == null)
            {
                return null;
            }
            IResourceProvider source = (IResourceProvider)this.m_dictSources[str];
            //if (!MarkupVisibility.IsPermittedResourceProvider(source))
            //{
            //    source = null;
            //}
            if (source != null)
            {
                resource = source.GetResource(str2, stUri, dictUriData);
            }
            return resource;
        }

        public static void ParseUri(string stUri, out string stScheme, out string stResource)
        {
            int index = stUri.IndexOf("://");
            if (index > 0)
            {
                stScheme = stUri.Substring(0, index);
                stResource = stUri.Substring(index + "://".Length);
            }
            else
            {
                stScheme = null;
                stResource = stUri;
            }
        }

        public void RegisterSource(string stScheme, IResourceProvider source)
        {
            this.m_dictSources[stScheme] = source;
        }

        public void UnregisterSource(string stScheme)
        {
            this.m_dictSources.Remove(stScheme);
        }

        // Properties
        public static ResourceManager Instance
        {
            get
            {
                return s_instance;
            }
        }
    }

    public interface IResourceProvider
    {
        // Methods
        Resource GetResource(string stResource, string stUri, IDictionary dictUriData);
    }

    public class StreamFromNative : Stream
    {
        // Fields
        private IntPtr m_buffer;
        private long m_nLength;
        private long m_nPosition;

        // Methods
        public StreamFromNative(IntPtr buffer, long nLength)
        {
            this.m_buffer = buffer;
            this.m_nLength = nLength;
            this.m_nPosition = 0L;
        }

        public override void Flush()
        {
            throw new InvalidOperationException("Unsupported stream operation");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int length = (int)Math.Min(this.m_nLength - this.m_nPosition, (long)count);
            //if (length > 0)
            //{
            //    IntPtr zero = IntPtr.Zero;
            //    zero = (IntPtr)(this.m_buffer.ToPointer() + this.m_nPosition);
            //    Marshal.Copy(zero, buffer, offset, length);
            //    this.m_nPosition += length;
            //}
            return length;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException("Unsupported stream operation");
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException("Unsupported stream operation");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException("Unsupported stream operation");
        }

        // Properties
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override long Length
        {
            get
            {
                return this.m_nLength;
            }
        }

        public override long Position
        {
            get
            {
                return this.m_nPosition;
            }
            set
            {
                throw new InvalidOperationException("Unsupported stream operation");
            }
        }
    }
}
