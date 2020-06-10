using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Models
{
    [TestClass]
    public abstract class XmlTestBase<T>
    {
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(T));
        private T _model;

        protected XElement Element { get; private set; }

        [TestInitialize]
        public void TestInitialize()
        {
            _model = CreateModel();
            Element = ToXElement(_model);
        }

        protected abstract T CreateModel();
        
        private string Serialize(T model)
        {
            using var writer = new StringWriter();
            _serializer.Serialize(writer, model);
            return writer.ToString();
        }

        private XElement ToXElement(T model) => XElement.Parse(Serialize(model));
    }
}