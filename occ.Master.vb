Imports Classcatalogoch

Public Class occ
    Inherits System.Web.UI.MasterPage
    Private lg As New ClassLogin
    Private Shared xt As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim X As String = lg.perfil
        lg.panel_sup(Panel1, Nothing, Drawing.Color.White)
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
    Private Sub carga_cartera()
        Dim dsfn As New carga_dssql("v_cartera")
        Dim xcar As Integer
        If lg.perfil > 1 Then
            xcar = dsfn.valor_campo_OTROS("count(kfn)", "fecha_cuota <='" + Now.ToString("yyyy-MM-dd") + "'")
        Else
            xcar = dsfn.valor_campo_OTROS("count(kfn)", "asesor='" + Web.HttpContext.Current.User.Identity.Name + "' and fecha_cuota <='" + Now.ToString("yyyy-MM-dd") + "'")
        End If
        If xcar > 0 Then
            Dim Lb As New Label : Lb.ForeColor = Drawing.Color.Red : Lb.Text = "<h1>*** HAY CARTERA PENDIENTE POR CONFIRMAR PAGO. Consulte en <a href='default.aspx?fr=CARTERA'>cartera</a>***</h1>"
            Panel1.Controls.Add(Lb)
        End If

    End Sub
End Class