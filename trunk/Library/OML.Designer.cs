﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1434
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Library {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class OML : global::System.Configuration.ApplicationSettingsBase {
        
        private static OML defaultInstance = ((OML)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new OML())));
        
        public static OML Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int DebugLevel {
            get {
                return ((int)(this["DebugLevel"]));
            }
            set {
                this["DebugLevel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ShowFileLocation {
            get {
                return ((bool)(this["ShowFileLocation"]));
            }
            set {
                this["ShowFileLocation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("FrontCoverArt")]
        public string GalleryView {
            get {
                return ((string)(this["GalleryView"]));
            }
            set {
                this["GalleryView"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("List")]
        public string ActorView {
            get {
                return ((string)(this["ActorView"]));
            }
            set {
                this["ActorView"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("List")]
        public string DirectorView {
            get {
                return ((string)(this["DirectorView"]));
            }
            set {
                this["DirectorView"] = value;
            }
        }
    }
}
