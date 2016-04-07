// Patched version of DependencyFinder for Windows 10 UWP manifest support
// Type: Microsoft.Phone.Tools.Deploy.DependencyFinder
// Assembly: Microsoft.Phone.Tools.Deploy, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86EA135B-1DD4-4D0E-BB40-E685CB8C02D7
// Assembly location: C:\Program Files (x86)\Microsoft SDKs\Windows Phone\v8.1\Tools\AppDeploy\Microsoft.Phone.Tools.Deploy.dll
namespace Microsoft.Phone.Tools.Deploy.Patched
{
    #region

    using System;
    using System.IO;
    using System.Linq;

    using Microsoft.Phone.Tools.Common;

    #endregion

    public class DependencyFinder
    {
        #region Public Methods and Operators

        public static string[] GetAppDependencyPackages(string packageFilePath)
        {
            var info = Utils.ReadAppManifestInfoFromPackage(packageFilePath);

            return
                info.AppDependencies.Select(depend => GetAppDependencyPackageFileName(packageFilePath, depend, info))
                    .Where(dependencyPackageFileName => !string.IsNullOrEmpty(dependencyPackageFileName))
                    .ToArray();
        }

        #endregion

        #region Methods

        private static bool DoesPackageFileMatchDependencyRequirement(string fileNameToCheck, AppDependency depend)
        {
            // NOTE: Uses patched Utils
            var appManifestInfo = Utils.ReadAppManifestInfoFromPackage(fileNameToCheck);
            if (StringComparer.InvariantCultureIgnoreCase.Equals(appManifestInfo.Name, depend.Name))
            {
                return appManifestInfo.AppVersion >= depend.MinVersion;
            }

            return false;
        }

        private static string FindMatchinDependencyRequirementInDirectory(
            string packageFileName, 
            string directory, 
            AppDependency depend)
        {
            if (!Directory.Exists(directory))
            {
                return null;
            }

            foreach (var str in Directory.GetFiles(directory, "*.appx", SearchOption.AllDirectories))
            {
                if (!StringComparer.InvariantCultureIgnoreCase.Equals(packageFileName, str)
                    && DoesPackageFileMatchDependencyRequirement(str, depend))
                {
                    return str;
                }
            }

            return null;
        }

        private static string GetAppDependencyPackageFileName(
            string packageFileName, 
            AppDependency depend, 
            IAppManifestInfo info)
        {
            var path1 = Path.Combine(Path.GetDirectoryName(packageFileName), "Dependencies");
            var str = Path.Combine(path1, info.ProcessorArchitecture);
            var strArray = new[]
                               {
                                   Path.GetDirectoryName(packageFileName), str, path1, Constants.WindowsPhoneApp81SDKRoot
                               };
            foreach (var directory in strArray)
            {
                var requirementInDirectory = FindMatchinDependencyRequirementInDirectory(
                    packageFileName, 
                    directory, 
                    depend);
                if (!string.IsNullOrEmpty(requirementInDirectory))
                {
                    return requirementInDirectory;
                }
            }

            return null;
        }

        #endregion
    }
}
