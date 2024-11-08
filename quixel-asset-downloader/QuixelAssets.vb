Namespace QuixelAssets
    Public Class QuixelAssetsObject
        Public Property assets As List(Of Asset)
        Public Property page As Integer
        Public Property pages As Integer
        Public Property total As Integer
        Public Property count As Integer
        Public Property facets As Facets
    End Class

    Public Class Facets
        Public Property type As Type
        Public Property biome As Biome
        Public Property region As Region
        Public Property category As Category
        Public Property subCategory As Subcategory
    End Class

    Public Class Type
        Public Property surface As Integer
        Public Property atlas As Integer
        Public Property _3d As Integer
        Public Property brush As Integer
        Public Property _3dplant As Integer
    End Class

    Public Class Biome
        Public Property undefined As Integer
        Public Property mediterraneanforest As Integer
        Public Property urban As Integer
        Public Property none As Integer
        Public Property rural As Integer
        Public Property forestbiome As Integer
        Public Property temperateforest As Integer
        Public Property desertbiome As Integer
        Public Property oceanbiome As Integer
        Public Property industrial As Integer
        Public Property tundrabiome As Integer
        Public Property ocean As Integer
        Public Property tundra As Integer
        Public Property semiariddesert As Integer
        Public Property grasslandbiome As Integer
        Public Property forest As Integer
        Public Property tropicaljunglebiome As Integer
        Public Property grassland As Integer
        Public Property medieval As Integer
        Public Property savannagrassland As Integer
        Public Property europe As Integer
        Public Property ancient As Integer
        Public Property jungle As Integer
        Public Property custom As Integer
        Public Property taigabiome As Integer
        Public Property gothenburg As Integer
        Public Property asia As Integer
        Public Property nordicforest As Integer
        Public Property tropicalbiome As Integer
        Public Property taiga As Integer
    End Class

    Public Class Region
        Public Property _2 As Integer
        Public Property Europe As Integer
        Public Property undefined As Integer
        Public Property Asia As Integer
        Public Property southwestaustralia As Integer
        Public Property Oceania As Integer
        Public Property middleeast As Integer
        Public Property Americas As Integer
        Public Property scandinavia As Integer
        Public Property none As Integer
        Public Property Africa As Integer
        Public Property southeastasia As Integer
        Public Property _Global As Integer
        Public Property iceland As Integer
        Public Property Sweden As Integer
        Public Property europeanunion As Integer
        Public Property Rome As Integer
        Public Property easterneurope As Integer
        Public Property austrailia As Integer
        Public Property southamerica As Integer
        Public Property pakistan As Integer
        Public Property Cambodia As Integer
        Public Property centralamerica As Integer
        Public Property thecarribbean As Integer
        Public Property UnitedKingdom As Integer
        Public Property australia As Integer
        Public Property strassa As Integer
        Public Property custom As Integer
        Public Property northamerica As Integer
    End Class

    Public Class Category
        Public Property decals As Integer
        Public Property rock As Integer
        Public Property plants As Integer
        Public Property nature As Integer
        Public Property concrete As Integer
        Public Property tree As Integer
        Public Property stone As Integer
        Public Property wood As Integer
        Public Property grass As Integer
        Public Property debris As Integer
        Public Property soil As Integer
        Public Property plant As Integer
        Public Property imperfections As Integer
        Public Property ground As Integer
        Public Property brick As Integer
        Public Property scatter As Integer
        Public Property floors As Integer
        Public Property fabric As Integer
        Public Property metal As Integer
        Public Property building As Integer
        Public Property props As Integer
        Public Property sand As Integer
        Public Property other As Integer
        Public Property asphalt As Integer
        Public Property street As Integer
        Public Property paintbrushes As Integer
        Public Property edible As Integer
        Public Property wall As Integer
        Public Property industrial As Integer
        Public Property misc As Integer
    End Class

    Public Class Subcategory
        Public Property rough As Integer
        Public Property misc As Integer
        Public Property rock As Integer
        Public Property perennials As Integer
        Public Property leaf As Integer
        Public Property branch As Integer
        Public Property wild As Integer
        Public Property nature As Integer
        Public Property wall As Integer
        Public Property smooth As Integer
        Public Property stone As Integer
        Public Property dried As Integer
        Public Property mud As Integer
        Public Property other As Integer
        Public Property floor As Integer
        Public Property jungle As Integer
        Public Property tree As Integer
        Public Property weed As Integer
        Public Property damaged As Integer
        Public Property forest As Integer
        Public Property tiles As Integer
        Public Property granite As Integer
        Public Property tile As Integer
        Public Property _3d As Integer
        Public Property leaves As Integer
        Public Property dirty As Integer
        Public Property painted As Integer
        Public Property jagged As Integer
        Public Property seaweed As Integer
        Public Property wood As Integer
    End Class

    Public Class Asset
        Public Property acl As Acl
        Public Property tags As List(Of String)
        Public Property previews As Previews
        Public Property points As Integer
        Public Property categories As List(Of String)
        Public Property averageColor As String
        Public Property name As String
        Public Property physicalSize As String
        Public Property id As String
        Public Property extendedinfo As AssetExtended
        Public Property HasErrors As Boolean
        Public Property LastErrorMessage As String
    End Class

    Public Class Acl
        Public Property resolution As Integer
    End Class

    Public Class Previews
        Public Property images As List(Of Image)
        Public Property scaleReferences As List(Of Object)
        Public Property relativeSize As String
    End Class

    Public Class Image
        Public Property contentLength As Long
        Public Property resolution As String
        Public Property uri As String
        Public Property tags As List(Of String)
    End Class

    ' Extended set
    Public Class AssetExtended
        Public Property pack As Object
        Public Property comp_version As Integer
        Public Property semanticTags As Semantictags
        Public Property name As String
        Public Property tags As List(Of String)
        Public Property brushes As List(Of Object)
        Public Property previews As PreviewsExtended
        Public Property environment As EnvironmentExtended
        Public Property json As Json
        Public Property points As Integer
        Public Property meta As List(Of Meta)
        Public Property maps As List(Of Map1)
        Public Property categories As List(Of String)
        Public Property components As List(Of Component)
        Public Property references As List(Of Object)
        Public Property referencePreviews As Referencepreviews
        Public Property properties As List(Of Property1)
        Public Property averageColor As String
        Public Property meshes As List(Of Mesh)
        Public Property assetCategories As Assetcategories
        Public Property revision As Integer
        Public Property revised As Boolean
        Public Property uasset As List(Of Uasset)
        Public Property id As String
        Public Property physicalSize As String
        Public Property similarAssets As Object
    End Class

    Public Class Semantictags
        Public Property subject_matter As String
        Public Property name As String
        Public Property asset_type As String
        Public Property contains As List(Of String)
        Public Property theme As List(Of String)
        Public Property descriptive As List(Of String)
        Public Property environment As List(Of String)
        Public Property country As String
        Public Property region As String
        Public Property color As List(Of String)
        Public Property _3d_mesh As String
        Public Property locations As Locations
        Public Property maxSize As String
        Public Property minSize As String
    End Class

    Public Class Locations
        Public Property Americas As Americas
    End Class

    Public Class Americas
        Public Property UnitedStates As UnitedStates
    End Class

    Public Class UnitedStates
    End Class

    Public Class PreviewsExtended
        Public Property images As List(Of ImageExtended)
        Public Property relativeSize As String
    End Class

    Public Class ImageExtended
        Public Property contentLength As Long
        Public Property resolution As String
        Public Property uri As String
        Public Property tags As List(Of String)
    End Class

    Public Class EnvironmentExtended
        Public Property biome As String
        Public Property region As String
    End Class

    Public Class Json
        Public Property contentLength As Long
        Public Property uri As String
    End Class

    Public Class Referencepreviews
        Public Property models As List(Of Model)
        Public Property maps As List(Of Map)
    End Class

    Public Class Model
        Public Property mimeType As String
        Public Property contentLength As Long
        Public Property type As String
        Public Property uri As String
    End Class

    Public Class Map
        Public Property mimeType As String
        Public Property resolution As String
        Public Property contentLength As Long
        Public Property type As String
        Public Property uri As String
    End Class

    Public Class Map1
        Public Property mimeType As String
        Public Property minIntensity As Single
        Public Property bitDepth As Integer
        Public Property name As String
        Public Property resolution As String
        Public Property contentLength As Integer
        Public Property colorSpace As String
        Public Property uri As String
        Public Property physicalSize As String
        Public Property maxIntensity As Single
        Public Property type As String
        Public Property averageColor As String
    End Class

    Public Class Assetcategories
        Public Property _3Dasset As _3DAsset
    End Class

    Public Class _3DAsset
        Public Property nature As Nature
    End Class

    Public Class Nature
        Public Property rock As Rock
    End Class

    Public Class Rock
    End Class

    Public Class Meta
        Public Property key As String
        Public Property name As String
        Public Property value As Object
    End Class

    Public Class Component
        Public Property minIntensity As Double
        Public Property name As String
        Public Property colorSpace As String
        Public Property maxIntensity As Double
        Public Property type As String
        Public Property uris As List(Of Uri)
        Public Property averageColor As String
    End Class

    Public Class Uri
        Public Property physicalSize As String
        Public Property resolutions As List(Of Resolution)
    End Class

    Public Class Resolution
        Public Property resolution As String
        Public Property formats As List(Of Format)
    End Class

    Public Class Format
        Public Property mimeType As String
        Public Property bitDepth As Integer
        Public Property contentLength As Long
        Public Property uri As String
        Public Property lodType As String
        Public Property tris As Integer
    End Class

    Public Class Property1
        Public Property key As String
        Public Property value As String
    End Class

    Public Class Mesh
        Public Property type As String
        Public Property uris As List(Of Uri1)
        Public Property tris As Integer
    End Class

    Public Class Uri1
        Public Property mimeType As String
        Public Property contentLength As Long
        Public Property uri As String
    End Class

    Public Class Uasset
        Public Property type As String
        Public Property ueVersion As String
        Public Property tier As Integer
        Public Property mimeType As String
        Public Property uri As String
        Public Property subtype As String
    End Class

End Namespace
