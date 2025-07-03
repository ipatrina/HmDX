Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

Public Class PLUGIN_DeIQ
    Dim BlockDecryptionKey As Byte() = New Byte() {}
    Dim DecryptionKey As Byte() = New Byte() {}
    Dim FileRead As MemoryStream
    Dim FileReader As BinaryReader
    Dim FileWrite As MemoryStream
    Dim FileWriter As BinaryWriter
    Dim PID_1 As Integer = -1
    Dim PID_1_BUFFER As Byte() = New Byte() {}
    Dim PID_1_OFFSET As New List(Of String)
    Dim PID_1_PES_PAYLOAD As Byte() = New Byte() {}
    Dim PID_2 As Integer = -1
    Dim PID_2_BUFFER As Byte() = New Byte() {}
    Dim PID_2_OFFSET As New List(Of String)
    Dim PID_2_PES_PAYLOAD As Byte() = New Byte() {}
    ReadOnly TS_PACKET_SIZE As Integer = 188

    Public Function DecryptTS(InputStream As Byte(), Key As String) As Byte()
        Try
            DecryptionKey = HexToBytes(Key)
            Dim InputFileSize As Long = InputStream.Length
            FileRead = New MemoryStream(InputStream)
            FileReader = New IO.BinaryReader(FileRead)
            FileWrite = New MemoryStream()
            FileWriter = New IO.BinaryWriter(FileWrite)
            Do
                Dim _loc_1 As Integer = TS_PACKET_SIZE - 4
                Dim PACKET_HEADER As Byte() = FileReader.ReadBytes(4)
                If PACKET_HEADER(0) = &H47 Then
                    Dim PACKET_HEADER_BIT As New BitArray(PACKET_HEADER)
                    Dim PACKET_PID As Integer = (PACKET_HEADER(1) And &H1F) << 8 Or PACKET_HEADER(2)

                    If PACKET_PID = 17 Then
                        Dim SDT_PAYLOAD As Byte() = FileReader.ReadBytes(_loc_1)
                        FileReader.BaseStream.Position -= _loc_1
                        Dim _loc_11 As New StringBuilder()
                        For Each _loc_12 As Byte In SDT_PAYLOAD
                            If _loc_12 >= 32 AndAlso _loc_12 <= 126 Then _loc_11.Append(ChrW(_loc_12))
                        Next
                        Dim _loc_13 As MatchCollection = Regex.Matches(_loc_11.ToString(), "\|v([0-9a-fA-F]{32})\|")
                        If _loc_13.Count > 0 Then BlockDecryptionKey = HexToBytes(_loc_13(0).Groups(1).Value)
                    End If

                    Dim _loc_3 As Boolean = False
                    Dim _loc_4 As Boolean = False
                    If PACKET_HEADER_BIT(14) And (PACKET_PID >= 32) And (PACKET_PID <= 256) Then
                        _loc_3 = True
                        _loc_4 = True
                        If PID_1 < 0 Then
                            PID_1 = PACKET_PID
                        ElseIf PID_2 < 0 And (Not PACKET_PID = PID_1) Then
                            PID_2 = PACKET_PID
                        End If
                    Else
                        If PACKET_PID = PID_1 Or PACKET_PID = PID_2 Then
                            _loc_3 = True
                        End If
                    End If

                    If _loc_3 Then
                        Dim PACKET_DATA As Byte() = New Byte(TS_PACKET_SIZE - 1) {}
                        Array.Copy(PACKET_HEADER, 0, PACKET_DATA, 0, PACKET_HEADER.Length)

                        If PACKET_HEADER_BIT(29) Then
                            Dim _loc_5 As Byte() = FileReader.ReadBytes(1)
                            PACKET_DATA(TS_PACKET_SIZE - _loc_1) = _loc_5(0)
                            _loc_1 -= 1

                            Dim _loc_6 As Integer = Int(_loc_5(0))
                            If _loc_6 > 0 Then
                                _loc_5 = FileReader.ReadBytes(_loc_6)
                                Array.Copy(_loc_5, 0, PACKET_DATA, TS_PACKET_SIZE - _loc_1, _loc_5.Length)
                                _loc_1 -= _loc_6
                            End If
                        End If

                        Dim Offset As Integer = TS_PACKET_SIZE - _loc_1
                        Dim PES_DATA As Byte() = FileReader.ReadBytes(_loc_1)
                        Array.Copy(PES_DATA, 0, PACKET_DATA, Offset, PES_DATA.Length)
                        If _loc_4 Then
                            Dim _loc_7 As Integer = 9 + Int(PES_DATA(8))
                            Dim _loc_8 As Byte() = New Byte(PES_DATA.Length - _loc_7 - 1) {}
                            Array.Copy(PES_DATA, _loc_7, _loc_8, 0, PES_DATA.Length - _loc_7)
                            Offset += _loc_7
                            PES_DATA = _loc_8
                        End If

                        If BlockDecryptionKey.Length <> 16 Then Return InputStream

                        If PACKET_PID = PID_1 Then
                            If _loc_4 Then FlushData("PID_1")
                            PID_1_OFFSET.Add(Offset)
                            PID_1_BUFFER = AppendTo(PACKET_DATA, PID_1_BUFFER)
                            PID_1_PES_PAYLOAD = AppendTo(PES_DATA, PID_1_PES_PAYLOAD)
                        ElseIf PACKET_PID = PID_2 Then
                            If _loc_4 Then FlushData("PID_2")
                            PID_2_OFFSET.Add(Offset)
                            PID_2_BUFFER = AppendTo(PACKET_DATA, PID_2_BUFFER)
                            PID_2_PES_PAYLOAD = AppendTo(PES_DATA, PID_2_PES_PAYLOAD)
                        End If
                    Else
                        FileWriter.Write(PACKET_HEADER)
                        FileWriter.Write(FileReader.ReadBytes(_loc_1))
                    End If
                End If
            Loop Until FileReader.BaseStream.Position >= InputFileSize - 1
            FlushData("PID_1")
            FlushData("PID_2")
            Return FileWrite.ToArray()
        Catch ex As Exception
            Return InputStream
        End Try
    End Function

    Private Sub FlushData(PID As String)
        Try
            If PID = "PID_1" And PID_1_BUFFER.Length > 0 Then
                Dim _loc_1 As Byte() = DecryptES(PID_1_PES_PAYLOAD, DecryptionKey, BlockDecryptionKey)
                Dim _loc_2 As Integer = 0
                Dim _loc_3 As Integer = 0
                For Each _loc_4 In PID_1_OFFSET
                    Dim _loc_5 As Byte() = New Byte(TS_PACKET_SIZE - 1) {}
                    Array.Copy(PID_1_BUFFER, _loc_2, _loc_5, 0, Convert.ToInt64(_loc_4))
                    _loc_2 += TS_PACKET_SIZE
                    Array.Copy(_loc_1, _loc_3, _loc_5, Convert.ToInt64(_loc_4), TS_PACKET_SIZE - Convert.ToInt64(_loc_4))
                    _loc_3 += TS_PACKET_SIZE - Int(_loc_4)
                    FileWriter.Write(_loc_5)
                Next
                PID_1_BUFFER = New Byte() {}
                PID_1_OFFSET.Clear()
                PID_1_PES_PAYLOAD = New Byte() {}
            ElseIf PID = "PID_2" And PID_2_BUFFER.Length > 0 Then
                Dim _loc_1 As Byte() = DecryptES(PID_2_PES_PAYLOAD, DecryptionKey, BlockDecryptionKey)
                Dim _loc_2 As Integer = 0
                Dim _loc_3 As Integer = 0
                For Each _loc_4 In PID_2_OFFSET
                    Dim _loc_5 As Byte() = New Byte(TS_PACKET_SIZE - 1) {}
                    Array.Copy(PID_2_BUFFER, _loc_2, _loc_5, 0, Convert.ToInt64(_loc_4))
                    _loc_2 += TS_PACKET_SIZE
                    Array.Copy(_loc_1, _loc_3, _loc_5, Convert.ToInt64(_loc_4), TS_PACKET_SIZE - Convert.ToInt64(_loc_4))
                    _loc_3 += TS_PACKET_SIZE - Int(_loc_4)
                    FileWriter.Write(_loc_5)
                Next
                PID_2_BUFFER = New Byte() {}
                PID_2_OFFSET.Clear()
                PID_2_PES_PAYLOAD = New Byte() {}
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Function DecryptES(InputStream As Byte(), Key As Byte(), BlockKey As Byte()) As Byte()
        Dim _loc_1 As System.Security.Cryptography.Aes = System.Security.Cryptography.Aes.Create("AES")
        _loc_1.BlockSize = 128
        _loc_1.Key = Key
        _loc_1.Mode = CipherMode.ECB
        _loc_1.Padding = PaddingMode.None

        Dim _loc_2 As Byte() = New Byte(InputStream.Length - 1) {}
        Array.Copy(InputStream, _loc_2, InputStream.Length)

        Dim _loc_3 As Integer = -1
        Dim _loc_4 As Integer = 0
        While _loc_4 <= _loc_2.Length - 4
            If _loc_2(_loc_4) = 0 AndAlso _loc_2(_loc_4 + 1) = 0 AndAlso _loc_2(_loc_4 + 2) = 0 AndAlso _loc_2(_loc_4 + 3) = 1 Then
                If _loc_3 <> -1 Then
                    Dim _loc_5 As Integer = _loc_4 - _loc_3
                    Dim _loc_6(_loc_5 - 1) As Byte
                    Array.Copy(_loc_2, _loc_3, _loc_6, 0, _loc_5)
                    Dim _loc_7 As Byte() = DecryptNAL(_loc_6, BlockKey, _loc_1)
                    Array.Copy(_loc_7, 0, _loc_2, _loc_3, _loc_5)
                End If
                _loc_3 = _loc_4
                _loc_4 += 4
            ElseIf _loc_2(_loc_4) = 0 AndAlso _loc_2(_loc_4 + 1) = 0 AndAlso _loc_2(_loc_4 + 2) = 1 Then
                If _loc_3 <> -1 Then
                    Dim _loc_5 As Integer = _loc_4 - _loc_3
                    Dim _loc_6(_loc_5 - 1) As Byte
                    Array.Copy(_loc_2, _loc_3, _loc_6, 0, _loc_5)
                    Dim _loc_7 As Byte() = DecryptNAL(_loc_6, BlockKey, _loc_1)
                    Array.Copy(_loc_7, 0, _loc_2, _loc_3, _loc_5)
                End If
                _loc_3 = _loc_4
                _loc_4 += 3
            Else
                _loc_4 += 1
            End If
        End While
        If _loc_3 <> -1 AndAlso _loc_3 < _loc_2.Length Then
            Dim _loc_5 As Integer = _loc_2.Length - _loc_3
            Dim _loc_6(_loc_5 - 1) As Byte
            Array.Copy(_loc_2, _loc_3, _loc_6, 0, _loc_5)
            Dim _loc_7 As Byte() = DecryptNAL(_loc_6, BlockKey, _loc_1)
            Array.Copy(_loc_7, 0, _loc_2, _loc_3, _loc_5)
        End If

        Return _loc_2
    End Function

    Public Function DecryptNAL(InputStream As Byte(), BlockKey As Byte(), AesHandle As Aes) As Byte()
        Dim _loc_11 As Byte() = New Byte(InputStream.Length - 1) {}
        Array.Copy(InputStream, _loc_11, InputStream.Length)

        Dim _loc_9(_loc_11.Length - 1) As Byte
        Dim _loc_22 As Integer = 0
        Dim _loc_23 As Integer = 0
        While _loc_22 < _loc_11.Length
            If _loc_22 + 3 < _loc_11.Length AndAlso _loc_11(_loc_22) = &H0 AndAlso _loc_11(_loc_22 + 1) = &H0 AndAlso _loc_11(_loc_22 + 2) = &H3 AndAlso (_loc_11(_loc_22 + 3) = &H0 OrElse _loc_11(_loc_22 + 3) = &H1 OrElse _loc_11(_loc_22 + 3) = &H2 OrElse _loc_11(_loc_22 + 3) = &H3) Then
                _loc_9(_loc_23) = _loc_11(_loc_22) : _loc_23 += 1
                _loc_9(_loc_23) = _loc_11(_loc_22 + 1) : _loc_23 += 1
                _loc_9(_loc_23) = _loc_11(_loc_22 + 3) : _loc_23 += 1
                _loc_22 += 4
            Else
                _loc_9(_loc_23) = _loc_11(_loc_22)
                _loc_22 += 1
                _loc_23 += 1
            End If
        End While

        Dim _loc_12 As Byte() = New Byte(_loc_23 - 5 - 2 - 1) {}
        Array.Copy(_loc_9, 5, _loc_12, 0, _loc_12.Length)

        Dim _loc_13 As Integer = Math.Ceiling(_loc_12.Length / 16)
        Dim _loc_14 As Integer = 0
        For _loc_15 As Integer = 1 To _loc_13
            Dim _loc_16 As Byte() = BitConverter.GetBytes(_loc_15)
            If BitConverter.IsLittleEndian Then Array.Reverse(_loc_16)
            Dim _loc_17 As Byte() = BlockKey.Take(12).Concat(_loc_16).ToArray()
            If _loc_15 Mod 10 = 1 Or _loc_15 = _loc_13 Then _loc_17 = AesHandle.CreateEncryptor().TransformFinalBlock(_loc_17, 0, _loc_17.Length)
            For _loc_18 As Integer = 0 To _loc_17.Length - 1
                _loc_12(_loc_14) = _loc_12(_loc_14) Xor _loc_17(_loc_18)
                _loc_14 += 1
                If _loc_14 = _loc_12.Length Then Exit For
            Next
        Next

        Array.Copy(_loc_12, 0, _loc_11, 5, _loc_12.Length)
        Array.Copy(_loc_9, 5 + _loc_12.Length, _loc_11, 5 + _loc_12.Length, 2)
        Dim _loc_10 As Integer = _loc_11.Length - 5 - _loc_12.Length - 2
        If _loc_10 > 0 Then
            For _loc_19 As Integer = 1 To _loc_10
                _loc_11(_loc_11.Length - _loc_10) = 0
            Next
        End If

        Return _loc_11
    End Function

    Public Shared Function HexToBytes(param1 As String) As Byte()
        Return Enumerable.Range(0, param1.Length).Where(Function(x) x Mod 2 = 0).[Select](Function(x) Convert.ToByte(param1.Substring(x, 2), 16)).ToArray()
    End Function

    Public Shared Function BytesToHex(param1 As Byte()) As String
        Return BitConverter.ToString(param1).Replace("-", "").ToUpper
    End Function

    Public Shared Function BytesToBin(param1() As Byte) As String
        Dim _loc_1 As New StringBuilder
        For Each _loc_2 In param1
            _loc_1.Append(Convert.ToString(_loc_2, 2).PadLeft(8, "0"))
        Next
        Return _loc_1.ToString
    End Function

    Public Shared Function BinToBytes(param1 As String) As Byte()
        Dim _loc_1 As Integer = param1.Length / 8
        Dim _loc_2 As Byte() = New Byte(_loc_1 - 1) {}
        For _loc_3 As Integer = 0 To _loc_1 - 1
            _loc_2(_loc_3) = Convert.ToByte(param1.Substring(8 * _loc_3, 8), 2)
        Next
        Return _loc_2
    End Function

    Private Function AppendTo(param1 As Byte(), param2 As Byte()) As Byte()
        Dim _loc_1 As Byte() = New Byte(param1.Length + param2.Length - 1) {}
        If param2.Length > 0 Then
            Array.Copy(param2, _loc_1, param2.Length)
        End If
        Array.Copy(param1, 0, _loc_1, param2.Length, param1.Length)
        Return _loc_1
    End Function
End Class
