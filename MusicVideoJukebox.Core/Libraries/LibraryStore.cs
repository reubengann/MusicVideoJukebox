namespace MusicVideoJukebox.Core.Libraries
{
    public class LibraryStore
    {
        public int? LibraryId { get; private set; }
        public string? FolderPath { get; private set; }
        public int? PlaylistId { get; private set; }

        public void SetLibrary(int? libraryId, string? folderPath)
        {
            LibraryId = libraryId;
            FolderPath = folderPath;
        }

        public void SetPlaylist(int? playlistId)
        {
            PlaylistId = playlistId;
        }
    }
}
