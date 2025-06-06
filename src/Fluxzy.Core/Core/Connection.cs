// Copyright 2021 - Haga Rakotoharivelo - https://github.com/haga-rak

using System;
using System.IO;
using System.Net;
using System.Threading;
using Fluxzy.Clients;

namespace Fluxzy.Core
{
    /// <summary>
    ///     Contains information about transport layer
    /// </summary>
    public class Connection : IRemoteLink
    {
        private int _requestProcessed;

        public Connection(Authority authority, IIdProvider idProvider)
        {
            Authority = authority;
            Id = idProvider.NextConnectionId();
        }

        public int Id { get; set; }

        public string? HttpVersion { get; set; }

        public int RequestProcessed => _requestProcessed;

        public Authority Authority { get; set; }

        public IPAddress? RemoteAddress { get; set; }

        public SslInfo? SslInfo { get; set; }

        public DateTime DnsSolveStart { get; set; }

        public DateTime DnsSolveEnd { get; set; }

        public DateTime TcpConnectionOpening { get; set; }

        public DateTime TcpConnectionOpened { get; set; }

        public DateTime SslNegotiationStart { get; set; }

        public DateTime SslNegotiationEnd { get; set; }

        public DateTime ProxyConnectStart { get; set; }

        public DateTime ProxyConnectEnd { get; set; }

        
        public int LocalPort { get; set; }

        public string? LocalAddress { get; set; }

        public Stream? WriteStream { get; set; }

        public Stream? ReadStream { get; set; }

        public int TimeoutIdleSeconds { get; set; } = -1; 

        public void AddNewRequestProcessed()
        {
            Interlocked.Increment(ref _requestProcessed);
        }
    }
}
