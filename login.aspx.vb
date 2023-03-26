Imports Classcatalogoch

Public Class login
    Inherits System.Web.UI.Page
    Private lg As New ClassLogin

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lg.carga_fr(Panel1)
        lg.MODULOS = "TAREAS,CLIENTES,COTIZACIONES,MULTIORDENES,ESTADISTICO"
    End Sub

End Class