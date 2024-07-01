Public Class PLUGIN_LEM3U8Descrambler
    Public Function Decrypt(Input As Byte()) As Byte()
        Dim _loc_2() As Byte = CapByteArray(Input, 5, Input.Length - 1)
        Dim _loc_3 = _loc_2.Length
        Dim _loc_4() As Byte = New Byte(_loc_3 * 2.5) {}
        Dim _loc_5 As Integer = 0

        While _loc_5 < _loc_3
            _loc_4(2 * _loc_5) = _loc_2(_loc_5) >> 4
            _loc_4(2 * _loc_5 + 1) = _loc_2(_loc_5) And 15
            _loc_5 += 1
        End While

        _loc_4 = ClrByteArray(_loc_4)

        Dim _loc_6() As Byte = ClrByteArray(ComByteArray(CapByteArray(_loc_4, _loc_4.Length - 11, _loc_4.Length - 1), CapByteArray(_loc_4, 0, _loc_4.Length - 12)))

        Dim _loc_7() As Byte = New Byte(_loc_3 * 2.5) {}
        _loc_5 = 0

        While _loc_5 < _loc_3
            _loc_7(_loc_5) = (_loc_6(2 * _loc_5) << 4) + _loc_6(2 * _loc_5 + 1)
            _loc_5 += 1
        End While

        _loc_7 = ClrByteArray(_loc_7)
        Return _loc_7
    End Function

    Private Function ClrByteArray(ByVal MyArray As System.Array) As Byte()
        Dim baO As New List(Of Byte)
        Dim i As Integer = MyArray.Length - 1
        Do Until i = 0
            If MyArray(i) <> 0 Then
                Exit Do
            End If
            i -= 1
        Loop
        For j = 0 To i
            baO.Add(MyArray(j))
        Next
        Return baO.ToArray
    End Function

    Private Function ComByteArray(ByVal Array1 As System.Array, ByVal Array2 As System.Array) As Byte()
        Dim baO As New List(Of Byte)
        For Each bt In Array1
            baO.Add(bt)
        Next
        For Each bt In Array2
            baO.Add(bt)
        Next
        Return baO.ToArray
    End Function

    Private Function CapByteArray(ByVal MyArray As System.Array, ByVal StartByte As Integer, ByVal EndByte As Integer) As Byte()
        Dim baO As New List(Of Byte)
        For i = StartByte To EndByte
            baO.Add(MyArray(i))
        Next
        Return baO.ToArray
    End Function
End Class
