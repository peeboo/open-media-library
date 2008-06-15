#Region "Import Declaratives"

Imports System
Imports Toub.MediaCenter.Dvrms.Metadata

#End Region

Public Class DVRMSItem
    Implements IDisposable

#Region "Private Variables"

    Private _filename As String
    Private _meta As IDictionary = Nothing
    Private _IsDirty As Boolean = False
    Private _Genre As New List(Of String)
    Private _Credits As New List(Of String)
    Private _Actors As New List(Of String)
    Private _Directors As New List(Of String)
    Private _Other As New List(Of String)
    Private _Crew As New List(Of String)

#End Region

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal filename As String)
        _filename = filename
        Using _editor As New DvrmsMetadataEditor(filename)
            _meta = _editor.GetAttributes
        End Using
        Dim genre As String = Genres
        If Not String.IsNullOrEmpty(genre) Then _Genre.AddRange(genre.Split(","))
        Dim cast As String = Credits + ";;;;"
        _Credits.AddRange(cast.Split(";"c))
        If _Credits.Count > 4 Then _Credits.RemoveRange(4, _Credits.Count - 4)
        If Not String.IsNullOrEmpty(_Credits(0)) Then _Actors.AddRange(_Credits(0).Split("/"c))
        If Not String.IsNullOrEmpty(_Credits(1)) Then _Directors.AddRange(_Credits(1).Split("/"c))
        If Not String.IsNullOrEmpty(_Credits(2)) Then _Other.AddRange(_Credits(2).Split("/"c))
        If Not String.IsNullOrEmpty(_Credits(3)) Then _Crew.AddRange(_Credits(3).Split("/"c))
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function ToString() As String
        Return IO.Path.GetFileNameWithoutExtension(_filename)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Save()
        If _IsDirty Then
            Try
                Using _editor As New DvrmsMetadataEditor(filename)
                    _editor.SetAttributes(_meta)
                End Using
                _IsDirty = False
            Catch ex As Exception

            End Try
        End If
    End Sub

#Region "Properties"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsDirty() As Boolean
        Get
            Return _IsDirty
        End Get
        Set(ByVal value As Boolean)
            _IsDirty = value
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property filename() As String
        Get
            Return _filename
        End Get
        Set(ByVal value As String)
            _filename = value
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsMovie() As Boolean
        Get
            Return GetMetaData("WM/MediaIsMovie")
        End Get
        Set(ByVal value As Boolean)
            SetMetaData("WM/MediaIsMovie", value, MetadataItemType.Boolean)
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ReleaseDate() As DateTime
        Get
            Dim dt As DateTime, sDT As String = GetMetaData(DvrmsMetadataEditor.MediaOriginalBroadcastDateTime)
            If DateTime.TryParse(sDT, dt) Then
                Return dt
            Else
                Return DateTime.MinValue
            End If
        End Get
        Set(ByVal value As DateTime)
            Dim sDT As String = value.ToString("yyyy-MM-ddThh:mm:ssZ")
            SetMetaData(DvrmsMetadataEditor.MediaOriginalBroadcastDateTime, sDT, MetadataItemType.String)
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RunningTime() As Integer
        Get
            Dim runtime As Long = GetMetaData(DvrmsMetadataEditor.Duration)
            runtime = CLng(runtime / (60 * 10000000))
            Return runtime
        End Get
        Set(ByVal value As Integer)
            Dim runtime As Long = value
            runtime = runtime * 60 * 10000000
            SetMetaData(DvrmsMetadataEditor.Duration, runtime, MetadataItemType.Qword)
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Station() As String
        Get
            Return GetMetaData(DvrmsMetadataEditor.StationName)
        End Get
        Set(ByVal value As String)
            SetMetaData(DvrmsMetadataEditor.StationName, value, MetadataItemType.String)
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Synopsis() As String
        Get
            Return GetMetaData(DvrmsMetadataEditor.SubtitleDescription)
        End Get
        Set(ByVal value As String)
            SetMetaData(DvrmsMetadataEditor.SubtitleDescription, value, MetadataItemType.String)
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ParentalRating() As String
        Get
            Return GetMetaData(DvrmsMetadataEditor.ParentalRating, "--")
        End Get
        Set(ByVal value As String)
            SetMetaData(DvrmsMetadataEditor.ParentalRating, value, MetadataItemType.String)
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Subtitle() As String
        Get
            Return GetMetaData(DvrmsMetadataEditor.Subtitle)
        End Get
        Set(ByVal value As String)
            SetMetaData(DvrmsMetadataEditor.Subtitle, value, MetadataItemType.String)
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Title() As String
        Get
            Return GetMetaData(DvrmsMetadataEditor.Title)
        End Get
        Set(ByVal value As String)
            SetMetaData(DvrmsMetadataEditor.Title, value, MetadataItemType.String)
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Genres() As String
        Get
            Return GetMetaData(DvrmsMetadataEditor.Genre, "")
        End Get
        Set(ByVal value As String)
            SetMetaData(DvrmsMetadataEditor.Genre, value, MetadataItemType.String)
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Credits() As String
        Get
            Return GetMetaData(DvrmsMetadataEditor.Credits, "")
        End Get
        Set(ByVal value As String)
            SetMetaData(DvrmsMetadataEditor.Credits, value, MetadataItemType.String)
        End Set
    End Property

