using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Models
{
    [TestClass]
    public abstract class XmlTestBase<T>
    {
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(T));
        private T _model;
        private XElement _element;

        protected XElement Element
        {
            get { return _element; }
        }
        
        [TestInitialize]
        public void TestInitialize()
        {
            _model = CreateModel();
            _element = ToXElement(_model);
        }

        protected abstract T CreateModel();
        
        private string Serialize(T model)
        {
            var writer = new StringWriter();
            _serializer.Serialize(writer, model);
            return writer.ToString();
        }

        private XElement ToXElement(T model) => XElement.Parse(Serialize(model));
    }
}