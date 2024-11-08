Imports Classcatalogoch

Public Class login
    Inherits System.Web.UI.Page
    Private lg As New ClassLogin

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache)
        Response.Cache.SetAllowResponseInBrowserHistory(False)
        Response.Cache.SetNoStore()
        lg.carga_fr(Panel1)
        lg.MODULOS = "TAREAS,CLIENTES,COTIZACIONES,MULTIORDENES,ESTADISTICO,INVENTARIO"
    End Sub

End Class