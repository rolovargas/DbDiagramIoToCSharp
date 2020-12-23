using System;
using System.IO;

namespace DbDiagramIoToCSharp {
  class Program {
    static void Main(string[] args) {

      if (args.Length != 1) {
        Console.WriteLine("Required input file path missing");
        return;
      }

      try {
        var fileName = args[0];
        StreamReader sr = new StreamReader(fileName);
        string fileContent = sr.ReadToEnd();
        sr.Close();

        Converter c = new Converter();
        string convertedScript = c.ConvertToCSharp(fileContent);
        Console.WriteLine(convertedScript);
      } catch (Exception ex) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Error processing file");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex);
        Console.ResetColor();
      }
    }
  }
}
