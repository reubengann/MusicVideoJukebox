using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Test.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicVideoJukebox.Test.Unit
{
    public class MetadataManagerTest
    {
        MetadataManager dut;
        FakeVideoRepo videoRepo;
        FakeFileSystemService fileSystemService;

        public MetadataManagerTest()
        {
            fileSystemService = new FakeFileSystemService();
            videoRepo = new FakeVideoRepo();
            dut = new MetadataManager("thepath", videoRepo, fileSystemService);
        }

        [Fact]
        public async Task WhenNotExistingCreatesTables()
        {
            await dut.EnsureCreated();
            Assert.True(videoRepo.TablesCreated);
        }

        [Fact]
        public async Task WhenExistsDoNothing()
        {
            fileSystemService.ExistingFiles.Add(Path.Combine("thepath", "meta.db"));
            await dut.EnsureCreated();
            Assert.False(videoRepo.TablesCreated);
        }
    }
}
