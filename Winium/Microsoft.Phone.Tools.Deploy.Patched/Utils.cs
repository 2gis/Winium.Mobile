// Patched version of Utils for Windows 10 UWP manifest support
// Type: Microsoft.Phone.Tools.Deploy.Utils
// Assembly: Microsoft.Phone.Tools.Deploy, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86EA135B-1DD4-4D0E-BB40-E685CB8C02D7
// Assembly location: C:\Program Files (x86)\Microsoft SDKs\Windows Phone\v8.1\Tools\AppDeploy\Microsoft.Phone.Tools.Deploy.dll
namespace Microsoft.Phone.Tools.Deploy.Patched
{
    #region

    using System;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Xml.Linq;

    using Microsoft.Phone.Tools.Appx;
    using Microsoft.Phone.Tools.Common;
    using Microsoft.Phone.Tools.Deploy;
    using Microsoft.Phone.Tools.Deploy.Properties;
    using Microsoft.SmartDevice.Connectivity.Interface;
    using Microsoft.SmartDevice.MultiTargeting.Connectivity;

    #endregion

    public static class Utils
    {
        #region Constants

        private const string AppXBundleManifestFile = "AppxMetadata/AppxBundleManifest.xml";

        private const string AppXManifestFile = "AppxManifest.xml";

        private const string Windows10ManifestNamespace =
            "http://schemas.microsoft.com/appx/manifest/foundation/windows10";

        #endregion

        #region Static Fields

        private static readonly string[] MdilListFileNames =
            {
                "MDILFileList.xml", "MDILFileListXap.xml", 
                "MDILFileListAppx.xml"
            };

        private static readonly Version WindowsPhone8Dot0Version = new Version(8, 0);

        private static readonly Version WindowsPhone8Dot1Version = new Version(8, 1);

        #endregion

        #region Delegates

        #endregion

        #region Enums

        private enum DeployAppMode
        {
            Install, 

            Update, 
        }

        #endregion

        #region Public Properties

        public static int ErrorCode { get; set; }

        #endregion

        #region Public Methods and Operators

        public static DeviceInfo[] GetDevices()
        {
            return
                new MultiTargetingConnectivity(CultureInfo.CurrentUICulture.LCID).GetConnectableDevices()
                    .Select(connectableDevice => new DeviceInfo(connectableDevice.Id, connectableDevice.Name))
                    .ToArray();
        }

        public static void InstallApplication(
            DeviceInfo deviceInfo, 
            IAppManifestInfo manifestInfo, 
            DeploymentOptions deploymentOptions, 
            string packageFile)
        {
            DeployApplication(DeployAppMode.Install, deviceInfo, manifestInfo, deploymentOptions, packageFile);
        }

        public static IAppManifestInfo ReadAppManifestInfoFromPackage(string path)
        {
            switch (DetermineAppType(path))
            {
                case TypeOfApp.XAP:
                    return Deploy.Utils.ReadAppManifestInfoFromPackage(path);
                case TypeOfApp.APPX:
                    return ReadAppManifestInfoFromAppx(path);
                case TypeOfApp.APPXBUNDLE:
                    return ReadAppxManifestXamlWithinBundle(path);
                default:
                    throw new Exception(Resources.ExceptionUnsupportedFileType);
            }
        }

        #endregion

        #region Methods

        internal static void ApplySideloadFlags(IAppManifestInfo manifestInfo, ref DeploymentOptions deploymentOptions)
        {
            if (manifestInfo.PackageType == PackageType.Main || manifestInfo.PackageType == PackageType.Bundle
                || manifestInfo.PackageType == PackageType.UnknownAppx)
            {
                deploymentOptions |= DeploymentOptions.Sideload;
            }
            else
            {
                if (manifestInfo.PackageType != PackageType.Framework)
                {
                    return;
                }

                deploymentOptions &= ~DeploymentOptions.Sideload;
            }
        }

