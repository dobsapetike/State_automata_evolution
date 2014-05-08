using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace BenchmarkDepot.Classes.Misc
{

    /// <summary>
    /// Class for linking all the graph data info
    /// into a single CSV file
    /// </summary>
    public static class CSVCreator
    {

        public static void CreateCSV(string path)
        {
            var fileCount = 0;
            for (var i = 1; ; ++i)
            {
                if (!File.Exists(path + i + ".txt")) break;
                fileCount++;
            }
            if (fileCount == 0) return;

            var fileContent = new List<string>[fileCount];
            for (var i = 1; i <= fileCount ; i++)
            {
                fileContent[i - 1] = new List<string>(); 
                using (var sr = new StreamReader(path + i + ".txt"))
                {
                    while (sr.Peek() >= 0)
                    {
                        fileContent[i - 1].Add(sr.ReadLine());
                    }
                }
            }

            var writer = new StreamWriter(path + "result.csv");
            writer.NewLine = "\r\n";
            var max = fileContent.Max(x => x.Count);
            for (var i = 0; i < max; ++i)
            { 
                for (var j = 0; j < fileCount; ++j)
                {
                    if (i < fileContent[j].Count)
                    {
                        writer.Write(fileContent[j][i]);
                    }
                    if (j == fileCount - 1) break;
                    writer.Write(";");
                }
                writer.WriteLine();
            }
            writer.Close();
            writer.Dispose();
        }

    }

}
