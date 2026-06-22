Imports System.ComponentModel
Imports System.Data
Imports System.IO
Imports Amazon.S3
Imports Newtonsoft.Json

Public Class frmUploader

    Dim intCurrentDownloadingItem As Integer = 1

    Dim DO_KEY As String
    Dim DO_Secret As String

    Const WINDOWS_REGISTRY_SUBKEY_LOCATION As String = "HKEY_CURRENT_USER\Rapsody_Karaoke"
    Const WINDOWS_REGISTRY_KEY_IP_SOURCE As String = "SourceIpAddress"
    Const WINDOWS_REGISTRY_KEY_IP_CLOUD As String = "CloudIpAddress"
    Const WINDOWS_REGISTRY_KEY_KARAOKE_2_LOCATION As String = "Karaoke2LocationUploader"
    Const FORM_CAPTION As String = "Song Cloud Update"

    Dim connSourceDB As New ADODB.Connection
    Dim blnIsKaraoke2Connected As Boolean = False
    Dim blnIsCloudDBConnected As Boolean = False
    Dim movieDataGridViewClass As New clsDGV

    Const COLUMN_CHECKBOX As Integer = 0
    Const COLUMN_KODE As Integer = 1
    Const COLUMN_KDCATEGORY As Integer = 2
    Const COLUMN_TITLE As Integer = 3
    Const COLUMN_TITLE_UNICODE As Integer = 4
    Const COLUMN_ARTIST As Integer = 5
    Const COLUMN_ARTIST_UNICODE As Integer = 6
    Const COLUMN_INISIAL_LAGU As Integer = 7
    Const COLUMN_VOLUME As Integer = 8
    Const COLUMN_KDCOUNTRY As Integer = 9
    Const COLUMN_COUNTRY_NAME As Integer = 10
    Const COLUMN_TOP40 As Integer = 11
    Const COLUMN_CHANNEL_KARAOKE As Integer = 12
    Const COLUMN_DRIVE As Integer = 13
    Const COLUMN_PATH As Integer = 14
    Const COLUMN_FRAME_WIDTH As Integer = 15
    Const COLUMN_FRAME_HEIGHT As Integer = 16
    Const COLUMN_CREATED_DATE_TIME As Integer = 17

    Dim COLOR_UPLOADED_ITEM As System.Drawing.Color = Color.OrangeRed

    Dim strDriveDownloadLocation As String
    Dim connLocalMySQL As New ADODB.Connection
    Dim selectedSongsCollection As New Collection
    'Dim updatedSongsCollection As New Collection
    Dim updatedSongsCollection As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
    Dim uploadPosition As Integer

    Dim bucketName = "rapsodykaraokesystemsgp"
    'Dim bucketName = "unishopwawa"  '--- For testing purposes ---
    Dim cloudServerUrl = "https://sgp1.digitaloceanspaces.com"
    Dim s3Client As AmazonS3Client
    Const appName As String = "Song Cloud Uploader"

    Private Sub uploadButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles uploadButton.Click

        If Not blnIsKaraoke2Connected Then
            MsgBox("Anda harus tentukan lokasi program karaoke terlebih dahulu", MsgBoxStyle.Information, appName)
            Exit Sub
        Else
            selectedSongsCollection.Clear()

            Dim n As Integer
            Dim songItem As Song

            For n = 0 To movieDataGridView.RowCount - 1
                If movieDataGridView.Rows(n).Cells(COLUMN_CHECKBOX).Value Then
                    songItem = New Song

                    songItem.Kode = movieDataGridView.Rows(n).Cells(COLUMN_KODE).Value
                    songItem.KdCategory = movieDataGridView.Rows(n).Cells(COLUMN_KDCATEGORY).Value
                    songItem.Title = movieDataGridView.Rows(n).Cells(COLUMN_TITLE).Value

                    If IsNothing(movieDataGridView.Rows(n).Cells(COLUMN_TITLE_UNICODE).Value) Or IsDBNull(movieDataGridView.Rows(n).Cells(COLUMN_TITLE_UNICODE).Value) Then
                        songItem.TitleUnicode = ""
                    Else
                        songItem.TitleUnicode = movieDataGridView.Rows(n).Cells(COLUMN_TITLE_UNICODE).Value
                    End If

                    songItem.Artist = movieDataGridView.Rows(n).Cells(COLUMN_ARTIST).Value

                    If IsNothing(movieDataGridView.Rows(n).Cells(COLUMN_ARTIST_UNICODE).Value) Or IsDBNull(movieDataGridView.Rows(n).Cells(COLUMN_ARTIST_UNICODE).Value) Then
                        songItem.ArtistUnicode = ""
                    Else
                        songItem.ArtistUnicode = movieDataGridView.Rows(n).Cells(COLUMN_ARTIST_UNICODE).Value
                    End If

                    songItem.InisialLagu = movieDataGridView.Rows(n).Cells(COLUMN_INISIAL_LAGU).Value
                    songItem.Volume = movieDataGridView.Rows(n).Cells(COLUMN_VOLUME).Value
                    songItem.KdCountry = movieDataGridView.Rows(n).Cells(COLUMN_KDCOUNTRY).Value
                    songItem.CountryName = movieDataGridView.Rows(n).Cells(COLUMN_COUNTRY_NAME).Value
                    songItem.Top40 = movieDataGridView.Rows(n).Cells(COLUMN_TOP40).Value
                    songItem.ChannelKaraoke = movieDataGridView.Rows(n).Cells(COLUMN_CHANNEL_KARAOKE).Value
                    songItem.Drive = movieDataGridView.Rows(n).Cells(COLUMN_DRIVE).Value
                    songItem.Path = movieDataGridView.Rows(n).Cells(COLUMN_PATH).Value
                    songItem.FrameWidth = movieDataGridView.Rows(n).Cells(COLUMN_FRAME_WIDTH).Value
                    songItem.FrameHeight = movieDataGridView.Rows(n).Cells(COLUMN_FRAME_HEIGHT).Value
                    songItem.CreatedDateTime = movieDataGridView.Rows(n).Cells(COLUMN_CREATED_DATE_TIME).Value

                    selectedSongsCollection.Add(songItem)
                End If
            Next
        End If

        If selectedSongsCollection.Count > 0 Then
            uploadPosition = 1

            Dim config = New AmazonS3Config With {
                .ServiceURL = "https://sgp1.digitaloceanspaces.com",
                .AuthenticationRegion = "sgp1",
                .EndpointDiscoveryEnabled = False,
                .ForcePathStyle = False
            }
            s3Client = New AmazonS3Client(SystemMod.AWS_Access, SystemMod.AWS_Secret, config)

            SetLogMessage("Begin Upload to " & Me.bucketName)

            SystemMod.InsertCloudLog(SystemMod.PRE_USER_ID, "UPLOAD", "BEGIN")

            bw_updater.RunWorkerAsync()
            SetButtonStatus(False)
        Else
            MsgBox("Anda belum memilih lagu untuk diupload", vbInformation, appName)
        End If
    End Sub

    Private Sub frmUploader_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        lblVersion.Text = "Version : " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString

        If Not SystemMod.HaveInternetConnection Then
            MsgBox("Tidak terhubung ke Internet. Proses dibatalkan", vbExclamation, appName)
            Me.Close()
        End If

        Dim drv As DriveInfo
        For Each drv In My.Computer.FileSystem.Drives
            If drv.IsReady Then
                songLocationDrive.Items.Add(String.Concat(Strings.Left(drv.Name, 2), " [", drv.VolumeLabel, "]"))
            End If
        Next

        System.Net.WebRequest.DefaultWebProxy = Nothing

        ' --- ANTI-FIREWALL NETWORK SETTINGS ---
        ' 1. Ignore slow proxy auto-detection
        System.Net.WebRequest.DefaultWebProxy = Nothing

        ' 2. KILL THE "100-CONTINUE" TIMEOUT (This fixes the 7-minute delay)
        System.Net.ServicePointManager.Expect100Continue = False

        ' 3. Force modern TLS 1.2/1.3 (Enterprise firewalls block older SSL)
        System.Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls12 Or Net.SecurityProtocolType.Tls13
        ' --------------------------------------

        Dim wrapper As New Desi(SystemMod.PRE_Enc_DB_Credential)

        PRE_DB_Name = wrapper.Decr("G91bXzXQSMgVosmRVfgmzjJA3/OxVCxrXP695oKWRhb3fm4OwSpHKZm5CyL8UOFE")
        PRE_DB_UID = wrapper.Decr("zTRXDkIy/5jp4Ms3npHdSw==")
        PRE_DB_Pass = wrapper.Decr("IAYfkMAYjWaqSq0OmherDaC1efoVzpJ5")

        DB_UID_LOCAL = wrapper.Decr("G91bXzXQSMgVosmRVfgmzvhEqfYp0aVI")
        DB_PWD_LOCAL = wrapper.Decr("ZNjHaMECk92K060A0oUK9NO+97BGM5Mp")
        '        DB_DBNAME_LOCAL = wrapper.Decr("5ubseMMqxHld7v/7LSGPAa2qH1Lu4cEhBVxOjJhfufePf80PwKtxYV8OF35lBqkH")
        DB_DBNAME_LOCAL = "dbkaraoke_outlet"

        '        If Not OpenLocalKaraokeUNKAsMySQL() Then End

        bw_updater.WorkerSupportsCancellation = True
        bw_updater.WorkerReportsProgress = True

        lblKaraoke2UnkLocation.Text = ""
        lblCloudDBConnectionStatus.Text = ""

        showCloudSonglistCheckbox.Checked = False

        monthComboBox.Enabled = False
        yearComboBox.Enabled = False
        FillMonthAndYearComboBox()

        progressBarRefreshList.Visible = False
        waitLabel.Visible = False

        movieDataGridViewClass.BindDGV(movieDataGridView)

        SetDefaultSongDrive()

        uploadedRemarkLabel.BackColor = COLOR_UPLOADED_ITEM

        deleteCloudFileButton.Enabled = False
        uploadButton.Enabled = True

    End Sub

    Sub SetDefaultSongDrive()
        Dim n As Integer

        songLocationDrive.SelectedIndex = 0

        For n = 0 To songLocationDrive.Items.Count - 1
            If InStr(UCase(songLocationDrive.Items(n).ToString), "KARAOKE") Then
                songLocationDrive.SelectedIndex = n
                Exit For
            End If
        Next

    End Sub

    Sub FillMonthAndYearComboBox()
        Dim n As Integer

        monthComboBox.Items.Clear()
        yearComboBox.Items.Clear()

        monthComboBox.Items.Add("")
        For n = 1 To 12
            monthComboBox.Items.Add(MonthName(n))
        Next

        yearComboBox.Items.Add("")
        For n = 2017 To Year(Now)
            yearComboBox.Items.Add(n)
        Next

    End Sub


    Private Sub bw_updater_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bw_updater.DoWork
        '        Dim url As String
        Dim currentSong As Song = selectedSongsCollection.Item(uploadPosition)
        Dim worker As BackgroundWorker = DirectCast(sender, BackgroundWorker)

        Dim uploaderAmazon As AmazonSDK = New AmazonSDK
        Dim blnErrorOccured As Boolean = False
        Dim strErrorMessage As String = ""
        Dim result As String = ""

        While result <> "Success"
            result = uploaderAmazon.UploadFile(s3Client,
                                               bucketName,
                                               currentSong.Path,
                                               strDriveDownloadLocation & ":" & currentSong.Path,
                                               worker,
                                               blnErrorOccured,
                                               strErrorMessage)

            If InStr(result, "File Not Found") Then
                Exit While
            ElseIf result <> "Success" Then
                SetLogMessage("Retry upload file " & currentSong.Title & ". Pesan : " & result)

                s3Client = Nothing

                Dim config = New AmazonS3Config With {
                    .ServiceURL = cloudServerUrl,
                    .AuthenticationRegion = "sgp1",
                    .EndpointDiscoveryEnabled = False,
                    .ForcePathStyle = False
                }
                s3Client = New AmazonS3Client(SystemMod.AWS_Access, SystemMod.AWS_Secret, config)

            End If
        End While

        e.Result = result

    End Sub

    Sub InsertRecordToRemoteDb(ByVal songItem As Song)
        ' 1. Keep your existing encryption logic intact
        Dim strKey As String
        Dim strPath As String
        Dim strTitle As String = BeriKarakterEscape(songItem.Title)

        strKey = IIf(Len(strTitle) >= 10, Microsoft.VisualBasic.Right(strTitle, 10), strTitle) & songItem.Kode
        strKey = FilterString(strKey)

        strPath = szEncryptDecrypt(songItem.Path, strKey)

        ' Prepare song object for API
        Dim cleanTitle As String = TambahkanKarakterEscape(songItem.Title)
        Dim cleanArtist As String = TambahkanKarakterEscape(songItem.Artist)

        ' 2. Package data into a Dictionary for the API
        Dim postData As New Dictionary(Of String, String) From {
            {"Kode", songItem.Kode},
            {"KdCategory", songItem.KdCategory},
            {"Artist", cleanArtist},
            {"ArtistUnicode", songItem.ArtistUnicode},
            {"Title", cleanTitle},
            {"TitleUnicode", songItem.TitleUnicode},
            {"InisialLagu", songItem.InisialLagu},
            {"Volume", songItem.Volume},
            {"KdCountry", songItem.KdCountry},
            {"Top40", songItem.Top40},
            {"ChannelKaraoke", songItem.ChannelKaraoke},
            {"Path", strPath},
            {"FrameWidth", songItem.FrameWidth.ToString()},
            {"FrameHeight", songItem.FrameHeight.ToString()},
            {"CreatedDateTime", songItem.CreatedDateTime.ToString()},
            {"UploaderUserId", SystemMod.PRE_USER_ID}
        }

        ' 3. Serialize and send to your new API endpoint
        Dim jsonPayload As String = JsonConvert.SerializeObject(postData)
        Dim response As ApiResponse = ApiHelper.PostJsonRequest("/api/songs/insert", jsonPayload)

        ' 4. Error handling
        If Not response.Success Then
            Throw New Exception("API Error while inserting song: " & response.ErrorMessage)
        End If
    End Sub

    Private Sub bw_updater_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles bw_updater.ProgressChanged
        ' Make sure the progress bar elements are visible
        progressBarRefreshList.Visible = True
        waitLabel.Visible = True

        ' Update the visual progress bar matching the current byte transfer chunk
        progressBarRefreshList.Value = e.ProgressPercentage

        ' Update the label text to keep users informed
        Dim currentSong As Song = selectedSongsCollection.Item(uploadPosition)
        waitLabel.Text = $"Uploading: {HilangkanKarakterEscape(currentSong.Title)} ({e.ProgressPercentage}%)"
    End Sub

    Private Sub bw_updater_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bw_updater.RunWorkerCompleted
        Dim uploadedSong As Song = selectedSongsCollection.Item(uploadPosition)

        If e.Result = "Success" Then
            progressBarRefreshList.Value = 100

            Call InsertRecordToRemoteDb(uploadedSong)

            SetLogMessage("File no " & uploadPosition & " dari " & selectedSongsCollection.Count & " (" &
                          HilangkanKarakterEscape(uploadedSong.Title) & ") berhasil di-upload")

            If uploadPosition < selectedSongsCollection.Count Then
                uploadPosition = uploadPosition + 1
                bw_updater.RunWorkerAsync()
            Else
                '=== Selesai ===
                SetButtonStatus(True)
                SetLogMessage("End Upload")

                s3Client = Nothing

                SystemMod.InsertCloudLog(SystemMod.PRE_USER_ID, "UPLOAD", "END")

                updatedSongsCollection = SystemMod.GetUpdatedSongsCollection()
                Call RefreshList()

            End If
        ElseIf InStr(e.Result, "File Not Found") Then
            SetLogMessage("File " & uploadedSong.Title & " pada lokasi " & uploadedSong.Path & " tidak ditemukan. Proses dihentikan. " &
                          "Pesan error : " & e.Result)

            SetButtonStatus(True)
            SetLogMessage("End Upload")

            s3Client = Nothing

            SystemMod.InsertCloudLog(SystemMod.PRE_USER_ID, "UPLOAD", "END BY ERROR : " & e.Result)

            updatedSongsCollection = SystemMod.GetUpdatedSongsCollection()
            Call RefreshList()

        Else
            SetLogMessage("Retry upload file " & uploadedSong.Title & ". Pesan : " & e.Result)

            s3Client = Nothing

            Dim config = New AmazonS3Config With {
                .ServiceURL = "https://sgp1.digitaloceanspaces.com",
                .AuthenticationRegion = "sgp1",
                .EndpointDiscoveryEnabled = False,
                .ForcePathStyle = False
            }
            s3Client = New AmazonS3Client(SystemMod.AWS_Access, SystemMod.AWS_Secret, config)

            bw_updater.RunWorkerAsync()


        End If

    End Sub

    Private Sub karaokeLocationButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles karaokeLocationButton.Click

        Dim strSourceServerIp As String
        Dim strConn As String

        strSourceServerIp = InputBox("Masukkan alamat IP DB Sumber",
                                     "DB Sumber",
                                     My.Computer.Registry.GetValue(
                                        WINDOWS_REGISTRY_SUBKEY_LOCATION,
                                        WINDOWS_REGISTRY_KEY_IP_SOURCE,
                                        "127.0.0.1"
                                     )
                                    )

        If Trim(strSourceServerIp) = "" Then
            MsgBox("Anda harus memasukkan alamat IP DB Sumber", vbExclamation, appName)
            Exit Sub
        End If

        Call SetRegistryKey(WINDOWS_REGISTRY_SUBKEY_LOCATION, WINDOWS_REGISTRY_KEY_IP_SOURCE, strSourceServerIp)

        strConn = "driver={" & PRE_DB_Driver & "};" &
                  "server=" & strSourceServerIp & ";" &
                  "uid=" & DB_UID_LOCAL & ";" &
                  "pwd=" & DB_PWD_LOCAL & ";" &
                  "database=" & DB_DBNAME_LOCAL

        connSourceDB.ConnectionString = strConn

        Try
            connSourceDB.Open()

            Call SetRegistryKey(WINDOWS_REGISTRY_SUBKEY_LOCATION, WINDOWS_REGISTRY_KEY_IP_SOURCE, strSourceServerIp)

            blnIsKaraoke2Connected = True
            karaokeLocationButton.Enabled = False
            DB_SERVER_LOCAL = strSourceServerIp
            lblKaraoke2UnkLocation.Text = "Source Server Connected"

            Call ConnectToCloud()
        Catch ex As Exception
            MsgBox("Error " & ex.Message, vbInformation, appName)
            Exit Sub
        End Try


    End Sub

    Private Sub ConnectToCloud()

        Try
            frmConfirmation.ShowDialog()

            DO_KEY = ApiHelper.GetSetting("EncAccessWindows")
            DO_Secret = ApiHelper.GetSetting("EncSecretWindows")

            blnIsCloudDBConnected = True
            lblCloudDBConnectionStatus.Left = lblKaraoke2UnkLocation.Left + lblKaraoke2UnkLocation.Width + 2
            lblCloudDBConnectionStatus.Text = ", Cloud Server Connected"

            Me.Text = Me.Text & " - User : " & SystemMod.PRE_USER_NAME & " - Server : " & Me.bucketName

            monthComboBox.Enabled = True
            yearComboBox.Enabled = True
        Catch ex As Exception
            MsgBox("Gagal koneksi ke API: " & ex.Message, vbInformation, appName)
            Exit Sub
        End Try

        If Not String.IsNullOrWhiteSpace(DO_KEY) And Not String.IsNullOrWhiteSpace(DO_Secret) Then
            Dim wrapper As New Desi(SystemMod.Enc_AWS_Access)
            SystemMod.AWS_Access = wrapper.Decr(DO_KEY)

            wrapper = New Desi(SystemMod.Enc_AWS_Secret)
            SystemMod.AWS_Secret = wrapper.Decr(DO_Secret)
        Else
            MsgBox("Gagal baca file setting dari API", vbExclamation, appName)
            End
        End If

        ' Fetch the list of already updated songs via API
        updatedSongsCollection = SystemMod.GetUpdatedSongsCollection()

        keepConnectionTimer.Enabled = True
    End Sub

    Sub RefreshList()
        Dim strSQL As String = "", strSQLSort As String = ""

        If monthComboBox.SelectedIndex > 0 And yearComboBox.SelectedIndex > 0 Then

            If showCloudSonglistCheckbox.Checked Then
                ' --- API PATH ---
                Dim year As String = yearComboBox.SelectedItem.ToString()
                Dim month As String = monthComboBox.SelectedIndex.ToString()

                ' Fetch JSON from API
                Dim response As ApiResponse = ApiHelper.GetRequest("/api/songs/" & year & "/" & month)

                If response.Success Then
                    ' Deserialize JSON to DataTable
                    ' Note: Newtonsoft.Json is excellent for this.
                    ' If you don't have it, you can use System.Text.Json to populate a DataTable
                    Dim dt As DataTable = JsonConvert.DeserializeObject(Of DataTable)(response.Data)

                    ' Inject directly into your existing DGV class
                    movieDataGridViewClass.InjectDataTable(dt)
                Else
                    MsgBox("Gagal memuat data dari Cloud: " & response.ErrorMessage, vbExclamation, appName)
                End If
            Else
                strSQL = "SELECT a.kode, " &
                                 "a.kdcategory, " &
                                 "a.title, " &
                                 "a.title_unicode, " &
                                 "a.artist, " &
                                 "a.artist_unicode, " &
                                 "a.inisial_lagu, " &
                                 "a.volume, " &
                                 "a.kdcountry, " &
                                 "b.country, " &
                                 "a.top40, " &
                                 "a.channel_karaoke, " &
                                 "a.drive, " &
                                 "a.path, " &
                                 "a.frame_width, " &
                                 "a.frame_height, " &
                                 "DATE_FORMAT(a.tglmasuk, '%Y-%m-%d') AS tgl_masuk " &
                           "FROM tblmovie a " &
                     "INNER JOIN tblcountry b " &
                             "ON a.kdcountry = b.kdcountry " &
                          "WHERE MONTH(a.tglmasuk) = " & monthComboBox.SelectedIndex & " " &
                            "AND YEAR(a.tglmasuk) = " & yearComboBox.SelectedItem.ToString & " " &
                          "ORDER BY b.country, a.title "

                strSQLSort = ""

                With movieDataGridViewClass
                    .Init(Me,
                      "tblmovie",
                      strSQL,
                          "",
                      strSQLSort,
                      "",
                      "kode",
                      PRE_DB_Driver,
                      IIf(showCloudSonglistCheckbox.Checked, SystemMod.API_BASE_URL, DB_SERVER_LOCAL),
                      IIf(showCloudSonglistCheckbox.Checked, PRE_DB_Port, DB_PORT_LOCAL),
                      IIf(showCloudSonglistCheckbox.Checked, PRE_DB_Name, DB_DBNAME_LOCAL),
                      IIf(showCloudSonglistCheckbox.Checked, PRE_DB_UID, DB_UID_LOCAL),
                      IIf(showCloudSonglistCheckbox.Checked, PRE_DB_Pass, DB_PWD_LOCAL))

                End With
            End If

            Call RefreshSongList()
            Call GridFormat()
        End If
    End Sub

    Sub GridFormat()
        If movieDataGridView.Columns.Count <= COLUMN_KODE Then
            Exit Sub
        End If

        Dim n As Integer

        With movieDataGridView
            For n = 1 To .Columns.Count - 1
                movieDataGridView.Columns(n).ReadOnly = True
            Next n

            .Columns(COLUMN_CHECKBOX).Width = 50
            .Columns(COLUMN_KODE).Visible = False
            .Columns(COLUMN_KDCATEGORY).Visible = False
            .Columns(COLUMN_TITLE).Width = 250
            .Columns(COLUMN_TITLE_UNICODE).Width = 160
            .Columns(COLUMN_ARTIST).Width = 200
            .Columns(COLUMN_ARTIST_UNICODE).Width = 160
            .Columns(COLUMN_INISIAL_LAGU).Visible = False
            .Columns(COLUMN_VOLUME).Visible = False
            .Columns(COLUMN_KDCOUNTRY).Visible = False
            .Columns(COLUMN_COUNTRY_NAME).Width = 80
            .Columns(COLUMN_TOP40).Visible = False
            .Columns(COLUMN_CHANNEL_KARAOKE).Visible = False
            .Columns(COLUMN_DRIVE).Visible = False
            .Columns(COLUMN_PATH).Width = 300
            .Columns(COLUMN_FRAME_WIDTH).Visible = False
            .Columns(COLUMN_FRAME_HEIGHT).Visible = False
            .Columns(COLUMN_CREATED_DATE_TIME).Width = 100

            .Columns(COLUMN_TITLE).HeaderText = "Judul"
            .Columns(COLUMN_TITLE_UNICODE).HeaderText = "Judul (Unicode)"
            .Columns(COLUMN_ARTIST).HeaderText = "Penyanyi"
            .Columns(COLUMN_ARTIST_UNICODE).HeaderText = "Penyanyi (Unicode)"
            .Columns(COLUMN_COUNTRY_NAME).HeaderText = "Negara"
            .Columns(COLUMN_PATH).HeaderText = "Path"
            .Columns(COLUMN_CREATED_DATE_TIME).HeaderText = "Tgl Masuk"

            .Width = modGeneral.DGVWidth(movieDataGridView) + 20
        End With

    End Sub

    Sub RefreshSongList()
        Dim intTempRow As Integer

        progressBarRefreshList.Visible = True
        waitLabel.Visible = True

        SetButtonStatus(False)

        With movieDataGridView
            .Visible = False
            .Enabled = False

            If IsNothing(.CurrentRow) Then
                intTempRow = 0
            Else
                intTempRow = .CurrentRow.Index
            End If

            If Not showCloudSonglistCheckbox.Checked Then
                movieDataGridViewClass.RefreshList(1, 0)
            End If

            .Visible = True

            Dim n As Long
            Dim strKey As String

            Dim strKode As String
            Dim strTitle As String, strTitleUnicode As String
            Dim strArtist As String, strArtistUnicode As String
            Dim strPath As String

            '=== Reformat the table

            ' =======================================================================
            ' PERFORMANCE OPTIMIZATION: Process in memory buffer instead of UI cells
            ' =======================================================================
            Dim dt As DataTable = movieDataGridViewClass.dbTable

            If dt IsNot Nothing AndAlso .RowCount > 0 Then

                ' Define column name variables
                Dim colKode As String = "kode"
                Dim colTitle As String = "title"
                Dim colPath As String = "path"
                Dim colArtist As String = "artist"
                Dim colTitleUnicode As String
                Dim colArtistUnicode As String
                Dim colCreatedDate As String

                ' =======================================================================
                ' EXPLICIT SOURCE MAPPING: No more searching the table structure!
                ' =======================================================================
                If showCloudSonglistCheckbox.Checked Then
                    ' Target Cloud API structure (camelCase)
                    colTitleUnicode = "titleUnicode"
                    colArtistUnicode = "artistUnicode"
                    colCreatedDate = "createdDateTime"
                Else
                    ' Target Local Database structure (snake_case)
                    colTitleUnicode = "title_unicode"
                    colArtistUnicode = "artist_unicode"
                    colCreatedDate = "tgl_masuk"
                End If
                ' =======================================================================

                ' Freeze UI layout calculations and DataTable event chatter
                dt.BeginLoadData()
                movieDataGridView.SuspendLayout()

                Dim currentPercent As Integer = 0
                Dim totalRows As Integer = .RowCount
                Dim uploadedCellStyle As New DataGridViewCellStyle With {.BackColor = COLOR_UPLOADED_ITEM}

                For n = 0 To totalRows - 1
                    Dim row As DataGridViewRow = .Rows(n)
                    Dim rowView As DataRowView = TryCast(row.DataBoundItem, DataRowView)

                    If rowView IsNot Nothing Then
                        ' Read values directly from the memory cache using explicit keys
                        strKode = rowView(colKode).ToString()
                        strTitle = rowView(colTitle).ToString()
                        strTitleUnicode = rowView(colTitleUnicode).ToString()
                        strArtist = rowView(colArtist).ToString()
                        strArtistUnicode = rowView(colArtistUnicode).ToString()
                        strPath = rowView(colPath).ToString()

                        ' Run your unchanged encryption decryption math
                        strKey = IIf(Len(strTitle) >= 10, Microsoft.VisualBasic.Right(strTitle, 10), strTitle) & strKode
                        strKey = FilterString(strKey)
                        strPath = Strings.LCase(szEncryptDecrypt(strPath, strKey))

                        ' Write values back directly to memory buffer using the exact same keys
                        rowView(colTitle) = HilangkanKarakterEscape(strTitle)
                        rowView(colTitleUnicode) = HilangkanKarakterEscape(strTitleUnicode)
                        rowView(colArtist) = HilangkanKarakterEscape(strArtist)
                        rowView(colArtistUnicode) = HilangkanKarakterEscape(strArtistUnicode)
                        rowView(colPath) = strPath

                        ' Apply visual row coloring for local files
                        If Not showCloudSonglistCheckbox.Checked Then
                            If CheckSongAlreadyUpdated(strTitle & strArtist) Then
                                row.Cells(0).ReadOnly = True
                                row.Cells(COLUMN_TITLE).Style = uploadedCellStyle
                            End If
                        End If
                    End If

                    ' Throttle progress bar updates
                    Dim newPercent As Integer = CInt((n / totalRows) * 100)
                    If newPercent > currentPercent Then
                        currentPercent = newPercent
                        progressBarRefreshList.Value = currentPercent
                    End If
                Next

                ' Unfreeze layout and push all data updates to the screen
                dt.EndLoadData()
                movieDataGridView.ResumeLayout()
            End If

            Me.Refresh()

            If n = 0 Then
                Call GridFormat()
            End If

            .Enabled = True

        End With

        progressBarRefreshList.Visible = False
        waitLabel.Visible = False

        SetButtonStatus(True)

    End Sub

    Function CheckSongAlreadyUpdated(ByVal titleArtistString As String) As Boolean
        If updatedSongsCollection Is Nothing Then Return False

        ' Modern .NET HashSet check. Takes microseconds and NEVER throws errors.
        Return updatedSongsCollection.Contains(titleArtistString)
    End Function

    Private Sub comboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles yearComboBox.SelectedIndexChanged, monthComboBox.SelectedIndexChanged
        Call RefreshList()
    End Sub

    Private Sub selectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles selectAllButton.Click, unselectAllButton.Click
        Dim n As Integer
        Dim button As Button = DirectCast(sender, Button)

        For n = 0 To movieDataGridView.Rows.Count - 1
            movieDataGridView.Rows(n).Cells(COLUMN_CHECKBOX).Value = IIf(button.Name = "selectAllButton", True, False)
        Next

    End Sub

    Private Sub movieDataGridView_ColumnHeaderMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles movieDataGridView.ColumnHeaderMouseClick
        ' =======================================================================
        ' FIX 1: If looking at the Cloud list, do not run custom local coloring!
        ' Let Windows Forms sort and display the text naturally, then exit.
        ' =======================================================================
        If showCloudSonglistCheckbox.Checked Then
            Exit Sub
        End If
        ' =======================================================================

        progressBarRefreshList.Visible = True
        waitLabel.Visible = True

        ' Freeze layout rules while re-coloring sorted rows
        movieDataGridView.SuspendLayout()

        Dim n As Integer
        Dim strTitle As String, strArtist As String
        Dim totalRows As Integer = movieDataGridView.RowCount

        ' Use memory-optimized shared styles
        Dim uploadedCellStyle As New DataGridViewCellStyle With {.BackColor = COLOR_UPLOADED_ITEM}
        Dim defaultCellStyle As New DataGridViewCellStyle With {.BackColor = Color.White}

        With movieDataGridView
            For n = 0 To totalRows - 1
                Dim row As DataGridViewRow = .Rows(n)

                ' Read directly from the underlying data cache for maximum speed
                Dim rowView As DataRowView = TryCast(row.DataBoundItem, DataRowView)

                If rowView IsNot Nothing Then
                    strTitle = rowView("title").ToString()
                    strArtist = rowView("artist").ToString()
                Else
                    ' Fallback safety check
                    strTitle = row.Cells(COLUMN_TITLE).Value?.ToString()
                    strArtist = row.Cells(COLUMN_ARTIST).Value?.ToString()
                End If

                ' FIX 2: Fast HashSet check (.Contains) replaces the slow exception logic
                If CheckSongAlreadyUpdated(strTitle & strArtist) Then
                    row.Cells(0).ReadOnly = True
                    row.Cells(COLUMN_TITLE).Style = uploadedCellStyle
                Else
                    row.Cells(COLUMN_TITLE).Style = defaultCellStyle
                End If

                ' Throttle progress bar rendering
                If n Mod 50 = 0 Then
                    progressBarRefreshList.Value = CInt((n / totalRows) * 100)
                End If
            Next n
        End With

        movieDataGridView.ResumeLayout()

        progressBarRefreshList.Visible = False
        waitLabel.Visible = False

    End Sub

    Private Sub movieDataGridView_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles movieDataGridView.KeyDown
        If e.KeyCode = Keys.Space Then
            Dim row As DataGridViewRow = movieDataGridView.CurrentRow
            Dim value As Boolean = row.Cells(0).Value

            If value Then
                row.Cells(0).Value = False
            Else
                If row.Cells(COLUMN_TITLE).Style.BackColor = COLOR_UPLOADED_ITEM And Not showCloudSonglistCheckbox.Checked Then
                    MsgBox("Anda tidak dapat upload file yang sudah pernah di-upload", vbInformation, appName)
                Else
                    row.Cells(0).Value = True
                End If
            End If

            movieDataGridView.RefreshEdit()
        End If

    End Sub

    Private Sub songLocationDrive_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles songLocationDrive.SelectedIndexChanged
        strDriveDownloadLocation = Strings.Left(songLocationDrive.SelectedItem.ToString, 1)
    End Sub

    Private Sub showCloudSonglist_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles showCloudSonglistCheckbox.CheckedChanged
        Call RefreshList()

        If showCloudSonglistCheckbox.Checked Then
            deleteCloudFileButton.Enabled = True
            uploadButton.Enabled = False
        Else
            deleteCloudFileButton.Enabled = False
            uploadButton.Enabled = True
        End If

    End Sub

    Sub RemoveRecordFromRemoteDb(ByVal fileCode As String)
        ' Assuming you add a DELETE method to your ApiHelper wrapper
        ' Or simply use a POST to an endpoint that performs the delete
        ApiHelper.PostJsonRequest("/api/songs/delete/" & fileCode, "{}")
    End Sub

    Private Sub deleteCloudFileButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles deleteCloudFileButton.Click
        If showCloudSonglistCheckbox.Checked = False Then
            MsgBox("Anda harus menampilkan terlebih dahulu daftar lagu di CLOUD", vbInformation, appName)
            Exit Sub
        End If

        If Not blnIsKaraoke2Connected Then
            MsgBox("Anda harus tentukan lokasi program karaoke terlebih dahulu", MsgBoxStyle.Information, appName)
            Exit Sub
        ElseIf Not blnIsCloudDBConnected Then
            MsgBox("Anda harus koneksi ke Cloud Database terlebih dahulu", MsgBoxStyle.Information, appName)
            Exit Sub
        Else

            If MsgBox("Yakin hapus lagu ?", vbQuestion + vbYesNo, appName) <> vbYes Then
                Exit Sub
            End If

            Dim strTobeDeleteFiles As String = ""
            Dim n As Integer
            Dim tobeDeleteFilesCodeCollection As New Collection

            SetLogMessage("Begin Delete")
            SetButtonStatus(False)

            For n = 0 To movieDataGridView.RowCount - 1
                If movieDataGridView.Rows(n).Cells(COLUMN_CHECKBOX).Value Then

                    Dim deleterAmazon As AmazonSDK = New AmazonSDK
                    Dim result As String

                    result = deleterAmazon.DeleteFile(bucketName, movieDataGridView.Rows(n).Cells(COLUMN_PATH).Value)

                    If result.Equals("Success") Then
                        RemoveRecordFromRemoteDb(movieDataGridView.Rows(n).Cells(COLUMN_KODE).Value)
                        SetLogMessage(movieDataGridView.Rows(n).Cells(COLUMN_TITLE).Value & " berhasil dihapus")

                        SystemMod.InsertCloudLog(SystemMod.PRE_USER_ID,
                                                 "DELETE SONG",
                                                    movieDataGridView.Rows(n).Cells(COLUMN_KODE).Value & " - " &
                                                    Replace(movieDataGridView.Rows(n).Cells(COLUMN_TITLE).Value, "'", "\'"))

                    Else
                        SetLogMessage(movieDataGridView.Rows(n).Cells(COLUMN_TITLE).Value & " gagal dihapus. Error : " & result)
                    End If
                End If
            Next

            updatedSongsCollection = SystemMod.GetUpdatedSongsCollection()
            Call RefreshList()

            SetLogMessage("End Delete")
            MsgBox("File di cloud berhasil dihapus", vbInformation, appName)

            SetButtonStatus(True)

        End If

    End Sub

    Sub SetButtonStatus(ByVal stat As Boolean)
        uploadButton.Enabled = stat
        selectAllButton.Enabled = stat
        unselectAllButton.Enabled = stat
        '        deleteCloudFileButton.Enabled = Not stat
        songLocationDrive.Enabled = stat
        monthComboBox.Enabled = stat
        yearComboBox.Enabled = stat
        showCloudSonglistCheckbox.Enabled = stat
        movieDataGridView.Enabled = stat
    End Sub

    Sub SetLogMessage(ByVal message As String)
        statusTextBox.Text = Date.Now & " : " & message & vbCrLf & statusTextBox.Text
        statusTextBox.SelectionStart = 0
        statusTextBox.SelectionLength = 0
        statusTextBox.Refresh()
    End Sub

    Private Sub keepConnectionTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles keepConnectionTimer.Tick
        Dim rs As New ADODB.Recordset
        Dim sql As String = "SELECT NOW()"

        Try
            Call OpenADORS(rs, sql, connSourceDB)
            Debug.Print("Local Time :  " & rs.Fields(0).Value)
            rs.Close()
        Catch ex As Exception
            Try
                Try
                    connSourceDB.Close()
                Catch ex3 As Exception
                    SetLogMessage(ex3.Message.ToString & " - Try reconnecting...")
                End Try
                connSourceDB.Open()
            Catch ex2 As Exception
                SetLogMessage(ex2.Message.ToString & " - Try reconnecting...")
            End Try

            SetLogMessage(ex.Message.ToString & " - Try reconnecting...")
        End Try

    End Sub

End Class
