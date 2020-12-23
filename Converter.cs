using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbDiagramIoToCSharp {
  public class Converter {

    /// <summary>
    /// Converts a script from dbdiagrams.io to csharp class
    /// </summary>
    public string ConvertToCSharp(string dbDiagramsScript) {
      StringBuilder result = new StringBuilder();

      // split the string to individual lines
      var scriptLines = new List<string>();
      scriptLines.AddRange(dbDiagramsScript.Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries));

      List<Class> classes = new List<Class>();

      for (int currentLineIndex = 0; currentLineIndex < scriptLines.Count; currentLineIndex++) {
        var currentLine = scriptLines[currentLineIndex];
        if (currentLine.TrimStart().StartsWith("table ", StringComparison.InvariantCultureIgnoreCase)) {
          var currentClass = ProcessTable(scriptLines, ref currentLineIndex);
          classes.Add(currentClass);
        } else if (currentLine.TrimStart().StartsWith("ref: ", StringComparison.InvariantCultureIgnoreCase)) {
          this.ProcessRef(currentLine, classes);
        }
      }

      foreach (var currentProcessedClass in classes) {
        result.AppendLine($"public class {currentProcessedClass.Name} {{");
        foreach (var currentProperty in currentProcessedClass.Properties) {
          foreach (var currentAttribute in currentProperty.Attributes) {
            result.AppendLine($"  [{currentAttribute}]");
          }
          result.AppendLine($"  public {currentProperty.Type} {currentProperty.Name} {{ get; set; }}");
        }
        result.AppendLine("}");
        result.AppendLine();
      }

      return result.ToString();
    }

    /// <summary>
    /// Extracts the table name from the script line
    /// </summary>
    public Class ProcessTable(List<string> scriptLines, ref int currentLineIndex) {
      var tableLine = scriptLines[currentLineIndex];
      var cleanedUpString = this.RemoveComments(tableLine.Trim().Substring(5));
      if (cleanedUpString.EndsWith("{")) {
        cleanedUpString = cleanedUpString.Substring(0, cleanedUpString.Length - 1);
      }
      var alias = "";
      if (cleanedUpString.Contains(" as ")) {
        alias = cleanedUpString.Substring(cleanedUpString.IndexOf(" as ")).Trim();
        cleanedUpString = cleanedUpString.Substring(0, cleanedUpString.IndexOf(" as "));
      }

      var result = new Class {
        Name = cleanedUpString.Trim(),
        Alias = alias.Trim()
      };

      // move on to the next line
      currentLineIndex++;

      // keep reading until the closing curly bracket is found
      for (; currentLineIndex < scriptLines.Count; currentLineIndex++) {
        string line = this.RemoveComments(scriptLines[currentLineIndex]);
        if (!string.IsNullOrWhiteSpace(line)) {
          if (!line.Contains("}")) {
            var property = ConvertColumnToProperty(line);
            result.Properties.Add(property);
          } else {
            break;
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Converts a column to a class property
    /// </summary>
    public Property ConvertColumnToProperty(string columnLine) {
      var spaceIndex = columnLine.IndexOf(" ");
      string name = columnLine.Substring(0, spaceIndex);
      string type = columnLine.Substring(spaceIndex).Split(" ", StringSplitOptions.RemoveEmptyEntries) [0].Trim().ToLower();
      var attributes = new List<string>();

      if (type.StartsWith("varchar", StringComparison.InvariantCultureIgnoreCase) || type.StartsWith("nvarchar", StringComparison.InvariantCultureIgnoreCase)) {
        type = "string";
      } else if (type.StartsWith("decimal", StringComparison.InvariantCultureIgnoreCase) || type.StartsWith("money", StringComparison.InvariantCultureIgnoreCase)) {
        type = "decimal";
      } else if (type.StartsWith("bigint", StringComparison.InvariantCultureIgnoreCase)) {
        type = "long";
      } else if (type.StartsWith("int", StringComparison.InvariantCultureIgnoreCase)) {
        type = "int";
      } else if (type.StartsWith("bigint", StringComparison.InvariantCultureIgnoreCase)) {
        type = "long";
      } else if (type.StartsWith("bit", StringComparison.InvariantCultureIgnoreCase)) {
        type = "bool";
      } else if (type.StartsWith("datetime", StringComparison.InvariantCultureIgnoreCase)) {
        type = "DateTime";
      } else if (type.StartsWith("timestamp", StringComparison.InvariantCultureIgnoreCase)) {
        attributes.Add("Timestamp");
        attributes.Add("JsonIgnore");
        type = "byte[]";
      }

      return new Property {
        Name = name.Trim(),
          Type = type.Trim(),
          Attributes = attributes
      };
    }

    /// <summary>
    /// Removes the comments from a line
    /// </summary>
    public string RemoveComments(string line) {
      if (line.Contains("//")) {
        return line.Substring(0, line.IndexOf("//")).Trim();
      } else {
        return line.Trim();
      }
    }

    /// <summary>
    /// Processes a ref line
    /// Adds a new reference property
    /// </summary>
    public void ProcessRef(string refLine, List<Class> cls) {
      refLine = this.RemoveComments(refLine);
      var splits = refLine.Substring(4).Split("<", StringSplitOptions.RemoveEmptyEntries);

      var destinationTableField = splits[0].Trim();
      var sourceTableField = splits[1].Trim();
      var destinationTable = destinationTableField.Split(".") [0].Replace("\"", "").Trim();
      var sourceTable = sourceTableField.Split(".") [0].Replace("\"", "").Trim();
      var sourceField = sourceTableField.Split(".") [1].Replace("\"", "").Trim();
      var c = cls.First(x => x.Name == destinationTable || x.Alias == destinationTable);
      var p = c.Properties.First(x => x.Name == sourceField);
      int index = c.Properties.IndexOf(p);
      c.Properties.Insert(index + 1, new Property {
        Name = sourceTable.Trim(),
          Type = sourceTable.Trim()
      });

    }
  }
}
