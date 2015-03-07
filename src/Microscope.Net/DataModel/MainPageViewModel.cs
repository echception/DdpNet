namespace Microscope.Net.DataModel
{
    using DdpNet;

    public class MainPageViewModel
    {
        public DdpCollection<Post> Posts { get; private set; }

        public MainPageViewModel(DdpCollection<Post> posts)
        {
            this.Posts = posts;
        }
    }
}
