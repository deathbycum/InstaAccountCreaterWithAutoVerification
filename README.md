# InstaAccountCreaterWithAutoVerification
Instagram Account Creater With Email Verification

It is written in Vb.net and usese temp-mail api for email verifcation, Here is the usage.


Dim methodOBJ As New Methods
''For Registration

MsgBox("Registered :" & methodOBJ.Register(TextBox4.Text & ComboBox1.SelectedItem, TextBox2.Text, TextBox1.Text, TextBox3.Text))
     
''For Email Verification
          
MsgBox("Email Verified :" & methodOBJ.DoConfirmation(TextBox1.Text, TextBox2.Text, TextBox4.Text & ComboBox1.SelectedItem))


Credits : DeathByCum
Contact : https://hackforums.net/member.php?action=profile&uid=2064850
