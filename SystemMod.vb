Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.Json
Imports Newtonsoft.Json

Module SystemMod

    Public Const ProgramTitle As String = "Rapsody Cloud Song Update"
    Public Const PRE_DB_Driver As String = "MySQL ODBC 5.3 Unicode Driver"
    Public PRE_DB_Port As String = "3306"
    Public PRE_Enc_DB_Credential As String = "VdLbD2/9O6kRA37hf218294Z=="
    Public PRE_DB_Name As String
    Public PRE_DB_UID As String
    Public PRE_DB_Pass As String

    Public DB_SERVER_LOCAL As String = "127.0.0.1"
    Public DB_PORT_LOCAL As String = "3306"
    Public DB_UID_LOCAL As String
    Public DB_PWD_LOCAL As String
    Public DB_DBNAME_LOCAL As String

    Public Enc_AWS_Access As String = "GPTSkdBp8wuRk7HANshgqKzCkgIp6hqf"
    Public Enc_AWS_Secret As String = "1qCnaYZ+sc2kuNyripI0Ng=="
    Public AWS_Access As String = ""
    Public AWS_Secret As String = ""

    Public PRE_USER_ID As String
    Public PRE_USER_NAME As String

    'Public API_BASE_URL As String = "http://159.65.1.198"'
    'Public API_BASE_URL As String = "http://127.0.0.1:5000"
    Public API_BASE_URL As String = "https://savegoldfish.com"

    Function GetHash(ByVal theInput As String) As String

        Using hasher As MD5 = MD5.Create()    ' create hash object

            ' Convert to byte array and get hash
            Dim dbytes As Byte() =
                 hasher.ComputeHash(Encoding.UTF8.GetBytes(theInput))

            ' sb to create string from bytes
            Dim sBuilder As New StringBuilder()

            ' convert byte data to hex string
            For n As Integer = 0 To dbytes.Length - 1
                sBuilder.Append(dbytes(n).ToString("X2"))
            Next n

            Return sBuilder.ToString()
        End Using

    End Function

    Public Function HaveInternetConnection() As Boolean
        Try
            Return My.Computer.Network.Ping("www.google.com")
        Catch ex As Exception
            Return False
        End Try
    End Function

    Function GetSetting(ByVal cn As ADODB.Connection, ByVal strKey As String)
        Try
            Return SystemMod.RecordExist(cn, "settings", "setting_key", strKey, "setting_value")("setting_value")
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
            Return ""
        End Try
    End Function

    Function RecordExist(ByVal cn As ADODB.Connection, ByVal strTableName As String, ByVal strKey As String, ByVal strValue As String, Optional ByVal strField As String = "", Optional ByVal RecordOrder As String = "ASC", Optional ByVal OrderField As String = "") As Collection
        Dim rs As New ADODB.Recordset
        Dim strSQL As String
        Dim n As Integer
        Dim arrFieldName() As String

        '=== NB : Kalau fieldnya ada fungsi yang ada koma seperti LEFT(no_shift,7) maka ditulis LEFT(no_shift&&7)
        RecordExist = New Collection

        If strField = "" Then strField = strKey
        If OrderField = "" Then OrderField = Replace(strKey, "&&", ",")

        strSQL = "SELECT " & Replace(strField, "&&", ",") & " " &
            "FROM " & strTableName & " " &
            "WHERE " & Replace(strKey, "&&", ",") & " = '" & strValue & "' " &
            "ORDER BY " & OrderField & " " & RecordOrder

        Call OpenADORS(rs, strSQL, cn)

        arrFieldName = Split(strField, ",")

        If rs.RecordCount > 0 Then
            For n = 0 To UBound(arrFieldName)
                If String.IsNullOrEmpty(rs.Fields(n).Value) Then
                    RecordExist.Add(vbNull, Trim(arrFieldName(n)))
                Else
                    RecordExist.Add(Trim(CStr(rs.Fields(n).Value)), Trim(arrFieldName(n)))
                End If
            Next n
        End If

    End Function

    Sub OpenADORS(ByRef rs As ADODB.Recordset, ByVal strSQL As String, ByVal cn As ADODB.Connection)

        On Error Resume Next
        rs.Close()
        On Error GoTo 0

        rs.CursorLocation = ADODB.CursorLocationEnum.adUseClient
        rs.Open(strSQL, cn)

    End Sub

    Function TambahkanKarakterEscape(ByVal str As String) As String
        Return Replace(str, "'", "\\")
    End Function

    Function BeriKarakterEscape(ByVal str As String) As String
        '=== Khusus ketika encrypt ===
        BeriKarakterEscape = Replace(str, "'", "\")
    End Function

    Function HilangkanKarakterEscape(ByVal str As String) As String
        Return Replace(str, "\", "'")
    End Function

    Function FilterString(ByVal strSource As String) As String
        Dim strTemp As String
        Dim n As Integer
        Dim strPick As String
        Const strAllowed As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"

        strTemp = ""
        For n = 1 To Len(strSource)
            strPick = Mid(strSource, n, 1)
            If InStr(strAllowed, strPick) Then
                strTemp = strTemp & strPick
            End If
        Next n

        FilterString = strTemp

    End Function

    Function szEncryptDecrypt(ByVal szData As String, ByVal Key_Text As String) As String
        Const KEY_OFFSET As Long = 38

        Dim bytKey() As Byte
        Dim bytData() As Byte
        Dim lNum As Long
        Dim szKey As String = ""

        For lNum = 1 To ((Len(szData) \ Len(Key_Text)) + 1)
            szKey = szKey & Key_Text
        Next lNum

        bytKey = System.Text.Encoding.Unicode.GetBytes(Microsoft.VisualBasic.Left(szKey, Len(szData)))
        bytData = System.Text.Encoding.Unicode.GetBytes(szData)

        For lNum = LBound(bytData) To UBound(bytData)
            If lNum Mod 2 Then
                bytData(lNum) = bytData(lNum) Xor (bytKey(lNum) _
                    + KEY_OFFSET)
            Else
                bytData(lNum) = bytData(lNum) Xor (bytKey(lNum) _
                    - KEY_OFFSET)
            End If
        Next lNum

        szEncryptDecrypt = System.Text.Encoding.Unicode.GetString(bytData)
    End Function

    Function ExportDataKeMySQL(ByVal strKaraoke2UNKLocation As String, ByVal connMySQL As ADODB.Connection) As Boolean

        Dim rs As New ADODB.Recordset
        Dim strAppPath As String

        Dim myStream As ADODB.Stream
        Dim sSQL As String

        Dim cnKar2 As New ADODB.Connection


        cnKar2.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" &
                                    "Data Source=" & strKaraoke2UNKLocation & ";" &
                                    "Persist Security Info=False;" &
                                    "Jet OLEDB:Database Password=m4m4m1alezatos"

        Try
            cnKar2.Open()
        Catch ex As Exception
            MsgBox("Error : " & ex.Message, MsgBoxStyle.Exclamation, "Error Open Karaoke-2.unk")
            Return False
        End Try


        strAppPath = Application.StartupPath

        myStream = CreateObject("ADODB.Stream")

        With myStream
            .Type = ADODB.StreamTypeEnum.adTypeText
            .Charset = "UTF-8"
            .Open()

            sSQL = "SELECT '~~~' & Movie.kode AS kode, " &
                        "kdcategory, " &
                        "artist, " &
                        "title, " &
                        "inisial_lagu, " &
                        "volume, " &
                        "kdcountry, " &
                        "top40, " &
                        "ChannelKaraoke, " &
                        "LEFT(Movie.drive, LEN(Movie.drive) - 1) AS drive, " &
                        "path, " &
                        "FrameWidth, " &
                        "FrameHeight, " &
                        "FORMAT(tglmasuk, 'yyyy-MM-dd') " &
                        "FROM Movie"

            '=== Setelah export, harus update kolom tglmasuk ke NOW() di mysql ===

            rs = cnKar2.Execute(sSQL)

            Do Until rs.EOF
                myStream.WriteText(rs.GetString(, 2000, "|", vbCrLf, ""))
            Loop

            If File.Exists(strAppPath & "\temp.txt") Then
                File.Delete(strAppPath & "\temp.txt")
            End If

            .SaveToFile(strAppPath & "\temp.txt")
            .Close()
        End With


        connMySQL.Execute("TRUNCATE tblmovie")

        connMySQL.Execute("LOAD DATA LOCAL " &
            "INFILE '" & Replace(strAppPath, "\", "\\") & "\\temp.txt' " &
            "INTO TABLE tblmovie " &
            "CHARACTER SET UTF8 " &
            "FIELDS TERMINATED BY '|' " &
            "ESCAPED BY '' " &
            "LINES TERMINATED BY '\r\n'" &
            "STARTING BY '~~~' ")

        'conKar.Execute "TRUNCATE tblcountry"
        '
        'With deKaraoke.rscmCountry
        '    For n = 1 To .RecordCount
        '        .AbsolutePosition = n
        '        conKar.Execute "INSERT INTO tblcountry " & _
        '            "(kdcountry, country) VALUES (" & _
        '            "'" & .Fields("kdcountry") & "', " & _
        '            "'" & .Fields("country") & "') "
        '    Next n
        'End With

        Return True
    End Function

    Public Function GetUpdatedSongsCollection() As HashSet(Of String)
        ' Initialize a case-insensitive HashSet
        Dim hashSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        Dim response As ApiResponse = ApiHelper.GetRequest("/api/songs/uploaded")

        If response.Success Then
            Dim songsList As List(Of String) = JsonConvert.DeserializeObject(Of List(Of String))(response.Data)

            For Each item As String In songsList
                hashSet.Add(item) ' Adds to hash index seamlessly
            Next
        Else
            Console.WriteLine("Failed to fetch updated songs: " & response.ErrorMessage)
        End If

        Return hashSet
    End Function

    Sub SetRegistryKey(ByVal SubkeyLocation As String, ByVal key As String, ByVal value As String)
        Const SubkeyName As String = "Rapsody_Karaoke"

        If IsNothing(My.Computer.Registry.GetValue(SubkeyLocation, key, Nothing)) Then
            My.Computer.Registry.CurrentUser.CreateSubKey(SubkeyName)
        End If

        My.Computer.Registry.SetValue(SubkeyLocation, key, value)

    End Sub

    Public Sub InsertCloudLog(ByVal user_id As String, ByVal activity As String, ByVal remark As String)
        ' 1. Package the log data
        Dim logData As New Dictionary(Of String, String) From {
        {"userId", user_id},
        {"activity", activity},
        {"remark", remark}
    }

        ' 2. Send to API via your existing PostJsonRequest
        Dim jsonPayload As String = JsonConvert.SerializeObject(logData)
        Dim response As ApiResponse = ApiHelper.PostJsonRequest("/api/logs/user", jsonPayload)

        ' 3. Optional: check response.Success to see if the log hit the database
        If Not response.Success Then
            Console.WriteLine("Failed to log activity: " & response.ErrorMessage)
        End If
    End Sub

End Module
