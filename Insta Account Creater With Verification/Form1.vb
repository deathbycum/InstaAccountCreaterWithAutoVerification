Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim methodOBJ As New Methods

        ''For Registration
        MsgBox("Registered :" & methodOBJ.Register(TextBox4.Text & ComboBox1.SelectedItem, TextBox2.Text, TextBox1.Text, TextBox3.Text))
        If CheckBox1.Checked Then

            ''For Email Verification
            MsgBox("Email Verified :" & methodOBJ.DoConfirmation(TextBox1.Text, TextBox2.Text, TextBox4.Text & ComboBox1.SelectedItem))

        End If
    End Sub
End Class
