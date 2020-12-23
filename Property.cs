using System.Collections.Generic;

namespace DbDiagramIoToCSharp {
  public class Property {
    public string Name { get; set; }
    public string Type { get; set; }
    public List<string> Attributes { get; set; } = new List<string>();
  }
}