#End Region

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Genre() As IList(Of String)
        Get
            Return _Genre
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Actors() As IList(Of String)
        Get
            Return _Actors
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Directors() As IList(Of String)
        Get
            Return _Directors
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Other() As IList(Of String)
        Get
            Return _Other
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Crew() As IList(Of String)
        Get
            Return _Crew
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="theList"></param>
    ''' <param name="separator"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ArrayToString(ByVal theList As List(Of String), ByVal separator As String) As String
        Dim sList As String = ""
        For Each a As String In theList
            sList += a + separator
        Next
        Return sList.TrimEnd(separator.ToCharArray)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Actor"></param>
    ''' <remarks></remarks>
    Public Sub AddActor(ByVal Actor As String)
        If _Actors.Contains(Actor) Then Exit Sub
        IsDirty = True
        _Actors.Add(Actor)
        _Credits(0) = ArrayToString(_Actors, "/")
        Credits = ArrayToString(_Credits, ";")
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Director"></param>
    ''' <remarks></remarks>
    Public Sub AddDirector(ByVal Director As String)
        If _Directors.Contains(Director) Then Exit Sub
        IsDirty = True
        _Directors.Add(Director)
        _Credits(1) = ArrayToString(_Directors, "/")
        Credits = ArrayToString(_Credits, ";")
    End Sub

    Public Sub ClearGenre()
        IsDirty = True
        _Genre.Clear()
        Genres = ArrayToString(_Genre, ",")
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Genre"></param>
    ''' <remarks></remarks>
    Public Sub AddGenre(ByVal Genre As String)
        If String.IsNullOrEmpty(Genre) Then Exit Sub
        If _Genre.Contains(Genre) Then Exit Sub
        IsDirty = True
        _Genre.Add(Genre)
        Genres = ArrayToString(_Genre, ",")
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="key"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetMetaData(ByVal key As String) As Object
        Return GetMetaData(key, Nothing)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="key"></param>
    ''' <param name="default"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetMetaData(ByVal key As String, ByVal [default] As Object) As Object
        If Not _meta.Contains(key) Then Return [default]
        Dim item As MetadataItem = _meta(key)
        If item IsNot Nothing Then
            If String.IsNullOrEmpty(CStr(item.Value)) Then
                Return [default]
            Else
                Return item.Value
            End If
        End If
        Return [default]
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="key"></param>
    ''' <param name="value"></param>
    ''' <param name="type"></param>
    ''' <remarks></remarks>
    Private Sub SetMetaData(ByVal key As String, ByVal value As Object, ByVal type As MetadataItemType)
        _IsDirty = True
        If Not _meta.Contains(key) Then
            Dim item As MetadataItem = New MetadataItem(key, value, type)
            _meta.Add(item.Name, item)
        Else
            CType(_meta(key), MetadataItem).Value = value
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property meta() As IDictionary
        Get
            If _meta Is Nothing Then
                Using _editor As New DvrmsMetadataEditor(filename)
                    _meta = _editor.GetAttributes
                End Using
            End If
            Return _meta
        End Get
        Set(ByVal value As IDictionary)
            _meta = value
        End Set
    End Property

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free managed resources when explicitly called
                _meta = Nothing
                _Genre = Nothing
                _Credits = Nothing
                _Actors = Nothing
                _Directors = Nothing
                _Other = Nothing
                _Crew = Nothing
            End If

            ' TODO: free shared unmanaged resources

        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
