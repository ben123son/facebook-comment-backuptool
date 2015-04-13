Imports System.Security.Permissions, System.Runtime.InteropServices

<PermissionSet(SecurityAction.Demand, name:="FullTrust"),
 ComVisible(True)>
Public Class Form2

    Public Sub New()

        ' 此為設計工具所需的呼叫。
        InitializeComponent()

        ' 在 InitializeComponent() 呼叫之後加入任何初始設定。
    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        WebBrowser1.ObjectForScripting = Me
    End Sub

    Public Function closeDialog()
        closeDialogi()
        Return False
    End Function

    Private Sub closeDialogi()
        DialogResult = Windows.Forms.DialogResult.OK
        Close()
    End Sub

    Public Function setAccessToken(value As String)
        Form1.TextBox1.Text = value
        Form1.fb = New Facebook.FacebookClient(value)
        Return False
    End Function

    Public Function setUpTimer(sec As Integer)
        Return False
    End Function
End Class