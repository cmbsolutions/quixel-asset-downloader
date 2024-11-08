Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports quixel_asset_downloader.QuixelAssets


Module Program
    Private authCookie As String = ""
    Private BearerToken As String = ""
    Private Resolution As String = "4096x4096"
    Private ResolutionInt As Integer = 4096
    Private DownloadFilePath As String
    Private ExtendedInfoPath As String
    Private ExtendedInfo As Boolean
    Private download As Boolean
    Private extract As Boolean
    Private ZipInputFolder As String
    Private ExtractFolder As String
    Private ZipTest As Boolean

    Sub Main(args As String())
        Try
            Dim consolecolor = Console.ForegroundColor

            Console.WriteLine("Quixel Megascans assets downloader v0.1")

            If Not args.Any Then
                ' Start questions for data
                Console.WriteLine("Please enter the missing data...")
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.Write("authCookie: ")
                Console.ForegroundColor = consolecolor
                authCookie = Console.ReadLine
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.Write("Bearer token: ")
                Console.ForegroundColor = consolecolor
                BearerToken = Console.ReadLine
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.Write("Resolution [8192/4096/2048/1024]: ")
                Console.ForegroundColor = consolecolor
                Resolution = Console.ReadLine
                Resolution = $"{Resolution}x{Resolution}"
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.Write("Download folder: ")
                Console.ForegroundColor = consolecolor
                DownloadFilePath = Console.ReadLine

            Else

                For i As Integer = 0 To args.Length - 1
                    Select Case args(i)
                        Case "-t"
                            ZipTest = True
                        Case "-e"
                            extract = True
                        Case "-AuthCookie"
                            authCookie = args(i + 1)
                            i += 1
                        Case "-BearerToken"
                            BearerToken = args(i + 1)
                            i += 1
                        Case "-Resolution"
                            Resolution = $"{args(i + 1)}x{args(i + 1)}"
                            ResolutionInt = CInt(args(i + 1))
                            If Resolution <> "8192x8192" And Resolution <> "4096x4096" And Resolution <> "2048x2048" And Resolution <> "1024x1024" Then
                                Resolution = "4096x4096"
                                ResolutionInt = 4096
                            End If
                            i += 1
                        Case "-DownloadFolder"
                            DownloadFilePath = args(i + 1)
                            i += 1
                        Case "-GetExtendedInfoPath"
                            ExtendedInfoPath = args(i + 1)
                            i += 1
                        Case "-ei" '  extended info
                            ExtendedInfo = True
                        Case "-dl"
                            download = True
                        Case "-zipinputfolder"
                            ZipInputFolder = args(i + 1)
                            i += 1
                        Case "-extractfolder"
                            ExtractFolder = args(i + 1)
                            i += 1
                        Case Else
                            Console.WriteLine("Unknown argument given.")
                            Environment.Exit(4)
                    End Select
                Next
            End If

            If Not extract AndAlso Not ZipTest AndAlso (authCookie.Length = 0 OrElse BearerToken.Length = 0) Then
                Console.WriteLine("Please login to your quixel.com account and get cookie and bearertoken from the browser")
            End If

            If ExtendedInfo Then GetExtendedInfos(ExtendedInfoPath)
            If download Then DownloadAssets(ExtendedInfoPath, DownloadFilePath)
            If extract Then DoExtract(ZipInputFolder, ExtractFolder)

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub GetAllAssets(storagePath As String)
        Dim PageNumber As Integer = 94
        Dim totalPages As Integer = 99999

        If Not IO.Directory.Exists(storagePath) Then IO.Directory.CreateDirectory(storagePath)

        While PageNumber <= totalPages

            Using client As New HttpClient
                Dim request As New HttpRequestMessage(HttpMethod.Get, $"https://quixel.com/v1/assets?page={PageNumber}&limit=200")
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.6367.118 Safari/537.36")
                request.Headers.Add("Cookie", authCookie)
                request.Headers.Add("Authorization", $"Bearer {BearerToken}")

                Dim response = client.SendAsync(request).Result

                response.EnsureSuccessStatusCode()

                Dim qa As QuixelAssetsObject = JsonConvert.DeserializeObject(Of QuixelAssetsObject)(response.Content.ReadAsStringAsync.Result)
                totalPages = qa.pages

                Dim exportfile As String = $"{storagePath}\QuixelAssets-{(PageNumber - 1) * 200}-{(PageNumber * 200) - 1}.json"
                If IO.File.Exists(exportfile) Then IO.File.Delete(exportfile)

                IO.File.WriteAllText(exportfile, JsonConvert.SerializeObject(qa))

                PageNumber += 1
            End Using
        End While
    End Sub
    Private Sub GetExtendedInfos(inputFolder As String)

        Using p As New CmbConsoleControls.CmbMultiBar()
            Dim inputFiles = IO.Directory.GetFiles(inputFolder, "*.json").Where(Function(c) Not c.EndsWith("_extended.json") AndAlso Not IO.File.Exists(c.Replace(".json", "_extended.json"))).ToList
            Dim overall = p.Add("Overall progress:", True, True, True, inputFiles.Count)
            Dim i As Integer = 1
            Dim y As Integer = 1
            Dim fileprogress = p.Add("File progress:", True, True, True, 200)
            Using client As New HttpClient
                For Each f In inputFiles
                    Dim qa As QuixelAssetsObject = JsonConvert.DeserializeObject(Of QuixelAssetsObject)(IO.File.ReadAllText(f))
                    p.Report(i, overall)
                    i += 1
                    y = 1

                    For Each asset In qa.assets
                        Try
                            y += 1
                            p.Report(y, fileprogress)

                            Dim success As Boolean = False
                            Dim response As HttpResponseMessage = Nothing

                            While Not success
                                Dim request As New HttpRequestMessage(HttpMethod.Get, $"https://quixel.com/v1/assets/{asset.id}/extended")
                                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.6367.118 Safari/537.36")
                                request.Headers.Add("Cookie", authCookie)
                                request.Headers.Add("Authorization", $"Bearer {BearerToken}")
                                response = client.SendAsync(request).Result
                                Select Case response.StatusCode
                                    Case 502 ' bad gateway
                                        Console.SetCursorPosition(0, 6)
                                        Console.WriteLine("Bad gateway, retry")
                                        Threading.Thread.Sleep(1000) ' sleep for 5 seconds and then send again
                                        Continue While
                                    Case 403 ' Forbidden
                                        Console.Clear()
                                        Console.WriteLine("Got 403, change IP")
                                        Environment.Exit(4)
                                    Case Else
                                        success = True
                                End Select
                            End While

                            response.EnsureSuccessStatusCode()

                            asset.extendedinfo = JsonConvert.DeserializeObject(Of AssetExtended)(response.Content.ReadAsStringAsync.Result)
                        Catch ex As Exception
                            Console.WriteLine($"Error with asset: {asset.id}. {ex.Message}")
                            asset.HasErrors = True
                            asset.LastErrorMessage = ex.Message
                        End Try

                    Next

                    Dim exportfile As String = $"{f.Replace(".json", "_extended")}.json"
                    If IO.File.Exists(exportfile) Then IO.File.Delete(exportfile)

                    IO.File.WriteAllText(exportfile, JsonConvert.SerializeObject(qa))
                Next
            End Using
        End Using
    End Sub

    Private Sub DownloadAssets(inputFolder As String, savePath As String)
        Try
            Using p As New CmbConsoleControls.CmbMultiBar()
                Dim inputFiles As New List(Of String)
                If IO.File.Exists(inputFolder) Then
                    inputFiles.Add(inputFolder)
                Else
                    inputFiles = IO.Directory.GetFiles(inputFolder, "*_extended.json").Where(Function(c) Not IO.File.Exists(c.Replace(".json", "_processed.json"))).ToList
                End If
                Dim overall = p.Add("File progress:", True, True, True, inputFiles.Count)
                Dim i As Integer = 1
                Dim y As Integer = 1
                Dim fileprogress = p.Add("Asset progress:", True, True, True, 0)

                For Each f In inputFiles
                    Dim qa As QuixelAssetsObject = JsonConvert.DeserializeObject(Of QuixelAssetsObject)(IO.File.ReadAllText(f))
                    p.SetMaxProgressCount(qa.assets.Count, fileprogress)
                    p.Report(i, overall)
                    i += 1
                    y = 1
                    Using client As New HttpClient
                        For Each asset In qa.assets
                            p.Report(y, fileprogress)
                            y += 1
                            If AssetAlreadyDownloaded(savePath, asset.id) Then Continue For

                            If asset.extendedinfo IsNot Nothing Then
                                Try

                                    Dim success As Boolean = False
                                    Dim response As HttpResponseMessage = Nothing
                                    Dim postdata = BuildRequestContent(asset)
                                    Dim content As New StringContent(JsonConvert.SerializeObject(postdata), Nothing, "application/json")

                                    While Not success
                                        Dim request As New HttpRequestMessage(HttpMethod.Post, $"https://quixel.com/v1/downloads")
                                        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.6367.118 Safari/537.36")
                                        request.Headers.Add("Cookie", authCookie)
                                        request.Headers.Add("Authorization", $"Bearer {BearerToken}")
                                        request.Content = content
                                        response = client.SendAsync(request).Result

                                        Select Case response.StatusCode
                                            Case HttpStatusCode.BadGateway
                                                Console.WriteLine("Bad gateway, retrying")
                                                Threading.Thread.Sleep(1000) ' sleep for 1 seconds and then send again
                                            Case HttpStatusCode.Forbidden
                                                Console.Clear()
                                                Console.WriteLine("Got 403, change IP")
                                                Environment.Exit(4)
                                            Case Else
                                                success = True
                                        End Select
                                    End While

                                    response.EnsureSuccessStatusCode()

                                    Dim downloadInfo As AssetDownloads.Rootobject = JsonConvert.DeserializeObject(Of AssetDownloads.Rootobject)(response.Content.ReadAsStringAsync.Result)

                                    Dim downloadUrl As String = $"https://assetdownloads.quixel.com/download/{downloadInfo.id}?preserveStructure=true&url=https%3A%2F%2Fquixel.com%2Fv1%2Fdownloads"

                                    DownloadFileAsync(downloadUrl, savePath, asset.id).GetAwaiter().GetResult()
                                    ' chunks of 64Mb
                                    'DownloadChunksAsync(downloadUrl, savePath, asset.id, 67108864).GetAwaiter.GetResult()

                                Catch ex As Exception
                                    Console.WriteLine($"Error with asset: {asset.id}. {ex.Message}")
                                    asset.HasErrors = True
                                    asset.LastErrorMessage = ex.Message
                                End Try
                            Else
                                asset.HasErrors = True
                                asset.LastErrorMessage = "No extendedinfo!"
                            End If
                        Next
                    End Using

                    Dim exportfile As String = $"{f.Replace(".json", "_processed")}.json"
                    If IO.File.Exists(exportfile) Then IO.File.Delete(exportfile)

                    IO.File.WriteAllText(exportfile, JsonConvert.SerializeObject(qa))
                Next
            End Using

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub

    Private Function BuildRequestContent(ByRef a As Asset) As QuixelAssetRequestData
        Dim qard As New QuixelAssetRequestData With {
            .asset = a.id
        }
        qard.config.ztool = False
        qard.config.albedo_lods = True
        qard.config.meshMimeType = "application/x-fbx"
        qard.config.brushes = False
        qard.config.lowerlod_normals = True
        qard.config.highpoly = False

        If a.extendedinfo.meshes?.Count > 0 Then
            qard.config.lods.AddRange((From x In a.extendedinfo.meshes Where x.type = "lod" Select x.tris).ToList)
        End If

        Try
            If a.extendedinfo.components?.Count > 0 Then
                For Each comp In a.extendedinfo.components
                    Select Case comp.type
                        Case "albedo", "normal", "displacement", "ao", "roughness", "opacity"
                            qard.components.Add(New AssetComponent With {
                                        .type = comp.type,
                                        .mimeType = "image/jpeg",
                                        .resolution = comp.uris.First.resolutions.First.resolution
                                        })
                            If comp.name = "Displacement" Then
                                qard.components.Add(New AssetComponent With {
                                        .type = comp.type,
                                        .mimeType = "image/x-exr",
                                        .resolution = comp.uris.First.resolutions.First.resolution
                                        })
                            End If
                        Case Else
                            Continue For
                    End Select
                Next
            Else
                If a.extendedinfo.maps Is Nothing Then
                    GetExtendedInfo(a)
                End If

                If a.extendedinfo.maps?.Count > 0 Then
                    Dim highestResolution = a.extendedinfo.maps.Select(Function(item) New With {
                                                                       .original = item,
                                                                       .width = Integer.Parse(item.resolution.Split("x"c)(0)),
                                                                       .height = Integer.Parse(item.resolution.Split("x"c)(1))
                                                                       }).Where(Function(m) m.height <= 8192 And m.width <= 8192).OrderByDescending(Function(res) res.width * res.height).First.original.resolution

                    For Each comp In a.extendedinfo.maps.Where(Function(c) c.resolution = highestResolution)
                        If comp.mimeType = "image/x-exr" And comp.type <> "displacement" Then Continue For
                        Select Case comp.type
                            Case "albedo", "normal", "displacement", "ao", "roughness", "opacity"
                                qard.components.Add(New AssetComponent With {
                                    .type = comp.type,
                                    .mimeType = comp.mimeType,
                                    .resolution = comp.resolution
                                })
                            Case Else
                                Continue For
                        End Select

                    Next

                Else
                    ' add defaults
                    qard.components.Add(New AssetComponent With {
                        .type = "albedo",
                        .mimeType = "image/jpeg",
                        .resolution = Resolution
                    })
                    qard.components.Add(New AssetComponent With {
                        .type = "normal",
                        .mimeType = "image/jpeg",
                        .resolution = Resolution
                    })
                    qard.components.Add(New AssetComponent With {
                        .type = "displacement",
                        .mimeType = "image/jpeg",
                        .resolution = Resolution
                    })
                    qard.components.Add(New AssetComponent With {
                        .type = "displacement",
                        .mimeType = "image/x-exr",
                        .resolution = Resolution
                    })
                    qard.components.Add(New AssetComponent With {
                        .type = "ao",
                        .mimeType = "image/jpeg",
                        .resolution = Resolution
                    })
                    qard.components.Add(New AssetComponent With {
                        .type = "roughness",
                        .mimeType = "image/jpeg",
                        .resolution = Resolution
                    })
                End If
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        Return qard
    End Function

    Public Async Function DownloadChunksAsync(downloadUrl As String, savePath As String, alt As String, chunkSize As Long) As Task
        Using client As New HttpClient()
            Dim totalBytesDownloaded As Long = 0
            Dim totalSize As Long? = Nothing
            Dim fileName As String = ""
            Using p As New CmbConsoleControls.CmbProgressBar("Download progress", 3)
                p.ColoredOutput = True
                p.ShowNumericProgress = True
                p.ShowPercentage = True
                Try
                    Dim sizeRequest As New HttpRequestMessage(HttpMethod.Get, downloadUrl)
                    sizeRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.6367.118 Safari/537.36")
                    sizeRequest.Headers.Add("Cookie", authCookie)
                    sizeRequest.Headers.Add("Authorization", $"Bearer {BearerToken}")
                    sizeRequest.Headers.Range = New RangeHeaderValue(0, chunkSize - 1)

                    ' Get the total size of the file
                    Using initialResponse As HttpResponseMessage = Await client.SendAsync(sizeRequest, HttpCompletionOption.ResponseHeadersRead)
                        initialResponse.EnsureSuccessStatusCode()

                        ' Check if the server supports range requests
                        If initialResponse.Headers.AcceptRanges.Contains("bytes") Then
                            totalSize = initialResponse.Content.Headers.ContentLength + chunkSize - 1
                            p.MaxProgressCount = totalSize.Value
                        End If

                        ' Get filename from Content-Disposition header if available
                        fileName = GetFileNameFromContentDisposition(initialResponse.Content.Headers)
                        If String.IsNullOrEmpty(fileName) Then
                            fileName = $"{alt}.zip"
                        End If

                        ' Create the file and write the first chunk
                        Using fileStream As New FileStream($"{savePath}{fileName}", FileMode.Create, FileAccess.Write, FileShare.None)
                            Await WriteStreamToFile(initialResponse.Content.ReadAsStreamAsync().Result, fileStream, chunkSize)
                            totalBytesDownloaded += chunkSize
                        End Using
                    End Using

                    Dim tries As Integer = 0
                    If totalSize.HasValue Then
                        ' Read the content stream from the response
                        Using fileStream As New FileStream($"{savePath}{fileName}", FileMode.Append, FileAccess.Write, FileShare.Read)
                            While totalBytesDownloaded < totalSize
                                Try

                                    Dim rangeEnd As Long = Math.Min(totalBytesDownloaded + chunkSize - 1, totalSize.Value - 1)

                                    Dim request As New HttpRequestMessage(HttpMethod.Get, downloadUrl)
                                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.6367.118 Safari/537.36")
                                    request.Headers.Add("Cookie", authCookie)
                                    request.Headers.Add("Authorization", $"Bearer {BearerToken}")
                                    request.Headers.Range = New RangeHeaderValue(totalBytesDownloaded, rangeEnd)

                                    Using chunkResponse As HttpResponseMessage = Await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                                        chunkResponse.EnsureSuccessStatusCode()
                                        Await WriteStreamToFile(chunkResponse.Content.ReadAsStreamAsync().Result, fileStream, chunkSize)
                                        totalBytesDownloaded += rangeEnd - totalBytesDownloaded + 1

                                        p.Report(totalBytesDownloaded)
                                    End Using

                                Catch ex As Exception
                                    tries += 1
                                    Console.SetCursorPosition(0, 6)
                                    Console.WriteLine($"Error download chunk for {alt}. retry {tries}")
                                    Threading.Thread.Sleep(500)

                                    If tries >= 5 Then
                                        Console.SetCursorPosition(0, 8)
                                        Console.ForegroundColor = ConsoleColor.Red
                                        Console.WriteLine($"Error downloadfile {alt}.")
                                        Console.ResetColor()
                                        Exit While
                                    End If
                                End Try
                            End While
                        End Using
                    End If
                Catch ex As Exception
                    Console.SetCursorPosition(0, 6)
                    Console.WriteLine($"Error downloadfile {alt}.")
                End Try
            End Using
        End Using
    End Function

    ' Helper function to write content stream to file
    Private Async Function WriteStreamToFile(contentStream As Stream, fileStream As FileStream, chunkSize As Integer) As Task
        Dim buffer(8192 - 1) As Byte ' 8 KB buffer
        Dim bytesRead As Integer
        Do
            bytesRead = Await contentStream.ReadAsync(buffer, 0, buffer.Length)
            If bytesRead > 0 Then
                Await fileStream.WriteAsync(buffer, 0, bytesRead)
            End If
        Loop While bytesRead > 0
    End Function


    ' Async function to download a file using HttpClient
    Public Async Function DownloadFileAsync(downloadUrl As String, savePath As String, alt As String) As Task
        Using client As New HttpClient()
            client.Timeout = TimeSpan.FromHours(1)

            Try
                Dim success As Boolean = False
                Dim tries As Integer = 0

                While Not success
                    tries += 1
                    Dim request As New HttpRequestMessage(HttpMethod.Get, downloadUrl)
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.6367.118 Safari/537.36")
                    request.Headers.Add("Cookie", authCookie)
                    request.Headers.Add("Authorization", $"Bearer {BearerToken}")

                    ' Send a request and get the response stream
                    Using response As HttpResponseMessage = Await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        Select Case response.StatusCode
                            Case HttpStatusCode.BadGateway
                                Console.SetCursorPosition(0, 6)
                                Console.WriteLine("Bad gateway, retrying")
                                Threading.Thread.Sleep(1000) ' wait for 5 seconds and try again
                                If tries <= 5 Then
                                    Continue While
                                Else
                                    Exit While
                                End If
                            Case HttpStatusCode.Forbidden
                                Console.Clear()
                                Console.WriteLine("Received a 403 Forbidden. The bearertoken has expired or your IP is blocked for some time.")
                                Environment.Exit(4)
                            Case Else
                                success = True
                        End Select

                        Try
                            response.EnsureSuccessStatusCode()
                        Catch ex As Exception
                            Console.SetCursorPosition(0, 6)
                            Console.WriteLine($"Error downloadfile {alt}: {ex.Message}")
                            If tries <= 5 Then
                                Continue While
                            Else
                                Exit While
                            End If
                        End Try
                        ' Get filename from Content-Disposition header if available
                        Dim fileName As String = GetFileNameFromContentDisposition(response.Content.Headers)
                        If String.IsNullOrEmpty(fileName) Then
                            fileName = $"{alt}.zip"
                        End If

                        Dim downloadError As Boolean = True
                        Dim downloadErrorCount As Integer = 0

                        'While downloadError
                        Try
                            Using contentStream As Stream = Await response.Content.ReadAsStreamAsync()
                                ' Read the content stream from the response
                                Using fileStream As New FileStream($"{savePath}{fileName}", FileMode.Create, FileAccess.Write, FileShare.Read, 8192, useAsync:=True)

                                    Dim buffer(8192 - 1) As Byte
                                    Dim bytesRead As Integer

                                    Do
                                        bytesRead = Await contentStream.ReadAsync(buffer, 0, buffer.Length)
                                        If bytesRead > 0 Then
                                            Await fileStream.WriteAsync(buffer, 0, bytesRead)
                                        End If
                                    Loop While bytesRead > 0

                                    success = True
                                End Using
                            End Using
                        Catch ex As Exception
                            success = False
                            downloadErrorCount += 1
                            Console.SetCursorPosition(0, 6)
                            Console.ForegroundColor = ConsoleColor.Red
                            Console.WriteLine($"Error downloadfile {alt}: {ex.Message}")
                            Console.WriteLine($"Retry attempt: {downloadErrorCount}")
                            Console.ResetColor()
                            Threading.Thread.Sleep(1000)
                        End Try
                        'End While
                        'Dim downloadedFile As New IO.FileInfo($"{savePath}{fileName}")

                        'If Not downloadedFile.Exists Or downloadedFile.Length < 1048576 * 2 Then ' 2 Mb
                        '    success = False
                        'End If

                        If Not success And tries > 5 Then
                            Console.SetCursorPosition(0, 9)
                            Console.WriteLine($"Error downloadfile {alt}: retryed it 5 times.")
                            Exit While
                        End If
                    End Using
                End While
            Catch ex As Exception
                Console.SetCursorPosition(0, 6)
                Console.WriteLine($"Error downloadfile {alt}. {ex.Message}")
            End Try
        End Using
    End Function

    ' Function to extract filename from Content-Disposition header
    Private Function GetFileNameFromContentDisposition(headers As HttpContentHeaders) As String
        If headers.Contains("Content-Disposition") Then
            Dim contentDisposition As String = headers.GetValues("Content-Disposition").FirstOrDefault()

            ' Look for the filename in the Content-Disposition header
            If Not String.IsNullOrEmpty(contentDisposition) Then
                Dim fileNamePart As String = contentDisposition.Split(";"c).FirstOrDefault(Function(s) s.Trim().StartsWith("filename="))
                If Not String.IsNullOrEmpty(fileNamePart) Then
                    ' Extract the filename from the header, removing any quotes
                    Return fileNamePart.Substring(fileNamePart.IndexOf("="c) + 1).Trim(""""c)
                End If
            End If
        End If
        Return Nothing
    End Function

    Private Function AssetAlreadyDownloaded(savePath As String, assetId As String) As Boolean
        Return IO.Directory.EnumerateFiles(savePath, $"*{assetId}*", SearchOption.TopDirectoryOnly).Count > 0
    End Function

    Public Sub DoExtract(inputPath As String, outputPath As String)
        Using p As New CmbConsoleControls.CmbMultiBar()
            Dim overall = p.Add("Zipfiles progress:", True, True)
            Dim i As Integer = 1
            Dim y As Integer = 1
            Dim fileprogress = p.Add("Extraction progress:", True, True)
            Dim zipfiles = (From f In IO.Directory.GetFiles(inputPath, "*.zip") Select New IO.FileInfo(f)).ToList
            For Each zf In zipfiles
                Try
                    p.Report(CInt((i / zipfiles.Count) * 100), overall)
                    i += 1
                    Dim RegexObj As New Regex("(.+)_(\dK)_([^_]+)", RegexOptions.IgnoreCase Or RegexOptions.Multiline)

                    Dim subfoldername = RegexObj.Match(zf.Name).Groups(1).Value
                    Dim resolutionfolder = RegexObj.Match(zf.Name).Groups(2).Value
                    Dim categoryfolder = RegexObj.Match(zf.Name).Groups(3).Value
                    Dim unzippath = outputPath
                    If Not IO.Directory.Exists(unzippath) Then IO.Directory.CreateDirectory(unzippath)
                    unzippath = IO.Path.Combine(unzippath, categoryfolder)
                    If Not IO.Directory.Exists(unzippath) Then IO.Directory.CreateDirectory(unzippath)
                    unzippath = IO.Path.Combine(unzippath, resolutionfolder)
                    If Not IO.Directory.Exists(unzippath) Then IO.Directory.CreateDirectory(unzippath)
                    unzippath = IO.Path.Combine(unzippath, subfoldername)
                    If Not IO.Directory.Exists(unzippath) Then
                        IO.Directory.CreateDirectory(unzippath)
                    Else
                        Continue For ' already extracted?
                    End If

                    y = 1
                    Using fs As New IO.FileStream(zf.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)
                        Using z As New ZipArchive(fs)
                            For Each entry In z.Entries
                                p.Report(CInt((y / z.Entries.Count) * 100), fileprogress)
                                y += 1
                                entry.ExtractToFile($"{unzippath}\{entry.Name}")
                            Next
                        End Using
                    End Using
                Catch ex As Exception
                    Console.SetCursorPosition(0, 6)
                    Console.WriteLine(ex.Message)
                End Try
            Next
        End Using

    End Sub

    Private Sub GetExtendedInfo(ByRef asset As Asset)
        Using client As New HttpClient
            Try
                Dim success As Boolean = False
                Dim response As HttpResponseMessage = Nothing

                While Not success
                    Dim request As New HttpRequestMessage(HttpMethod.Get, $"https://quixel.com/v1/assets/{asset.id}/extended")
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.6367.118 Safari/537.36")
                    request.Headers.Add("Cookie", authCookie)
                    request.Headers.Add("Authorization", $"Bearer {BearerToken}")
                    response = client.SendAsync(request).Result
                    Select Case response.StatusCode
                        Case 502 ' bad gateway
                            Console.SetCursorPosition(0, 9)
                            Console.WriteLine("Got 502 from extendedinfo, sleeping 1 second and trying again")
                            Threading.Thread.Sleep(1000) ' sleep for 5 seconds and then send again
                        Case 403 ' Forbidden
                            Console.Clear()
                            Console.WriteLine("Got 403, change IP")
                            Environment.Exit(4)
                        Case Else
                            success = True
                    End Select
                End While

                response.EnsureSuccessStatusCode()

                asset.extendedinfo = JsonConvert.DeserializeObject(Of AssetExtended)(response.Content.ReadAsStringAsync.Result)
            Catch ex As Exception
                Console.WriteLine($"Error with asset: {asset.id}. {ex.Message}")
                asset.HasErrors = True
                asset.LastErrorMessage = ex.Message
            End Try
        End Using
    End Sub


End Module
