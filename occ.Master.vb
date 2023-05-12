Imports Classcatalogoch

Public Class occ
    Inherits System.Web.UI.MasterPage
    Private lg As New ClassLogin
    Private Shared xt As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim X As String = lg.perfil
        lg.panel_sup(Panel1, Nothing, Drawing.Color.White)
    End Sub

    Protected Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If xt = 60 Then
            'Response.Redirect("login.aspx")
        Else
            xt += 1
        End If
    End Sub
End Class