        internal static TypeOfApp DetermineAppType(string packagePath)
        {
            var extension = Path.GetExtension(packagePath);
            if (!string.IsNullOrEmpty(extension))
            {
                switch (extension.ToLower(CultureInfo.InvariantCulture))
                {
                    case ".appxbundle":
                        return TypeOfApp.APPXBUNDLE;
                    case ".appx":
                        return TypeOfApp.APPX;
                    case ".xap":
                        return TypeOfApp.XAP;
                }
            }

            throw new NotImplementedException("This file extension is not supported by the tool.");
        }

        internal static string GenerateNDeployMdil(
            string packageFile, 
            IRemoteApplication app, 
            TypeOfApp appType, 
            IAppManifestInfo appManifest)
        {
            var generateMdil = (IGenerateMDIL)new MDILGeneratorFromPackage(packageFile, appType, appManifest);
            generateMdil.GenerateMDILTask();
            if (app != null)
            {
                try
                {
                    app.UpdateInstalledFilesInfo(
                        generateMdil.MSILFileList.ToArray(), 
                        generateMdil.RelativeMSILFileList.ToArray());
                    var num = app.UpdateInstalledFiles(
                        generateMdil.MDILFileList.ToArray(), 
                        generateMdil.RelativeMDILFileList.ToArray());
                    if (num != 0)
                    {
                        throw new Exception(
                            string.Format(Resources.MDILDeploymentFailure, num));
                    }
                }
                finally
                {
                    generateMdil.Cleanup();
                }
            }
            else
            {
                generateMdil.Cleanup();
            }

            return generateMdil.RepackedPackage;
        }

        internal static bool IsMdilFirst(TypeOfApp appType, IAppManifestInfo manifestInfo)
        {
            switch (appType)
            {
                case TypeOfApp.XAP:
                    return manifestInfo.PlatformVersion.Major == 8 && manifestInfo.PlatformVersion.Minor >= 1;
                case TypeOfApp.APPX:
                case TypeOfApp.APPXBUNDLE:
                    return true;
                default:
                    throw new NotImplementedException();
            }
        }

        internal static bool IsTargetApplicableforMdilGeneration(
            ConnectableDevice connectableDevice, 
            Version deviceVersion, 
            IAppManifestInfo info, 
            string appFile)
        {
            switch (info.PackageType)
            {
                case PackageType.UnknownAppx:
                    return IsXapApplicableForMdilGeneration(connectableDevice, deviceVersion, info.IsNative, appFile);
                case PackageType.Main:
                case PackageType.Resource:
                case PackageType.Bundle:
                    return IsAppxApplicableForMdilGeneration(connectableDevice, deviceVersion);
                case PackageType.Framework:
                    return false;
                default:
                    throw new NotImplementedException(
                        "MDIL generation applicability cannot be determined for unexpected file type.");
            }
        }

