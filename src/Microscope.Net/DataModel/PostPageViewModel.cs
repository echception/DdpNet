using DdpNet;

namespace Microscope.Net.DataModel
{
    public class PostPageViewModel
    {
        public Post Post { get; set; }

        public DdpFilteredCollection<Comment> Comments { get; set; }

        public MeteorClient Client { get; set; }
    }
}
