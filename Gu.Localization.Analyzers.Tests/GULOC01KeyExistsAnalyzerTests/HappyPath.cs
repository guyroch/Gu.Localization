namespace Gu.Localization.Analyzers.Tests.GULOC01KeyExistsAnalyzerTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    internal class HappyPath
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly DiagnosticDescriptor Descriptor = GULOC01KeyExists.Descriptor;

        private static readonly string ResourcesCode = @"
namespace RoslynSandbox.Properties {
    using System;

    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""System.Resources.Tools.StronglyTypedResourceBuilder"", ""15.0.0.0"")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute(""Microsoft.Performance"", ""CA1811:AvoidUncalledPrivateCode"")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager(""RoslynSandbox.Properties.Resources"", typeof(Resources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value.
        /// </summary>
        public static string Key {
            get {
                return ResourceManager.GetString(""Key"", resourceCulture);
            }
        }
    }
}";

        private static readonly string TranslateCode = @"
namespace RoslynSandbox.Properties
{
    using Gu.Localization;

    public static class Translate
    {
        /// <summary>Call like this: Translate.Key(nameof(Resources.Saved_file__0_)).</summary>
        /// <param name=""key"">A key in Properties.Resources</param>
        /// <param name=""errorHandling"">How to handle translation errors like missing key or culture.</param>
        /// <returns>A translation for the key.</returns>
        public static string Key(string key, ErrorHandling errorHandling = ErrorHandling.ReturnErrorInfoPreserveNeutral)
        {
            return TranslationFor(key, errorHandling).Translated;
        }

        /// <summary>Call like this: Translate.Key(nameof(Resources.Saved_file__0_)).</summary>
        /// <param name=""key"">A key in Properties.Resources</param>
        /// <param name=""errorHandling"">How to handle translation errors like missing key or culture.</param>
        /// <returns>A translation for the key.</returns>
        public static ITranslation TranslationFor(string key, ErrorHandling errorHandling = ErrorHandling.ReturnErrorInfoPreserveNeutral)
        {
            return Gu.Localization.Translation.GetOrCreate(Resources.ResourceManager, key, errorHandling);
        }
    }
}";

        [Test]
        public void TranslatorTranslateStringLiteralWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Translator.Translate(Resources.ResourceManager, ""Key"");
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, testCode);
        }

        [Test]
        public void TranslatorTranslateUnknownKey()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo(string key)
        {
            var translate = Translator.Translate(Resources.ResourceManager, key);
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public void TranslatorTranslateStringLiteralFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translate = Translator.Translate(Properties.Resources.ResourceManager, ""Key"");
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, testCode);
        }

        [Test]
        public void TranslatorTranslateNameofPropertyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Translator.Translate(Resources.ResourceManager, nameof(Resources.Key));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public void TranslatorTranslateNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translate = Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.Key));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public void TranslateKeyStringLiteralWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Translate.Key(""Key"");
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void TranslateKeyStringLiteralFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Translate.Key(""Key"");
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void TranslateKeyNameofPropertyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Translate.Key(nameof(Resources.Key));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void TranslateKeyNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Translate.Key(nameof(Properties.Resources.Key));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void TranslationGetOrCreateStringLiteralWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translation = Translation.GetOrCreate(Resources.ResourceManager, ""Key"");
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, testCode);
        }

        [Test]
        public void TranslationGetOrCreateStringLiteralFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, ""Key"");
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, testCode);
        }

        [Test]
        public void TranslationGetOrCreateNameofPropertyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translation = Translation.GetOrCreate(Resources.ResourceManager, nameof(Resources.Key));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public void TranslationGetOrCreateNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.Key));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public void ResourceManagerGetObjectStringLiteralWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Resources.ResourceManager.GetObject(""Key"");
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetObjectStringLiteralFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Resources.ResourceManager.GetObject(""Key"");
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetObjectNameofPropertyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Resources.ResourceManager.GetObject(nameof(Resources.Key));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetObjectNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Resources.ResourceManager.GetObject(nameof(Properties.Resources.Key));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetObjectWithCultureStringLiteralWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Globalization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Resources.ResourceManager.GetObject(""Key"", CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetObjectWithCultureStringLiteralFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Globalization;

    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Resources.ResourceManager.GetObject(""Key"", CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetObjectWithCultureNameofPropertyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Globalization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Resources.ResourceManager.GetObject(nameof(Resources.Key), CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetObjectWithCultureNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Globalization;

    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Resources.ResourceManager.GetObject(nameof(Properties.Resources.Key), CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetStringStringLiteralWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Resources.ResourceManager.GetString(""Key"");
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetStringStringLiteralFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Resources.ResourceManager.GetString(""Key"");
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetStringNameofPropertyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Resources.ResourceManager.GetString(nameof(Resources.Key));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetStringNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Globalization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Resources.ResourceManager.GetString(nameof(Properties.Resources.Key));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetStringWithCultureStringLiteralWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Globalization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Resources.ResourceManager.GetString(""Key"", CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetStringWithCultureStringLiteralFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Globalization;

    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Resources.ResourceManager.GetString(""Key"", CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, Descriptor, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetStringWithCultureNameofPropertyWithUsing()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Globalization;
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Resources.ResourceManager.GetString(nameof(Resources.Key), CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public void ResourceManagerGetStringWithCultureNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Globalization;

    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Resources.ResourceManager.GetString(nameof(Properties.Resources.Key), CultureInfo.GetCultureInfo(""sv-SE""));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, testCode);
        }

        [Test]
        public void ResourceManagerGetStringWithEnumToString()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo(SomeEnum someEnum)
        {
            var translate = Properties.Resources.ResourceManager.GetString(someEnum.ToString());
        }
    }

    public enum SomeEnum
    {
        Key,
    }
}";
            AnalyzerAssert.Valid(Analyzer, ResourcesCode, testCode);
        }
    }
}
