namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public class Sender
    {
        public string Id { get; set; }
        public string Name { get; set; }

        internal long CacheSize => Id.Length + Name.Length;
    }
}