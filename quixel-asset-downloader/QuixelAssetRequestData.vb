Namespace QuixelAssets
    Public Class QuixelAssetRequestData
        Property asset As String
        Property components As New List(Of AssetComponent)
        Property config As New AssetConfig
    End Class

    Public Class AssetComponent
        Property type As String
        Property mimeType As String
        Property resolution As String
    End Class

    Public Class AssetConfig
        Property highpoly As Boolean
        Property ztool As Boolean
        Property lowerlod_normals As Boolean
        Property albedo_lods As Boolean
        Property meshMimeType As String
        Property brushes As Boolean
        Property lods As New List(Of Integer)
    End Class

End Namespace
