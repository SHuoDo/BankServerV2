Public Class Test
    Private _name As String
    Public Property Name As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property
    Private _population As Integer
    Public Property Population As Integer
        Get
            Return _population
        End Get
        Set(ByVal value As Integer)
            _population = value
        End Set
    End Property
End Class
