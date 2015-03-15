using DdpNet;

namespace Microscope.Net.DataModel
{
    public class PostPageViewModel : BaseViewModel
    {
        public Post Post { get; set; }

        public DdpFilteredCollection<Comment> Comments { get; set; }
    }
}
