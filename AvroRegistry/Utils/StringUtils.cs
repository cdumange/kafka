using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JOS.Result;

namespace AvroRegistry.Utils
{
    internal abstract class StringUtils
    {
        public const string ErrVersionExtraction = "version";
        private const string fileDot = ".";
        private const string fileSlash = "/";
        internal static Result<int> ExtractVersionFromFile(string file)
        {
            var parts = file.Split(fileSlash);
            parts = parts[^1].Split(fileDot);
            if (parts.Length != 2)
            {
                return Result.Failure<int>(new Error(ErrVersionExtraction, "could not extract the version."));
            }

            var v = parts[0].Replace("v", "");
            int res = -1;
            if (!int.TryParse(v, out res))
            {
                return Result.Failure<int>(new Error(ErrVersionExtraction, $"{v} is not a int"));
            }

            return Result.Success(res);
        }
    }
}