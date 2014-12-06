using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RoslynExts.CS
{
  static public class Exts
  {
    public static MethodDeclarationSyntax InWhichMethod(SyntaxNode sn)
    {
      return sn.FirstAncestorOrSelf<MethodDeclarationSyntax>();
    }

    public static TypeDeclarationSyntax InWhichTypeDeclaration(SyntaxNode sn)
    {
      return sn.FirstAncestorOrSelf<TypeDeclarationSyntax>();
    }

    public static T ToExpr<T>(this string code) where T : ExpressionSyntax
    {
      return ParseExpression(code).As<T>();
    }

    public static T ToSExpr<T>(this string code) where T : StatementSyntax
    {
      return ParseStatement(code).As<T>();
    }

    public static Boolean KindIsAnyOf<T0>(T0 value, params SyntaxKind[] values)
      where T0 : SyntaxNode
    {
      return values.Any(v => value.IsKind(v));
    }

    public static T0 WithSameTriviaAs<T0, T1>(this T0 target, T1 source)
      where T0 : SyntaxNode
      where T1 : SyntaxNode
    {
      if (target == null) throw new ArgumentNullException(nameof(target));
      if (source == null) throw new ArgumentNullException(nameof(source));
      return target
                .WithLeadingTrivia(source.GetLeadingTrivia())
                .WithTrailingTrivia(source.GetTrailingTrivia());
    }

    public static T1 @Try<T1>(this object o) where T1 : class
    {
      return o as T1;

    }
    public static T1 @As<T1>(this object o) where T1 : class
    {
      return (T1)o;
    }

    public static void RegisterSyntaxNodeAction<TLanguageKindEnum>
      ( this AnalysisContext context,
             LanguageVersion languageVersion,
      Action<SyntaxNodeAnalysisContext> action,
      params TLanguageKindEnum[] syntaxKinds
      )
      where TLanguageKindEnum : struct
    {
      context.RegisterCompilationStartAction( LanguageVersion.CSharp6,
        compilationContext => compilationContext.RegisterSyntaxNodeAction(action, syntaxKinds));
    }

    public static void RegisterCompilationStartAction
      ( this AnalysisContext context,
             LanguageVersion languageVersion,
       Action<CompilationStartAnalysisContext> registrationAction
      )
    {
      context.RegisterCompilationStartAction(
        compilationContext => compilationContext.RunIfCSharp6OrGreater(
          () => registrationAction(compilationContext)));
    }

    public static void RunIfCSharp6OrGreater
      ( this CompilationStartAnalysisContext context,
                                      Action action
      )
    {
      context.Compilation.RunIfCSharp6OrGreater(action);
    }

    public static void RunIfCSharp6OrGreater
      ( this Compilation compilation,
                  Action action
      )
    {
      compilation.Try<CSharpCompilation>()?.LanguageVersion.RunIfCSharp6OrGreater(action);
    }

    public static void RunIfCSharp6OrGreater
      ( this LanguageVersion languageVersion,
                      Action action
      )
      {
        if (languageVersion >= LanguageVersion.CSharp6) action();
      }

    public static ConditionalAccessExpressionSyntax ToConditionalAccessExpression
      ( this MemberAccessExpressionSyntax memberAccess )
    {
      return ConditionalAccessExpression( memberAccess.Expression,
                                          MemberBindingExpression(memberAccess.Name));
    }

    public static StatementSyntax GetSingleStatementFromPossibleBlock(this StatementSyntax statement)
    {
      var block = statement.Try<BlockSyntax>();
      return (block == null) ? statement : block.Statements.FirstOrDefault();
    }

  }
}
