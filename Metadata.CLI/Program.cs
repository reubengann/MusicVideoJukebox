using CommandLine;
using Metadata.CLI;
using MusicVideoJukebox.Core;


await Parser.Default.ParseArguments<CreateOptions>(args)
        .MapResult(
      HandleCreate,
      errs => Task.FromResult(0));

static async Task HandleCreate(CreateOptions options)
{
    await BackfillVideoDatabaseBuilder.BuildAsync(options.LibraryPath, Path.Combine(options.FolderPath, "meta.db"));
}
