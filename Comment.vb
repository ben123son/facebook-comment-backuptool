Public Class Comment

    Public Sub New(senderId As String, sender As String, content As String, atDate As Date, index As Integer)

        ' 此為設計工具所需的呼叫。
        InitializeComponent()

        ' 在 InitializeComponent() 呼叫之後加入任何初始設定。
        Label1.Text = sender
        Label2.Text = index & " 樓"
        Label3.Text = String.Format("{0}, {1}", atDate.ToLongDateString, atDate.ToLongTimeString)
        TextBox1.Text = content.Replace(vbLf, vbCrLf)
        _Index = index
        _SenderName = sender
        _SenderId = senderId
        _AtDate = atDate
        _Content = content.Replace(vbLf, vbCrLf)
    End Sub

    Public Property Index As Integer
    Public Property SenderId As String
    Public Property SenderName As String
    Public Property AtDate As Date
    Public Property Content As String

    Private Sub Comment_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Public Sub ResizeControl()
        Me.Width = Me.Parent.Width - 19
    End Sub

    Public Sub LoadImage()
        PictureBox1.LoadAsync(String.Format("https://graph.facebook.com/v1.0/{0}/picture", SenderId))
    End Sub

    Public Overrides Function ToString() As String
        Dim sb As New System.Text.StringBuilder

        sb.AppendFormat("{0}{2}{1} 樓", SenderName, Index.ToString.PadLeft(5), New String(" "c, 62 - System.Text.Encoding.Default.GetBytes(SenderName).Length))
        sb.AppendLine()
        sb.AppendLine(String.Format("{0}, {1}", AtDate.ToLongDateString, AtDate.ToLongTimeString))
        sb.AppendLine(Content)
        sb.AppendLine(New String("="c, 70))
        Return sb.ToString()
    End Function
End Class
Public Class CommentContext

    Public Sub New(senderId As String, sender As String, content As String, atDate As Date, index As Integer)
        _Index = index
        _SenderName = sender
        _SenderId = senderId
        _AtDate = atDate
        _Content = content.Replace(vbLf, vbCrLf)
    End Sub

    Public Property Index As Integer
    Public Property SenderId As String
    Public Property SenderName As String
    Public Property AtDate As Date
    Public Property Content As String

    Public Overrides Function ToString() As String
        Dim sb As New System.Text.StringBuilder

        sb.AppendFormat("{0}{2}{1} 樓", SenderName, Index.ToString.PadLeft(5), New String(" "c, 62 - System.Text.Encoding.Default.GetBytes(SenderName).Length))
        sb.AppendLine()
        sb.AppendLine(String.Format("{0}, {1}", AtDate.ToLongDateString, AtDate.ToLongTimeString))
        sb.AppendLine(Content)
        sb.AppendLine(New String("="c, 70))
        Return sb.ToString()
    End Function
End Class
