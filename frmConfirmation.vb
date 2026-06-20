Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Text.Json

<Obfuscation(Exclude:=True, ApplyToMembers:=True)>
Public Class LoginResponse
    Public Property success As Boolean
    Public Property userName As String
    Public Property userLevel As String
    Public Property message As String
End Class

Public Class frmConfirmation

    Dim intChance As Integer = 3

    Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
        txtID.Text = UCase(txtID.Text)

        ' 1. Package the login data into a Dictionary
        Dim loginData As New Dictionary(Of String, String) From {
            {"userId", txtID.Text},
            {"password", SystemMod.GetHash(txtPassword.Text)}
        }

        Try
            ' 2. Convert the Dictionary to a JSON string
            Dim jsonPayload As String = JsonSerializer.Serialize(loginData)

            ' 3. Send it using YOUR existing PostJsonRequest function
            Dim response As ApiResponse = ApiHelper.PostJsonRequest("/api/auth/login", jsonPayload)

            ' 4. Check if the network request itself succeeded (No 404 or 500 errors)
            If response.Success Then

                ' 5. Deserialize the JSON string returned by the server
                Dim options As New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
                Dim responseData As LoginResponse = JsonSerializer.Deserialize(Of LoginResponse)(response.Data, options)

                ' 6. Check if the backend API said the password was correct
                If responseData IsNot Nothing AndAlso responseData.success Then
                    ' Successfully logged in!
                    SystemMod.PRE_USER_ID = txtID.Text
                    SystemMod.PRE_USER_NAME = responseData.userName

                    SystemMod.InsertCloudLog(txtID.Text, "LOGIN", "SUCCESS")

                    MsgBox("Password diterima", vbInformation, "Password")

                    txtPassword.Text = ""
                    txtID.Text = ""

                    Me.Close()
                Else
                    ' Password was wrong
                    Dim errorMsg As String = If(responseData IsNot Nothing AndAlso Not String.IsNullOrEmpty(responseData.message), responseData.message, "User tidak ditemukan atau Password salah")
                    MsgBox(errorMsg, vbExclamation, "Password")
                    HandleFailedLogin()
                End If

            Else
                ' Network failed (Server offline, bad API key, etc.)
                MsgBox("Server Error: " & response.ErrorMessage, vbCritical, "API Error")
            End If

        Catch ex As Exception
            MsgBox("System Error: " & ex.Message, vbCritical, "Error")
        End Try

    End Sub

    ' Modern replacement for the "GoTo Akhir" logic
    Private Sub HandleFailedLogin()
        intChance -= 1
        If intChance <= 0 Then
            Application.Exit() ' Modern .NET 10 way to gracefully close the application
        End If
    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Application.Exit() ' Modern .NET 10 way
    End Sub

    Private Sub frmConfirmation_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress
        If e.KeyChar = ChrW(Keys.Return) Then
            SendKeys.Send("{TAB}")
        End If
    End Sub

    Private Sub frmConfirmation_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        intChance = 3
    End Sub

    Private Sub txtPassword_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtPassword.KeyPress
        If Asc(e.KeyChar) = Keys.Return Then
            Call OK_Click(sender, e)
        End If
    End Sub

End Class