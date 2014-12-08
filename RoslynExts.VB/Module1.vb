Option Strict On

Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory
Imports  RoslynExts

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
      <Extension>
      Public Function ArgumentType(arg As ArgumentSyntax, sm As SemanticModel, ct As CancellationToken) As ITypeSymbol
        Try
          Return sm.GetTypeInfo(CType(arg, SimpleArgumentSyntax).Expression, ct).Type
        Catch ex As Exception
        End Try
        Return Nothing
      End Function

      <Extension>
      Public Function GetArgumentTypes(args As ArgumentListSyntax, sm As SemanticModel, ct As CancellationToken) As IEnumerable(Of ITypeSymbol)
        If (args Is Nothing) OrElse (sm Is Nothing) Then Return Enumerable.Empty(Of ITypeSymbol)

        Return args.Arguments.Select(Function(arg ) arg.ArgumentType(sm, ct))
      End Function

      <Extension>
      Public Function GetArgumentTypesNames(args As ArgumentListSyntax, sm As SemanticModel, ct As CancellationToken) As IEnumerable(Of String)
        If (args Is Nothing) OrElse (sm Is Nothing) Then Return Enumerable.Empty(Of String)
        Return args.GetArgumentTypes(sm, ct).Select(Function(tsym) If(tsym Is Nothing, String.Empty, tsym.ToFullyQualifiedName))
      End Function

      <Extension>
      Public Iterator Function GetArgumentAsObjects(args As ArgumentListSyntax, sm As SemanticModel, ct As CancellationToken) As IEnumerable(Of Object)
        If (args Is Nothing) OrElse (sm Is Nothing) Then Exit Function
        Dim ArgTypes = args.GetArgumentTypes(sm, ct)
        For i = 0 To args.Arguments.Count - 1
          Dim ov As Object
          Dim Arg = CType(args.Arguments(i), SimpleArgumentSyntax)
          If TypeOf Arg.Expression Is IdentifierNameSyntax Then
            ov = IdentifierValue(DirectCast(Arg.Expression, IdentifierNameSyntax), sm, ct)
          Else
            Try
              ov = Convert.ChangeType(Arg.DescendantTokens.First.Value, ArgTypes(i).GetType)
            Catch ex As Exception
              ov = Nothing
            End Try
          End If
          Yield ov
        Next
      End Function

      <Extension>
      Function IsExternal(Of T As SyntaxNode)(sn As T, sm As SemanticModel, ct As CancellationToken) As Boolean
        If (sn Is Nothing) OrElse (sm Is Nothing) Then Return True
        Return sm.GetSymbolInfo(sn, ct).Symbol.IsExtern
      End Function



      <Extension>
      Public Function IdentifierValue(ThisIdentifier As IdentifierNameSyntax, sm As SemanticModel, ct As CancellationToken) As Object
        If (ThisIdentifier Is Nothing) OrElse (sm Is Nothing)  Then Return Nothing

        Dim FoundSymbol = sm.LookupSymbols(ThisIdentifier.Span.Start, name:=ThisIdentifier.Identifier.Text)(0)
        Dim VariableDeclarationSite = TryCast(FoundSymbol.DeclaringSyntaxReferences(0).GetSyntax.Parent, VariableDeclaratorSyntax)
        If VariableDeclarationSite Is Nothing Then Return Nothing
        If VariableDeclarationSite.Initializer Is Nothing Then Return Nothing
        If VariableDeclarationSite.Initializer.Value Is Nothing Then Return Nothing
        Dim f = VariableDeclarationSite.Initializer.Value.DescendantTokens.First
        Dim TheValueOfTheVariable = f.Value
        Return Convert.ChangeType(TheValueOfTheVariable, sm.GetTypeInfo(ThisIdentifier,ct).GetType )
      End Function

      <Extension>
      Public Function CalledOnType(n As MemberAccessExpressionSyntax, sm As SemanticModel, ct As CancellationToken) As INamedTypeSymbol
        If (n Is Nothing) OrElse (sm Is Nothing) Then Return Nothing
        Dim s = sm.GetSymbolInfo(n, ct).Symbol
        Return If(s Is Nothing, Nothing, s.ContainingType)
      End Function

      <Extension>
      Public Function ToFullyQualifiedName(s As ISymbol) As String
        If s Is Nothing Then Return String.Empty
        Return s.ToDisplayString(New SymbolDisplayFormat(typeQualificationStyle:=SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces))
      End Function


    End Module

  End Namespace

End Namespace