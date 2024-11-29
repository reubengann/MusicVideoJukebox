using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class MetadataEditViewModelTest
    {
        MetadataEditViewModel dut;
        FakeMetadataManagerFactory metadataManagerFactory;
        LibraryStore libraryStore;

        public MetadataEditViewModelTest()
        {
            libraryStore = new LibraryStore();
            metadataManagerFactory = new FakeMetadataManagerFactory();
            dut = new MetadataEditViewModel(metadataManagerFactory, libraryStore);
        }

        [Fact]
        public async Task OnInitializePopulateTable() 
        {
            libraryStore.FolderPath = "";
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist1", Filename = "filename1", Title = "title1" });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist2", Filename = "filename2", Title = "title2" });
            await dut.Initialize();
            Assert.Equal(2, dut.MetadataEntries.Count);
        }

    }
}
