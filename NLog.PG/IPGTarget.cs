using System.Collections.Generic;

namespace NLog.PG.NetCore
{ 
    public interface IPGTarget
    { 
        string ConnectionString { get; set; } 
        string TableName { get; set; } 
        IList<PGField> Fields { get; } 
        IList<PGField> Properties { get; }
    }
}
