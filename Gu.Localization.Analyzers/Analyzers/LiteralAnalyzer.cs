namespace Gu.Localization.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class LiteralAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.GULOC03UseResource,
            Descriptors.GULOC06UseInterpolation);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.StringLiteralExpression);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (!context.IsExcludedFromAnalysis() &&
                context.Node is LiteralExpressionSyntax literal &&
                !string.IsNullOrWhiteSpace(literal.Token.ValueText) &&
                !IsExcludedFile(literal.SyntaxTree.FilePath))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.GULOC03UseResource, context.Node.GetLocation()));

                if (literal.Token.ValueText.Contains("{") &&
                    literal.Token.ValueText.Contains("}"))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptors.GULOC06UseInterpolation, context.Node.GetLocation()));
                }
            }
        }

        private static bool IsExcludedFile(string fileName)
        {
            return !fileName.EndsWith(".cs", StringComparison.Ordinal) ||
                   fileName.EndsWith(".g.cs", StringComparison.Ordinal) ||
                   fileName.EndsWith("Designer.cs", StringComparison.Ordinal) ||
                   fileName.EndsWith("AssemblyInfo.cs", StringComparison.Ordinal);
        }
    }
}
