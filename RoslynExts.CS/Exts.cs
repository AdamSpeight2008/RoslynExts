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
   public  static MethodDeclarationSyntax InWhichMethod(SyntaxNode sn)
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
  }
}
