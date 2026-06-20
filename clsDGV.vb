Imports System.Data
Imports System.Data.Odbc

Public Class clsDGV
    Public connDB As Odbc.OdbcConnection
    '    Public HoldTransaction As Odbc.OdbcTransaction

    Dim p_BoundForm As Form
    Dim WithEvents p_DataGridView1 As DataGridView
    Public SQL_String As String
    Public SQL_Filter As String
    Public SQL_Sort As String
    Public SQL_Limit As String

    Public Table_Name As String
    Public Primary_Key As String
    Public dbTable As Data.DataTable
    Public RecPerGrid As Long

    Dim blnDBConnected As Boolean
    Dim edtDGVTemplate As ENUM_DGV_Template

    Dim p_AddRowIfLastRow As Boolean
    Dim p_MinRowsRemaining As Integer

    Public Enum ENUM_DB_Trans
        Begin = 1
        Commit = 2
        RollBack = 3
    End Enum

    Public Enum ENUM_DGV_Template
        ReadOnlyList = 1
        EditableList = 2
    End Enum

    Sub BindDGV(ByRef dgv As DataGridView)
        p_DataGridView1 = dgv
    End Sub

    Public Sub InjectDataTable(ByVal incomingTable As Data.DataTable)
        Me.dbTable = incomingTable

        If Not IsNothing(p_DataGridView1) Then
            p_DataGridView1.DataSource = Me.dbTable
            Call AlignColumnHeader()
        End If
    End Sub

    Friend Sub Init(ByRef BoundForm As Form, ByVal Table_Name As String, ByVal pSQL_String As String, ByVal pSQL_Filter As String,
                    ByVal pSQL_Sort As String, ByVal pSQL_Limit As String, ByVal Primary_Key As String,
                    ByVal conDriver As String, ByVal conServer As String, ByVal conPort As String, ByVal conDB As String, ByVal conUser As String, ByVal conPassword As String)

        p_AddRowIfLastRow = False

        p_BoundForm = BoundForm

        Me.SQL_String = pSQL_String
        Me.SQL_Filter = pSQL_Filter
        Me.SQL_Sort = pSQL_Sort
        Me.SQL_Limit = pSQL_Limit

        Me.Table_Name = Table_Name
        Me.Primary_Key = Primary_Key

        dbTable = New Data.DataTable

        blnDBConnected = False
        If p_ConnectDatabase(conDriver, conServer, conPort, conDB, conUser, conPassword) = False Then
            MsgBox("Database tidak terkoneksi", vbExclamation, "Error DB")
        End If

    End Sub

    Sub RefreshList(ByVal GridLoc As Integer, ByVal intRecPerGrid As Integer)


        Dim lngAwal As Long

        '        p_DataGridView1.Rows.Clear()

        If intRecPerGrid = 0 Then
            '=== intRecPerGrid isi 0 artinya semua record
            Me.SQL_Limit = " "
        Else
            lngAwal = GridLoc * intRecPerGrid
            Me.SQL_Limit = "LIMIT " & lngAwal & ", " & intRecPerGrid
        End If

        Call Me.TableRequery()

        If Not (IsNothing(p_DataGridView1)) Then
            p_DataGridView1.DataSource = dbTable
            Call AlignColumnHeader()
        End If

    End Sub

    Sub TableRequery()
        Dim strSQL As String

        strSQL = Me.SQL_String & " " & Me.SQL_Filter & " " & Me.SQL_Sort & " " & Me.SQL_Limit

        dbTable = Nothing
        dbTable = Me.CreateDataTable(strSQL)
    End Sub

    Sub AlignColumnHeader()
        Dim n As Integer

        With p_DataGridView1
            For n = 0 To .ColumnCount - 1
                .Columns(n).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
            Next
        End With
    End Sub

    Function p_ConnectDatabase(ByVal strDriver As String, ByVal strServer As String, ByVal strPort As String, ByVal strDB As String,
                                ByVal strUser As String, ByVal strPassword As String) As Boolean
        ' ----- Connect to the database. Return True on success.
        Dim connectionString As String

        ' ----- Initialize.
        '        HoldTransaction = Nothing

        ' ----- Build the connection string.
        ' !!! WARNING: Hardcoded for now.
        connectionString = "driver={" & strDriver & "};" _
                         & "server=" & strServer & ";" _
                         & "port=" & strPort & ";" _
                         & "database=" & strDB & ";" _
                         & "uid=" & strUser & ";" _
                         & "pwd=" & strPassword

        ' ----- Attempt to open the database.
        Try
            connDB = New Odbc.OdbcConnection(connectionString)
            connDB.Open()
        Catch ex As Exception
            GeneralError("ConnectDatabase", ex)
            blnDBConnected = False
            Return False
        End Try

        blnDBConnected = True
        ' ----- Success.
        Return True
    End Function

    Function CreateDataTable(ByVal sqlText As String, Optional ByVal strTableName As String = "") As Data.DataTable
        ' ----- Given a SQL statement, return a data table.
        Dim dbCommand As Odbc.OdbcCommand
        Dim dbAdapter As Odbc.OdbcDataAdapter
        Dim dbTable As Data.DataTable

        ' ----- Try to run the statement. Note that no error trapping is done
        '       here. It is up to the calling routine to set up error checking.
        Try
            dbCommand = New Odbc.OdbcCommand(sqlText, connDB)
            '            If Not (HoldTransaction Is Nothing) Then dbCommand.Transaction = HoldTransaction
            dbAdapter = New Odbc.OdbcDataAdapter(dbCommand)
            dbTable = New Data.DataTable
            dbAdapter.Fill(dbTable)
            dbAdapter = Nothing
            dbCommand = Nothing

            If strTableName <> "" Then
                dbTable.TableName = strTableName
            End If

            Return dbTable
        Catch ex As Exception
            GeneralError("CreateDataTAble ", ex)
            blnDBConnected = False
            Return Nothing
        End Try

    End Function

    Public Property DGV_Template() As ENUM_DGV_Template
        Get
            Return edtDGVTemplate
        End Get
        Set(ByVal value As ENUM_DGV_Template)
            edtDGVTemplate = value
            Call FormatDGVWithTemplate()
        End Set
    End Property

    Public Property Min_Row_Remaining() As Integer
        Get
            Return p_MinRowsRemaining
        End Get
        Set(ByVal value As Integer)
            p_MinRowsRemaining = value
        End Set
    End Property

    Public Property ColumnWidth(ByVal strColumn As String) As Integer
        Get
            Return p_DataGridView1.Columns(strColumn).Width
        End Get
        Set(ByVal value As Integer)
            p_DataGridView1.Columns(strColumn).Width = value
            If value = 0 Then
                p_DataGridView1.Columns(strColumn).Visible = False
            End If
        End Set
    End Property

    Public Property AddRowIfLastRow() As Boolean
        Get
            Return p_AddRowIfLastRow
        End Get
        Set(ByVal value As Boolean)
            p_AddRowIfLastRow = value
        End Set
    End Property

    Public Property ColumnAlignment(ByVal strColumn As String) As DataGridViewContentAlignment
        Get
            Return p_DataGridView1.Columns(strColumn).DefaultCellStyle.Alignment
        End Get
        Set(ByVal value As DataGridViewContentAlignment)
            p_DataGridView1.Columns(strColumn).DefaultCellStyle.Alignment = value
        End Set
    End Property

    Public Property ColumnFormat(ByVal strColumn As String) As String
        Get
            Return p_DataGridView1.Columns(strColumn).DefaultCellStyle.Format
        End Get
        Set(ByVal value As String)
            '            p_DataGridView1.Columns(strColumn).DefaultCellStyle.Format = value
            p_DataGridView1.Columns.Item(strColumn).DefaultCellStyle.Format = value
            '            p_DataGridView1.Columns.Item(strColumn).ValueType = GetType(Double)
        End Set
    End Property

    Private Sub FormatDGVWithTemplate()
        With p_DataGridView1
            If edtDGVTemplate = ENUM_DGV_Template.ReadOnlyList Then
                .ReadOnly = True
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect
                .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray
                .RowHeadersVisible = False
                .BackgroundColor = Color.LightGray
            ElseIf edtDGVTemplate = ENUM_DGV_Template.EditableList Then
                .ReadOnly = False
                .SelectionMode = DataGridViewSelectionMode.CellSelect
                .RowHeadersVisible = False
                .BackgroundColor = Color.LightGray

            End If
        End With
    End Sub

    Public Sub MasukkanDataCBO(ByVal cbo As ComboBox, ByVal obj As FieldComboBox)
        Dim dtTable As New Data.DataTable

        If obj.TipeDataCBO = JenisDataCBO.jdcSQL Then
            Try
                dtTable = CreateDataTable(obj.CboSQL)
            Catch ex As Exception
                GeneralError("Masukkan data cbo", ex)
            End Try

            cbo.DataSource = dtTable
            cbo.ValueMember = obj.KolomCBOSave
            cbo.DisplayMember = obj.KolomCBODisplay
        End If
    End Sub

    Sub GeneralError(ByVal routineName As String,
        ByVal theError As System.Exception)
        ' ----- Report an error to the user.
        On Error Resume Next

        MsgBox("The following error occurred at location '" & routineName &
            "':" & vbCrLf & vbCrLf & theError.Message,
            MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation, ProgramTitle)
        My.Application.Log.WriteException(theError)
        On Error GoTo 0
    End Sub

    Public Function CreateReader(ByVal sqlText As String) As Odbc.OdbcDataReader
        ' ----- Given a SQL statement, return a data reader.
        Dim dbCommand As Odbc.OdbcCommand
        Dim dbScan As Odbc.OdbcDataReader

        ' ----- Try to run the statement. Note that no error trapping is done
        '       here. It is up to the calling routine to set up error checking.
        dbCommand = New Odbc.OdbcCommand(sqlText, connDB)
        '        If Not (HoldTransaction Is Nothing) Then dbCommand.Transaction = HoldTransaction
        dbScan = dbCommand.ExecuteReader()
        dbCommand = Nothing
        Return dbScan
    End Function

    Public Sub Dispose()
        Me.Finalize()
    End Sub

    Protected Overrides Sub Finalize()
        On Error Resume Next
        If Not IsNothing(connDB) Then
            connDB.Close()
        End If
        On Error GoTo 0
        MyBase.Finalize()
    End Sub

    Public Function ExecuteSQL(ByVal sqlText As String, Optional ByVal blnReturnLastInsertID As Boolean = False) As Long
        ' ----- Given a SQL statement, run it.
        Dim DBCommand As Odbc.OdbcCommand
        Dim lngRetVal As Long = 0

        ' ----- Try to run the statement. Note that no error trapping is done
        '       here. It is up to the calling routine to set up error checking.
        DBCommand = New Odbc.OdbcCommand(sqlText, connDB)
        '        If Not (HoldTransaction Is Nothing) Then DBCommand.Transaction = HoldTransaction


        DBCommand.ExecuteNonQuery()

        If blnReturnLastInsertID Then
            lngRetVal = CLng(ExecuteSQLReturn("SELECT LAST_INSERT_ID()"))
        End If

        Return lngRetVal
    End Function

    Public Function ExecuteSQLReturn(ByVal sqlText As String) As Object
        ' ----- Given a SQL statement, run it, and return a related result.
        Dim DBCommand As Odbc.OdbcCommand

        ' ----- Try to run the statement. Note that no error trapping is done
        '       here. It is up to the calling routine to set up error checking.
        DBCommand = New Odbc.OdbcCommand(sqlText, connDB)
        '        If Not (HoldTransaction Is Nothing) Then DBCommand.Transaction = HoldTransaction
        Return DBCommand.ExecuteScalar()
    End Function

    Public Function NumChildAkun(ByVal strNoAkun As String) As Integer
        Dim intRes As Integer
        Dim strSQL As String

        strSQL = "SELECT COUNT(*) FROM tbaccount WHERE child_of = '" & strNoAkun & "' "
        intRes = CInt(ExecuteSQLReturn(strSQL))


        Return intRes
    End Function

    Public Function NumJurnalTransaction(ByVal strNoAkun As String) As Integer
        Dim intRes As Integer
        Dim strSQL As String

        '=== Cek apakah sudah pernah ada transaksi ===
        strSQL = "SELECT COUNT(*) FROM tbjurnal_lines WHERE nomor_account = '" & strNoAkun & "' "
        intRes = CInt(ExecuteSQLReturn(strSQL))

        Return intRes
    End Function

    Public Function LastChildAccount(ByVal strNoAccount As String) As String
        Dim strSQL As String
        Dim strRes As String

        strSQL = "SELECT MAX(nomor_account) FROM tbaccount WHERE nomor_account LIKE '" & strNoAccount & "-___'"

        '        If IsNothing(CStr(ExecuteSQLReturn(strSQL))) Then
        ' strRes = ""
        'Else
        strRes = CStr(ExecuteSQLReturn(strSQL).ToString)
        'strRes = CStr(ExecuteSQLReturn(strSQL))
        'End If

        Return strRes
    End Function

    Private Sub p_DataGridView1_CellBeginEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles p_DataGridView1.CellBeginEdit
        Dim strFormat As String
        Dim strNilai As String

        strFormat = p_DataGridView1.Columns(e.ColumnIndex).DefaultCellStyle.Format.ToString
        strNilai = p_DataGridView1.CurrentCell.Value

        If strFormat.IndexOf("N") > -1 And Not IsNothing(strNilai) And strNilai <> "" Then
            p_DataGridView1.CurrentCell.Value = Decimal.Parse(strNilai, Globalization.NumberStyles.Currency).ToString
        End If

    End Sub

    Private Sub p_DataGridView1_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles p_DataGridView1.CellEndEdit
        Dim strFormat As String
        Dim strNilai As String

        strFormat = p_DataGridView1.Columns(e.ColumnIndex).DefaultCellStyle.Format.ToString

        If strFormat.IndexOf("N") > -1 Then
            If Not p_DataGridView1.CurrentCell.Value Is Nothing Then
                strNilai = NilaiDesimalByRegional(p_DataGridView1.CurrentCell.Value.ToString)
                If strNilai <> "" Then
                    Try
                        p_DataGridView1.CurrentCell.Value = CDbl(strNilai).ToString(strFormat)
                    Catch ex As Exception
                        p_DataGridView1.CurrentCell.Value = ""
                    End Try

                End If
            End If
        End If
    End Sub

    Private Sub p_DataGridView1_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles p_DataGridView1.EditingControlShowing
        With p_DataGridView1
            RemoveHandler CType(e.Control, TextBox).KeyPress, AddressOf TextBoxNumeric_keyPress
            RemoveHandler CType(e.Control, TextBox).KeyPress, AddressOf TextBoxAlphaNumeric_keyPress

            If p_DataGridView1.Columns(.CurrentCell.ColumnIndex).DefaultCellStyle.Format.ToString.IndexOf("N") > -1 Then
                AddHandler CType(e.Control, TextBox).KeyPress, AddressOf TextBoxNumeric_keyPress
            Else
                AddHandler CType(e.Control, TextBox).KeyPress, AddressOf TextBoxAlphaNumeric_keyPress
            End If

        End With

    End Sub

    Private Sub TextBoxNumeric_keyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs)

        '        If Char.IsDigit(CChar(CStr(e.KeyChar))) = False Then e.Handled = True   ===> Buat INTEGER saja
        If Not (Char.IsDigit(CChar(CStr(e.KeyChar))) Or e.KeyChar = "." Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub

    Private Sub TextBoxAlphaNumeric_keyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs)

        '        If Char.IsDigit(CChar(CStr(e.KeyChar))) = False Then e.Handled = True   ===> Buat INTEGER saja
        If Not (Char.IsLetterOrDigit(CChar(CStr(e.KeyChar))) Or e.KeyChar = " " Or e.KeyChar = "." Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub

    Private Sub p_DataGridView1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles p_DataGridView1.KeyDown

        If DGV_Template = ENUM_DGV_Template.EditableList Then
            With p_DataGridView1


                If e.KeyCode = Keys.Down And .CurrentCell.RowIndex = .Rows.Count - 1 Then
                    .Rows.Add()
                ElseIf e.KeyCode = Keys.Up And .CurrentCell.RowIndex = .Rows.Count - 1 Then
                    'Kalau tidak ada isi dan baris terakhir, maka hapus saja
                    If IsRowEmpty(.CurrentRow.Index) And .CurrentCell.RowIndex >= Me.Min_Row_Remaining Then
                        .Rows.RemoveAt(.CurrentCell.RowIndex)
                        e.Handled = True
                    End If
                ElseIf e.KeyCode = Keys.Delete Then
                    .BeginEdit(True)
                End If
            End With
        End If
    End Sub

    Function IsRowEmpty(ByVal intPos As Integer) As Boolean
        Dim n As Integer

        IsRowEmpty = True

        With p_DataGridView1
            For n = 0 To .Columns.Count - 1
                If .Rows(intPos).Cells(n).Value = "" Or IsNothing(.Rows(intPos).Cells(n).Value) Then
                Else
                    IsRowEmpty = False
                    Exit For
                End If
            Next
        End With

    End Function

    Function CekConstraint(ByVal strReferenceValue As String, ByVal strConstraintTable As String, ByVal strConstraintColumn As String)
        Dim strSQL As String
        Dim dt As DataTable

        CekConstraint = True
        If strConstraintColumn = "" Then Exit Function

        strSQL = "SELECT " & strConstraintColumn & " " &
                   "FROM " & strConstraintTable & " " &
                  "WHERE " & strConstraintColumn & " = '" & strReferenceValue & "' "

        dt = CreateDataTable(strSQL)

        If dt.Rows.Count = 0 Then
            CekConstraint = True
        Else
            CekConstraint = False
        End If
    End Function

End Class
