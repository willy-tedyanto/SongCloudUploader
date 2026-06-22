<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmUploader
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUploader))
        lblSongLocationDrive = New Label()
        unselectAllButton = New Button()
        selectAllButton = New Button()
        monthComboBox = New ComboBox()
        waitLabel = New Label()
        yearComboBox = New ComboBox()
        progressBarRefreshList = New ProgressBar()
        uploadedRemarkLabel = New Label()
        Label1 = New Label()
        movieDataGridView = New DataGridView()
        Column1 = New DataGridViewCheckBoxColumn()
        lblCloudDBConnectionStatus = New Label()
        bw_updater = New ComponentModel.BackgroundWorker()
        lblKaraoke2UnkLocation = New Label()
        karaokeLocationButton = New Button()
        statusTextBox = New TextBox()
        uploadButton = New Button()
        folderBrowserDialog = New FolderBrowserDialog()
        showCloudSonglistCheckbox = New CheckBox()
        deleteCloudFileButton = New Button()
        songLocationDrive = New ComboBox()
        keepConnectionTimer = New Timer(components)
        lblVersion = New Label()
        CType(movieDataGridView, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' lblSongLocationDrive
        ' 
        lblSongLocationDrive.AutoSize = True
        lblSongLocationDrive.Font = New Font("Arial", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lblSongLocationDrive.Location = New Point(13, 53)
        lblSongLocationDrive.Margin = New Padding(4, 0, 4, 0)
        lblSongLocationDrive.Name = "lblSongLocationDrive"
        lblSongLocationDrive.Size = New Size(176, 16)
        lblSongLocationDrive.TabIndex = 36
        lblSongLocationDrive.Text = "Lokasi Penyimpanan Lagu"
        ' 
        ' unselectAllButton
        ' 
        unselectAllButton.BackColor = Color.FromArgb(CByte(255), CByte(192), CByte(192))
        unselectAllButton.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        unselectAllButton.Location = New Point(147, 497)
        unselectAllButton.Margin = New Padding(4, 3, 4, 3)
        unselectAllButton.Name = "unselectAllButton"
        unselectAllButton.Size = New Size(120, 29)
        unselectAllButton.TabIndex = 7
        unselectAllButton.Text = "Jangan Pilih"
        unselectAllButton.UseVisualStyleBackColor = False
        ' 
        ' selectAllButton
        ' 
        selectAllButton.BackColor = Color.FromArgb(CByte(192), CByte(255), CByte(255))
        selectAllButton.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        selectAllButton.Location = New Point(20, 497)
        selectAllButton.Margin = New Padding(4, 3, 4, 3)
        selectAllButton.Name = "selectAllButton"
        selectAllButton.Size = New Size(120, 29)
        selectAllButton.TabIndex = 6
        selectAllButton.Text = "Pilih Semua"
        selectAllButton.UseVisualStyleBackColor = False
        ' 
        ' monthComboBox
        ' 
        monthComboBox.DropDownStyle = ComboBoxStyle.DropDownList
        monthComboBox.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        monthComboBox.FormattingEnabled = True
        monthComboBox.Location = New Point(103, 109)
        monthComboBox.Margin = New Padding(4, 3, 4, 3)
        monthComboBox.Name = "monthComboBox"
        monthComboBox.Size = New Size(123, 24)
        monthComboBox.TabIndex = 3
        ' 
        ' waitLabel
        ' 
        waitLabel.AutoSize = True
        waitLabel.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        waitLabel.Location = New Point(851, 109)
        waitLabel.Margin = New Padding(4, 0, 4, 0)
        waitLabel.Name = "waitLabel"
        waitLabel.Size = New Size(103, 16)
        waitLabel.TabIndex = 31
        waitLabel.Text = "Mohon tunggu"
        ' 
        ' yearComboBox
        ' 
        yearComboBox.DropDownStyle = ComboBoxStyle.DropDownList
        yearComboBox.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        yearComboBox.FormattingEnabled = True
        yearComboBox.Location = New Point(234, 109)
        yearComboBox.Margin = New Padding(4, 3, 4, 3)
        yearComboBox.Name = "yearComboBox"
        yearComboBox.Size = New Size(104, 24)
        yearComboBox.TabIndex = 4
        ' 
        ' progressBarRefreshList
        ' 
        progressBarRefreshList.Location = New Point(728, 109)
        progressBarRefreshList.Margin = New Padding(4, 3, 4, 3)
        progressBarRefreshList.Name = "progressBarRefreshList"
        progressBarRefreshList.Size = New Size(117, 28)
        progressBarRefreshList.TabIndex = 30
        ' 
        ' uploadedRemarkLabel
        ' 
        uploadedRemarkLabel.AutoSize = True
        uploadedRemarkLabel.BackColor = Color.OrangeRed
        uploadedRemarkLabel.Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        uploadedRemarkLabel.Location = New Point(274, 497)
        uploadedRemarkLabel.Margin = New Padding(4, 0, 4, 0)
        uploadedRemarkLabel.Name = "uploadedRemarkLabel"
        uploadedRemarkLabel.Padding = New Padding(6)
        uploadedRemarkLabel.Size = New Size(168, 25)
        uploadedRemarkLabel.TabIndex = 29
        uploadedRemarkLabel.Text = "Lagu yang sudah diupload"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label1.Location = New Point(9, 109)
        Label1.Margin = New Padding(4, 0, 4, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(74, 16)
        Label1.TabIndex = 28
        Label1.Text = "Lagu Bulan"
        ' 
        ' movieDataGridView
        ' 
        movieDataGridView.AllowUserToAddRows = False
        movieDataGridView.AllowUserToDeleteRows = False
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = SystemColors.Control
        DataGridViewCellStyle1.Font = New Font("Segoe UI", 11F)
        DataGridViewCellStyle1.ForeColor = SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True
        movieDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        movieDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        movieDataGridView.Columns.AddRange(New DataGridViewColumn() {Column1})
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = SystemColors.Window
        DataGridViewCellStyle2.Font = New Font("Segoe UI", 11F)
        DataGridViewCellStyle2.ForeColor = SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.False
        movieDataGridView.DefaultCellStyle = DataGridViewCellStyle2
        movieDataGridView.Location = New Point(13, 143)
        movieDataGridView.Margin = New Padding(4, 3, 4, 3)
        movieDataGridView.Name = "movieDataGridView"
        movieDataGridView.RowHeadersVisible = False
        movieDataGridView.Size = New Size(1320, 344)
        movieDataGridView.TabIndex = 5
        movieDataGridView.Visible = False
        ' 
        ' Column1
        ' 
        Column1.HeaderText = ""
        Column1.Name = "Column1"
        ' 
        ' lblCloudDBConnectionStatus
        ' 
        lblCloudDBConnectionStatus.AutoSize = True
        lblCloudDBConnectionStatus.Font = New Font("Arial", 11.25F, FontStyle.Bold)
        lblCloudDBConnectionStatus.Location = New Point(548, 15)
        lblCloudDBConnectionStatus.Margin = New Padding(4, 0, 4, 0)
        lblCloudDBConnectionStatus.Name = "lblCloudDBConnectionStatus"
        lblCloudDBConnectionStatus.Size = New Size(132, 18)
        lblCloudDBConnectionStatus.TabIndex = 26
        lblCloudDBConnectionStatus.Text = "Belum terkoneksi"
        ' 
        ' bw_updater
        ' 
        ' 
        ' lblKaraoke2UnkLocation
        ' 
        lblKaraoke2UnkLocation.AutoSize = True
        lblKaraoke2UnkLocation.Font = New Font("Arial", 11.25F, FontStyle.Bold)
        lblKaraoke2UnkLocation.Location = New Point(364, 15)
        lblKaraoke2UnkLocation.Margin = New Padding(4, 0, 4, 0)
        lblKaraoke2UnkLocation.Name = "lblKaraoke2UnkLocation"
        lblKaraoke2UnkLocation.Size = New Size(181, 18)
        lblKaraoke2UnkLocation.TabIndex = 24
        lblKaraoke2UnkLocation.Text = "lblKaraoke2UnkLocation"
        ' 
        ' karaokeLocationButton
        ' 
        karaokeLocationButton.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(128))
        karaokeLocationButton.Font = New Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        karaokeLocationButton.Location = New Point(18, 5)
        karaokeLocationButton.Margin = New Padding(4, 3, 4, 3)
        karaokeLocationButton.Name = "karaokeLocationButton"
        karaokeLocationButton.Size = New Size(327, 39)
        karaokeLocationButton.TabIndex = 0
        karaokeLocationButton.Text = "Koneksi ke DB Sumber"
        karaokeLocationButton.UseVisualStyleBackColor = False
        ' 
        ' statusTextBox
        ' 
        statusTextBox.BackColor = Color.White
        statusTextBox.Location = New Point(18, 592)
        statusTextBox.Margin = New Padding(4, 3, 4, 3)
        statusTextBox.Multiline = True
        statusTextBox.Name = "statusTextBox"
        statusTextBox.ReadOnly = True
        statusTextBox.ScrollBars = ScrollBars.Vertical
        statusTextBox.Size = New Size(1320, 125)
        statusTextBox.TabIndex = 22
        ' 
        ' uploadButton
        ' 
        uploadButton.BackColor = Color.Lime
        uploadButton.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        uploadButton.Location = New Point(18, 533)
        uploadButton.Margin = New Padding(4, 3, 4, 3)
        uploadButton.Name = "uploadButton"
        uploadButton.Size = New Size(153, 53)
        uploadButton.TabIndex = 8
        uploadButton.Text = "Start Upload"
        uploadButton.UseVisualStyleBackColor = False
        ' 
        ' showCloudSonglistCheckbox
        ' 
        showCloudSonglistCheckbox.AutoSize = True
        showCloudSonglistCheckbox.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        showCloudSonglistCheckbox.Location = New Point(428, 111)
        showCloudSonglistCheckbox.Margin = New Padding(4, 3, 4, 3)
        showCloudSonglistCheckbox.Name = "showCloudSonglistCheckbox"
        showCloudSonglistCheckbox.Size = New Size(209, 20)
        showCloudSonglistCheckbox.TabIndex = 9
        showCloudSonglistCheckbox.Text = "Lihat daftar lagu di CLOUD"
        showCloudSonglistCheckbox.UseVisualStyleBackColor = True
        ' 
        ' deleteCloudFileButton
        ' 
        deleteCloudFileButton.BackColor = Color.FromArgb(CByte(255), CByte(128), CByte(128))
        deleteCloudFileButton.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        deleteCloudFileButton.Location = New Point(1125, 497)
        deleteCloudFileButton.Margin = New Padding(4, 3, 4, 3)
        deleteCloudFileButton.Name = "deleteCloudFileButton"
        deleteCloudFileButton.Size = New Size(208, 29)
        deleteCloudFileButton.TabIndex = 39
        deleteCloudFileButton.TabStop = False
        deleteCloudFileButton.Text = "Hapus Lagu di Cloud"
        deleteCloudFileButton.UseVisualStyleBackColor = False
        ' 
        ' songLocationDrive
        ' 
        songLocationDrive.DropDownStyle = ComboBoxStyle.DropDownList
        songLocationDrive.FormattingEnabled = True
        songLocationDrive.Location = New Point(241, 52)
        songLocationDrive.Margin = New Padding(4, 3, 4, 3)
        songLocationDrive.Name = "songLocationDrive"
        songLocationDrive.Size = New Size(140, 23)
        songLocationDrive.TabIndex = 2
        ' 
        ' keepConnectionTimer
        ' 
        keepConnectionTimer.Interval = 10000
        ' 
        ' lblVersion
        ' 
        lblVersion.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        lblVersion.AutoSize = True
        lblVersion.Location = New Point(1236, 5)
        lblVersion.Margin = New Padding(4, 0, 4, 0)
        lblVersion.Name = "lblVersion"
        lblVersion.Size = New Size(58, 15)
        lblVersion.TabIndex = 40
        lblVersion.Text = "lblVersion"
        ' 
        ' frmUploader
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1350, 729)
        Controls.Add(lblVersion)
        Controls.Add(songLocationDrive)
        Controls.Add(deleteCloudFileButton)
        Controls.Add(showCloudSonglistCheckbox)
        Controls.Add(lblSongLocationDrive)
        Controls.Add(unselectAllButton)
        Controls.Add(selectAllButton)
        Controls.Add(monthComboBox)
        Controls.Add(waitLabel)
        Controls.Add(yearComboBox)
        Controls.Add(progressBarRefreshList)
        Controls.Add(uploadedRemarkLabel)
        Controls.Add(Label1)
        Controls.Add(movieDataGridView)
        Controls.Add(lblCloudDBConnectionStatus)
        Controls.Add(lblKaraoke2UnkLocation)
        Controls.Add(karaokeLocationButton)
        Controls.Add(statusTextBox)
        Controls.Add(uploadButton)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(4, 3, 4, 3)
        Name = "frmUploader"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Song Cloud Uploader"
        CType(movieDataGridView, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()

    End Sub
    Friend WithEvents lblSongLocationDrive As System.Windows.Forms.Label
    Friend WithEvents unselectAllButton As System.Windows.Forms.Button
    Friend WithEvents selectAllButton As System.Windows.Forms.Button
    Friend WithEvents monthComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents waitLabel As System.Windows.Forms.Label
    Friend WithEvents yearComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents progressBarRefreshList As System.Windows.Forms.ProgressBar
    Friend WithEvents uploadedRemarkLabel As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents movieDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents Column1 As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents lblCloudDBConnectionStatus As System.Windows.Forms.Label
    Friend WithEvents bw_updater As System.ComponentModel.BackgroundWorker
    Friend WithEvents lblKaraoke2UnkLocation As System.Windows.Forms.Label
    Friend WithEvents karaokeLocationButton As System.Windows.Forms.Button
    Friend WithEvents statusTextBox As System.Windows.Forms.TextBox
    Friend WithEvents uploadButton As System.Windows.Forms.Button
    Friend WithEvents folderBrowserDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents showCloudSonglistCheckbox As System.Windows.Forms.CheckBox
    Friend WithEvents deleteCloudFileButton As System.Windows.Forms.Button
    Friend WithEvents songLocationDrive As System.Windows.Forms.ComboBox
    Friend WithEvents keepConnectionTimer As System.Windows.Forms.Timer
    Friend WithEvents lblVersion As System.Windows.Forms.Label

End Class
