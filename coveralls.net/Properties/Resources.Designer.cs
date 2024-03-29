﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace coveralls.net.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("coveralls.net.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to - Did you prefix your token with &apos;secure:&apos; without encrypting it?
        ///- Is this a Pull Request? AppVeyor does not decrypt environment variables for pull requests..
        /// </summary>
        internal static string AppVeyorBlankToken {
            get {
                return ResourceManager.GetString("AppVeyorBlankToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Blank or invalid Coveralls Repo Token..
        /// </summary>
        internal static string BlankTokenErrorMessage {
            get {
                return ResourceManager.GetString("BlankTokenErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Service: {0}
        ///      Job ID: {1}
        ///       Files: {2}
        ///      Commit: {3}
        ///Pull Request: {4}.
        /// </summary>
        internal static string CoverallsDebug {
            get {
                return ResourceManager.GetString("CoverallsDebug", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [debug] &gt; Coveralls Data:
        ///.
        /// </summary>
        internal static string CoverallsJsonHeader {
            get {
                return ResourceManager.GetString("CoverallsJsonHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unknown exception has occurred..
        /// </summary>
        internal static string GenericError {
            get {
                return ResourceManager.GetString("GenericError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No coverage statistics files..
        /// </summary>
        internal static string NoCoverageFilesErrorMessage {
            get {
                return ResourceManager.GetString("NoCoverageFilesErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [debug] &gt; $env.COVERALLS_REPO_TOKEN: {0}.
        /// </summary>
        internal static string RepoTokenDebug {
            get {
                return ResourceManager.GetString("RepoTokenDebug", resourceCulture);
            }
        }
    }
}
