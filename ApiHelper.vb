Imports System.Net
Imports System.Net.Http


' A simple class to hold the result of an API call so we don't have to guess if it failed
Public Class ApiResponse
    Public Property Success As Boolean
    Public Property StatusCode As HttpStatusCode
    Public Property Data As String
    Public Property ErrorMessage As String
End Class

Public Module ApiHelper

    ''' <summary>
    ''' Sends an HTTP GET request to the specified endpoint.
    ''' </summary>
    ''' 
    Dim key As String = "Aning-The-Super-Gendut-390233"
    Private ReadOnly _httpClient As New HttpClient()

    Public Function GetRequest(ByVal endpoint As String) As ApiResponse
        Dim result As New ApiResponse()
        Dim targetUrl As String = SystemMod.API_BASE_URL & endpoint

        ' Use a Shared HttpClient in your class to avoid socket exhaustion
        ' If not already defined, add: Private Shared ReadOnly _httpClient As New HttpClient()

        Try
            Dim request As New HttpRequestMessage(HttpMethod.Get, targetUrl)
            request.Headers.Add("X-API-KEY", key)

            ' Send the request
            Dim response As HttpResponseMessage = _httpClient.Send(request)

            ' Read result
            result.Data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            result.StatusCode = response.StatusCode
            result.Success = response.IsSuccessStatusCode

        Catch ex As HttpRequestException
            ' Handles network-level issues
            result.Success = False
            result.ErrorMessage = "Network error: " & ex.Message
        Catch ex As Exception
            ' Handles general issues
            result.Success = False
            result.ErrorMessage = ex.Message
        End Try

        Return result
    End Function

    ''' <summary>
    ''' Sends an HTTP POST request with a JSON payload.
    ''' </summary>
    Public Function PostJsonRequest(ByVal endpoint As String, ByVal jsonPayload As String) As ApiResponse
        Dim result As New ApiResponse()
        Dim targetUrl As String = SystemMod.API_BASE_URL & endpoint

        Try
            ' Prepare the request
            Dim content As New StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json")

            ' Create a request message to add headers
            Using request As New HttpRequestMessage(HttpMethod.Post, targetUrl)
                request.Content = content
                request.Headers.Add("X-API-KEY", key)

                ' Send request
                Dim response As HttpResponseMessage = _httpClient.Send(request)

                ' Populate result
                result.StatusCode = response.StatusCode
                result.Success = response.IsSuccessStatusCode

                ' Read the response body
                result.Data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            End Using

        Catch ex As HttpRequestException
            ' Handles network connection issues
            result.Success = False
            result.ErrorMessage = "Network Error: " & ex.Message
        Catch ex As Exception
            ' Handles general issues
            result.Success = False
            result.ErrorMessage = ex.Message
        End Try

        Return result
    End Function

    Function GetSetting(ByVal strKey As String) As String
        ' Call the API using the ApiHelper module
        Dim response = GetRequest("/api/settings/" & strKey)

        If response.Success Then
            Return response.Data
        Else
            ' Maintains your original error-handling behavior (returns empty string if not found or error)
            Return ""
        End If
    End Function


End Module