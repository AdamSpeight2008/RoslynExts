Imports System.Runtime.CompilerServices
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

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

      <Extension>
      Public Function KindIsAnyOf(value As SyntaxToken, ParamArray values() As SyntaxKind) As Boolean
        Return values.Any(Function(v) value.IsKind(v))
      End Function

      <Extension>
      Public Function WithSameTriviaAs(Of T0 As SyntaxNode,T1 As SyntaxNode)(target As T0, source As T1) As T0
        If target Is Nothing Then Throw New ArgumentNullException("target")
        If source Is Nothing Then Throw New ArgumentNullException("source")
        Return target.WithLeadingTrivia(source.GetLeadingTrivia).
                      WithTrailingTrivia(source.GetTrailingTrivia)
      End Function
      
    End Module

  End Namespace

End Namespace