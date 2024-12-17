using System;
using System.Linq;

namespace Syrna.FileManagement.Blazor.Components;

public static class SharedFunctions
{
    private static readonly string[] TextMimeTypes = ["text/plain", "application/json", "application/xml", "text/html", "text/richtext"];

    public static bool IsTextFile(string mimeType)
    {
        return TextMimeTypes.Contains(mimeType.ToLower());
    }

    private static readonly string[] ImageMimeTypes = ["image/jpg", "image/jpeg", "image/png", "image/bmp", "image/gif", "image/tiff"];

    public static bool IsImageFile(string mimeType)
    {
        return ImageMimeTypes.Contains(mimeType.ToLower());
    }

    public static string HumanFileSize(long bytes, bool si = false, int dp = 1)
    {
        var thresh = si ? 1000 : 1024;

        if (bytes == 0)
        {
            return "0 B";
        }

        if (Math.Abs(bytes) < thresh)
        {
            return bytes + " B";
        }

        var units = si
            ? new string[] { "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" }
            : new string[] { "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };

        var u = -1;
        var b = bytes;
        const double r = 10.0d;

        do
        {
            b /= thresh;
            u++;
        } while (Math.Round(Math.Abs(b) * r) / r >= thresh && u < units.Length - 1);

        return b.ToString("F" + dp) + " " + units[u];
    }
}
