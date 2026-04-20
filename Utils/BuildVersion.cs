using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Telinha.Utils
{
    public static class BuildInfo
    {
        /// <summary>
        /// Retorna a data e hora da compilação (build) do assembly.
        /// </summary>
        public static DateTime GetBuildDate(Assembly assembly = null!)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            var baseDirectory = AppContext.BaseDirectory;
            var filePath = Path.Combine(baseDirectory, $"{assembly.GetName().Name}.exe");

            const int PeHeaderOffset = 60;
            const int LinkerTimestampOffset = 8;

            byte[] buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                stream.ReadExactly(buffer);
            }

            int offset = BitConverter.ToInt32(buffer, PeHeaderOffset);
            int secondsSince1970 = BitConverter.ToInt32(buffer, offset + LinkerTimestampOffset);

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(secondsSince1970).ToLocalTime();
        }
    }