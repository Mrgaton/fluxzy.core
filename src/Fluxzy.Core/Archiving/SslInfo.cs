// Copyright 2021 - Haga Rakotoharivelo - https://github.com/haga-rak

using System.Net.Security;
using System.Security.Authentication;
using System.Text.Json.Serialization;
using Fluxzy.Clients.Ssl.BouncyCastle;

namespace Fluxzy
{
    /// <summary>
    ///    Represent a SSL information
    /// </summary>
    public class SslInfo
    {
        /// <summary>
        ///     Building  from OsDefault
        /// </summary>
        /// <param name="sslStream"></param>
        public SslInfo(SslStream sslStream)
        {
            CipherAlgorithm = sslStream.CipherAlgorithm;
            HashAlgorithm = sslStream.HashAlgorithm;
            KeyExchangeAlgorithm = sslStream.KeyExchangeAlgorithm.ToString();
            NegotiatedApplicationProtocol = sslStream.NegotiatedApplicationProtocol.ToString();
            RemoteCertificateSubject = sslStream.RemoteCertificate?.Subject;
            RemoteCertificateIssuer = sslStream.RemoteCertificate?.Issuer;
            LocalCertificateIssuer = sslStream.LocalCertificate?.Issuer;
            LocalCertificateSubject = sslStream.LocalCertificate?.Subject;
            SslProtocol = sslStream.SslProtocol;
        }

        /// <summary>
        ///     Building from BouncyCastle
        /// </summary>
        /// <param name="clientProtocol"></param>
        internal SslInfo(FluxzyClientProtocol clientProtocol)
        {
//#if NET6_0
//            CipherAlgorithm = ((System.Net.Security.TlsCipherSuite) clientProtocol.SessionParameters.CipherSuite).ToString(); 
//#endif
            
            NegotiatedApplicationProtocol = clientProtocol.GetApplicationProtocol().ToString();
            SslProtocol = clientProtocol.GetSChannelProtocol();

            if (BcCertificateHelper.ReadInfo(clientProtocol.SessionParameters.LocalCertificate,
                    out var localSubject, out var localIssuer)) {
                LocalCertificateIssuer = localIssuer;
                LocalCertificateSubject = localSubject;
            }

            if (BcCertificateHelper.ReadInfo(clientProtocol.SessionParameters.PeerCertificate,
                    out var remoteSubject, out var remoteIssuer)) {
                RemoteCertificateIssuer = remoteIssuer;
                RemoteCertificateSubject = remoteSubject;
            }

            KeyExchangeAlgorithm = string.Empty;
        }

        [JsonConstructor]
        public SslInfo(
            SslProtocols sslProtocol, string? remoteCertificateIssuer, string? remoteCertificateSubject,
            string? localCertificateSubject, string? localCertificateIssuer, string negotiatedApplicationProtocol,
            string keyExchangeAlgorithm, HashAlgorithmType hashAlgorithm, CipherAlgorithmType cipherAlgorithm)
        {
            SslProtocol = sslProtocol;
            RemoteCertificateIssuer = remoteCertificateIssuer;
            RemoteCertificateSubject = remoteCertificateSubject;
            LocalCertificateSubject = localCertificateSubject;
            LocalCertificateIssuer = localCertificateIssuer;
            NegotiatedApplicationProtocol = negotiatedApplicationProtocol;
            KeyExchangeAlgorithm = keyExchangeAlgorithm;
            HashAlgorithm = hashAlgorithm;
            CipherAlgorithm = cipherAlgorithm;
        }

        public SslProtocols SslProtocol { get; }

        public string? RemoteCertificateIssuer { get; }

        public string? RemoteCertificateSubject { get; }

        public string? LocalCertificateSubject { get; }

        public string? LocalCertificateIssuer { get; }

        public string NegotiatedApplicationProtocol { get; }

        public string KeyExchangeAlgorithm { get; }

        public HashAlgorithmType HashAlgorithm { get; }

        public CipherAlgorithmType CipherAlgorithm { get; }

        public byte[]? RemoteCertificate { get; set; }

        public byte[]? LocalCertificate { get; set; }
    }
}