        private static void DeployApplication(
            DeployAppMode mode, 
            DeviceInfo deviceInfo, 
            IAppManifestInfo manifestInfo, 
            DeploymentOptions deploymentOptions, 
            string packageFile, 
            bool disconnect = true)
        {
            if (PackageType.Framework != manifestInfo.PackageType && WinTrust.VerifyEmbeddedSignature(packageFile))
            {
                throw new Exception(
                    string.Format(
                        CultureInfo.CurrentUICulture, 
                        Resources.InvalidPackaging, 
                        new object[0]));
            }

            var connectableDevice =
                new MultiTargetingConnectivity(CultureInfo.CurrentUICulture.LCID).GetConnectableDevice(
                    deviceInfo.DeviceId);
            var device = connectableDevice.Connect(true);
            var systemInfo = device.GetSystemInfo();
            var deviceVersion = new Version(systemInfo.OSMajor, systemInfo.OSMinor);
            if (manifestInfo.PlatformVersion.CompareTo(deviceVersion) > 0)
            {
                device.Disconnect();
                throw new Exception(
                    string.Format(
                        CultureInfo.CurrentUICulture, 
                        Resources.XapNotSupportedOnDevice, 
                        new object[0]));
            }

            var flag = IsTargetApplicableforMdilGeneration(
                connectableDevice, 
                deviceVersion, 
                manifestInfo, 
                packageFile);
            ApplySideloadFlags(manifestInfo, ref deploymentOptions);
            if (mode == DeployAppMode.Install)
            {
                if (device.IsApplicationInstalled(manifestInfo.ProductId))
                {
                    if (manifestInfo.PackageType == PackageType.Framework)
                    {
                        return;
                    }

                    device.GetApplication(manifestInfo.ProductId).Uninstall();
                }

                foreach (var str in DependencyFinder.GetAppDependencyPackages(packageFile))
                {
                    var manifestInfo1 = ReadAppManifestInfoFromPackage(str);
                    DeployApplication(
                        DeployAppMode.Install, 
                        deviceInfo, 
                        manifestInfo1, 
                        DeploymentOptions.OptOutSD, 
                        str, 
                        false);
                }
            }

            var app = (IRemoteApplication)null;
            if (mode == DeployAppMode.Update)
            {
                app = device.GetApplication(manifestInfo.ProductId);
            }

            var typeOfApp = DetermineAppType(packageFile);
            var applicationGenre = ((int)deploymentOptions).ToString(CultureInfo.InvariantCulture);
            var iconPath = ((int)manifestInfo.PackageType).ToString(CultureInfo.InvariantCulture);
            switch (mode)
            {
                case DeployAppMode.Install:
                    break;
                case DeployAppMode.Update:
                    break;
            }

            var path = (string)null;
            try
            {
                if (IsMdilFirst(typeOfApp, manifestInfo))
                {
                    if (flag)
                    {
                        path = GenerateNDeployMdil(packageFile, null, typeOfApp, manifestInfo);
                        packageFile = path;
                    }

                    switch (mode)
                    {
                        case DeployAppMode.Install:
                            app = device.InstallApplication(
                                manifestInfo.ProductId, 
                                manifestInfo.ProductId, 
                                applicationGenre, 
                                iconPath, 
                                packageFile);
                            break;
                        case DeployAppMode.Update:
                            app.UpdateApplication(applicationGenre, iconPath, packageFile);
                            break;
                    }
                }
                else
                {
                    switch (mode)
                    {
                        case DeployAppMode.Install:
                            app = device.InstallApplication(
                                manifestInfo.ProductId, 
                                manifestInfo.ProductId, 
                                applicationGenre, 
                                iconPath, 
                                packageFile);
                            break;
                        case DeployAppMode.Update:
                            app.UpdateApplication(applicationGenre, iconPath, packageFile);
                            break;
                    }

                    if (flag)
                    {
                        path = GenerateNDeployMdil(packageFile, app, typeOfApp, manifestInfo);
                    }
                }
            }
            finally
            {
                if (!GlobalOptions.LeaveBehindOptimized && !string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            if (GlobalOptions.LaunchAfterInstall && app != null)
            {
                app.Launch();
            }

            if (!disconnect)
            {
                return;
            }

            device.Disconnect();
        }

        private static bool IsAppxApplicableForMdilGeneration(
            ConnectableDevice connectableDevice, 
            Version deviceVersion)
        {
            return !connectableDevice.IsEmulator() && !(deviceVersion < WindowsPhone8Dot1Version);
        }

        private static bool IsXapApplicableForMdilGeneration(
            ConnectableDevice connectableDevice, 
            Version deviceVersion, 
            bool isNative, 
            string xapFile)
        {
            if (connectableDevice.IsEmulator() || deviceVersion < WindowsPhone8Dot0Version || isNative)
            {
                return false;
            }

            var fileStream = (FileStream)null;
            try
            {
                fileStream = new FileStream(xapFile, FileMode.Open, FileAccess.Read);
                using (var zipArchive = new ZipArchive(fileStream))
                {
                    fileStream = null;
                    var flag = MdilListFileNames.Any(entryName => zipArchive.GetEntry(entryName) != null);

                    if (flag)
                    {
                        return false;
                    }
                }
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }

            return true;
        }

        private static IAppManifestInfo ReadAppManifestInfoFromAppx(string packagePath)
        {
            try
            {
                using (var fileStream = new FileStream(packagePath, FileMode.Open, FileAccess.Read))
                {
                    return ReadAppManifestInfoFromAppxStream(fileStream);
                }
            }
            catch (Exception ex)
            {
                ErrorCode = ex.HResult;
                throw;
            }
        }

        private static IAppManifestInfo ReadAppManifestInfoFromAppxStream(Stream packageStream, bool fromBundle = false)
        {
            try
            {
                var entry = new ZipArchive(packageStream).GetEntry(AppXManifestFile);
                if (entry == null)
                {
                    throw new Exception(Resources.ExceptionManifestNotFound);
                }

                // NOTE: Following line is patched replacement of original code
                return ReadAppManifestInfoFromEntry(fromBundle, entry);
            }
            catch (Exception ex)
            {
                ErrorCode = ex.HResult;
                throw;
            }
        }

        /// <summary>
        /// Reads AppManifest from zip entry. Supports both 8.1 and pre 8.1 appx manifest format and UWP 10 manifest format.
        /// Patched version of code responsible for reading manifest from ZipEntry. Patch adds support for UWP 10 manifest.
        /// </summary>
        /// <param name="fromBundle"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        private static IAppManifestInfo ReadAppManifestInfoFromEntry(bool fromBundle, ZipArchiveEntry entry)
        {
            var ns = string.Empty;
            using (var stream = entry.Open())
            {
                var root = XDocument.Load(stream).Root;
                if (root != null)
                {
                    ns = root.GetDefaultNamespace().NamespaceName;
                }
            }

            if (ns.Equals(Windows10ManifestNamespace))
            {
                using (var stream = entry.Open())
                {
                    return Package10.LoadFromStream(stream, fromBundle);
                }
            }

            using (var stream = entry.Open())
            {
                return Package.LoadFromStream(stream, fromBundle);
            }
        }

        private static IAppManifestInfo ReadAppxManifestXamlWithinBundle(string packagePath)
        {
            try
            {
                using (var fileStream = new FileStream(packagePath, FileMode.Open, FileAccess.Read))
                {
                    var zipArchive = new ZipArchive(fileStream);
                    var entry1 = zipArchive.GetEntry(AppXBundleManifestFile);
                    if (entry1 == null)
                    {
                        throw new Exception(Resources.ExceptionManifestNotFound);
                    }

                    using (var stream = entry1.Open())
                    {
                        var bundle = Bundle.LoadFromStream(stream);
                        if (bundle.Packages != null)
                        {
                            if (bundle.Packages.Length > 0)
                            {
                                var bundlePackage =
                                    bundle.Packages.FirstOrDefault(pkg => pkg.Type == ST_PackageType.application);
                                if (bundlePackage == null)
                                {
                                    throw new Exception(
                                        Resources.ExceptionAppInBundleNotFound);
                                }

                                var fileName = bundlePackage.FileName;
                                var entry2 = zipArchive.GetEntry(fileName);
                                if (entry2 == null)
                                {
                                    throw new Exception(
                                        Resources.ExceptionAppInBundleNotFound);
                                }

                                using (var packageStream = entry2.Open())
                                {
                                    return ReadAppManifestInfoFromAppxStream(packageStream, true);
                                }
                            }
                        }
                    }

                    throw new Exception(Resources.ExceptionManifestNotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorCode = ex.HResult;
                throw;
            }
        }

        #endregion
    }
}
