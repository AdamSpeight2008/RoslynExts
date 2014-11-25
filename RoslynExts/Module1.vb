Imports System.Runtime.CompilerServices

Namespace Global.DotNetAnalyzers.RoslynExts
  Namespace VB
    <HideModuleName()>
    Public Module Exts

      <Extension>
      Public Function InWhichMethod(sn As SyntaxNode) As MethodBaseSyntax
        Return sn.FirstAncestorOrSelf(Of MethodBaseSyntax)
      End Function

      <Extension>
      Public Function InWhichTypeDeclaration(sn As SyntaxNode) As TypeBlockSyntax
        Return sn.FirstAncestorOrSelf(Of TypeBlockSyntax)
      End Function

      <Extension>
      Public Function ToExpr(Of T As ExpressionSyntax)(code As String) As T
        Return DirectCast(SyntaxFactory.ParseExpression(code), T)
      End Function

      <Extension>
      Public Function ToSExpr(Of T As StatementSyntax)(code As String) As T
        Return DirectCast(SyntaxFactory.ParseExecutableStatement(code), T)
      End Function

    End Module

  End Namespace

End Namespace