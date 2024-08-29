using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvroRegistry.Utils;
using Xunit;
using Xunit.Sdk;

namespace AvroRegistry.Tests.Utils
{
    public class StringUtilsTests
    {
        [Theory]
        [InlineData("/test/v1.json", true, 1)]
        [InlineData("/test/v5.json", true, 5)]
        [InlineData("/test/toto/v1.json", true, 1)]
        [InlineData("v1.json", true, 1)]
        [InlineData("/test/va.json", false, null)]
        public void Test_ExtractVersionFromFile(string folder, bool isSuccess, int? version)
        {
            var res = StringUtils.ExtractVersionFromFile(folder);
            Assert.Equal(isSuccess, res.Succeeded);

            if (res.Succeeded && version.HasValue)
            {
                Assert.Equal(version.Value, res.Data);
            }
        }
    }
}