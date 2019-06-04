using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Im1Connection;

namespace NHSOnline.Backend.GpSystems.UnitTests.Im1Connection
{
    [TestClass]
    public class Im1ConnectionErrorCodesTests
    {
        [TestMethod]
        public void ErrorResponses_Successful()
        {
            var errorCodesClass = new Im1ConnectionErrorCodes();
            var errorResponses = errorCodesClass.ErrorResponses;

            Assert.IsNotNull(errorResponses);
            Assert.AreEqual(33, errorResponses.Count, "Number of responses");
            
            var singleCode = (int) Im1ConnectionErrorCodes.Code.InvalidLinkageDetails;
            
            var specificResponse = errorResponses[singleCode];
            Assert.AreEqual(singleCode, specificResponse.ErrorCode);
            Assert.AreEqual("Invalid linkage details", specificResponse.ErrorMessage);
        }
    }
}
