Imports System.Runtime.InteropServices
Imports System.Text

Public Class PLUGIN_SOHUM3U8Descrambler

    'Key Version 1 (Cache):   SOHU@HoT^~123
    'Key Version 2 (Mobile):  S#^%o~.(><)@16$&
    'Key Version 2 (OTT):     q@V~*?)x.=opBnu4

    ReadOnly DecryptionKey As String() = {"SOHU@HoT^~123", "q@V~*?)x.=opBnu4", "S#^%o~.(><)@16$&"}

    Public Function DecryptM3U8(Input As String) As String
        Try
            For _loc_1 = 0 To DecryptionKey.Length - 1
                Dim _loc_2 As String = SOHU_Decrypt(Input, _loc_1)
                If _loc_2.StartsWith("#EXT") Then Return _loc_2
            Next
            Return Input
        Catch ex As Exception
            Return Input
        End Try
    End Function

    Public Function SOHU_Decrypt(Input As String, KeyID As Integer) As String
        Try
            Dim _loc_2 As Integer = Int(Input.Substring(5, 2))
            Dim _loc_3 As String = DecryptionKey(KeyID) & Encoding.UTF8.GetString(Convert.FromBase64String(Input.Substring(7, _loc_2)))
            Dim _loc_4 As String = Input.Substring(7 + _loc_2, Input.Length - (7 + _loc_2)).Replace("-", "+").Replace("_", "/").Replace(".", "=")
            Dim _loc_5 As String = Encoding.UTF8.GetString(XXTEA_Decrypt(Convert.FromBase64String(_loc_4), Encoding.UTF8.GetBytes(_loc_3)))
            Return _loc_5.Substring(0, _loc_5.Length - 4)
        Catch ex As Exception
            Return Input
        End Try
    End Function

    Public Function XXTEA_Decrypt(Data As Byte(), Key As Byte()) As Byte()
        Dim _loc_1 As UInteger() = ToUInt32Array(Data)
        Dim _loc_2 As UInteger() = ToUInt32Array(Key)
        Dim _loc_3 As UInteger = _loc_1.Length
        Dim _loc_4 As UInteger
        Dim _loc_5 As UInteger = _loc_1(0)
        Dim _loc_6 As UInteger = 2654435769
        Dim _loc_7 As UInteger
        Dim _loc_8 As UInteger = 6 + Int(52 / _loc_3)
        Dim _loc_9 As UInteger = (Convert.ToInt64(_loc_8) * Convert.ToInt64(_loc_6)) Mod 4294967296
        Dim _loc_10 As UInteger
        Dim _loc_11 As Long = 4294967296
        While Not _loc_9 = 0
            _loc_7 = _loc_9 >> 2 And 3
            _loc_10 = _loc_3 - 1
            While _loc_10 >= 0
                If _loc_10 = 0 Then
                    _loc_4 = _loc_1(_loc_3 - 1)
                Else
                    _loc_4 = _loc_1(_loc_10 - 1)
                End If
                Dim _loc_21 As UInteger = (Convert.ToInt64(_loc_4 >> 5 Xor _loc_5 << 2) + Convert.ToInt64(_loc_5 >> 3 Xor _loc_4 << 4)) Mod _loc_11
                Dim _loc_22 As UInteger = (Convert.ToInt64(_loc_9 Xor _loc_5) + Convert.ToInt64(_loc_2(_loc_10 And 3 Xor _loc_7) Xor _loc_4)) Mod _loc_11
                Dim _loc_23 As UInteger = _loc_21 Xor _loc_22
                Dim _loc_24 As UInteger = (Convert.ToInt64(_loc_1(_loc_10)) - Convert.ToInt64(_loc_23) + _loc_11) Mod _loc_11
                _loc_1(_loc_10) = _loc_24
                _loc_5 = _loc_1(_loc_10)
                If _loc_10 = 0 Then
                    _loc_9 = (Convert.ToInt64(_loc_9) - Convert.ToInt64(_loc_6) + _loc_11) Mod _loc_11
                    Exit While
                Else
                    _loc_10 -= 1
                End If
            End While
        End While
        Return ToByteArray(_loc_1)
    End Function

    Public Function ToUInt32Array(Input As Byte()) As UInteger()
        Dim _loc_1 As Integer = Math.Floor(Input.Length / 4)
        Dim _loc_2 As UInteger() = New UInteger(_loc_1 - 1) {}
        For _loc_3 = 0 To _loc_1 - 1
            _loc_2(_loc_3) = BitConverter.ToUInt32(New Byte() {Input(_loc_3 * 4), Input(_loc_3 * 4 + 1), Input(_loc_3 * 4 + 2), Input(_loc_3 * 4 + 3)}, 0)
        Next
        Return _loc_2
    End Function

    Public Function ToByteArray(Input As UInteger()) As Byte()
        Dim _loc_1 As Integer = Input.Length * 4
        Dim _loc_2 As Byte() = New Byte(_loc_1 - 1) {}
        For _loc_3 = 0 To Input.Count - 1
            Dim _loc_4 As Byte() = BitConverter.GetBytes(Input(_loc_3))
            _loc_2(_loc_3 * 4) = _loc_4(0)
            _loc_2(_loc_3 * 4 + 1) = _loc_4(1)
            _loc_2(_loc_3 * 4 + 2) = _loc_4(2)
            _loc_2(_loc_3 * 4 + 3) = _loc_4(3)
        Next
        Return _loc_2
    End Function
End Class