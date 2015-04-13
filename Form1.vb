Public Class Form1
    Friend fb As Facebook.FacebookClient


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        fb = New Facebook.FacebookClient(TextBox1.Text)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        FlowLayoutPanel1.Controls.Clear()
        Label3.Text = "正在取得資料..."
        Refresh()
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        SaveFileDialog1.FileName = TextBox2.Text
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim str As IO.FileStream = SaveFileDialog1.OpenFile()
            Dim bx() As Byte = System.Text.Encoding.UTF8.GetBytes(String.Format("帖子 {0} 的所有留言" & vbCrLf, TextBox2.Text))
            str.Write(bx, 0, bx.Length)
            For Each c As Comment In FlowLayoutPanel1.Controls
                Dim b() As Byte = System.Text.Encoding.UTF8.GetBytes(c.ToString)
                str.Write(b, 0, b.Length)
            Next
            str.Flush()
            str.Close()
        End If
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            Dim lst As New List(Of Comment)
            Dim r As Facebook.JsonObject = fb.Get(Of Facebook.JsonObject)(TextBox2.Text & "/comments", New With {.limit = 1000})
            Dim i As Integer = 0
            Dim p As Integer = 0
            While r("data").Count <> 0
                For Each d As Facebook.JsonObject In r("data")
                    i += 1
                    Dim c As New Comment(d("from")("id"), d("from")("name"), d("message"), Date.Parse(d("created_time")), i)
                    lst.Add(c)
                    BackgroundWorker1.ReportProgress(i / (r("data").Count + 1000 * p), New UserState(i, (r("data").Count + 1000 * p), 1))
                Next
                p += 1
                BackgroundWorker1.ReportProgress(100, New UserState("正在取得資料..."))
                r = fb.Get(Of Facebook.JsonObject)("v2.3/" & TextBox2.Text & "/comments", New With {.limit = 1000, .after = r("paging")("cursors")("after")})
            End While
            e.Result = lst
        Catch ex As Exception
            MsgBox("無法取得資訊: " & vbCrLf & ex.ToString, 16, "無法擷取留言")
        End Try
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged, BackgroundWorker2.ProgressChanged
        Dim x As UserState = e.UserState
        Label3.Text = x.StatusText
        Refresh()
    End Sub

    Private Class UserState
        Public Sub New(current As Integer, count As Integer, mode As Integer)
            If mode = 1 Then
                _StatusText = String.Format("正在讀取 {1} 個留言中的第 {0} 個...", current, count)
            Else
                _StatusText = String.Format("正在寫入 {1} 個留言中的第 {0} 個...", current, count)
            End If

        End Sub
        Public Sub New(statusText As String)
            _StatusText = statusText

        End Sub

        Public Property StatusText As String
    End Class

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Label3.Text = "正在讀入資訊...如留言較多，讀入時間較長，請見諒"
        Refresh()
        Dim lst As List(Of Comment) = e.Result
        FlowLayoutPanel1.Controls.AddRange(lst.ToArray)
        For Each c As Comment In FlowLayoutPanel1.Controls
            c.LoadImage()
            c.ResizeControl()
        Next
        Label3.Text = "資訊讀取完成"

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        SaveFileDialog1.FileName = TextBox2.Text
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Label3.Text = "正在取得資料..."
            BackgroundWorker2.RunWorkerAsync(SaveFileDialog1.OpenFile())
        End If
    End Sub

    Private Sub BackgroundWorker2_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker2.RunWorkerCompleted
        Label3.Text = "已備份到 " & SaveFileDialog1.FileName
    End Sub

    Private Sub BackgroundWorker2_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        Dim str As IO.FileStream = e.Argument
        Try
            Dim bx() As Byte = System.Text.Encoding.UTF8.GetBytes(String.Format("帖子 {0} 的所有留言，擷取於 {1}, {2}" & vbCrLf, TextBox2.Text, Now.ToLongDateString, Now.ToLongTimeString))
            str.Write(bx, 0, bx.Length)
            Dim r As Facebook.JsonObject = fb.Get(Of Facebook.JsonObject)("v2.3/" & TextBox2.Text & "/comments", New With {.limit = 1000})
            Dim i As Integer = 0
            Dim p As Integer = 0
            While r("data").Count <> 0
                For Each d As Facebook.JsonObject In r("data")
                    i += 1
                    Dim c As New CommentContext(d("from")("id"), d("from")("name"), d("message"), Date.Parse(d("created_time")), i)
                    Dim b() As Byte = System.Text.Encoding.UTF8.GetBytes(c.ToString)
                    str.Write(b, 0, b.Length)
                    BackgroundWorker1.ReportProgress(i / (r("data").Count + 1000 * p), New UserState(i, (r("data").Count + 1000 * p), 0))
                Next
                p += 1
                BackgroundWorker1.ReportProgress(100, New UserState("正在取得資料..."))
                r = fb.Get(Of Facebook.JsonObject)(TextBox2.Text & "/comments", New With {.limit = 1000, .after = r("paging")("cursors")("after")})
            End While
        Catch ex As Exception
            MsgBox("無法取得資訊: " & vbCrLf & ex.ToString, 16, "無法擷取留言")
        End Try
        str.Flush()
        str.Close()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        Timer1.Enabled = CheckBox1.Checked
        If CheckBox1.Checked Then
            SaveFileDialog1.FileName = TextBox2.Text
            If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                Label3.Text = "正在取得資料..."
                BackgroundWorker2.RunWorkerAsync(SaveFileDialog1.OpenFile())
            End If
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label3.Text = "正在取得資料..."
        BackgroundWorker2.RunWorkerAsync(SaveFileDialog1.OpenFile())
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form2.Show()
    End Sub
End Class
