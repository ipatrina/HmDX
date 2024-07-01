Public Class PLUGIN_ThousandBillion
    Public Function ThousandBillionDeAESKey(param1 As Byte()) As Byte()
        Dim _loc_2 As Integer
        Dim _loc_3 As Byte() = New Byte(param1.Length - 1) {}
        Dim _loc_4 As Integer = 0
        Do Until _loc_4 >= param1.Length
            _loc_2 = _loc_4 Mod 4
            If _loc_2 = 0 Then
                _loc_3(_loc_4) = param1(_loc_4) Xor 17
            ElseIf _loc_2 = 1 Then
                _loc_3(_loc_4) = param1(_loc_4) Xor 34
            ElseIf _loc_2 = 2 Then
                _loc_3(_loc_4) = param1(_loc_4) Xor 51
            ElseIf _loc_2 = 3 Then
                _loc_3(_loc_4) = param1(_loc_4) Xor 68
            End If
            _loc_4 += 1
        Loop
        Return _loc_3
    End Function
End Class
