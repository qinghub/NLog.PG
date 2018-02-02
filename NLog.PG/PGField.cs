using NLog.Config;
using NLog.Layouts;
using System.ComponentModel;

namespace NLog.PG.NetCore
{ 
    [NLogConfigurationItem]
    public sealed class PGField
    { 
        public PGField()
            : this(null, null, "String")
        {
        }

        public PGField(string name, Layout layout)
            : this(name, layout, "String")
        {
        }

        public PGField(string name, Layout layout, string pgType)
        {
            Name = name;
            Layout = layout;
            PGType = pgType ?? "String";
        }
         
        [RequiredParameter]
        public string Name { get; set; }
 
        [RequiredParameter]
        public Layout Layout { get; set; }
 
        [DefaultValue("String")]
        public string PGType { get; set; }
    }
}
