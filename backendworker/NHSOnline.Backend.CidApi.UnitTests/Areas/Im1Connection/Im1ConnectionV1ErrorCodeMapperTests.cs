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
    public class Im1ConnectionV1ErrorCodeMapperTests
    {
        [TestMethod]
        public void Mapping_Successful()
        {
            var result = Im1ConnectionV1ErrorCodeMapper.Map(Im1ConnectionErrorCodes.InternalCode.InvalidLinkageDetails);
            result.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public void AllErrorCodesHaveMappings()
        {
            var unmappedCodes = Enum
                .GetValues(typeof(Im1ConnectionErrorCodes.InternalCode))
                .Cast<Im1ConnectionErrorCodes.InternalCode>()
                .Where(errorCode => Im1ConnectionV1ErrorCodeMapper.Map(errorCode) == 0)
                .ToList();

            if (unmappedCodes.Any())
            {
                Assert.Fail(
                    "The following Im1ConnectionErrorCodes do not have mappings in Im1ConnectionV1ErrorCodeMapper:\n" +
                    string.Join("\n", unmappedCodes));
            }
        }
    }
}
