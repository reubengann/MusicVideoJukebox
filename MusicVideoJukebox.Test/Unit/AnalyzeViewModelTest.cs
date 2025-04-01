using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class AnalyzeViewModelTest
    {
        AnalyzeViewModel dut;
        FakeStreamAnalyzer streamAnalyzer;
        FakeThreadDispatcher threadDispatcher;
        LibraryStore libraryStore;
        FakeLibrarySetRepo librarySetRepo;
        FakeMetadataManagerFactory metadataManagerFactory;
        FakeAudioNormalizer audioNormalizer;

        public AnalyzeViewModelTest()
        {
            librarySetRepo = new FakeLibrarySetRepo();
            metadataManagerFactory = new FakeMetadataManagerFactory();
            libraryStore = new LibraryStore(librarySetRepo, metadataManagerFactory);
            libraryStore.SetLibrary(1, "foobar").Wait();
            threadDispatcher = new FakeThreadDispatcher();
            streamAnalyzer = new FakeStreamAnalyzer();
            audioNormalizer = new FakeAudioNormalizer();
            dut = new AnalyzeViewModel(streamAnalyzer, threadDispatcher, metadataManagerFactory, libraryStore, audioNormalizer);
        }

        
    }
}
