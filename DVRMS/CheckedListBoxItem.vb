Public Class CheckedListBoxItem
    Private _state As CheckState = CheckState.Indeterminate
    Private _item As Object
    Private _description As String

    Public Sub New()
        Me.New(Nothing)
    End Sub

    Public Sub New(ByVal description As String)
        Me.New(description, description)
    End Sub

    Public Sub New(ByVal item As Object, ByVal description As String)
        Me.New(item, description, CheckState.Indeterminate)
    End Sub

    Public Sub New(ByVal item As Object, ByVal description As String, ByVal state As CheckState)
        Me.state = state
        Me.item = item
        Me.description = description
    End Sub

    Public Overridable Property item() As Object
        Get
            Return _item
        End Get
        Set(ByVal value As Object)
            _item = value
        End Set
    End Property

    Public Property description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Property state() As CheckState
        Get
            Return _state
        End Get
        Set(ByVal value As CheckState)
            _state = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return description
    End Function

End Class
