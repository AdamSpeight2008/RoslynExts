Imports System.Runtime.CompilerServices

Namespace Global.RoslynExts
  <HideModuleName>
Public Module Exts
    <Extension>
    Public Function AnyIsNull(Of T As Class)(Values() As T) As Boolean
      Return Values.Any(Function(value) value Is Nothing)
    End Function


  End Module
End Namespace

