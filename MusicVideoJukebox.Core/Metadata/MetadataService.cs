namespace MusicVideoJukebox.Core.Metadata
{
    public class MetadataService : IMetadataService
    {
        public MetadataService(IMetadataWebService metadataService)
        {

        }

        public Task<MetadataGetResult> GetMetadata(string artist, string track)
        {
            throw new NotImplementedException();
        }
    }
}
