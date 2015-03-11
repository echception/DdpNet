namespace Microscope.Net.DataModel
{
    using DdpNet;

    public class MainPageViewModel
    {
        public DdpFilteredCollection<Post> Posts { get; private set; }

        public MainPageViewModel(DdpCollection<Post> posts)
        {
            this.Posts = posts.Filter(sortFilter: (post1, post2) => post2.Submitted.DateTime.CompareTo(post1.Submitted.DateTime));
        }
    }
}
