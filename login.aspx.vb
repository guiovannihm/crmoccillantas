Imports Classcatalogoch

Public Class login
    Inherits System.Web.UI.Page
    Private lg As New ClassLogin

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lg.carga_fr(Panel1)
    End Sub

End Class