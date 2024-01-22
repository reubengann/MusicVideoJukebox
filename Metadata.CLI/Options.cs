using CommandLine;

namespace Metadata.CLI
{
    [Verb("create", HelpText = "Create metadata from a folder with nothing")]
    public class CreateOptions
    {
        [Value(0, MetaName = "library", HelpText = "The path to the library db")]
        public string LibraryPath { get; set; } = null!;

        [Value(1, MetaName = "folderPath", HelpText = "The path to the folder")]
        public string FolderPath { get; set; } = null!;
    }

    [Verb("shuffled", HelpText = "Create shuffled playlist")]
    public class ShuffledOptions
    {
        [Value(0, MetaName = "folderPath", HelpText = "The path to the folder")]
        public string FolderPath { get; set; } = null!;
    }
}
