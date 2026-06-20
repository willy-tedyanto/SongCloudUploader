Public Module modGeneral

    Public Enum ShowType
        Dialog
        Modeless
    End Enum

    Public Enum AllowedCharSet As Integer
        acsAlpha = 1
        acsNumeric = 2
        acsAlphaNumeric = 3
        acsAlphaNumericSymbol = 4
    End Enum

    Public Enum JenisDataCBO
        jdcSQL = 1
        jdcTypeList = 2
    End Enum

    Public Structure AdditionalBotton
        Dim Caption As String
        Dim FormToOpen As String
        Dim Width As Integer
        Dim Color As System.Drawing.Color
        Dim FormTitle As String
    End Structure

    Public Structure FieldComboBox
        Dim TipeDataCBO As JenisDataCBO
        Dim CboSQL As String
        Dim CboTypeList As String()
        Dim JumlahKolomCBO As Integer
        Dim KolomCBODisplay As String
        Dim KolomCBOSave As String
        Dim KolomCBOWidth As Integer()
        Dim DrawMode As DrawMode
    End Structure

    Public Structure DaftarField
        Dim Nomor As Integer
        Dim NamaField As String
        Dim Caption As String
        Dim JenisControl As String
        Dim AllowedChar As AllowedCharSet
        Dim KolomCBOSpek As FieldComboBox
        Dim Required As Boolean
        Dim AutoNumField As Boolean
        Dim MD5 As Boolean
    End Structure

    Function NilaiDesimalByRegional(ByVal strNilai As String, Optional ByVal blnForceDot As Boolean = False) As String
        If blnForceDot Then
            strNilai = Replace(strNilai, ",", ".")
        Else
            Dim strDecimalSeparator As String = My.Application.Culture.NumberFormat.NumberDecimalSeparator

            If strDecimalSeparator = "," Then
                strNilai = Replace(strNilai, ".", ",")
            Else
                strNilai = Replace(strNilai, ",", ".")
            End If
        End If
        Return strNilai

    End Function

    Function DGVWidth(ByVal dgv As DataGridView) As Long
        Dim n As Integer

        DGVWidth = 0

        For n = 0 To dgv.Columns.Count - 1
            DGVWidth = DGVWidth + IIf(dgv.Columns(n).Visible, dgv.Columns(n).Width, 0)
        Next

    End Function

End Module
