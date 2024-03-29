﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DbToRest.Core
{
    public static class DbToRestVersion
    {
        private static readonly Version s_infoVersion = new Version("1.0.0.0");
        private static readonly Version s_version = Assembly.GetExecutingAssembly().GetName().Version;

        private static readonly List<Version> s_breakingChangesHistory = new List<Version>
        {
            // IMPORTANT: Add app versions from low to high
            // NOTE: do not specify build & revision unless you have good reasons for it.
            //       A release with breaking changes should definitely have at least
            //       a greater minor version.
            new Version("1.2"),
            new Version("1.2.1") // MC: had to be :-(
        };

        static DbToRestVersion()
        {
            s_breakingChangesHistory.Reverse();

            // get informational version
            var infoVersionAttr = Assembly.GetExecutingAssembly().GetAttribute<AssemblyInformationalVersionAttribute>(false);
            if (infoVersionAttr != null)
            {
                s_infoVersion = new Version(infoVersionAttr.InformationalVersion);
            }
        }

        /// <summary>
        /// Gets the app version
        /// </summary>
        public static string CurrentVersion
        {
            get
            {
                return "{0}.{1}".FormatInvariant(s_infoVersion.Major, s_infoVersion.Minor);
            }
        }

        /// <summary>
        /// Gets the app full version
        /// </summary>
        public static string CurrentFullVersion
        {
            get
            {
                return s_infoVersion.ToString();
            }
        }

        internal static Version FullVersion
        {
            get
            {
                //return s_version;
                return s_infoVersion; // MC: (???)
            }
        }

        /// <summary>
        /// Gets a list of SmartStore.NET versions in which breaking changes occured,
        /// which could lead to plugins malfunctioning.
        /// </summary>
        /// <remarks>
        /// A plugin's <c>MinAppVersion</c> is checked against this list to assume
        /// it's compatibility with the current app version.
        /// </remarks>
        internal static IEnumerable<Version> BreakingChangesHistory
        {
            get
            {
                return s_breakingChangesHistory.AsEnumerable();
            }
        }
    }
}