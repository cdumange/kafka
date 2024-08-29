


using AvroRegistry.Utils;
using Confluent.SchemaRegistry;
using Microsoft.VisualBasic;

internal class Program
{
    const string schemaDirectory = "Schemas";
    private static void Main(string[] args)
    {
        foreach (var folder in Directory.EnumerateDirectories("./Schemas"))
        {
            var f = folder + "/" + schemaDirectory;
            if (!Directory.Exists(f))
            {
                continue;
            }

            var versions = Directory.EnumerateFiles(f);
            if (versions.Any())
            {
                foreach (var file in versions)
                {
                    var version = StringUtils.ExtractVersionFromFile(file);
                    if (version.Failed)
                    {
                        Console.WriteLine($"failed parsing for {file}: {version.Error}");
                        continue;
                    }

                    Console.WriteLine($"treating version {version.Data} for {folder}");
                }
                continue;
            }
        }
    }
}