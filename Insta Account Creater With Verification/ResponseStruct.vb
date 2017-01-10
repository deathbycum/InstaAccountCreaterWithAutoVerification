Public Class ResponseStruct
    Public Class LoginResponse
        Public status As String
        Public user As String
        Public authenticated As Boolean
    End Class
    Public Class RegisterResponse
        Public account_created As Boolean
    End Class
    Public Class TempMailResponse
        Public _id As Object
        Public CreatedAt As Object
        Public mail_add_id As String
        Public mail_from As String
        Public mail_html As String
        Public mail_id As String
        Public mail_preview As String
        Public mail_subject As String
        Public mail_text As String
        Public mail_text_only As String
        Public mail_timestamp As Double
    End Class
End Class