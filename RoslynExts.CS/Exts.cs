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
  static class Exts
  {
    static MethodDeclarationSyntax InWhichMethod(SyntaxNode sn)
    {
      return sn.FirstAncestorOrSelf<MethodDeclarationSyntax>();
    }

    static TypeDeclarationSyntax InWhichTypeDeclaration(SyntaxNode sn)
    {
      return sn.FirstAncestorOrSelf<TypeDeclarationSyntax>();
    }

    static T ToExpr<T>(this string code) where T : ExpressionSyntax
    {
      return SyntaxFactory.ParseExpression(code) as T;
    }

    static T ToSExpr<T>(this string code) where T : StatementSyntax
    {
      return SyntaxFactory.ParseStatement(code) as T;
    }
  }
}
