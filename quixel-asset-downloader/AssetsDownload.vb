Namespace AssetDownloads
    Public Class Rootobject
        Public Property user As String
        Public Property asset As String
        Public Property components As List(Of Component)
        Public Property config As Config
        Public Property license As String
        Public Property paying As Boolean
        Public Property meshes As List(Of Mesh)
        Public Property json As Json
        Public Property previews As List(Of Preview)
        Public Property time As Date
        Public Property expires As Date
        Public Property id As String
    End Class

    Public Class Config
        Public Property highpoly As Boolean
        Public Property ztool As Boolean
        Public Property lowerlod_normals As Boolean
        Public Property albedo_lods As Boolean
        Public Property meshMimeType As String
        Public Property brushes As Boolean
        Public Property lods As List(Of Integer)
        Public Property lowerlod_meshes As Boolean
        Public Property maxlod As Integer
    End Class

    Public Class Json
        Public Property uri As String
        Public Property contentLength As Integer
    End Class

    Public Class Component
        Public Property type As String
        Public Property mimeType As String
        Public Property resolution As String
        Public Property physicalSize As String
        Public Property uri As String
        Public Property contentLength As Integer
    End Class

    Public Class Mesh
        Public Property uri As String
        Public Property contentLength As Integer
    End Class

    Public Class Preview
        Public Property uri As String
        Public Property tags As List(Of String)
        Public Property resolution As String
        Public Property contentLength As Integer
    End Class
End Namespace
