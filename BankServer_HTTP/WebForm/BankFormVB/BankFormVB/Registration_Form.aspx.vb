Public Class Registration_Form
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub ButtonSubmit_Click(sender As Object, e As EventArgs) Handles ButtonSubmit.Click

        Dim Name As String = TextBoxName.Text
        Dim User As String = TextBoxUser.Text
        Dim Password As String = TextBoxPassword.Text
        Dim Amount As Double = CDbl(TextBoxAmount.Text)
    End Sub


End Class