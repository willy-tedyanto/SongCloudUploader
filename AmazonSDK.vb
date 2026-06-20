Imports System.ComponentModel
Imports Amazon.S3
Imports Amazon.S3.Model
Imports Amazon.S3.Transfer

Public Class AmazonSDK

    Public Function DeleteFile(ByVal targetBucketName As String, ByVal keyName As String) As String
        Dim config = New AmazonS3Config With {.ServiceURL = "https://sgp1.digitaloceanspaces.com"}

        Dim client As AmazonS3Client = New AmazonS3Client(SystemMod.AWS_Access, SystemMod.AWS_Secret, config)

        Try

            Dim deleteRequest As DeleteObjectRequest = New DeleteObjectRequest With
                                                 {.BucketName = targetBucketName,
                                                 .Key = keyName}

            ' THE FIX: Bridging the modern Async requirement to your synchronous app
            Dim response As DeleteObjectResponse = client.DeleteObjectAsync(deleteRequest).GetAwaiter().GetResult()

        Catch ex As AmazonS3Exception

            ' THE FIX: Swapped IsDBNull for the standard modern .NET string check
            If (Not String.IsNullOrEmpty(ex.ErrorCode) AndAlso (ex.ErrorCode.Equals("InvalidAccessKeyId") Or ex.ErrorCode.Equals("InvalidSecurity"))) Then
                Return "Error : Check the provided AWS Credentials."
            Else
                Return "Error : " & ex.Message.ToString()
            End If

        End Try

        Return "Success"

    End Function

    Public Function UploadFile(ByVal client As AmazonS3Client,
                               ByVal bucket As String,
                               ByVal s3Path As String,
                               ByVal localFilePath As String,
                               ByVal worker As BackgroundWorker,
                               ByRef errorOccured As Boolean,
                               ByRef errorMessage As String) As String
        Try
            ' 1. Check if local file exists
            If Not System.IO.File.Exists(localFilePath) Then
                Return "File Not Found: " & localFilePath
            End If

            ' 2. Initialize the modern TransferUtility engine
            Dim fileTransferUtility As New TransferUtility(client)

            ' 3. Switch to TransferUtilityUploadRequest
            Dim request As New TransferUtilityUploadRequest() With {
                .BucketName = bucket,
                .Key = s3Path,
                .FilePath = localFilePath
            }

            ' 4. Attach the modern progress event listener
            AddHandler request.UploadProgressEvent,
                Sub(sender As Object, args As UploadProgressArgs)
                    If worker IsNot Nothing AndAlso worker.WorkerReportsProgress Then
                        ' The SDK natively computes the precise completion percentage!
                        Dim percent As Integer = CInt(args.PercentDone)

                        ' Force a cap at 99% until the remote server finishes writing the file
                        If percent > 99 Then percent = 99

                        worker.ReportProgress(percent)
                    End If
                End Sub

            ' 5. Run the upload through TransferUtility synchronously 
            ' (Safe because this runs entirely inside your BackgroundWorker thread)
            fileTransferUtility.Upload(request)

            errorOccured = False
            Return "Success"

        Catch ex As AmazonS3Exception
            errorOccured = True
            errorMessage = ex.Message
            Return ex.Message
        Catch ex As Exception
            errorOccured = True
            errorMessage = ex.Message
            Return ex.Message
        End Try
    End Function



End Class