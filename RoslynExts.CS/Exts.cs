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
using System.Threading;
using RoslynExts;

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
     
      public static T0 AddEOL<T0>(this T0 node) where T0 : SyntaxNode
    {
      return node.WithTrailingTrivia(SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "\r\n"));
    }

    public static ITypeSymbol ArgumentType(this ArgumentSyntax arg, SemanticModel  sm, CancellationToken ct)
    {
      try
      {
        return sm.GetTypeInfo(arg.Expression, ct).Type;
      }
      catch(Exception e)
      {
        return null;
      }
    }

    public static IEnumerable<ITypeSymbol> GetArgumentTypes(this ArgumentListSyntax args, SemanticModel sm, CancellationToken ct)
    {
      if ((args == null) || (sm == null)) return Enumerable.Empty<ITypeSymbol>();
      return args.Arguments.Select(arg => arg.ArgumentType(sm, ct));
    }

    public static IEnumerable<String> GetArgumentTypeNames(this ArgumentListSyntax args, SemanticModel sm, CancellationToken ct)
    {
      if ((args == null) || (sm == null)) return Enumerable.Empty<String>();
      return args.GetArgumentTypes(sm, ct).Select(tsym => tsym == null ? String.Empty : tsym.ToFullyQualifiedName());
    }

    public static IEnumerable<Object> GetArgumentAsObjects(this ArgumentListSyntax args, SemanticModel sm, CancellationToken ct)
    {
      if ((args == null) || (sm == null)) yield break;
      var ArgTypes = args.GetArgumentTypes(sm, ct).ToArray();
      for(var i=0; i<args.Arguments.Count(); i++)
      {
        object ov=null;
        var arg = args.Arguments[0].Try<ArgumentSyntax>();//.As<ArgumentSyntax>();
        var iv = arg?.Expression.Try<IdentifierNameSyntax>();
        if( iv != null)
        {
          ov = iv.IdentifierValue(sm,ct);
        }
        else
        {
          try
          {

            ov = Convert.ChangeType(arg.DescendantTokens().First().Value, ArgTypes[i].GetType());
          }
          catch(Exception ex)
          {
            ov = null;
          }
        }
        yield return ov;
      }

    }

    public static bool IsExternal<T>(this T sn, SemanticModel sm, CancellationToken ct)
      where T : SyntaxNode
    {
      if ((sn == null) || (sm == null)) return true;
      return sm.GetSymbolInfo(sn, ct).Symbol.IsExtern;
    }

    public static object IdentifierValue(this IdentifierNameSyntax id, SemanticModel sm, CancellationToken ct)
    {
      if ((id == null) || (sm == null)) return null;
      var FoundSymbol = sm.LookupSymbols(id.Span.Start, name: id.Identifier.Text)[0];
      var varDeclSite = FoundSymbol.DeclaringSyntaxReferences[0].GetSyntax().Parent.Try<VariableDeclaratorSyntax>();
      if (varDeclSite == null) return null;
      if (varDeclSite.Initializer == null) return null;
      if (varDeclSite.Initializer.Value == null) return null;
      var f = varDeclSite.Initializer.Value.DescendantTokens().First();
      var value = f.Value;
      var t = value.GetType();
      return Convert.ChangeType(value, t);
    }

    public static INamedTypeSymbol CalledOnType(this MemberAccessExpressionSyntax n, SemanticModel sm, CancellationToken ct)
    {
      if ((n == null) || (sm == null)) return null;
      var s = sm.GetSymbolInfo(n, ct).Symbol;
      return (s == null) ? null : s.ContainingType; 
    }

    public static string ToFullyQualifiedName(this ISymbol s)
    {
      if (s == null) return String.Empty;
      return s.ToDisplayString(new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));
    }




  }
}
