Public Class _default
    Inherits System.Web.UI.Page
    Private CT As Classcatalogoch.ClassConstructor22
    Private lg As New Classcatalogoch.ClassLogin


    Private dsne As New Classcatalogoch.carga_dssql("negocios")
    Private dspa As New Classcatalogoch.carga_dssql("parametros")
    Private Shared kcl, kne, pf, cam As String

    Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CT = New Classcatalogoch.ClassConstructor22(Panel1, "default.aspx")
        carga_fr()
    End Sub
    Private Sub carga_fr()
        Select Case lg.perfil
            Case "SUPERADMIN", "ADMIN", "SUPERVISOR"
                pf = 2
            Case "OPERADOR"
                pf = 1
        End Select
        Dim itb As String = Nothing
        Select Case CT.reque("fr")
            Case "CONFIGURACION"
                lg.FR_CONFIG(Panel1, "OPERADOR,SUPERVISOR,ADMIN", "CONFIGURACION")
            Case "", "INICIO"
                If pf = 2 Then
                    itb = "NEGOCIOS,CLIENTES,MULTIORDENES,ESTADISTICO"
                ElseIf pf = 1 Then
                    itb = "NEGOCIOS,CLIENTES,MULTIORDENES"
                End If
                CT.tb_inicio(itb)
            Case "CLIENTES", "CLIENTE", "CONTACTO"
                Dim CL As New ClassCLIENTES(Panel1, pf)
            Case "NEGOCIOS", "NEGOCIO", "SEGUIMIENTO"
                Dim NE As New ClassNEGOCIO(Panel1, pf)
            Case "MULTIORDENES", "MULTIORDEN", "ITEMSMO"
                Dim MO As New ClassMULTIORDEN(Panel1, pf)
        End Select
    End Sub

End Class