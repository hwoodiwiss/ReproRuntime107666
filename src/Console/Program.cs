// See https://aka.ms/new-console-template for more information

using System.IO.Compression;

Console.WriteLine(GzipUtil.CompressDecompress("Hello, World!"));

class GzipUtil
{
    public static string CompressDecompress(string value)
    {
        var compressed = CompressToGZipString(value);

        return DecompressFromGZipString(compressed);
    }
    
    /// <summary>
    /// Compresses <paramref name="uncompressedString"/> and returns the result.
    /// </summary>
    /// <param name="uncompressedString">An uncompressed string.</param>
    /// <returns>A base-64 string.</returns>
    public static string CompressToGZipString(string uncompressedString)
    {
        using (var outputStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                // Write uncompressed string to the gzip stream which
                // writes to the underlying memory stream.
                using (var writer = new StreamWriter(gzipStream))
                {
                    writer.Write(uncompressedString);
                }
            }

            // Return a base-64 string from the byte array
            // in the memory stream.
            return Convert.ToBase64String(
                outputStream.ToArray()
            );
        }
    }

    /// <summary>
    /// Decompresses <paramref name="compressedBase64String"/> and returns the result.
    /// </summary>
    /// <param name="compressedBase64String">A compressed base-64 string.</param>
    /// <returns>A string.</returns>
    public static string DecompressFromGZipString(string compressedBase64String)
    {
        // Convert the base-64 string to a byte array.
        var bytes = Convert.FromBase64String(compressedBase64String);
        using (var ms = new MemoryStream(bytes))
        {
            using (var gs = new GZipStream(ms, CompressionMode.Decompress))
            {
                // Read from the gzip stream which reads 
                // from the underlying memory stream.
                using (var reader = new StreamReader(gs))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
