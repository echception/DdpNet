namespace DdpNet
{
    public class Subscription
    {
        public string Name { get; private set; }
        internal string Id { get; private set; }

        internal Subscription(string id, string name)
        {
            this.Name = name;
            this.Id = id;
        }
    }
}
