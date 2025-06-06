// Copyright 2021 - Haga Rakotoharivelo - https://github.com/haga-rak

using System;
using System.Collections.Generic;
using System.Reflection;
using Fluxzy.Rules.Filters;

namespace Fluxzy
{
    /// <summary>
    /// Meta information about an archive.
    /// </summary>
    public class ArchiveMetaInformation
    {
        /// <summary>
        /// The date when the archive was created.
        /// </summary>
        public DateTime CaptureDate { get; set; } = DateTime.Now;

        /// <summary>
        /// List of available tags on this archive.
        /// </summary>
        public HashSet<Tag> Tags { get; set; } = new();

        /// <summary>
        /// List of available filters on this archive.
        /// </summary>
        public List<Filter> ViewFilters { get; set; } = new();

        /// <summary>
        /// Archive version
        /// </summary>
        public string ArchiveVersion { get; set; } = "0.2.0";

        /// <summary>
        ///  Information about the environment where the archive was created.
        /// </summary>
        public EnvironmentInformation? EnvironmentInformation { get; set; }

        /// <summary>
        /// Fluxzy version used to create this archive
        /// </summary>
        public string FluxzyVersion { get; set; } = Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        /// <summary>
        ///  Can be used to store additional information about the archive.
        /// </summary>

        public Dictionary<string, string> Properties { get; set; } = new();
    }
}
