using CommandLine;
using Metadata.CLI;
using MusicVideoJukebox.Core;

await Parser.Default.ParseArguments<CreateOptions, ShuffledOptions>(args)
        .MapResult(
      (CreateOptions e) => HandleCreate(e),
      (ShuffledOptions e) => HandleShuffled(e),
      errs => Task.FromResult(0));

static async Task HandleShuffled(ShuffledOptions options)
{
    var path = Path.Combine(options.FolderPath, "meta.db");
    if (!File.Exists(path))
    {
        Console.WriteLine($"{path} does not exist");
        return;
    }

    var builder = new RandomPlaylistBuilder(path);
    await builder.BuildAsync();
}

static async Task HandleCreate(CreateOptions options)
{
    await BackfillVideoDatabaseBuilder.BuildAsync(options.LibraryPath, Path.Combine(options.FolderPath, "meta.db"));
}
