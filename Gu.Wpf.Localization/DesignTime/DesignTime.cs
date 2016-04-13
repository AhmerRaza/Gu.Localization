﻿namespace Gu.Wpf.Localization
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static partial class DesignTime
    {
        internal static void Setup()
        {
            var assembly = AppDomain.CurrentDomain.GetDesigntimeEntryAssembly();
            var buildDirectory = assembly.BuildDirectory();
            var location = assembly.Location;
            Debugger.Break();
        }

        private static Assembly GetDesigntimeEntryAssembly(this AppDomain appDomain)
        {
            var assembly = appDomain.GetAssemblies()
                                    .FirstOrDefault(x => x.EntryPoint != null && x.GetName().Name != "XDesProc");
            return assembly;
        }

        private static DirectoryInfo BuildDirectory(this Assembly assembly)
        {
            var fileName = System.IO.Path.GetFileName(assembly.Location);
            var currentDirectory = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
            var fileInfos = currentDirectory.EnumerateFiles(fileName, SearchOption.AllDirectories)
                                            .OrderByDescending(x => x.LastWriteTime)
                                            .ToArray();
            return fileInfos.FirstOrDefault()
                ?.Directory;
        }
    }
}
