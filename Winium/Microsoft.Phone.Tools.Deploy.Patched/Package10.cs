namespace Microsoft.Phone.Tools.Deploy.Patched
{
    #region

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    using Microsoft.Phone.Tools.Appx;
    using Microsoft.Phone.Tools.Deploy;

    #endregion

    [DesignerCategory("code")]
    [XmlRoot("Package", IsNullable = false, 
        Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    [Serializable]
    public class Package10 : IAppManifestInfo
    {
        #region Fields

        private bool fromBundle;

        #endregion

        #region Public Properties

        [XmlIgnore]
        public IEnumerable<AppDependency> AppDependencies
        {
            get
            {
                if (this.Dependencies != null)
                {
                    foreach (var packageDependency in this.Dependencies.PackageDependencies)
                    {
                        var ver = new Version(0, 0);
                        if (!string.IsNullOrEmpty(packageDependency.MinVersion))
                        {
                            ver = new Version(packageDependency.MinVersion);
                        }

                        yield return new AppDependency(packageDependency.Name, ver);
                    }
                }
            }
        }

        [XmlIgnore]
        public Version AppVersion
        {
            get
            {
                return new Version(this.Identity.Version);
            }
        }

        [XmlArrayItem("Application", IsNullable = false)]
        public CT_ApplicationsApplication[] Applications { get; set; }

        public CT_Capabilities Capabilities { get; set; }

        [XmlElement("Dependencies")]
        public PDependencies Dependencies { get; set; }

        [XmlArrayItem("Extension", IsNullable = false)]
        public CT_PackageExtensionsExtension[] Extensions { get; set; }

        public CT_Identity Identity { get; set; }

        [XmlAttribute]
        public string IgnorableNamespaces { get; set; }

        [XmlIgnore]
        public bool IsNative
        {
            get
            {
                return false;
            }
        }

        [XmlIgnore]
        public string Name
        {
            get
            {
                return this.Identity.Name;
            }
        }

        [XmlIgnore]
        public PackageType PackageType
        {
            get
            {
                if (this.fromBundle)
                {
                    return PackageType.Bundle;
                }

                return !this.Properties.Framework ? PackageType.Main : PackageType.Framework;
            }
        }

        [XmlElement(Namespace = "http://schemas.microsoft.com/appx/2014/phone/manifest")]
        public PhoneIdentity PhoneIdentity { get; set; }

        [XmlIgnore]
        public Version PlatformVersion
        {
            get
            {
                if (this.Dependencies == null)
                {
                    return new Version(10, 0);
                }

                var windowsUniversalTarget = this.Dependencies.TargetDeviceFamilyDependencies.FirstOrDefault(x => x.Name.Equals("Windows.Universal"));
                if (windowsUniversalTarget == null)
                {
                    return new Version(10, 0);
                }

                var minVersion = new Version(windowsUniversalTarget.MinVersion);

                return new Version(minVersion.Major, minVersion.Minor);
            }
        }

        public CT_Prerequisites Prerequisites { get; set; }

        [XmlIgnore]
        public string ProcessorArchitecture
        {
            get
            {
                return ((object)this.Identity.ProcessorArchitecture).ToString();
            }
        }

        [XmlIgnore]
        public Guid ProductId
        {
            get
            {
                if (this.PhoneIdentity != null)
                {
                    return new Guid(this.PhoneIdentity.PhoneProductId);
                }

                if (this.Applications != null)
                {
                    var applicationsApplication = this.Applications.FirstOrDefault();
                    if (applicationsApplication != null)
                    {
                        return new Guid(applicationsApplication.Id.Replace('y', '-').Replace('x', ' ').Trim());
                    }
                }

                return Guid.Empty;
            }
        }

        public CT_Properties Properties { get; set; }

        [XmlArrayItem("Resource", IsNullable = false)]
        public CT_ResourcesResource1[] Resources { get; set; }

        #endregion

        #region Public Methods and Operators

        public static IAppManifestInfo LoadFromStream(Stream stream, bool fromBundle = false)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var package = (IAppManifestInfo)new XmlSerializer(typeof(Package10)).Deserialize(streamReader);
                return package;
            }
        }

        #endregion

        public class PDependencies
        {
            #region Public Properties

            [XmlElement("PackageDependency")]
            public List<CT_DependenciesPackageDependency> PackageDependencies { get; set; }

            [XmlElement("TargetDeviceFamily")]
            public List<TargetDeviceFamily> TargetDeviceFamilyDependencies { get; set; }

            #endregion
        }

        public class TargetDeviceFamily : CT_DependenciesPackageDependency
        {
        }
    }
}
