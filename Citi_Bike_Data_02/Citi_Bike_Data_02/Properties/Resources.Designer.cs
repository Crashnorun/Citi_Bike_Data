﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Citi_Bike_Data_02.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Citi_Bike_Data_02.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #0b2a85.
        /// </summary>
        internal static string CitiBikeColorHEX {
            get {
                return ResourceManager.GetString("CitiBikeColorHEX", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 11,42,133.
        /// </summary>
        internal static string CitiBikeColorRGB {
            get {
                return ResourceManager.GetString("CitiBikeColorRGB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Data Source=(LocalDB)\\MSSQLLocalDB; database=master; Integrated security=True;.
        /// </summary>
        internal static string ConnectionStringBase {
            get {
                return ResourceManager.GetString("ConnectionStringBase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\cportelli\Documents\Personal\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\CitiBikeData.mdf;Integrated Security=True;Connect Timeout=30.
        /// </summary>
        internal static string ConnectionStringDebug {
            get {
                return ResourceManager.GetString("ConnectionStringDebug", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string ConnectionStringRelease {
            get {
                return ResourceManager.GetString("ConnectionStringRelease", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CitiBikeData.
        /// </summary>
        internal static string DBName {
            get {
                return ResourceManager.GetString("DBName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CSVFileNames.
        /// </summary>
        internal static string TableCSVFileName {
            get {
                return ResourceManager.GetString("TableCSVFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ZIPFileNames.
        /// </summary>
        internal static string TableZIPFileName {
            get {
                return ResourceManager.GetString("TableZIPFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://s3.amazonaws.com/tripdata.
        /// </summary>
        internal static string URLXML {
            get {
                return ResourceManager.GetString("URLXML", resourceCulture);
            }
        }
    }
}
