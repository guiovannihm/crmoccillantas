Imports Classcatalogoch
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
        carga_fr()
        Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache)
        Response.Cache.SetAllowResponseInBrowserHistory(False)
        Response.Cache.SetNoStore()
    End Sub
    Private Sub carga_fr()
        pf = lg.perfil
        Dim itb As String = Nothing
        es = New ClassESTADISTICAS(Panel1)
        Try
            If CT.val_parametro("CAMBIO_CLAVE", CT.USERLOGUIN) Is Nothing And CT.reque("fr") Is Nothing Then
                Response.Redirect("default.aspx?fr=CC")
            ElseIf CT.val_parametro("CAMBIO_CLAVE", CT.USERLOGUIN) Is Nothing And CT.reque("fr") = "CC" Then
                lg.CAMBIO_CLAVE(Panel1)
            ElseIf CDate(CT.val_parametro("CAMBIO_CLAVE", CT.USERLOGUIN)) = Now.ToShortDateString Then
                If CT.reque("fr") Is Nothing Then

                End If
                Response.Redirect("default.aspx?fr=CC")
            End If


            Select Case CT.reque("fr")
                Case "CONFIGURACION"
                    lg.FR_CONFIG(Panel1, "ASESOR,OPERADOR,SUPERVISOR,ADMIN", "CONFIGURACION")
                Case "CC"
                    lg.CAMBIO_CLAVE(Panel1)
                Case "", "INICIO"
                    Try
                        CT.tb_inicio(lg.MODULOS, CT.reque("fr"), Drawing.Color.Black, Drawing.Color.White)
                        'carga_cartera()
                    Catch ex As Exception
                        ' CT.redir("")
                    End Try
            End Select

            lg.MSN(Panel1)
            Dim CL As New ClassCLIENTES(Panel1, pf)
            Dim COT As New ClassCOTIZACION(Panel1, pf)
            Dim MO As New ClassMULTIORDEN(Panel1, pf)
            Dim IV As New ClassINVENTARIOS(Panel1)

            'Dim app As New ClassAPP(Panel1)

        Catch ex As Exception
            Response.Redirect("login.aspx")
        End Try

    End Sub

    Private Sub carga_cartera()
        Dim dsfn As New carga_dssql("v_cartera")
        Dim xcar As Integer
        Select Case pf
            Case "1"
                xcar = dsfn.valor_campo_OTROS("count(kfn)", "asesor='" + CT.USERLOGUIN + "' and fecha_cuota <='" + Now.ToString("yyyy-MM-dd") + "'")
            Case "2"
                xcar = 0
            Case "3"
                xcar = dsfn.valor_campo_OTROS("count(kfn)", "fecha_cuota <='" + Now.ToString("yyyy-MM-dd") + "'")

        End Select
        If xcar > 0 Then
            Dim Lb As New Label : Lb.ForeColor = Drawing.Color.Red : Lb.Text = "<h1>*** HAY CARTERA PENDIENTE POR CONFIRMAR PAGO. Consulte en <a href='default.aspx?fr=CARTERA'>cartera</a>***</h1>"
            Panel1.Controls.Add(Lb)
        End If


    End Sub

End Class