namespace Gu.Localization.Analyzers.Tests.GULOC01KeyExistsAnalyzerTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    internal class Diagnostics
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("GULOC01");

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
            var translate = Translator.Translate(Resources.ResourceManager, ↓""Missing"");
        }
    }
}";

            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, testCode);
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
            var translate = Translator.Translate(Properties.Resources.ResourceManager, ↓""Missing"");
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, testCode);
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
            var translate = Translator.Translate(Resources.ResourceManager, ↓nameof(Resources));
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, testCode);
        }

        [Test]
        public void TranslatorTranslateMissingNameof()
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
            var translate = Translator.Translate(Resources.ResourceManager, ↓Resources.Key);
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, testCode);
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
            var translate = Translator.Translate(Properties.Resources.ResourceManager, ↓nameof(Properties.Resources));
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, testCode);
        }

        [Test]
        public void TranslateTranslationForStringLiteralWithUsing()
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
            var translate = Translate.TranslationFor(↓""Missing"");
        }
    }
}";

            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void TranslateTranslationForStringLiteralFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Translate.TranslationFor(↓""Missing"");
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void TranslateTranslationForNameofPropertyWithUsing()
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
            var translate = Translate.TranslationFor(↓nameof(Resources));
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void TranslateTranslationForMissingNameof()
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
            var translate = Properties.Translate.TranslationFor(↓Resources.Key);
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void TranslateTranslationForNameofPropertyFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Translate.TranslationFor(↓nameof(Foo));
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
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
            var translate = Translate.Key(↓""Missing"");
        }
    }
}";

            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void TranslateKeyStringLiteralFullyQualified()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Localization;

    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Translate.Key(↓""Missing"");
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
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
            var translate = Translate.Key(↓nameof(Resources));
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void TranslateKeyMissingNameof()
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
            var translate = Properties.Translate.Key(↓Resources.Key);
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
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
            var translate = Properties.Translate.Key(↓nameof(Foo));
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
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
            var translation = Translation.GetOrCreate(Resources.ResourceManager, ↓""Missing"");
        }
    }
}";

            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, testCode);
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
            var translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, ↓""Missing"");
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, testCode);
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
            var translation = Translation.GetOrCreate(Resources.ResourceManager, ↓nameof(Resources));
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, testCode);
        }

        [Test]
        public void TranslationGetOrCreateMissingNameof()
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
            var translation = Translation.GetOrCreate(Resources.ResourceManager, ↓Resources.Key);
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, testCode);
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
            var translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, ↓nameof(Foo));
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, testCode);
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
            var translate = Resources.ResourceManager.GetObject(↓""Missing"");
        }
    }
}";

            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
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
            var translate = Properties.Resources.ResourceManager.GetObject(↓""Missing"");
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
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
            var translate = Resources.ResourceManager.GetObject(↓nameof(Resources));
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
        }

        [Test]
        public void ResourceManagerGetObjectMissingNameof()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class Foo
    {
        public Foo()
        {
            var translate = Properties.Resources.ResourceManager.GetObject(↓Resources.Key);
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
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
            var translate = Properties.Resources.ResourceManager.GetObject(↓nameof(Foo));
        }
    }
}";
            AnalyzerAssert.Diagnostics(Analyzer, ExpectedDiagnostic, ResourcesCode, TranslateCode, testCode);
        }
    }
}
