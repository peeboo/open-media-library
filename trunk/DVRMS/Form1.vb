#Region "Import Declaratives"

Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Imports Toub.MediaCenter.Dvrms.Metadata

#End Region

Public Class Form1

#Region "Private Variables"

    Private DVRMS As DVRMSItem = Nothing
    Private bIsDirty As Boolean = False
    Private SaveFolder As String = Nothing
    Private WMTags() As MetadataItem
    Private RedoBuffer As New Queue(Of String)
    Private xmlDVDProfiler As XmlDocument

#End Region

#Region "Form Events"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LoadMetaTags()
        InitUI()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If bIsDirty Then
            If MessageBox.Show("You have unsaved information.  Are you certain you want to exit?", _
                "DVR-MS Editor", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                e.Cancel = True
            End If
        End If
    End Sub

#End Region

#Region "Control Events"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ToolStripItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles OpenToolStripMenuItem.Click, OpenToolStripButton.Click, _
            SaveToolStripMenuItem.Click, SaveToolStripButton.Click, _
            CustomizeToolStripMenuItem.Click, _
            DVDProfilerToolStripMenuItem.Click, _
            ExitToolStripMenuItem.Click
        Select Case True
            Case sender.Equals(OpenToolStripMenuItem), _
                sender.Equals(OpenToolStripButton)
                LoadDVRMSFiles()
            Case sender.Equals(SaveAsToolStripMenuItem)
                SaveFiles()
            Case sender.Equals(SaveToolStripMenuItem), _
                sender.Equals(SaveToolStripButton)
                SaveDVRMSFiles()
            Case sender.Equals(CustomizeToolStripMenuItem)
                XMLDVRMSFiles()
            Case sender.Equals(DVDProfilerToolStripMenuItem)
                UpdateFromDVDProfilerInfo()
            Case sender.Equals(ExitToolStripMenuItem)
                SaveDVRMSFiles()
                Me.Close()
        End Select
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CheckedListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles CheckedListBox1.SelectedIndexChanged
        Dim clb As CheckedListBox = CType(sender, CheckedListBox)
        If bIsDirty Then
            SaveMetaAttributes(DVRMS)
        End If
        DVRMS = CType(clb.SelectedItem, CheckedListBoxItem).item
        RefreshEditData(DVRMS)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles CheckBox1.CheckedChanged
        Dim ii As Integer = Me.CheckedListBox1.SelectedIndex
        If ii >= 0 Then
            Me.CheckedListBox1.SetItemCheckState(Me.CheckedListBox1.SelectedIndex, CheckState.Unchecked)
        End If
        bIsDirty = True
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles ComboBox1.SelectedIndexChanged
        Dim ii As Integer = Me.CheckedListBox1.SelectedIndex
        If ii >= 0 Then
            Me.CheckedListBox1.SetItemCheckState(Me.CheckedListBox1.SelectedIndex, CheckState.Unchecked)
        End If
        bIsDirty = True
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CheckedListBox2_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles CheckedListBox2.SelectedValueChanged
        Dim ii As Integer = Me.CheckedListBox1.SelectedIndex
        If ii >= 0 Then
            Me.CheckedListBox1.SetItemCheckState(Me.CheckedListBox1.SelectedIndex, CheckState.Unchecked)
        End If
        bIsDirty = True
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles TextBox7.TextChanged, TextBox6.TextChanged, TextBox5.TextChanged, TextBox4.TextChanged, _
            TextBox3.TextChanged, TextBox2.TextChanged, TextBox1.TextChanged
        Dim ii As Integer = Me.CheckedListBox1.SelectedIndex
        If ii >= 0 Then
            Me.CheckedListBox1.SetItemCheckState(Me.CheckedListBox1.SelectedIndex, CheckState.Unchecked)
        End If
        bIsDirty = True
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub EditToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
        UndoToolStripMenuItem.Click, UndoToolStripMenuItem1.Click, _
        CutToolStripMenuItem.Click, CutToolStripMenuItem1.Click, _
        CopyToolStripMenuItem.Click, CopyToolStripMenuItem1.Click, _
        PasteToolStripMenuItem.Click, PasteToolStripMenuItem1.Click, _
        SelectAllToolStripMenuItem.Click, SelectAllToolStripMenuItem1.Click
        Dim tb As TextBox
        Select Case True
            Case Me.TextBox1.Focused : tb = Me.TextBox1
            Case Me.TextBox2.Focused : tb = Me.TextBox2
            Case Me.TextBox3.Focused : tb = Me.TextBox3
            Case Me.TextBox4.Focused : tb = Me.TextBox4
            Case Me.TextBox5.Focused : tb = Me.TextBox5
            Case Me.TextBox6.Focused : tb = Me.TextBox6
            Case Me.TextBox7.Focused : tb = Me.TextBox7
            Case Else : Exit Sub
        End Select
        Select Case CType(sender, ToolStripMenuItem).Name
            Case UndoToolStripMenuItem.Name, UndoToolStripMenuItem1.Name
                If tb.CanUndo Then
                    tb.Undo()
                    tb.ClearUndo()
                End If
            Case CutToolStripMenuItem.Name, CutToolStripMenuItem1.Name : If tb.SelectionLength > 0 Then tb.Cut()
            Case CopyToolStripMenuItem.Name, CopyToolStripMenuItem1.Name : If tb.SelectionLength > 0 Then tb.Copy()
            Case PasteToolStripMenuItem.Name, PasteToolStripMenuItem1.Name
                If Clipboard.GetDataObject.GetDataPresent(DataFormats.Text) Then
                    If tb.SelectionLength > 0 Then
                    End If
                    tb.Paste()
                End If
            Case SelectAllToolStripMenuItem.Name, SelectAllToolStripMenuItem1.Name : tb.SelectAll()
        End Select
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        Dim oAbout As New AboutBox1
        oAbout.ShowDialog()
    End Sub

#End Region

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadMetaTags()
        Dim fTags As String = System.AppDomain.CurrentDomain.BaseDirectory & "\" & "DVRMSTags.xml"
        If Not IO.File.Exists(fTags) Then
            IO.File.WriteAllText(fTags, My.Resources.DVRMSTags)
        End If
        Try
            Dim xmlDoc As XmlDocument = New XmlDocument()
            xmlDoc.Load(fTags)
            Dim xmlNode As XmlNode
            ReDim WMTags(xmlDoc.DocumentElement.ChildNodes.Count - 1)
            Dim ii As Integer = 0
            For Each xmlNode In xmlDoc.DocumentElement.ChildNodes
                If Not CType(xmlNode, XmlElement).HasAttribute("Name") Then Continue For
                If Not CType(xmlNode, XmlElement).HasAttribute("Type") Then Continue For

                Dim Name As String, Type As String, Value As String
                Dim oVal As Object, oType As MetadataItemType
                Name = CType(xmlNode, XmlElement).Attributes("Name").Value
                Type = CType(xmlNode, XmlElement).Attributes("Type").Value
                Value = CType(xmlNode, XmlElement).InnerText
                oType = [Enum].Parse(GetType(MetadataItemType), Type)
                Select Case oType
                    Case MetadataItemType.Binary : oVal = Encoding.ASCII.GetBytes(Value)
                    Case MetadataItemType.Boolean : oVal = CBool(Value)
                    Case MetadataItemType.Dword : oVal = Int32.Parse(Value)
                    Case MetadataItemType.Guid : oVal = Value
                    Case MetadataItemType.String : oVal = Value
                    Case MetadataItemType.Word : oVal = Int16.Parse(Value)
                    Case MetadataItemType.Qword : oVal = Int64.Parse(Value)
                    Case Else : Continue For
                End Select
                Dim item As New MetadataItem(Name, oVal, oType)
                WMTags(ii) = item : ii += 1
            Next
            xmlDoc = Nothing
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitUI()
        Try
            Dim sfGenre As String = AppDomain.CurrentDomain.BaseDirectory & "genre.txt"
            If IO.File.Exists(sfGenre) Then
                Dim sGenre As String = IO.File.ReadAllText(sfGenre)
                Dim genres() As String = sGenre.Replace(Environment.NewLine, "|").Split("|"c)
                Me.CheckedListBox2.Items.AddRange(genres)
            Else
                Me.CheckedListBox2.Items.Add("Action")
            End If
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub XMLDVRMSFiles()
        If SaveFolder Is Nothing Then
            If SaveFiles() Is Nothing Then Return
        End If
        Me.ofdDVRMS.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        If Me.ofdDVRMS.ShowDialog <> Windows.Forms.DialogResult.OK Then Return
        Dim sfname() As String = Me.ofdDVRMS.FileNames
        For Each fname As String In sfname
            Using editor As New DvrmsMetadataEditor(fname)
                WriteXMLMetaData(editor.GetAttributes, IO.Path.GetFileNameWithoutExtension(fname))
            End Using
        Next
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Function GetDVDProfilerXMLFile() As String
        Dim ofd As New OpenFileDialog
        If My.Settings.XMLDir = "~" Then
            ofd.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Else
            ofd.InitialDirectory = My.Settings.XMLDir
        End If
        ofd.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
        ofd.FilterIndex = 0
        ofd.RestoreDirectory = True
        ofd.DefaultExt = "xml"
        ofd.CheckFileExists = True
        ofd.FileName = Nothing
        If ofd.ShowDialog <> Windows.Forms.DialogResult.OK Then Return Nothing
        My.Settings.XMLDir = IO.Path.GetDirectoryName(ofd.FileName)
        Return ofd.FileName
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="DVRMS"></param>
    ''' <remarks></remarks>
    Private Sub RefreshEditData(ByVal DVRMS As DVRMSItem)
        If DVRMS.meta Is Nothing Then Exit Sub
        Me.TextBox7.Text = DVRMS.Title
        If String.IsNullOrEmpty(Me.TextBox7.Text) Then
            Me.TextBox7.Text = DVRMS.ToString
        End If
        Me.ComboBox1.Text = DVRMS.ParentalRating
        For ii As Integer = 0 To Me.CheckedListBox2.Items.Count - 1
            Me.CheckedListBox2.SetItemChecked(ii, False)
        Next
        Me.TextBox6.Text = ""
        Dim genres() As String = DVRMS.Genres.Split(","c)
        For Each genre As String In genres
            genre = genre.Trim
            If genre.ToUpper Like "MOVIE*" Then Continue For
            If Me.CheckedListBox2.Items.Contains(genre) Then
                Dim ii As Integer = Me.CheckedListBox2.Items.IndexOf(genre)
                Me.CheckedListBox2.SetItemChecked(ii, True)
            Else
                Dim str As String = Me.TextBox6.Text
                str += genre + Environment.NewLine
                Me.TextBox6.Text = str
            End If
        Next
        Me.CheckBox1.Checked = DVRMS.IsMovie
        Dim MediaCredits As String = DVRMS.Credits
        Dim persona As String = MediaCredits + ";;;;;"
        Dim credits() As String = persona.Split(";"c)
        Me.TextBox3.Text = credits(0)
        Me.TextBox4.Text = credits(1)
        Me.TextBox5.Text = credits(3)
        Me.TextBox1.Text = DVRMS.Subtitle
        Me.TextBox2.Text = DVRMS.Synopsis
        'bIsDirty = False
        'DVRMS.Save()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateFromDVDProfilerInfo()
        Dim DVDProfilerXMLFile As String = GetDVDProfilerXMLFile()
        If DVDProfilerXMLFile Is Nothing Then Return
        Me.ToolStripProgressBar1.Value = 0
        Me.ToolStripProgressBar1.Maximum = Me.CheckedListBox1.Items.Count
        Dim ds As New DataSet
        ds.ReadXml(DVDProfilerXMLFile)
        Dim checked() As Integer = {}, unchecked() As Integer = {}
        For Each clbi As CheckedListBoxItem In Me.CheckedListBox1.Items
            Dim dvrms As DVRMSItem = clbi.item
            Dim ii As Integer = Me.ToolStripProgressBar1.Value
            Me.ToolStripProgressBar1.Value += 1
            Dim dvds() As DataRow = {}
            Dim sTitle As String = dvrms.ToString.Replace("'", "&apos;")
            dvds = ds.Tables("DVD").Select(String.Format("Title='{0}'", sTitle))
            If dvds.Length = 0 Then
                If Not String.IsNullOrEmpty(dvrms.Title) Then
                    sTitle = dvrms.Title.Replace("'", "''")
                    dvds = ds.Tables("DVD").Select(String.Format("Title='{0}'", sTitle))
                End If
                If dvds.Length = 0 Then
                    If Not String.IsNullOrEmpty(dvrms.Title) Then
                        sTitle = dvrms.Title.Replace("'", "''")
                        dvds = ds.Tables("DVD").Select(String.Format("SortTitle='{0}'", sTitle))
                    End If
                    If dvds.Length = 0 Then
                        ReDim Preserve unchecked(unchecked.Length)
                        unchecked(unchecked.Length - 1) = ii
                        Continue For
                    End If
                End If
            End If
            Dim sRlsd As String, sPrdYr As String, sCountryOfOrigin As String
            Dim sRating As String, sDuration As String, sOverview As String
            Dim dtRlsd As DateTime, yrRlsd As String = Nothing, rInt As Integer
            For Each dvd As DataRow In dvds
                ReDim Preserve checked(checked.Length)
                checked(checked.Length - 1) = ii
                Dim lsTitle As String = CStr(dvd("Title"))
                sRlsd = dvd("Released")
                If DateTime.TryParse(sRlsd, dtRlsd) Then yrRlsd = dtRlsd.Year.ToString
                sPrdYr = dvd("ProductionYear")
                If String.IsNullOrEmpty(sPrdYr) Then sPrdYr = yrRlsd
                sCountryOfOrigin = dvd("CountryOfOrigin")
                sRating = dvd("Rating")
                sDuration = dvd("RunningTime")
                sOverview = dvd("Overview")

                Dim genre() As DataRow = {}
                Dim studio() As DataRow = {}
                Dim actor() As DataRow = {}
                Dim credit() As DataRow = {}

                Dim Genres() As DataRow = dvd.GetChildRows("DVD_Genres")
                If Genres.Length > 0 Then
                    genre = Genres(0).GetChildRows("Genres_Genre")
                End If
                Dim Studios() As DataRow = dvd.GetChildRows("DVD_Studios")
                If Studios.Length > 0 Then
                    studio = Studios(0).GetChildRows("Studios_Studio")
                End If
                Dim Actors() As DataRow = dvd.GetChildRows("DVD_Actors")
                If Actors.Length > 0 Then
                    actor = Actors(0).GetChildRows("Actors_Actor")
                End If
                Dim Credits() As DataRow = dvd.GetChildRows("DVD_Credits")
                If Credits.Length > 0 Then
                    credit = Credits(0).GetChildRows("Credits_Credit")
                End If

                If String.IsNullOrEmpty(dvrms.Title) Then dvrms.Title = dvrms.ToString
                dvrms.ParentalRating = sRating
                If String.IsNullOrEmpty(dvrms.Synopsis) Then dvrms.Synopsis = sOverview
                dvrms.ReleaseDate = New Date(sPrdYr, 1, 1)
                If Int32.TryParse(sDuration, rInt) Then dvrms.RunningTime = rInt
                If studio.Length > 0 Then
                    dvrms.Station = studio(0)(0)
                End If
                'dvrms.Genre.Clear()
                If genre.Length > 0 Then
                    For Each g As DataRow In genre
                        Dim sGenre As String = CStr(g(0)).Replace("-"c, " "c)
                        If Not String.IsNullOrEmpty(sGenre.Trim) Then
                            dvrms.AddGenre(sGenre)
                        End If
                    Next
                End If
                'dvrms.Actors.Clear()
                If actor.Length > 0 Then
                    For Each a As DataRow In actor
                        Dim sActor As String = a("FirstName") + " " + a("LastName")
                        dvrms.AddActor(sActor)
                    Next
                End If
                'dvrms.Directors.Clear()
                If credit.Length > 0 Then
                    For Each d As DataRow In credit
                        If d("CreditType") = "Direction" Then
                            Dim sActor As String = d("FirstName") + " " + d("LastName")
                            dvrms.AddDirector(sActor)
                        End If
                    Next

                End If
                Exit For
            Next
        Next
        ds.Dispose()
        xmlDVDProfiler = Nothing
        Me.ToolStripProgressBar1.Value = 0
        Me.ToolStripProgressBar1.Maximum = checked.Length
        For ii As Integer = 0 To checked.Length - 1
            Me.ToolStripProgressBar1.Value += 1
            Dim dvrms As DVRMSItem = CType(Me.CheckedListBox1.Items(ii), CheckedListBoxItem).item
            dvrms.Save()
            Me.CheckedListBox1.SetItemCheckState(checked(ii), CheckState.Checked)
        Next
        For ii As Integer = 0 To unchecked.Length - 1
            Me.CheckedListBox1.SetItemCheckState(unchecked(ii), CheckState.Unchecked)
        Next
        bIsDirty = False
        Me.CheckedListBox1.SetSelected(0, True)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub XMLUpdateFromDVDProfilerInfo()
        Dim DVDProfilerXMLFile As String = GetDVDProfilerXMLFile()
        If DVDProfilerXMLFile Is Nothing Then Return
        Me.ToolStripProgressBar1.Value = 0
        Me.ToolStripProgressBar1.Maximum = Me.CheckedListBox1.Items.Count
        xmlDVDProfiler = New XmlDocument
        xmlDVDProfiler.Load(DVDProfilerXMLFile)
        Dim oNode As XmlNode
        Const xNode As String = "//Collection/DVD[Title='{0}']"
        Dim ActorAttrs() As String = {"FirstName", "LastName", "Role"}
        Dim CrewAttrs() As String = {"FirstName", "LastName", "CreditType"}
        Dim checked() As Integer = {}, unchecked() As Integer = {}
        'Dim ListItems As CheckedListBox.ObjectCollection = Me.CheckedListBox1.Items
        For Each clbi As CheckedListBoxItem In Me.CheckedListBox1.Items
            Dim dvrms As DVRMSItem = clbi.item
            Dim ii As Integer = Me.ToolStripProgressBar1.Value
            Me.ToolStripProgressBar1.Value += 1
            Try
                Dim sTitle As String = dvrms.ToString.Replace("'", "&apos;")
                oNode = xmlDVDProfiler.SelectSingleNode(String.Format(xNode, sTitle))
                If oNode Is Nothing AndAlso Not String.IsNullOrEmpty(dvrms.Title) Then
                    oNode = xmlDVDProfiler.SelectSingleNode(String.Format(xNode, dvrms.Title))
                End If
            Catch ex As Exception
                ReDim Preserve unchecked(unchecked.Length)
                unchecked(unchecked.Length - 1) = ii
                oNode = Nothing
            End Try
            If oNode Is Nothing Then Continue For
            ReDim Preserve checked(checked.Length)
            checked(checked.Length - 1) = ii
            Dim sRlsd As String = NodeInnerText(oNode, "Released")
            Dim dtRlsd As DateTime, yrRlsd As String = Nothing, rInt As Integer
            If DateTime.TryParse(sRlsd, dtRlsd) Then yrRlsd = dtRlsd.Year.ToString
            Dim sPrdYr As String = NodeInnerText(oNode, "ProductionYear", yrRlsd)
            Dim sCountryOfOrigin As String = NodeInnerText(oNode, "CountryOfOrigin", "")
            Dim sRating As String = NodeInnerText(oNode, "Rating", "--")
            Dim sDuration As String = NodeInnerText(oNode, "RunningTime", "0")
            Dim sOverview As String = NodeInnerText(oNode, "Overview", "")
            Dim sGenre() As String = NodeInnerList(oNode, "Genres")
            Dim sActor() As String = NodeInnerList(oNode, "Actors", ActorAttrs)
            Dim sCrew() As String = NodeInnerList(oNode, "Credits", CrewAttrs)
            Dim sStudio() As String = NodeInnerList(oNode, "Studios")
            oNode = Nothing

            If String.IsNullOrEmpty(dvrms.Title) Then dvrms.Title = dvrms.ToString
            dvrms.ParentalRating = sRating
            If String.IsNullOrEmpty(dvrms.Synopsis) Then dvrms.Synopsis = sOverview
            dvrms.ReleaseDate = New Date(sPrdYr, 1, 1)
            If Int32.TryParse(sDuration, rInt) Then dvrms.RunningTime = rInt
            If sStudio IsNot Nothing Then
                dvrms.Station = sStudio(0)
            End If
            If sGenre IsNot Nothing Then
                For Each g As String In sGenre
                    If Not String.IsNullOrEmpty(g.Trim) Then
                        dvrms.AddGenre(g)
                    End If
                Next
            End If
            If sActor IsNot Nothing Then
                For Each a As String In sActor
                    Dim n() As String = a.Split("|")
                    dvrms.AddActor(n(0) + " " + n(1))
                Next
            End If
            If sCrew IsNot Nothing Then
                For Each d As String In sCrew
                    Dim n() As String = d.Split("|")
                    If n(2) = "Direction" Then
                        dvrms.AddDirector(n(0) + " " + n(1))
                    End If
                Next

            End If
            dvrms.Save()
        Next
        xmlDVDProfiler = Nothing
        For ii As Integer = 0 To checked.Length - 1
            Me.CheckedListBox1.SetItemCheckState(checked(ii), CheckState.Checked)
        Next
        For ii As Integer = 0 To unchecked.Length - 1
            Me.CheckedListBox1.SetItemCheckState(unchecked(ii), CheckState.Unchecked)
        Next
        bIsDirty = False
        Me.CheckedListBox1.SetSelected(0, True)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="meta"></param>
    ''' <param name="sf"></param>
    ''' <remarks></remarks>
    Private Sub WriteXMLMetaData(ByVal meta As IDictionary, ByVal sf As String)
        Dim inWM As Boolean = False
        Dim settings As New XmlWriterSettings
        settings.Indent = True
        settings.OmitXmlDeclaration = True
        Using writer As XmlWriter = XmlWriter.Create(SaveFolder & "\" & sf & ".xml", settings)
            writer.WriteStartDocument(True)
            writer.WriteStartElement("dvrms")
            Dim xs As SortedList = New SortedList(meta)
            For Each name As String In xs.Keys
                Dim item As MetadataItem = meta(name)
                If item.Name.StartsWith("WM/") Then
                    If Not inWM Then
                        writer.WriteStartElement("WM")
                        inWM = True
                    End If
                    name = item.Name.Substring(3)
                ElseIf inWM Then
                    writer.WriteEndElement()
                    inWM = False
                End If
                writer.WriteStartElement("item")
                writer.WriteAttributeString("name", name)
                writer.WriteAttributeString("type", item.Type.ToString)
                writer.WriteString(item.Value.ToString)
                writer.WriteEndElement()
            Next
            If inWM Then
                writer.WriteEndElement()
            End If
            writer.WriteEndElement()
            writer.WriteEndDocument()
            writer.Close()
        End Using
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function SaveFiles() As String
        If SaveFolder IsNot Nothing Then
            Me.fbdSave.SelectedPath = SaveFolder
        Else
            Me.fbdSave.SelectedPath = Environment.SpecialFolder.MyDocuments
        End If
        If Me.fbdSave.ShowDialog = Windows.Forms.DialogResult.OK Then
            SaveFolder = Me.fbdSave.SelectedPath
        End If
        Return SaveFolder
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadDVRMSFiles()
        Me.ofdDVRMS.InitialDirectory = My.Settings.RootDir
        If Me.ofdDVRMS.ShowDialog <> Windows.Forms.DialogResult.OK Then Return
        Dim sfname() As String = Me.ofdDVRMS.FileNames
        If sfname.Length = 0 Then Return
        My.Settings.RootDir = IO.Path.GetDirectoryName(sfname(0))
        Me.CheckedListBox1.Items.Clear()
        For Each fname As String In sfname
            FileMetaAttributes(fname)
            Dim dvrms As New DVRMSItem(fname)
            Dim clbi As New CheckedListBoxItem(dvrms, dvrms.ToString, CheckState.Indeterminate)
            Me.CheckedListBox1.Items.Add(clbi, clbi.state)
        Next
        'Me.DVRMS = Me.CheckedListBox1.Items(0)
        Me.bIsDirty = False
        Me.CheckedListBox1.SetSelected(0, True)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="fName"></param>
    ''' <remarks></remarks>
    Private Sub FileMetaAttributes(ByVal fName As String)
        Using editor As New DvrmsMetadataEditor(fName)
            Dim meta As IDictionary = editor.GetAttributes
            For Each item As MetadataItem In WMTags
                If Not meta.Contains(item.Name) Then
                    Dim i As MetadataItem = item.Clone
                    meta.Add(i.Name, i)
                End If
            Next
            Try
                editor.SetAttributes(meta)
            Catch ex As Exception

            End Try
        End Using
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="DVRMS"></param>
    ''' <remarks></remarks>
    Private Sub SaveMetaAttributes(ByVal DVRMS As DVRMSItem)
        DVRMS.Title = Me.TextBox7.Text
        DVRMS.IsMovie = Me.CheckBox1.Checked
        Dim sCredits As String = Me.TextBox3.Text + ";" + Me.TextBox4.Text + ";;" + Me.TextBox5.Text
        DVRMS.Credits = sCredits
        DVRMS.Subtitle = Me.TextBox1.Text
        DVRMS.Synopsis = Me.TextBox2.Text
        DVRMS.ParentalRating = Me.ComboBox1.Text
        Dim checkedItems As CheckedListBox.CheckedItemCollection = Me.CheckedListBox2.CheckedItems
        DVRMS.ClearGenre()
        For Each ci As String In checkedItems
            DVRMS.AddGenre(ci)
        Next
        Dim sGenre() As String = Me.TextBox6.Text.Split(System.Environment.NewLine)
        For Each ci As String In sGenre
            DVRMS.AddGenre(ci)
        Next
        DVRMS.Save()
        bIsDirty = False
        Me.CheckedListBox1.SetItemCheckState(Me.CheckedListBox1.SelectedIndex, CheckState.Checked)
    End Sub

    Private Sub SaveDVRMSFiles()
        My.Settings.Save()
        If bIsDirty Then
            Dim obj As CheckedListBoxItem = Me.CheckedListBox1.SelectedItem
            If obj IsNot Nothing Then _
                Me.SaveMetaAttributes(obj.item)
        End If
        For Each clbi As CheckedListBoxItem In Me.CheckedListBox1.Items
            CType(clbi.item, DVRMSItem).Save()
        Next
    End Sub

#Region "Node Parsing"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="oNode"></param>
    ''' <param name="xPath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function NodeInnerText(ByVal oNode As XmlNode, ByVal xPath As String) As String
        Return NodeInnerText(oNode, xPath, Nothing)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="oNode"></param>
    ''' <param name="xPath"></param>
    ''' <param name="sDefault"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function NodeInnerText(ByVal oNode As XmlNode, ByVal xPath As String, ByVal sDefault As String) As String
        Dim aNode As XmlNode = oNode.SelectSingleNode("./" + xPath)
        If aNode IsNot Nothing Then Return CType(aNode, XmlElement).InnerText
        Return sDefault
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="oNode"></param>
    ''' <param name="xPath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function NodeInnerList(ByVal oNode As XmlNode, ByVal xPath As String) As String()
        Dim aNode As XmlNode = oNode.SelectSingleNode("./" + xPath)
        If aNode IsNot Nothing Then
            If Not aNode.HasChildNodes Then Return Nothing
            Dim rslt(aNode.ChildNodes.Count - 1) As String, ii As Integer = 0
            For Each cNode As XmlNode In aNode.ChildNodes
                rslt(ii) = cNode.InnerText : ii += 1
            Next
            Return rslt
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="oNode"></param>
    ''' <param name="xPath"></param>
    ''' <param name="xAttr"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function NodeInnerList(ByVal oNode As XmlNode, ByVal xPath As String, ByVal xAttr() As String) As String()
        Dim aNode As XmlNode = oNode.SelectSingleNode("./" + xPath)
        If aNode IsNot Nothing Then
            If Not aNode.HasChildNodes Then Return Nothing
            Dim rslt(aNode.ChildNodes.Count - 1) As String, ii As Integer = 0
            For Each cNode As XmlNode In aNode.ChildNodes
                If CType(cNode, XmlElement).HasAttributes Then
                    For Each attr As String In xAttr
                        If CType(cNode, XmlElement).HasAttribute(attr) Then
                            rslt(ii) += cNode.Attributes(attr).Value
                        End If
                        rslt(ii) += "|"
                    Next
                    ii += 1
                Else
                    Return Nothing
                End If
            Next
            Return rslt
        End If
        Return Nothing
    End Function

#End Region

    Private Sub CheckedListBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckedListBox2.SelectedIndexChanged

    End Sub
End Class
