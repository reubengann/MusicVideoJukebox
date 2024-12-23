using MusicVideoJukebox.Core.ViewModels;

namespace MusicVideoJukebox.Test.Unit
{
    public class AnalyzeViewModelTest
    {
        AnalyzeViewModel dut;
        FakeStreamAnalyzer streamAnalyzer;
        FakeThreadDispatcher threadDispatcher;

        public AnalyzeViewModelTest()
        {
            threadDispatcher = new FakeThreadDispatcher();
            streamAnalyzer = new FakeStreamAnalyzer();
            dut = new AnalyzeViewModel(streamAnalyzer, threadDispatcher);
        }

        [Fact]
        public async Task LoadsThem()
        {
            await dut.Initialize();
            await Task.Delay(1);
            Assert.Equal(2, dut.AnalysisResults.Count);
        }
    }
}
