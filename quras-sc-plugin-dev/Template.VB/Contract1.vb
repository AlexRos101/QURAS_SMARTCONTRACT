﻿Public Class $itemname$ : Inherits SmartContract
    Public Shared Function Main(ByVal operation As String, ByVal args() As Object) As Boolean
        Storage.Put(Storage.CurrentContext, "Hello", "World")
        Return True
    End Function
End Class
