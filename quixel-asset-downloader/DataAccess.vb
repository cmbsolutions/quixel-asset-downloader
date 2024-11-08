Imports Microsoft.Data.Sqlite
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Text
Imports SQLitePCL

Public Class DataAccess
    Private Shared ConnectionString As String = "Data Source=assets.sqlite3;"
    Private Shared DatabaseConnection As SqliteConnection

    Public Shared Function Database() As SqliteConnection
        Try

            If DatabaseConnection Is Nothing Then
                Batteries.Init()
                DatabaseConnection = New SqliteConnection(ConnectionString)
                DatabaseConnection.Open()
            Else
                Select Case DatabaseConnection.State
                    Case Data.ConnectionState.Closed, Data.ConnectionState.Broken
                        DatabaseConnection.Open()
                End Select
            End If

            Return DatabaseConnection
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

        Return Nothing
    End Function

    Private Shared Function RunInsert(command As String) As Long
        Using c As New SqliteCommand(command, Database)
            c.ExecuteNonQuery()
        End Using

        ' Retrieve the last inserted row's id
        Dim lastInsertIdCmd As String = "SELECT last_insert_rowid();"
        Dim lastInsertId As Long
        Using c As New SqliteCommand(lastInsertIdCmd, Database)
            lastInsertId = CLng(c.ExecuteScalar())
        End Using

        Return lastInsertId
    End Function

    Public Shared Function InsertFacets(data As QuixelAssets.Facets) As Integer
        Try
            Dim ids As New Dictionary(Of String, Integer)
            Dim sql As New StringBuilder
            Dim fields As New StringBuilder
            Dim values As New StringBuilder
            Dim insertId As Integer

            ' Insert Types
            sql.Clear()
            fields.Clear()
            values.Clear()

            For Each prop As PropertyInfo In data.type.GetType.GetProperties
                fields.Append($"[{prop.Name}],")
                values.Append($"{prop.GetValue(data.type)},")
            Next

            fields = fields.Remove(fields.Length - 1, 1)
            values = values.Remove(values.Length - 1, 1)

            sql.Append($"INSERT INTO Type ({fields}) VALUES ({values});")

            insertId = RunInsert(sql.ToString)

            ids.Add("typeId", insertId)

            ' Insert Biomes
            sql.Clear()
            fields.Clear()
            values.Clear()

            For Each prop As PropertyInfo In data.biome.GetType.GetProperties
                fields.Append($"[{prop.Name}],")
                values.Append($"{prop.GetValue(data.biome)},")
            Next

            fields = fields.Remove(fields.Length - 1, 1)
            values = values.Remove(values.Length - 1, 1)

            sql.Append($"INSERT INTO Biome ({fields}) VALUES ({values});")

            insertId = RunInsert(sql.ToString)

            ids.Add("biomeId", insertId)

            ' Insert Regions
            sql.Clear()
            fields.Clear()
            values.Clear()

            For Each prop As PropertyInfo In data.region.GetType.GetProperties
                fields.Append($"[{prop.Name}],")
                values.Append($"{prop.GetValue(data.region)},")
            Next

            fields = fields.Remove(fields.Length - 1, 1)
            values = values.Remove(values.Length - 1, 1)

            sql.Append($"INSERT INTO Region ({fields}) VALUES ({values});")

            insertId = RunInsert(sql.ToString)

            ids.Add("regionId", insertId)

            ' Insert Categories
            sql.Clear()
            fields.Clear()
            values.Clear()

            For Each prop As PropertyInfo In data.category.GetType.GetProperties
                fields.Append($"[{prop.Name}],")
                values.Append($"{prop.GetValue(data.category)},")
            Next

            fields = fields.Remove(fields.Length - 1, 1)
            values = values.Remove(values.Length - 1, 1)

            sql.Append($"INSERT INTO Category ({fields}) VALUES ({values});")

            insertId = RunInsert(sql.ToString)

            ids.Add("categoryId", insertId)

            ' Insert subCategories
            sql.Clear()
            fields.Clear()
            values.Clear()

            For Each prop As PropertyInfo In data.subCategory.GetType.GetProperties
                fields.Append($"[{prop.Name}],")
                values.Append($"{prop.GetValue(data.subCategory)},")
            Next

            fields = fields.Remove(fields.Length - 1, 1)
            values = values.Remove(values.Length - 1, 1)

            sql.Append($"INSERT INTO Subcategory ({fields}) VALUES ({values});")

            insertId = RunInsert(sql.ToString)

            ids.Add("subCategoryId", insertId)

            ' Insert facets
            sql.Clear()
            fields.Clear()
            values.Clear()

            For Each entry In ids
                fields.Append($"[{entry.Key}],")
                values.Append($"{entry.Value},")
            Next

            fields = fields.Remove(fields.Length - 1, 1)
            values = values.Remove(values.Length - 1, 1)

            sql.Append($"INSERT INTO Facets ({fields}) VALUES ({values});")

            insertId = RunInsert(sql.ToString)


            Return insertId
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

        Return 0
    End Function

    Public Shared Function InsertAsset(data As QuixelAssets.Asset) As Integer
        Try
            Dim ids As New Dictionary(Of String, Integer)
            Dim sql As New StringBuilder
            Dim fields As New StringBuilder
            Dim values As New StringBuilder
            Dim insertId As Integer

            ' Insert Types
            sql.Clear()
            fields.Clear()
            values.Clear()

            For Each prop As PropertyInfo In data.GetType.GetProperties
                fields.Append($"[{prop.Name}],")
                values.Append($"{prop.GetValue(data)},")
            Next

            fields = fields.Remove(fields.Length - 1, 1)
            values = values.Remove(values.Length - 1, 1)

            sql.Append($"INSERT INTO Asset ({fields}) VALUES ({values});")

            insertId = RunInsert(sql.ToString)

            ids.Add("typeId", insertId)




            Return insertId
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
        Return 0
    End Function
End Class
