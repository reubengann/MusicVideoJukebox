using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class MetadataEditViewModelTest
    {
        MetadataEditViewModel dut;
        FakeVideoRepo videoRepo;
        FakeMetadataProvider metadataProvider;

        public MetadataEditViewModelTest()
        {
            metadataProvider = new FakeMetadataProvider();
            videoRepo = new FakeVideoRepo();
            dut = new MetadataEditViewModel(videoRepo, metadataProvider);
        }

        [Fact]
        public async Task OnInitializePopulateTable() 
        {
            videoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist1", Filename = "filename1", Title = "title1" });
            videoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist2", Filename = "filename2", Title = "title2" });
            await dut.Initialize();
            Assert.Equal(2, dut.MetadataEntries.Count);
        }

    }
}
