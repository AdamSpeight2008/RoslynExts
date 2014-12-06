using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace DotNetAnalyzers.RoslynExts.CS
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
      return SyntaxFactory.ParseExpression(code) as T;
    }

    public static T ToSExpr<T>(this string code) where T : StatementSyntax
    {
      return SyntaxFactory.ParseStatement(code) as T;
    }

    public static Boolean KindIsAnyOf<T0>(T0 value, params SyntaxKind[] values)
      where T0 : SyntaxNode 
    {
      return values.Any(v => value.IsKind(v));
    }

    public static T0 WithSameTriviaAs<T0,T1>(this T0 target, T1 source)
      where T0 : SyntaxNode
      where T1 : SyntaxNode
    {
      if (target == null) throw new ArgumentNullException(nameof(target));
      if (source == null) throw new ArgumentNullException(nameof(source));
      return target
                .WithLeadingTrivia(source.GetLeadingTrivia())
                .WithTrailingTrivia(source.GetTrailingTrivia());
    }
  }
}
