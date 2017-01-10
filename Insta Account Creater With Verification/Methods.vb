Imports System.Security.Cryptography
Imports System.Text
Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization

'-----------------
'Credits: DeathByCum 
'-----------------

Public Class Methods
    ''Set varibales
    Private mid As String
    Private csrf As String
    Private userid As String
    Private sessionID As String

    ''Sets serializer to desirialize json data
    Dim jd As New JavaScriptSerializer
    Private Sub GetStuff()
        ''Create a request to instagram.com to get mid and csrf token

        Dim req As HttpWebRequest = WebRequest.Create("https://www.instagram.com")
        req.Method = "GET"
        req.KeepAlive = True
        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36"
        req.AutomaticDecompression = DecompressionMethods.GZip

        ''Get response from request and sets mainSetcookie <-- response cookie

        Dim response As HttpWebResponse = req.GetResponse
        Dim mainSetcookie = response.GetResponseHeader("Set-Cookie")

        ''Gets and sets mid and csrftoken value from mainSetcookie string

        mid = Regex.Match(mainSetcookie, "mid=(.*?);").Groups(1).Value
        csrf = Regex.Match(mainSetcookie, "csrftoken=(.*?);").Groups(1).Value
    End Sub
    Private Function Login(ByVal user As String, ByVal pass As String) As Boolean
        Dim jd As New JavaScriptSerializer
        Dim struct As New ResponseStruct.LoginResponse

        Try
            ''Create post request to instagram/accounts/login/ajax/

            Call GetStuff()
            Dim postData() As Byte = Encoding.UTF8.GetBytes(String.Format("username={0}&password={1}", user, pass)) ''Get bytes of post data
            Dim request As HttpWebRequest = WebRequest.Create("https://www.instagram.com/accounts/login/ajax/")
            With request
                .Method = "POST"
                .Accept = "*/*"
                .UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36"
                .KeepAlive = True
                .ContentType = "application/x-www-form-urlencoded"
                .Referer = "https://www.instagram.com/"
                .AutomaticDecompression = DecompressionMethods.GZip
                .ContentLength = postData.Length
                .Headers.Add("X-Instagram-AJAX", "1")
                .Headers.Add("Origin", "https://www.instagram.com")
                .Headers.Add("Accept-Encoding", "gzip, deflate, br")
                .Headers.Add("Accept-Language", "en-US,en;q=0.8")
                .Headers.Add("X-CSRFToken", csrf)
                .Headers.Add("X-Requested-With", "XMLHttpRequest")
                .Headers.Add("Cookie", String.Format("mid={0}; fbm_124024574287414=base_domain=.instagram.com; ig_dau_dismiss=1483355851077; s_network=""""; ig_pr=1; ig_vw=1366; csrftoken={1}", mid, csrf))
            End With

            ''Write postBytes to request stream
            Dim reqstream As Stream = request.GetRequestStream
            reqstream.Write(postData, 0, postData.Length)
            reqstream.Flush()
            reqstream.Close()

            ''Get response from request
            Dim response As HttpWebResponse = request.GetResponse
            Dim respStream = response.GetResponseStream
            Dim reader As StreamReader = New StreamReader(respStream)
            Dim rawdata = reader.ReadToEnd
            reader.Dispose()

            ''Deserialize Data 
            struct = jd.Deserialize(Of ResponseStruct.LoginResponse)(rawdata)

            If struct.authenticated Then ''Check If loggedIn

                Dim mainCookie = response.GetResponseHeader("Set-Cookie")

                'Set sessionId,csrftoken and userid
                csrf = Regex.Match(mainCookie, "csrftoken=(.*?);").Groups(1).Value
                userid = Regex.Match(mainCookie, "ds_user_id=(.*?);").Groups(1).Value
                sessionID = Regex.Match(mainCookie, "sessionid=(.*?);").Groups(1).Value
                Return True
            Else
                Return False
            End If

        Catch e As Exception
            ''If exception then return False

            MsgBox(e.Message)
            Return False
        End Try
    End Function

    Public Function Register(ByVal email As String, ByVal password As String, ByVal username As String, ByVal first_name As String) As Boolean
        Try

            ''Create request to www.instagram.com/accounts/web_create_ajax/

            'Dim prxy As New WebProxy("Your Proxy If You Want.")
            Call GetStuff() ' set mid,csrf
            Dim postByte = Encoding.UTF8.GetBytes(String.Format("email={0}&password={1}&username={2}&first_name={3}", email, password, username, first_name))
            Dim request As HttpWebRequest = WebRequest.Create("https://www.instagram.com/accounts/web_create_ajax/")
            With request
                '.Proxy = prxy
                .Referer = "https://www.instagram.com/"
                .ContentLength = postByte.Length
                .Method = "POST"
                .UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36"
                .ContentType = "application/x-www-form-urlencoded"
                .AutomaticDecompression = DecompressionMethods.GZip
                .KeepAlive = True
                .Accept = "*/*"
                .Headers.Add("Origin", "https://www.instagram.com")
                .Headers.Add("X-Instagram-AJAX", "1")
                .Headers.Add("Accept-Encoding", "gzip, deflate, br")
                .Headers.Add("Accept-Language", "en-US,en;q=0.8")
                .Headers.Add("X-CSRFToken", csrf)
                .Headers.Add("X-Requested-With", "XMLHttpRequest")
                .Headers.Add("Cookie", String.Format("mid={0}; ig_dau_dismiss=1483887032036; ig_pr=1; ig_vw=1366; csrftoken={1}", mid, csrf))
            End With

            ''Write postBytes to requestStream
            Dim requestStream As Stream = request.GetRequestStream
            requestStream.Write(postByte, 0, postByte.Length)
            requestStream.Dispose()

            ''Get response and read it to rawdata
            Dim response As Stream = request.GetResponse.GetResponseStream
            Dim rawdata As String
            Using reader As New StreamReader(response)
                rawdata = reader.ReadToEnd
            End Using

            ''Deserialize Data
            Dim struct As New ResponseStruct.RegisterResponse
            struct = jd.Deserialize(Of ResponseStruct.RegisterResponse)(rawdata)

            ''If created then return true else false
            If struct.account_created Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            ''If exception then return false
            MsgBox(ex.Message)
            Return False
        End Try
    End Function
    Public Function DoConfirmation(ByVal username As String, ByVal password As String, ByVal email As String) As Boolean
        Try
            ''Create request to temp-email api
            Dim rawdata As String
            Dim link As String = Nothing
            Dim request As HttpWebRequest = WebRequest.Create(String.Format("https://api.temp-mail.org/request/mail/id/{0}/format/json/", GetMD5hash(email))) ''Email is encoded to MD5 hash as required by temp-email.
            Dim respose As Stream = request.GetResponse.GetResponseStream

            ''Read response
            Using reader As New StreamReader(respose)
                rawdata = reader.ReadToEnd
            End Using

            ''Deserialize Data
            Dim struct As New List(Of ResponseStruct.TempMailResponse)
            struct = jd.Deserialize(Of List(Of ResponseStruct.TempMailResponse))(rawdata)

            ''Read all emails and fetch confirmation link from email send from Instagram
            For i = 0 To struct.Count - 1
                Dim classOBJ = CType(struct.Item(i), ResponseStruct.TempMailResponse)
                If classOBJ.mail_from = """Instagram"" <no-reply@mail.instagram.com>" Then
                    link = Regex.Match(classOBJ.mail_text_only, "https://instagram.com/accounts/confirm_email/(.*?)/?app_redirect=False").Groups(0).Value
                    Login(username, password) ''To set login cookies for later use in confirmation
                    Return executeConfirmationLink(link) ''Run confirmation link 
                End If
            Next
            Return False
        Catch ex As Exception
            ''If exception then return false
            Return False
        End Try
    End Function
    Private Function executeConfirmationLink(ByVal link As String) As Boolean
        Try
            ''Create request to confirmation link fetched from the mail
            Dim rawdata As String
            Dim request As HttpWebRequest = WebRequest.Create(link)
            With request
                .Referer = "https://www.instagram.com/"
                .Method = "GET"
                .UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36"
                .ContentType = "application/x-www-form-urlencoded"
                .AutomaticDecompression = DecompressionMethods.GZip
                .KeepAlive = True
                .Accept = "*/*"
                .Headers.Add("Origin", "https://www.instagram.com")
                .Headers.Add("X-Instagram-AJAX", "1")
                .Headers.Add("Accept-Encoding", "gzip, deflate, br")
                .Headers.Add("Accept-Language", "en-US,en;q=0.8")
                .Headers.Add("X-CSRFToken", csrf)
                .Headers.Add("X-Requested-With", "XMLHttpRequest")
                .Headers.Add("Cookie", String.Format("mid={0}; ig_dru_dismiss=1483968379575; ig_dau_dismiss=1483977168645; sessionid={1}; s_network=""; ig_pr=1; ig_vw=1366; csrftoken={2}; ds_user_id={3}", mid, sessionID, csrf, userid))
            End With

            ''Get response from the server and reads it.
            Dim response As Stream = request.GetResponse.GetResponseStream
            Using reader As New StreamReader(response)
                rawdata = reader.ReadToEnd
            End Using

            ''If contains confirmation text then it will return true or elso it will return false
            If rawdata.Contains("Thanks! You have successfully confirmed your email.") Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            ''If exception then return false
            MsgBox(ex.Message)
            Return False
        End Try
    End Function
    Public Function GetMD5hash(ByVal Source As String) As String
        Dim Bytes() As Byte
        Dim sb As New StringBuilder()

        'Check for empty string.
        If String.IsNullOrEmpty(Source) Then
            Throw New ArgumentNullException
        End If

        'Get bytes from string.
        Bytes = Encoding.Default.GetBytes(Source)

        'Get md5 hash
        Bytes = MD5.Create().ComputeHash(Bytes)

        'Loop though the byte array and convert each byte to hex.
        For x As Integer = 0 To Bytes.Length - 1
            sb.Append(Bytes(x).ToString("x2"))
        Next

        'Return md5 hash.
        Return sb.ToString()

    End Function
End Class
