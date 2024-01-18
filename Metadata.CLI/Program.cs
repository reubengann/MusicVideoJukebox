using MusicVideoJukebox.Core;
using System.Data.SQLite;

using (SQLiteConnection connection = new SQLiteConnection(@"Data Source=C:\Users\reube\Downloads\songs.db"))
{
    connection.Open();
    var library = VideoLibrary.FromFileList(Directory.EnumerateFiles("E:\\Videos\\Music Videos\\On Media Center").ToList());
    var agent = new MetadataAgent(connection, library);
    agent.GetMetadata();
}
