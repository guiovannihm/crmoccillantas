Imports Classcatalogoch
Public Class _default
    Inherits System.Web.UI.Page
    Private CT As ClassConstructor22
    Private lg As New ClassLogin


    Private dsne As New carga_dssql("negocios")
    Private dspa As New carga_dssql("parametros")
    Private Shared kcl, kne, pf, cam As String

    Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CT = New ClassConstructor22(Panel1, "default.aspx")
        carga_fr()
    End Sub
    Private Sub carga_fr()
        pf = lg.perfil
        Dim itb As String = Nothing
        Select Case CT.reque("fr")
            Case "CONFIGURACION"
                lg.FR_CONFIG(Panel1, "1ASESOR,1OPERADOR,2SUPERVISOR,3ADMIN", "CONFIGURACION")
            Case "", "INICIO"
                CT.tb_inicio(lg.MODULOS)
            Case "CLIENTES", "CLIENTE", "CONTACTO"
                Dim CL As New ClassCLIENTES(Panel1, pf)
            Case "NEGOCIOS", "NEGOCIO", "SEGUIMIENTO"
                Dim NE As New ClassNEGOCIO(Panel1, pf)
            Case "MULTIORDENES", "MULTIORDEN", "ITEMSMO"
                Dim MO As New ClassMULTIORDEN(Panel1, pf)
        End Select
    End Sub

End Class