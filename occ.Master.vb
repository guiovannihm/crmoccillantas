Imports Classcatalogoch

Public Class occ
    Inherits System.Web.UI.MasterPage
    Private lg As New ClassLogin
    Private Shared xt As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack Then
            Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache)
            Response.Cache.SetAllowResponseInBrowserHistory(False)
            Response.Cache.SetNoStore()
        End If

        Dim X As String = lg.perfil
        lg.panel_sup(Panel1, Nothing, Drawing.Color.White)
        Label1.Text = "<h3 style='text-align:center'>VERSION " + lg.ultima_actualizacion.ToString("ddMMyy") + "</h3>"
        carga_cartera()
    End Sub

    Protected Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'Response.Write("<SCRIPT>window.alert('PAGINA ACTUALIZADA " + Now.ToLocalTime + "')</SCRIPT>")
        If xt = 60 Then
            'Response.Redirect("login.aspx")
        Else
            xt += 1
        End If
    End Sub
    'Private Sub carga_cartera()
    '    Dim dsfn As New carga_dssql("v_cartera")
    '    Dim xcar As Integer
    '    If lg.perfil > 1 Then
    '        xcar = dsfn.valor_campo_OTROS("count(kfn)", "fecha_cuota <='" + Now.ToString("yyyy-MM-dd") + "' and estado='PENDIENTE'")
    '    Else
    '        xcar = dsfn.valor_campo_OTROS("count(kfn)", "asesor='" + Web.HttpContext.Current.User.Identity.Name + "' and fecha_cuota <='" + Now.ToString("yyyy-MM-dd") + "' and estado='PENDIENTE'")
    '    End If
    '    If xcar > 0 Then
    '        Dim Lb As New Label : Lb.ForeColor = Drawing.Color.Red : Lb.Text = "<h1>*** HAY CARTERA PENDIENTE POR CONFIRMAR PAGO. Consulte en <a href='default.aspx?fr=CARTERA'>cartera</a>***</h1>"
    '        Panel1.Controls.Add(Lb)
    '    End If
    'End Sub
    Private Sub carga_cartera()
        Dim dsfn As New carga_dssql("v_cartera")
        Dim xcar As Integer
        Select Case lg.perfil
            Case "1"
                xcar = dsfn.valor_campo_OTROS("count(kfn)", "asesor='" + Web.HttpContext.Current.User.Identity.Name + "' and fecha_cuota <='" + Now.ToString("yyyy-MM-dd") + "' and estado='PENDIENTE' and estadomo<>'3 ANULADO'")
            Case "2"
                xcar = 0
            Case "3"
                xcar = dsfn.valor_campo_OTROS("count(kfn)", "fecha_cuota <='" + Now.ToString("yyyy-MM-dd") + "' AND ESTADO='PENDIENTE' and estadomo<>'3 ANULADO'")

        End Select
        If xcar > 0 Then
            Dim Lb As New Label : Lb.ForeColor = Drawing.Color.Red : Lb.Text = "<h1>*** HAY CARTERA PENDIENTE POR CONFIRMAR PAGO. Consulte en <a href='default.aspx?fr=CARTERA'>cartera</a>***</h1>"
            Panel1.Controls.Add(Lb)
        Else
            dsfn.actualizardb("ESTADO='PAGO'", "estadomo='3 ANULADO'")
        End If


    End Sub
End Class