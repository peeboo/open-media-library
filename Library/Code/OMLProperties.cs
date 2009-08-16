﻿using System;
using System.Collections.Generic;
using Microsoft.MediaCenter.Hosting;
using OMLEngine;

namespace Library
{
    public class OMLProperties : Dictionary<string, object>
    {
        public OMLProperties()
        {
            this["App"] = OMLApplication.Current;
            this["Host"] = AddInHost.Current;
        }

        public OMLProperties(Dictionary<string, object> newProperties)
        {
            this["App"] = OMLApplication.Current;
            this["Host"] = AddInHost.Current;

            foreach (string key in newProperties.Keys)
                this.Add(key, newProperties[key]);
        }

        public OMLApplication App
        {
            get { return (OMLApplication)this["Application"]; }
        }

        public AddInHost Host
        {
            get { return (AddInHost)this["Host"]; }
        }
    }
}
