Imports System.Runtime.CompilerServices
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory

Namespace Global.RoslynExts
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
        Return ParseExpression(code).As(Of T)
      End Function

      <Extension>
      Public Function ToSExpr(Of T As StatementSyntax)(code As String) As T
        Return ParseExecutableStatement(code).As(Of T)
      End Function

      <Extension>
      Public Function KindIsAnyOf(Of T0 As SyntaxNode)(value As T0, ParamArray values() As SyntaxKind) As Boolean
        Return values.Any(Function(v) value.IsKind(v))
      End Function

      <Extension>
      Public Function WithSameTriviaAs(Of T0 As SyntaxNode, T1 As SyntaxNode)(target As T0, source As T1) As T0
        If target Is Nothing Then Throw New ArgumentNullException("target") 'NameOf():
        If source Is Nothing Then Throw New ArgumentNullException("source") 'NameOf():
        Return target.WithLeadingTrivia(source.GetLeadingTrivia).
                      WithTrailingTrivia(source.GetTrailingTrivia)
      End Function

      <Extension>
      Public Function [Try](Of T1 As Class)(expr As Object) As T1
        Return TryCast(expr, T1)
      End Function

      <Extension>
      Public Function [As](Of T1 As Class)(expr As Object) As T1
        Return DirectCast(expr, T1)
      End Function

      <Extension>
      Public Function AddEOL(Of T0 As SyntaxNode)(node As T0) As T0
        Return node.WithTrailingTrivia(SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, Environment.NewLine))
      End Function

    End Module

  End Namespace

End Namespace