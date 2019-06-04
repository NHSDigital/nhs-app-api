using System;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.CidApi.Areas.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection;

namespace NHSOnline.Backend.CidApi.UnitTests.Areas.Im1Connection
{
    [TestClass]
    public class Im1ConnectionV2ErrorCodeMapperTests
    {
        [TestMethod]
        public void Mapping_Successful()
        {
            var result = Im1ConnectionV2ErrorCodeMapper.Map(Im1ConnectionErrorCodes.Code.InvalidLinkageDetails);
            result.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public void AllErrorCodesHaveMappings()
        {
            var unmappedCodes = Enum
                .GetValues(typeof(Im1ConnectionErrorCodes.Code))
                .Cast<Im1ConnectionErrorCodes.Code>()
                .Where(errorCode => Im1ConnectionV2ErrorCodeMapper.Map(errorCode) == 0)
                .ToList();

            if (unmappedCodes.Any())
            {
                Assert.Fail(
                    "The following Im1ConnectionErrorCodes do not have mappings in Im1ConnectionV2ErrorCodeMapper:\n" +
                    string.Join("\n", unmappedCodes));
            }
        }
    }
}
