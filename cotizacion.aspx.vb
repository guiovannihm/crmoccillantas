Imports Classcatalogoch
Public Class cotizacion
    Inherits System.Web.UI.Page
    Private dspa As New carga_dssql("parametros")
    Private lg As New ClassLogin
    Dim DSCT As New carga_dssql("cotizaciones")

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            TbCOT.Visible = False
            If Request.QueryString("cl") IsNot Nothing Then
                TbCOT.Visible = True
            End If
            CARGA_FR()
        End If
    End Sub
    Private Sub CARGA_FR()
        LbFECHA.Text = Now.ToString("yyyy-MM-dd")
        dr_par(DrTIPO_VEHICULO, "cotizacion", "tipo vehiculo")
        dr_par(DrTIPO_TERRENO, "cotizacion", "tipo terreno")
        dr_par(DrPOSICION, "cotizacion", "posicion")
        dr_par(DrEN_CALIDAD, "cotizacion", "EN CALIDAD")
        DrFORMA_PAGO.Items.Add("CONTADO")
        DrFORMA_PAGO.Items.Add("CREDITO")
        dr_par(DrCIUDAD_ENTREGA, "cliente", "ciudad")
    End Sub
    Private Sub dr_par(dr As DropDownList, fr As String, cr As String)
        dr.DataSource = dspa.Carga_tablas("formulario='" + fr + "' and criterio='" + cr + "'")
        dr.DataTextField = "valor"
        dr.DataBind()
        dr.SelectedIndex = -1
        'For Each ROW As DataRow In dspa.Carga_tablas("formulario='" + fr + "' and criterio='" + cr + "'", "VALOR").Rows
        '    dr.Items.Add(ROW.Item("VALOR"))
        'Next
    End Sub

    Protected Sub BtGUARDAR_Click(sender As Object, e As EventArgs) Handles BtGUARDAR.Click
        Dim CL, FE, TV, TT, PO, US, RF, TC, EC, FP, CE As String
        CL = Request.QueryString("cl")
        US = User.Identity.Name
        FE = LbFECHA.Text
        TV = DrTIPO_VEHICULO.SelectedItem.Value
        TT = DrTIPO_TERRENO.SelectedItem.Text
        PO = DrPOSICION.SelectedItem.Text
        RF = TmREFERENCIAS.Text
        TC = TxTIPO_CARGA.Text
        EC = DrEN_CALIDAD.SelectedItem.Text
        FP = DrFORMA_PAGO.SelectedItem.Text
        CE = DrCIUDAD_ENTREGA.SelectedItem.Text

        DSCT.insertardb(CL + ",'" + FE + "','" + TV + "','" + TT + "','" + PO + "','0 NUEVA','" + US + "','" + RF + "','" + FE + "','" + TC + "','" + EC + "','" + FP + "','" + CE + "'", True)
    End Sub
End Class