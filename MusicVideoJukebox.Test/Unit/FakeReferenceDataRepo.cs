using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Unit
{
    internal class FakeReferenceDataRepo : IReferenceDataRepo
    {
        public List<SearchResult> ToReturn = [];


        public Task<List<SearchResult>> SearchReferenceDb(string queryString)
        {
            return Task.FromResult(ToReturn);
        }
    }
}