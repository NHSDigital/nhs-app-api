namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models
{
    public abstract class Matcher
    {
        protected Matcher(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
