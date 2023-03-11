Imports Classcatalogoch

Public Class occ
    Inherits System.Web.UI.MasterPage
    Private lg As New ClassLogin

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim X As String = lg.perfil
        lg.panel_sup(Panel1, Nothing)
    End Sub

End Class