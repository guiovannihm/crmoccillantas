﻿Imports Classcatalogoch
Public Class _default
    Inherits System.Web.UI.Page
    Private CT As ClassConstructor22
    Private lg As New ClassLogin
    Private es As ClassESTADISTICAS


    Private dsct As New carga_dssql("COTIZACIONES")
    Private dspa As New carga_dssql("parametros")
    Private Shared kcl, kne, pf, cam As String

    Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CT = New ClassConstructor22(Panel1, "default.aspx")
        'Try
        carga_fr()
        'Catch ex As Exception
        '    dsct.txtError(ex)
        '    Response.Redirect("default.aspx")
        'End Try
    End Sub
    Private Sub carga_fr()
        pf = lg.perfil
        Dim itb As String = Nothing
        es = New ClassESTADISTICAS(Panel1)
        Try
            If CT.val_parametro("CAMBIO_CLAVE", CT.USERLOGUIN) Is Nothing And CT.reque("fr") Is Nothing Then
                Response.Redirect("default.aspx?fr=CC")
            ElseIf CDate(CT.val_parametro("CAMBIO_CLAVE", CT.USERLOGUIN)) = Now.ToShortDateString And CT.reque("fr") Is Nothing Then
                Response.Redirect("default.aspx?fr=CC")
            End If
        Catch ex As Exception

        End Try

        Select Case CT.reque("fr")
            Case "CONFIGURACION"
                lg.FR_CONFIG(Panel1, "ASESOR,OPERADOR,SUPERVISOR,ADMIN", "CONFIGURACION")
            Case "CC"
                lg.CAMBIO_CLAVE(Panel1)
            Case "", "INICIO"
                Try
                    CT.tb_inicio(lg.MODULOS, CT.reque("fr"), Drawing.Color.Black, Drawing.Color.White)
                    'es.PANEL_USUARIO()
                Catch ex As Exception
                    ' CT.redir("")
                End Try
        End Select

        lg.MSN(Panel1)
        Dim CL As New ClassCLIENTES(Panel1, pf)
        Dim COT As New ClassCOTIZACION(Panel1, pf)
        Dim MO As New ClassMULTIORDEN(Panel1, pf)

    End Sub

End Class