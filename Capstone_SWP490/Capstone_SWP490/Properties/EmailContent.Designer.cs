﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class EmailContent {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal EmailContent() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Capstone_SWP490.Properties.EmailContent", typeof(EmailContent).Assembly);
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
        ///   Looks up a localized string similar to &lt;p&gt;Wellcome to &lt;b&gt;&lt;mark&gt;ICPC Asia-VietNam Regional Contest&lt;/mark&gt;&lt;/b&gt;
        ///&lt;p&gt;Hi&lt;b&gt;{0}&lt;/b&gt;,&lt;/p&gt;
        ///&lt;p&gt;Your account has been accepted by Organization&lt;/p&gt;
        ///&lt;p&gt;password :&lt;b&gt;&lt;mark&gt;{1}&lt;/mark&gt;&lt;b&gt;
        ///&lt;p&gt;
        ///  visit: &lt;a href=&quot;{2}&quot;&gt;{3}&lt;/a&gt;
        ///&lt;/p&gt;
        ///.
        /// </summary>
        internal static string ConfirmAccount {
            get {
                return ResourceManager.GetString("ConfirmAccount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;Wellcome to &lt;b&gt;&lt;mark&gt;ICPC Asia-VietNam Regional Contest&lt;/mark&gt;&lt;/b&gt;
        ///&lt;p&gt;this is your temporary password: &lt;b&gt;{0}&lt;/b&gt;&lt;/p&gt;
        ///
        ///&lt;p&gt;Please &lt;mark&gt;change password and select your shirt size&lt;/mark&gt; for first login&lt;/p&gt;
        ///&lt;p&gt;
        ///  visit: &lt;a href=&quot;{1}&quot;&gt;{2}&lt;/a&gt;
        ///&lt;/p&gt;
        ///.
        /// </summary>
        internal static string CreateAccount {
            get {
                return ResourceManager.GetString("CreateAccount", resourceCulture);
            }
        }
    }
}
