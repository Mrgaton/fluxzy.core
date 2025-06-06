// Copyright 2021 - Haga Rakotoharivelo - https://github.com/haga-rak

using System.Collections.Generic;
using System.IO;

namespace Fluxzy
{
    internal static class DirectoryArchiveHelper
    {
        private static readonly int MaxItemPerDirectory = 100;

        internal static void CreateDirectory(string fullPath)
        {
            var fullDir = new FileInfo(fullPath).Directory;

            if (fullDir != null)
                Directory.CreateDirectory(fullDir.FullName);
        }

        internal static IEnumerable<FileInfo> EnumerateExchangeFileCandidates(string baseDirectory)
        {
            var targetPath = Path.Combine(baseDirectory, "exchanges");
            var directoryInfo = new DirectoryInfo(targetPath);

            return directoryInfo.EnumerateFiles("ex-*.mpack", SearchOption.AllDirectories);
        }

        internal static IEnumerable<FileInfo> EnumerateConnectionFileCandidates(string baseDirectory)
        {
            var targetPath = Path.Combine(baseDirectory, "connections");
            var directoryInfo = new DirectoryInfo(targetPath);

            return directoryInfo.EnumerateFiles("con-*.mpack", SearchOption.AllDirectories);
        }

        internal static string GetExchangePath(string baseDirectory, int exchangeId)
        {
            var baseNumber = exchangeId / MaxItemPerDirectory * 100;
            var directoryHint = $"{baseNumber}-{baseNumber + MaxItemPerDirectory}";

            var preDir = Path.Combine(baseDirectory, "exchanges", directoryHint);

            return Path.Combine(preDir, $"ex-{exchangeId}.mpack");
        }

        internal static string GetExchangePath(string baseDirectory, ExchangeInfo exchangeInfo)
        {
            return GetExchangePath(baseDirectory, exchangeInfo.Id);
        }

        internal static string GetMetaPath(string baseDirectory)
        {
            return Path.Combine(baseDirectory, "meta.json");
        }

        internal static string GetContentDirectory(string baseDirectory)
        {
            return Path.Combine(baseDirectory, "contents");
        }

        internal static string GetCaptureDirectory(string baseDirectory)
        {
            return Path.Combine(baseDirectory, "captures");
        }
        
        internal static string GetConnectionDirectory(string baseDirectory)
        {
            return Path.Combine(baseDirectory, "connections");
        }

        internal static string GetExchangeDirectory(string baseDirectory)
        {
            return Path.Combine(baseDirectory, "exchanges");
        }
        
        internal static string GetContentRequestPath(string baseDirectory, int exchangeId)
        {
            return Path.Combine(baseDirectory, "contents", $"req-{exchangeId}.data");
        }

        internal static string GetWebsocketContentRequestPath(string baseDirectory, int exchangeId, int messageId)
        {
            return Path.Combine(baseDirectory, "contents", $"req-{exchangeId}-ws-{messageId}.data");
        }

        internal static string GetWebsocketContentResponsePath(string baseDirectory, int exchangeId, int messageId)
        {
            return Path.Combine(baseDirectory, "contents", $"res-{exchangeId}-ws-{messageId}.data");
        }

        internal static string GetContentRequestPath(string baseDirectory, ExchangeInfo exchangeInfo)
        {
            return GetContentRequestPath(baseDirectory, exchangeInfo.Id);
        }

        internal static string GetContentResponsePath(string baseDirectory, ExchangeInfo exchangeInfo)
        {
            return GetContentResponsePath(baseDirectory, exchangeInfo.Id);
        }

        internal static string GetContentResponsePath(string baseDirectory, int exchangeId)
        {
            return Path.Combine(baseDirectory, "contents", $"res-{exchangeId}.data");
        }

        internal static string GetCapturePath(string baseDirectory, int connectionId)
        {
            return Path.Combine(baseDirectory, "captures", $"{connectionId}.pcapng");
        }

        internal static string GetErrorPath(string baseDirectory)
        {
            return Path.Combine(baseDirectory, "errors", $"errors.mpack");
        }

        internal static string GetCapturePathNssKey(string baseDirectory, int connectionId)
        {
            return Path.Combine(baseDirectory, "captures", $"{connectionId}.nsskeylog");
        }

        internal static string GetConnectionPath(string baseDirectory, int connectionId)
        {
            var baseNumber = connectionId / MaxItemPerDirectory * 100;
            var directoryHint = $"{baseNumber}-{baseNumber + MaxItemPerDirectory}";

            var preDir = Path.Combine(baseDirectory, "connections", directoryHint);

            return Path.Combine(preDir, $"con-{connectionId}.mpack");
        }

        internal static string GetConnectionPath(string baseDirectory, ConnectionInfo connectionInfo)
        {
            return GetConnectionPath(baseDirectory, connectionInfo.Id);
        }

        internal static bool TryParseIds(string directoryName, out (int StartId, int EndId) boundIds)
        {
            boundIds = (0, 0);
            
            var tab = directoryName.Split('-');

            if (tab.Length != 2)
                return false;

            if (!int.TryParse(tab[0], out var startId))
                return false;

            if (!int.TryParse(tab[1], out var endId))
                return false;

            boundIds = (startId, endId);
            return true;
        }
    }
}
