using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;

namespace NetBash
{
    internal static class AssemblyLocator
    {
        private static ReadOnlyCollection<Assembly> AllAssemblies = null;
        private static ReadOnlyCollection<Assembly> BinFolderAssemblies = null;

        public static ReadOnlyCollection<Assembly> GetAssemblies()
        {
            if (AllAssemblies == null)
            {
                AllAssemblies = new ReadOnlyCollection<Assembly>(BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList());
            }

            return AllAssemblies;
        }

        public static ReadOnlyCollection<Assembly> GetBinFolderAssemblies()
        {
            if (BinFolderAssemblies == null)
            {
                IList<Assembly> binFolderAssemblies = new List<Assembly>();

                string binFolder = HttpRuntime.AppDomainAppPath + "bin\\";
                IList<string> dllFiles = Directory.GetFiles(binFolder, "*.dll",
                    SearchOption.TopDirectoryOnly).ToList();

                foreach (string dllFile in dllFiles)
                {
                    try
                    {
                        AssemblyName assemblyName = AssemblyName.GetAssemblyName(dllFile);

                        Assembly locatedAssembly = AllAssemblies.FirstOrDefault(a =>
                            AssemblyName.ReferenceMatchesDefinition(a.GetName(), assemblyName));

                        if (locatedAssembly != null)
                        {
                            binFolderAssemblies.Add(locatedAssembly);
                        }
                    }
                    catch
                    {
                        //whatevs
                    }
                }

                BinFolderAssemblies = new ReadOnlyCollection<Assembly>(binFolderAssemblies);
            }

            return BinFolderAssemblies;
        }
    }
}
