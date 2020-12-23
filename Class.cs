using System.Collections.Generic;

namespace DbDiagramIoToCSharp {
  public class Class {
    public string Name { get; set; }
    public string Alias { get; set; }
    public Class InheritsFrom { get; set; }
    public List<Property> Properties { get; set; } = new List<Property>();
  }
}
