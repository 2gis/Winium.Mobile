#region

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Winium.StoreApps.InnerServer")]
[assembly:
    AssemblyDescription(
        "Essential part of Winium StoreApps (Selenium Remote WebDriver implementation) that should be included in tested app to enable automation."
        )]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("2gis")]
[assembly: AssemblyProduct("Winium.StoreApps.InnerServer")]
[assembly: AssemblyCopyright("Copyright ©  2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

// Version information for an assembly consists of the following four values:
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.6.0.*")]
[assembly: AssemblyFileVersionAttribute("1.6.0.0")]

[assembly: InternalsVisibleTo("Winium.StoreApps.InnerServer.Tests")]
[assembly: InternalsVisibleTo("UnitTestApp1")]