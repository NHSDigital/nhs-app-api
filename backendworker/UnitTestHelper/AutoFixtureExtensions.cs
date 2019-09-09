using AutoFixture;
using AutoFixture.Kernel;

namespace UnitTestHelper
{
    public static class AutoFixtureExtensions
    {
        private const string NhsNumberFormattedRegex = @"\d{3} \d{3} \d{4}";
        private const string NhsNumberUnformattedRegex = @"\d{10}";
        
        public static string CreateNhsNumberUnformatted(this IFixture fixture)
        {
            return CreateStringFromRegex(fixture,NhsNumberUnformattedRegex);
        }

        public static string CreateNhsNumberFormatted(this IFixture fixture)
        {
            return CreateStringFromRegex(fixture,NhsNumberFormattedRegex);
        }

        public static string CreateStringFromRegex(this IFixture fixture, string pattern)
        {
            return (string)new SpecimenContext(fixture).Resolve(new RegularExpressionRequest(pattern));
        }

        public static string CreateStringOfLength(this IFixture fixture, int length)
        {
            return (string) new SpecimenContext(fixture).Resolve(new RegularExpressionRequest($"[a-z0-9]{{{length}}}"));
        }
    }
}