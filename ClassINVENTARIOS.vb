Imports System.IO
Imports Classcatalogoch
Imports System.Data.SqlClient
Imports System.Net.WebRequestMethods
Imports System.Security.Policy

Public Class ClassINVENTARIOS
    Private Shadows _fr As Panel
    Private fr, frp As ClassConstructor22
    Private lg As New ClassLogin
    Private dsim As New carga_dssql("imagenes")
    Private dspi As New carga_dssql("proinv")
    Private dspa As New carga_dssql("parametros")
    Private dspd As New carga_dssql("prodis")
    Private dsvdp As New carga_dssql("v_invdis")
    Private dsinv As New carga_dssql("v_inv")
    Private dsinvd As New carga_dssql("v_invd")
    Private dsinvs As New carga_dssql("v_invs")
    Private dsinve As New carga_dssql("v_inve")
    Private dsct As New carga_dssql("cotizaciones")
    Private dsitc As New carga_dssql("itemct")
    'Private dsinvp As New carga_dssql("v_invp")
    'Private dsfilinv As New carga_dssql("v_filinv")
    Private PnBT, PnTL, Pn3 As New Panel
    Private Shadows idimg, idct, crf As String
    Private Shadows PnFP As Panel
    Sub New(Panelfr As Panel)
        _fr = Panelfr
        '_fr.Controls.Clear()

        fr = New ClassConstructor22(_fr)
        dsim.campostb = "kimagen-key,nombre-varchar(250),foto-image"
        dspi.campostb = "kproducto-key,referencia-varchar(250),diseno-varchar(250),marca-varchar(250),descripcion-varchar(500),precio_contado-money,precio_credito-money,grupo-varchar(250),disponible-bigint,plantilla-varchar(50),aplicacion-varchar(50),posicion-varchar(50)"
        dspd.campostb = "kdispo-key,kproducto-bigint,fingreso-date,bodega-varchar(250),cantidad-bigint,disponibleb-bigint"
        'dsinv.vistatb("v_inv", "prodis i", "proinv p", "i.kdispo,i.bodega,i.cantidad,i.disponibleb,P.*", "i.kproducto=p.kproducto and disponibleb > 0")
        dsinve.vistatb("v_inve", "prodis", Nothing, "kproducto,bodega,sum(cantidad) as cantidad", Nothing,, "kproducto,bodega")
        dsinv.vistatb("v_inv", "v_inve i", "proinv p", "i.bodega,i.cantidad,p.*", "i.kproducto=p.kproducto") ' and cantidad > 0")
        dsinvs.vistatb("v_invs", "itemmo i", "multiorden m", "i.*,m.estadomo", "i.kmo=m.kmo",,, "i.bodega<>'' and m.estadomo IS NOT NULL and m.estadomo<>'0 CREACION' and m.estadomo<>'3 ANULADO'")
        dsinvd.vistatb("v_invd", "v_inv i", Nothing, "(referencia+marca+diseno) as codigo,referencia,diseno,marca,aplicacion,posicion,precio_contado,precio_credito,bodega,sum(cantidad) as entrada,(select ISNULL(sum(cantidad),0) from v_invs m where ref=referencia and m.marca=i.marca and m.bodega=i.bodega) as salida", Nothing,, "referencia,bodega,diseno,marca,aplicacion,posicion,precio_contado,precio_credito")
        'dsinvp.vistatb("v_invp", "")
        'VALIDAR_INVENTARIO()
        '_fr.Controls.Clear()

        Select Case fr.reque("fr")
            Case "INVENTARIOS", "INVENTARIO"
                idct = fr.reque("ct")
                FRPN = _fr.FindControl("PnBOTONES")
                If FRPN Is Nothing Then
                    FRPN = New Panel
                    FRPN.ID = "PnBOTONES"
                End If
                carga_inventario()
                Select Case fr.reque("sfr")
                    Case "NUEVO PRODUCTO"
                        nuevo_pr()
                    Case "NUEVA PLANTILLA"
                        nueva_plantilla()
                    Case "NUEVA BODEGA"
                        nueva_bodega()
                    Case "PLANTILLA"
                        nuevo_iplantilla()
                    Case "NUEVO IPRODUCTO"
                        nuevo_iproducto()
                    Case "FOTOS"
                        nueva_foto()
                    Case "INGRESO PRODUCTO"
                        ingreso_producto()
                    Case "KARDEX"
                        KARDEX()
                End Select
                If _fr.FindControl(FRPN.ID) Is Nothing Then
                    _fr.Controls.Add(FRPN)
                End If

            Case "ADD_PRODUCTO"
                _fr.Controls.Clear()
                ADD_PRODUCTO()
        End Select
    End Sub

    Private Sub PnFR(CAMPOS As String)
        Dim pn As Panel = _fr.FindControl("FrINVENTARIO")
        For Each STR As String In CAMPOS.Split(",")
            Select Case STR.Remove(2)
                Case "Bt"
                    Bt = New Button : Bt.Text = STR.Replace("Bt", "")
                    pn.Controls.Add(Bt)
            End Select
        Next
    End Sub
    Private Sub KARDEX()
        If lg.perfil > 1 Then
            If fr.reque("cd") Is Nothing Then
                fr.FORMULARIO_GR("KARDEX INVENTARIO", "GrKD", "codigo-K,REFERENCIA-BT,MARCA-BT,-SUM(ENTRADA)ENTRADAS-BT,-SUM(SALIDA)SALIDAS-BT,-SUM(ENTRADA-SALIDA)SALDO-BT", Nothing, "V_INVD",, AddressOf sel_GrKD, "REFERENCIA,MARCA", "SALDO")
            Else
                fr.FORMULARIO_GR("KARDEX DETALLADO", "GrKDD", "codigo-K,REFERENCIA,MARCA,DISEÑO;DISENO,APLICACION,POSICION,BODEGA,-SUM(ENTRADA)ENTRADAS,-SUM(SALIDA)SALIDAS,-SUM(ENTRADA-SALIDA)SALDO", Nothing, "V_INVD", "CODIGO='" + fr.reque("cd") + "'")
            End If
        Else
            fr.redir("?fr=INVENTARIO")
        End If

    End Sub
    Private Sub sel_GrKD()
        fr.redir("?" + fr.urlac + "&cd=" + fr.FR_CONTROL("GrKD"))
    End Sub


    Public Function VALIDAR_INVENTARIO(cantidad As Integer, codigormd As String) As Boolean
        Dim x As String = dsinvd.valor_campo("sum(entrada - salida)", "codigo='" + codigormd + "'")
        If x IsNot Nothing Then
            If CInt(x) >= cantidad Then
                Return True
            End If
        End If

        Return False
    End Function
    Private FRPN As Panel
    Private valctr As Boolean
#Region "INVENTARIO"
    Public Shadows IDISPO As String
    Public Sub consulta_inventario(Optional CRITERIO As String = Nothing, Optional EVENTO As EventHandler = Nothing)
        Dim _CT, _FL As String : _CT = Nothing : _FL = "REFERENCIA,MARCA,DISENO,APLICACION,POSICION"
        _CT = "referencia-K,referencia-BT,diseno,MARCA,BODEGA,APLICACION,POSICION,PRECIO_CONTADO-M,PRECIO_CREDITO-M,-SUM(ENTRADA-SALIDA)DISPONIBLE"
        Select Case fr.reque("fr")
            Case "ITEMSMO"
                _FL = Nothing
                _CT = "referencia-K,BODEGA,referencia-BT,diseno,MARCA,PRECIO_CONTADO,PRECIO_CREDITO,-SUM(ENTRADA-SALIDA)DISPONIBLE"
            Case "INVENTARIO", "INVENTARIOS"
                _FL = Nothing
                _CT = "BODEGA,-SUM(ENTRADA-SALIDA)DISPONIBLE"
        End Select

        If CRITERIO IsNot Nothing Then
            If CRITERIO.Contains(CRITERIO) = False Then
                CRITERIO += " and " + CRITERIO
            End If
            CRITERIO = " and " + CRITERIO
        End If
        If EVENTO Is Nothing Then
            EVENTO = AddressOf SEL_GrINV
        End If
        If _fr.FindControl("GrINV") Is Nothing Then
            fr.FORMULARIO_GR("<br>INVENTARIO", "GrINV", _CT, lg.MODULOS, "V_INVD", "entrada > salida" + CRITERIO, EVENTO, _FL, SUBM_FR:=True)
            Dim GrINV As GridView = _fr.FindControl("GrINV")
            For Each grow As GridViewRow In GrINV.Rows
                If grow.Cells(0).Text = fr.reque("rf") And grow.Cells(1).Text = fr.reque("bd") Then
                    grow.BackColor = Drawing.Color.Yellow

                End If
            Next
        End If

    End Sub
    Private Sub carga_GrINV()

    End Sub
    Private Function DrREFERENCIA() As DropDownList
        DrREFERENCIA = New DropDownList
        DrREFERENCIA.DataSource = dspi.Carga_tablas("disponible > 0")
        DrREFERENCIA.DataTextField = "REFERENCIA"
        DrREFERENCIA.DataBind()
    End Function
    Private Function DrMARCA() As DropDownList

    End Function
    Private Function DrDISENO() As DropDownList

    End Function
    Public Function disponibilidad(referencia As String) As Integer
        Return dsvdp.valor_campo_OTROS("sum(disponibleb)", "referencia='" + referencia + "'")
    End Function
    Private Sub SEL_GrINV()
        Dim diru As String = Nothing
        Select Case fr.reque("fr")
            Case "ITEMSMO"
                Dim GrINV As GridView = _fr.FindControl("GrINV")
                Dim url As String = fr.urlac
                If fr.urlac.Split("&").Count > 2 Then
                    url = fr.urlac.Split("&")(0) + "&" + fr.urlac.Split("&")(1)
                End If
                'fr.redir("?" + url + "&rf=" + GrINV.SelectedRow.Cells(0).Text + "&bd=" + GrINV.SelectedRow.Cells(1).Text + "#finalp")
                fr.redir("?fr=ITEMSMO&mo=" + fr.reque("mo") + "&rf=" + GrINV.SelectedRow.Cells(0).Text + "&ds=" + GrINV.SelectedRow.Cells(3).Text + "&ma=" + GrINV.SelectedRow.Cells(4).Text + "&bd=" + GrINV.SelectedRow.Cells(5).Text + "#finalp")
            Case "COTIZACION"
                IDISPO = fr.FR_CONTROL("GrINV")
                If fr.urlac.Contains("&rf=") = False Then
                    diru = fr.urlac
                Else
                    diru = fr.urlac.Split("&")(3)
                    diru = fr.urlac.Replace("&" + diru, "")
                End If
                fr.redir("?" + diru + "&rf=" + fr.FR_CONTROL("GrINV"))
            Case "VALINV"
                Dim GrINV As GridView = _fr.FindControl("GrINV")
                fr.redir("?fr=ITEMSMO&mo=" + fr.reque("mo") + "&rf=" + GrINV.SelectedRow.Cells(0).Text + "&ds=" + GrINV.SelectedRow.Cells(2).Text + "&ma=" + GrINV.SelectedRow.Cells(3).Text + "&bd=" + GrINV.SelectedRow.Cells(4).Text + "#finalp")
                'fr.rewrite("window.open('default.aspx?fr=INVENTARIOS&rf=" + IDISPO + "')")
        End Select


        'fr.redir("?fr=ITEMSMO&mo=" + fr.reque("mo") + "&pi=" + fr.FR_CONTROL("GrINV"))
    End Sub
    Public Function VAL_ITEM(CAMPO As String, Optional CRITERIO As String = Nothing) As String
        If CRITERIO.Contains("referencia='ITEM COTIZACION'") = True Then
            Return "0"
        End If
        If CRITERIO IsNot Nothing Then
            Return dsinv.valor_campo(CAMPO, CRITERIO)
        Else
            IDISPO = fr.FR_CONTROL("GrINV")
            Return dsinv.valor_campo(CAMPO, "KDISPO=" + IDISPO)
        End If

    End Function

    Private Sub carga_inventario()
        FRPN = _fr.FindControl("PnBOTONES")
        If FRPN Is Nothing Then
            FRPN = New Panel
            FRPN.ID = "PnBOTONES"
        End If
        frp = New ClassConstructor22(FRPN)
        If _fr.FindControl("Lb_") IsNot Nothing Then
            Exit Sub
        End If
        If fr.urla = "ventana.aspx" Then
            fr.FORMULARIO(fr.reque("fr") + " " + fr.reque("sfr"), "Lb_")
            If fr.SESION_GH("crf") IsNot Nothing Then
                'If fr.SESION_GH("crf").ToString.Contains("referencia='" + dsct.valor_campo("referencia", "kcot=" + fr.reque("ct")) + "'") = False Then
                '    fr.SESION_GH("crf") = "referencia='" + dsct.valor_campo("referencia", "kcot=" + fr.reque("ct")) + "'"
                'End If
            Else
                'fr.SESION_GH("crf") = "referencia='" + dsct.valor_campo("referencia", "kcot=" + fr.reque("ct")) + "'"
            End If
            crf = fr.SESION_GH("crf")

        Else
            'fr.FORMULARIO("INVENTARIO", "TxBUSCAR,BtBUSCAR", True,, lg.MODULOS)
            fr.FORMULARIO(fr.reque("fr") + " " + fr.reque("sfr"), "Lb_",,, lg.MODULOS)
        End If

        If lg.perfil > 1 Then
            valctr = True
            fr.FR_BOTONES("NUEVA_PLANTILLA,NUEVA_BODEGA,NUEVO_PRODUCTO,INGRESO_PRODUCTO,KARDEX",, True)
            fr.FR_CONTROL("BtNUEVO_PRODUCTO", evento:=AddressOf sel_bt) = Nothing
            fr.FR_CONTROL("BtNUEVA_PLANTILLA", evento:=AddressOf sel_bt) = Nothing
            fr.FR_CONTROL("BtNUEVA_BODEGA", evento:=AddressOf sel_bt) = Nothing
            fr.FR_CONTROL("BtKARDEX", evento:=AddressOf sel_bt) = Nothing
            fr.FR_CONTROL("BtINGRESO_PRODUCTO", evento:=AddressOf sel_bt) = "INGRESO PRODUCTO"
        Else
            'fr.FR_CONTROL("BtGUARDAR", valctr) = "NUEVO PRODUCTO"
        End If

        Dim TbINV As New Table : TbINV.Width = Unit.Percentage(100)
        If fr.reque("sfr") Is Nothing Then
            productos()
        Else
            FRPN.Controls.Clear()
        End If

    End Sub

    Private Sub sel_bt(sender As Object, E As EventArgs)
        Dim BtS As Button = sender
        If fr.urlac.Contains("&ct=") = True Then
            fr.redir("?fr=INVENTARIO&ct=" + fr.reque("ct") + "&sfr=" + BtS.Text)
        Else
            fr.redir("?fr=INVENTARIO&sfr=" + BtS.Text)
        End If

    End Sub
#End Region
#Region "producto"
    Private Dr As DropDownList
    Private Lb As Label
    Private Tx As TextBox
    Private Bt As Button
    Private Sub productos()
        FRPN = _fr.FindControl("PnBOTONES")
        If FRPN Is Nothing Then
            FRPN = New Panel
            FRPN.ID = "PnBOTONES"
        End If
        Dim TbINV As New Table : TbINV.Width = Unit.Percentage(100)
        Dim tb As DataTable
        Dim ref As String = Nothing
        If lg.perfil > 1 Then
            'tb = dspi.Carga_tablas(, "disponible desc")
        Else
            If fr.reque("rf") IsNot Nothing Then
                ref = " and referencia='" + fr.reque("rf") + "'"
            ElseIf crf IsNot Nothing Then
                ref = crf
            End If
            'tb = dspi.Carga_tablas("disponible > 0" + ref, "disponible desc")
        End If
        FRPN.Controls.Add(pnfiltrop("referencia,marca,diseno,posicion,aplicacion"))
        If fr.SESION_GH("crf") IsNot Nothing Then
            ref = crf + fr.SESION_GH("crf2")
        End If
        tb = dsinvd.Carga_tablas(ref,, "referencia,marca,diseno as diseño,aplicacion,posicion,precio_contado,precio_credito,sum(entrada-salida) as disponible,codigo", True)

        For Each ROW As DataRow In tb.Rows
            Dim imgf As New ImageButton
            Dim TbR As New TableRow
            Dim Pnf1, Pnf2, Pnf3 As New Panel
            imgf.ImageUrl = dsim.imagendb(dsim.valor_campo("kimagen", "nombre='productoid=" + ROW.Item(0).ToString + "'"), 200).ImageUrl
            imgf.Height = Unit.Pixel(100) : imgf.ID = "img" + ROW.Item(8).ToString
            If imgf.ImageUrl = Nothing Then
                imgf.ImageUrl = "~/img/LogoOCCILLANTAS2024.jpeg"
            End If
            Dim kp As String = dspi.valor_campo("kproducto", "referencia='" + ROW.Item(0).ToString + "' and marca='" + ROW.Item(1).ToString + "' and diseno='" + ROW.Item(2).ToString + "' and aplicacion='" + ROW.Item(3).ToString + "' and posicion='" + ROW.Item(4).ToString + "'")
            imgf.PostBackUrl = "?" + fr.urlac + "&sfr=NUEVO PRODUCTO&id=" + kp
            Pnf1.Controls.Add(imgf)
            For x As Integer = 0 To tb.Columns.Count - 1
                Dim col As String = tb.Columns(x).ColumnName
                If col.Contains("precio") = False And col.Contains("descripcion") = False And col.Contains("disponible") = False And col.Contains("grupo") = False Then
                    Lb = New Label
                    Lb.Font.Size = FontUnit.Large
                    Lb.Text = "<b>" + col.ToUpper + ":" + "</b>" + ROW.Item(x).ToString.ToUpper + "</br>"
                    Pnf2.Controls.Add(Lb)
                ElseIf col.Contains("descripcion") Then
                ElseIf col.Contains("disponible") Then
                    Lb = New Label
                    Lb.Font.Size = FontUnit.Large
                    Lb.Text = "<b>" + col.ToUpper + ":" + "</b>" + ROW.Item(x).ToString + "</br>"
                    Pnf3.Controls.Add(Lb)
                ElseIf col.Contains("grupo") Then
                    Lb = New Label
                    Lb.Font.Size = FontUnit.Large
                    Lb.Text = "<b>" + col.ToUpper + ":" + "</b>" + ROW.Item(x).ToString + "</br>"
                    Pnf3.Controls.Add(Lb)
                Else
                    Lb = New Label
                    Lb.Font.Size = FontUnit.Large
                    Lb.Text = "<b>" + col.ToUpper + ":" + "</b>" + ROW.Item(x).ToString + "</br>"
                    Pnf3.Controls.Add(Lb)
                End If

            Next
            If lg.perfil > 1 Then
                'Pnf3.Controls.Add(BtPRODUCTO("editar", ROW.Item(0)))
            End If
            Dim TbC1, TbC2, TbC3 As New TableCell
            TbC1.BorderWidth = Unit.Pixel(1) : TbC2.BorderWidth = Unit.Pixel(1) : TbC3.BorderWidth = Unit.Pixel(1)
            TbC1.Controls.Add(Pnf1) : TbC2.Controls.Add(Pnf2) : TbC3.Controls.Add(Pnf3)
            TbR.Cells.Add(TbC1) : TbR.Cells.Add(TbC2) : TbR.Cells.Add(TbC3)
            TbR.BackColor = Drawing.Color.White
            TbINV.Rows.Add(TbR)
            TbINV.Width = Unit.Percentage(100) : TbINV.BorderWidth = Unit.Pixel(1)
            FRPN.Controls.Add(TbINV)
        Next

    End Sub
    Private Function BtPRODUCTO(NOMBRE As String, IDP As String) As Button
        BtPRODUCTO = New Button
        BtPRODUCTO.ID = "Bt" + NOMBRE.ToUpper + IDP
        BtPRODUCTO.CommandArgument = IDP
        BtPRODUCTO.Text = NOMBRE.ToUpper
        AddHandler BtPRODUCTO.Click, AddressOf bt_agregar
    End Function
    Private Function fr_producto(campos As String) As Panel
        fr_producto = New Panel
        fr_producto.EnableViewState = False
        Dim Tb As New Table : Tb.Width = Unit.Percentage(100)

        For Each str As String In campos.Split(",")
            Dim tbr As New TableRow
            Tb.Width = Unit.Percentage(100)
            Dim tbc1, tbc2 As New TableCell
            Lb = New Label : Lb.Font.Size = FontUnit.Large
            Lb.Text = str.Remove(0, 2)

            If str.Contains("Tn") Then
                Tx = New TextBox : Tx.ID = str
                Tx.Width = Unit.Percentage(90) : Tx.Text = 0 'Tx.TextMode = TextBoxMode.Number
                tbc1.Controls.Add(Lb)
                tbc2.Controls.Add(Tx)
            ElseIf str.Contains("Tx") Then
                Tx = New TextBox : Tx.ID = str
                Tx.Width = Unit.Percentage(90)
                tbc1.Controls.Add(Lb)
                tbc2.Controls.Add(Tx)
            ElseIf str.Contains("Dr") Then
                Dr = New DropDownList : Dr.Width = Unit.Percentage(90) : Dr.ID = str
                tbc1.Controls.Add(Lb) : tbc2.Controls.Add(Dr)
            ElseIf str.Contains("Bt") Then
                Bt = New Button ': Bt.CssClass = "boton"
                Bt.Text = str.Remove(0, 2) : Bt.ID = str
                AddHandler Bt.Click, AddressOf bt_agregar
                Bt.Width = Unit.Percentage(100)
                Bt.BorderStyle = BorderStyle.None
                Bt.BackColor = Drawing.Color.Red
                Bt.ForeColor = Drawing.Color.White
                Bt.Font.Bold = True
                If lg.perfil > 1 Then
                    valctr = True
                    tbc2.Controls.Add(Bt)
                    'fr_producto.Controls.Add(Bt)
                End If
            ElseIf str.Contains("Tl") Then
                PnTL.Width = Unit.Percentage(100)
                Lb = New Label : Lb.ID = "LbTI"
                Lb.Text = "<h1>" + str.Remove(0, 2).ToUpper + "</h1>"
                PnTL.Controls.Add(Lb)
            End If
            tbr.Cells.Add(tbc1) : tbr.Cells.Add(tbc2)
            Tb.Rows.Add(tbr)
        Next
        fr_producto.Controls.Add(PnTL)
        fr_producto.Controls.Add(Tb)
        fr_producto.Controls.Add(PnBT)
    End Function

    Private Function pnfiltrop(campos As String) As Panel
        pnfiltrop = PnFP
        If pnfiltrop Is Nothing Then
            pnfiltrop = New Panel
            pnfiltrop.ID = "PnFP"
        End If
        For Each srow As String In campos.Split(",")
            Dr = FRPN.FindControl("DrF" + srow.ToUpper)
            If Dr Is Nothing Then
                Dr = New DropDownList
                Dr.ID = "DrF" + srow.ToUpper
                Dr.Width = Unit.Percentage(100 / campos.Split(",").Count)
                If fr.SESION_GH("crf") IsNot Nothing And srow <> "REFERENCIA" Then
                    crf = fr.SESION_GH("crf")
                Else
                    crf = Nothing
                End If
                Dim X As Integer = dsinvd.Carga_tablas(crf, srow, srow, True).Rows.Count
                For Each row As DataRow In dsinvd.Carga_tablas(crf, srow, srow, True).Rows
                    If row.IsNull(0) = False Then
                        Dr.Items.Add(New ListItem(row.Item(0), srow + "='" + row.Item(0) + "'"))
                        If crf Is Nothing And fr.SESION_GH("crf") Is Nothing Then
                            fr.SESION_GH("crf") = srow + "='" + row.Item(0) + "'"
                        End If
                    End If
                Next
                If srow <> "REFERENCIA" Then
                    Dr.Items.Insert(0, "TODOS")
                    If fr.SESION_GH("crf2") IsNot Nothing Then
                        If fr.SESION_GH("crf2").ToString.Contains(srow) Then
                            Dr.Items.FindByValue(fr.SESION_GH("crf2").ToString.Replace(" and ", "")).Selected = True
                        End If
                    End If
                Else
                    Try
                        Dr.SelectedIndex = -1
                        Dr.Items.FindByValue(fr.SESION_GH("crf")).Selected = True

                    Catch ex As Exception

                    End Try

                End If
                Dr.AutoPostBack = True
                AddHandler Dr.SelectedIndexChanged, AddressOf seldrf
            End If

            pnfiltrop.Controls.Add(Dr)
        Next
        PnFP = pnfiltrop
    End Function
    Private Sub seldrf(sender As Object, e As EventArgs)
        If PnFP Is Nothing Then
            Exit Sub
        End If
        Dim Dr As DropDownList = sender
        If Dr.SelectedItem.Value.Contains("referencia") Then
            fr.SESION_GH("crf") = Dr.SelectedItem.Value
            fr.SESION_GH("crf2") = Nothing
        ElseIf Dr.SelectedItem.Text = "TODOS" Then
            fr.SESION_GH("crf2") = Nothing
        Else
            fr.SESION_GH("crf2") = " and " + Dr.SelectedItem.Value
        End If


        fr.redir("?" + fr.urlac)
        '    Catch ex As Exception

        '    End Try

        'Next
    End Sub
    Private Function PnPR() As Panel
        PnPR = New Panel
        If fr.reque("id") IsNot Nothing Then
            Dim TlBT As String = "INVENTARIO,PRODUCTO,FICHA TECNICA,FOTOS"
            For Each str As String In TlBT.Split(",")
                Bt = New Button
                Bt.Width = Unit.Pixel(250)
                Bt.Text = str : Bt.BackColor = Drawing.Color.Red : Bt.ForeColor = Drawing.Color.White
                AddHandler Bt.Click, AddressOf bt_PnPR
                PnPR.BackColor = Drawing.Color.Red
                PnPR.Controls.Add(Bt)
            Next
        End If
    End Function
    Private Sub bt_PnPR(sender As Object, e As EventArgs)
        Dim BtP As Button = sender
        Dim SFR, idp As String
        idp = fr.reque("id")
        Select Case BtP.Text
            Case "PRODUCTO"
                SFR = "&sfr=NUEVO PRODUCTO&id=" + fr.reque("id")
            Case "FICHA TECNICA"
                SFR = "&sfr=NUEVO IPRODUCTO&pl=" + dspi.valor_campo("PLANTILLA", "kproducto=" + fr.reque("id")) + "&id=" + fr.reque("id")
            Case "FOTOS"
                SFR = "&sfr=FOTOS&id=" + fr.reque("id")

        End Select
        If fr.urlac.Contains("&ct=") = True Then
            SFR = "&ct=" + fr.reque("ct") + SFR
        End If
        fr.redir("?fr=INVENTARIO" + SFR)
    End Sub
    Private Sub nuevo_pr()
        'fr = New ClassConstructor22(FRPN)
        Dim IDp As String = fr.reque("id")
        If fr.urla = "ventana.aspx" Then
            FRPN = _fr
        End If
        If FRPN Is Nothing Then
            Exit Sub
        End If
        FRPN.EnableViewState = False
        FRPN.Controls.Clear()
        FRPN.HorizontalAlign = HorizontalAlign.Center
        FRPN.Controls.Add(PnPR)

        FRPN.Controls.Add(fr_producto("TlPRODUCTO,DrPLANTILLA,DrGRUPO,TxREFERENCIA,TxDISEÑO,TxMARCA,TxAPLICACION,TxPOSICION,TnPRECIO_CONTADO,TnPRECIO_CREDITO,BtSIGUIENTE"))
        fr.DrPARAMETROS2("DrPLANTILLA", "INVENTARIO", "PLANTILLA") = Nothing
        fr.DrPARAMETROS2("DrGRUPO", "CLIENTE", "LLANTA INTERES") = Nothing
        If IDp IsNot Nothing Then
            fr.FR_CONTROL("DrPLANTILLA") = "V=" + dspi.valor_campo("PLANTILLA", "KPRODUCTO=" + IDp)
            fr.FR_CONTROL("DrGRUPO") = "V=" + dspi.valor_campo("GRUPO", "KPRODUCTO=" + IDp)
            fr.FR_CONTROL("TxREFERENCIA") = dspi.valor_campo("REFERENCIA", "KPRODUCTO=" + IDp)
            fr.FR_CONTROL("TxDISEÑO") = dspi.valor_campo("DISENO", "KPRODUCTO=" + IDp)
            fr.FR_CONTROL("TxMARCA") = dspi.valor_campo("MARCA", "KPRODUCTO=" + IDp)
            fr.FR_CONTROL("TxAPLICACION") = dspi.valor_campo("APLICACION", "KPRODUCTO=" + IDp)
            fr.FR_CONTROL("TxPOSICION") = dspi.valor_campo("POSICION", "KPRODUCTO=" + IDp)
            fr.FR_CONTROL("TnPRECIO_CONTADO") = FormatNumber(dspi.valor_campo("PRECIO_CONTADO", "KPRODUCTO=" + IDp).Replace(".0000", ""), 0)
            fr.FR_CONTROL("TnPRECIO_CREDITO") = FormatNumber(dspi.valor_campo("PRECIO_CREDITO", "KPRODUCTO=" + IDp).Replace(".0000", ""), 0)
            consulta_inventario("REFERENCIA='" + dspi.valor_campo("REFERENCIA", "KPRODUCTO=" + IDp) + "'")
            If fr.urla = "ventana.aspx" Then
                FRPN.Controls.Add(fr_producto("TnCANTIDAD,DrPRECIO"))
                fr.FR_CONTROL("DrPRECIO") = "CONTADO,CREDITO"
                Bt = New Button
                Bt.Text = "COTIZAR" : Bt.Font.Size = FontUnit.XLarge
                AddHandler Bt.Click, AddressOf sel_cotizar
                FRPN.Controls.Add(Bt)
            End If
        Else

        End If

    End Sub

    Private Sub sel_cotizar()
        Dim kcot As String = fr.reque("ct")
        If kcot IsNot Nothing Then

            Dim RF, MA, MD, DS, CA, PR, TL, PS, PG, AP As String
            RF = fr.FR_CONTROL("TxREFERENCIA") : PS = fr.FR_CONTROL("TxPOSICION") : PG = fr.FR_CONTROL("DrPRECIO") : AP = fr.FR_CONTROL("TxAPLICACION")

            RF = fr.FR_CONTROL("TxREFERENCIA") : MA = fr.FR_CONTROL("TxMARCA") : MD = RF.Split("R")(0) : DS = fr.FR_CONTROL("TxDISEÑO")
            CA = fr.FR_CONTROL("TnCANTIDAD") : PR = fr.FR_CONTROL("TnPRECIO_" + fr.FR_CONTROL("DrPRECIO"))


            If CA.Length > 0 And CA <> "0" And PR.Length > 0 And PR <> "0" Then
                If VALIDAR_INVENTARIO(CA, RF + MA + DS) = False Then
                    fr.alerta("LA CANTIDAD ES MAYOR AL DISPONIBLE")
                End If
                TL = (CInt(CA) * CInt(PR))

                dsct.actualizardb("referencia='" + RF + "',posicion='" + PS + "',fpago='" + PG + "',tterreno='" + AP + "'", "kcot=" + kcot)
                dsitc.insertardb(kcot + ",'" + RF + "','" + MA + "','" + MD + "','" + DS + "'," + CA + "," + PR + "," + TL)
                fr.rewrite("window.opener.location.reload()")
                fr.rewrite("window.close()")
            Else
                fr.alerta("EL CAMPO CANTIDAD Y PRECIO NO PUEDEN ESTAR VACIOS")
                End If

                'fr.rewrite("window.close()")
            End If
    End Sub
    Private Sub nuevo_iproducto()
        If fr.urla = "ventana.aspx" Then
            FRPN = _fr
        End If
        FRPN.Controls.Clear()
        FRPN.Controls.Add(PnPR)
        Dim CPLANTILLA As String = Nothing
        For Each ROW As DataRow In dspa.Carga_tablas("formulario='INVENTARIO' and criterio='" + fr.reque("pl") + "'").Rows
            'If CPLANTILLA IsNot Nothing Then
            '    CPLANTILLA += ","
            'End If
            CPLANTILLA += ",Tx" + ROW.Item("valor")
        Next
        FRPN.Controls.Add(fr_producto("TlFICHA TECNICA" + CPLANTILLA + ",BtFOTOS"))
        If fr.reque("id") IsNot Nothing And CPLANTILLA IsNot Nothing Then
            Dim ftec As String = Nothing
            For Each srow As String In dspi.valor_campo("descripcion", "kproducto=" + fr.reque("id")).Split("<br>")
                ftec = srow
                If ftec.Length > 1 Then
                    fr.FR_CONTROL("Tx" + ftec.Split(":")(0).Replace("br>", "")) = ftec.Split(":")(1)
                End If
            Next
        End If
    End Sub
    Private Sub nueva_foto()
        If fr.urla = "ventana.aspx" Then
            FRPN = _fr
        End If
        FRPN.Controls.Clear()
        FRPN.Controls.Add(PnPR)
        Lb = New Label
        Lb.Text = "<H1>FOTOS</H1>"
        FRPN.Controls.Add(Lb)
        If lg.perfil > 1 Then
            Dim fL As New FileUpload
            fL.ID = "FlFOTO"
            FRPN.Controls.Add(fL)

            Bt = New Button : Bt.ID = "BtNFOTO" : Bt.Text = "AGREGAR FOTO"
            AddHandler Bt.Click, AddressOf bt_agregar
            FRPN.Controls.Add(Bt)
            Lb = New Label : Lb.Text = "<hr>"
            FRPN.Controls.Add(Lb)
        End If


        For Each row As DataRow In dsim.Carga_tablas("nombre='productoid=" + fr.reque("id") + "'").Rows
            Dim img As New Image
            img = dsim.imagendb(row.Item("kimagen"), 600)
            FRPN.Controls.Add(img)
        Next
        Lb = New Label : Lb.Text = "<hr>"
        FRPN.Controls.Add(Lb)
    End Sub
    Private Sub ingreso_producto()
        dsvdp.vistatb("v_invdis", "prodis d", "proinv i", "d.*,i.grupo,i.plantilla,i.referencia", "d.kproducto=i.kproducto")
        frp = New ClassConstructor22(FRPN)
        frp.FORMULARIO_GR("productos", "GrIPR", "kproducto-K,grupo-BT,referencia-BT,diseno-BT,marca-BT", Nothing, "proinv", evento:=AddressOf sel_GrIPR)
    End Sub
    Private Sub sel_GrIPR()
        fr.redir("?fr=ADD_PRODUCTO&id=" + frp.FR_CONTROL("GrIPR"))
    End Sub
    Private Sub ADD_PRODUCTO()
        Dim TlP As String = "" ' "<H1>"
        frp = New ClassConstructor22(_fr)
        TlP += dspi.valor_campo("GRUPO", "KPRODUCTO=" + fr.reque("id")) + "<br>"
        TlP += dspi.valor_campo("REFERENCIA", "KPRODUCTO=" + fr.reque("id")) + "<br>"
        TlP += dspi.valor_campo("DISENO", "KPRODUCTO=" + fr.reque("id")) + "<br>"
        TlP += dspi.valor_campo("MARCA", "KPRODUCTO=" + fr.reque("id")) + "<br>"
        fr.FORMULARIO(TlP, "DrBODEGAS,TxCANTIDAD", True,, lg.MODULOS)
        fr.FR_CONTROL("DrBODEGAS") = frp.DrPARAMETROS("INVENTARIO", "BODEGA")
        fr.FR_CONTROL("BtGUARDAR", evento:=AddressOf bt_agregar) = "AGREGAR INVENTARIO"
        fr.FORMULARIO_GR("INGRESOS", "GrIINV", "FECHA;FINGRESO-D,BODEGA,CANTIDAD", Nothing, "v_invdis", "kproducto=" + fr.reque("id"), SUBM_FR:=True)
    End Sub
#End Region

#Region "PARAMETROS"
    Private Shared fcriterio, ccampo As String
    Private Function fr_agregar() As Panel
        FRPN.Controls.Clear()
        fr_agregar = New Panel
        Dim Tx As New TextBox
        Tx.ID = "TxVALOR"
        Tx.Width = Unit.Percentage(90)
        Dim BtA As New Button
        BtA.Text = "AGREGAR"
        BtA.CssClass = "boton"
        AddHandler BtA.Click, AddressOf bt_agregar
        Dim BtE As New Button
        BtE.Text = "ELIMINAR"
        BtE.CssClass = "boton"
        fr_agregar.Controls.Add(Tx)
        fr_agregar.Controls.Add(BtA)
        fr_agregar.Controls.Add(BtE)
    End Function
    Private Sub bt_agregar(sender As Object, e As EventArgs)
        Bt = sender
        Dim sfr As String = Nothing
        If fr.reque("sfr") IsNot Nothing Then
            sfr = "&sfr=" + fr.reque("sfr")
        End If
        If fr.reque("sfr") = "NUEVA PLANTILLA" Then
            dspa.insertardb("'INVENTARIO','PLANTILLA','" + fr.FR_CONTROL("TxVALOR") + "'", True)
        ElseIf fr.reque("sfr") = "NUEVA BODEGA" Then
            dspa.insertardb("'INVENTARIO','BODEGA','" + fr.FR_CONTROL("TxVALOR") + "'", True)
        ElseIf fr.reque("sfr") = "PLANTILLA" Then
            dspa.insertardb("'INVENTARIO','" + fr.reque("pl") + "','" + fr.FR_CONTROL("TxVALOR") + "'", True)
            sfr += "&pl=" + fr.reque("pl")
        ElseIf fr.reque("sfr") = "NUEVO PRODUCTO" Then
            Dim PL, RF, DS, MA, CO, CR, GR, AP, PS As String
            RF = fr.FR_CONTROL("TxREFERENCIA", VALIDAR:=True) : DS = fr.FR_CONTROL("TxDISEÑO", VALIDAR:=True) : MA = fr.FR_CONTROL("TxMARCA", VALIDAR:=True)
            CO = fr.FR_CONTROL("TnPRECIO_CONTADO", VALIDAR:=True) : CR = fr.FR_CONTROL("TnPRECIO_CREDITO", VALIDAR:=True) : GR = fr.FR_CONTROL("DrGRUPO") : PL = fr.FR_CONTROL("DrPLANTILLA")
            AP = fr.FR_CONTROL("TxAPLICACION") : PS = fr.FR_CONTROL("TxPOSICION")
            If valctr = True Then
                If fr.validacion_ct = False Then
                    sfr = "&sfr=NUEVO IPRODUCTO&pl=" + PL
                    If fr.reque("id") Is Nothing Then
                        If dspi.valor_campo("KPRODUCTO", "REFERENCIA='" + RF + "' and DISENO='" + DS + "' and MARCA='" + MA + "' and GRUPO='" + GR + "' and PLANTILLA='" + PL + "'") = Nothing Then
                            dspi.insertardb("'" + RF + "','" + DS + "','" + MA + "',''," + CO + "," + CR + ",'" + GR + "',0,'" + PL + "','" + AP + "','" + PS + "'", True)
                            sfr += "&id=" + dspi.valor_campo("KPRODUCTO", "REFERENCIA='" + RF + "' and DISENO='" + DS + "' and MARCA='" + MA + "' and GRUPO='" + GR + "' and PLANTILLA='" + PL + "'")
                        Else
                            fr.alerta("El producto ya existe")
                            Exit Sub
                        End If
                    Else
                        dspi.actualizardb("REFERENCIA='" + RF + "', DISENO='" + DS + "', MARCA='" + MA + "', GRUPO='" + GR + "',precio_contado=" + CO + ",precio_credito=" + CR + ", PLANTILLA='" + PL + "',aplicacion='" + AP + "',posicion='" + PS + "'", "kproducto=" + fr.reque("id"))
                        sfr += "&id=" + fr.reque("id")
                    End If
                Else
                    'fr.alerta("Todos los campos son obligatorios")
                    Exit Sub
                End If
            End If
        ElseIf fr.reque("sfr") = "FOTOS" Then
            nueva_foto()
        End If
        Select Case Bt.Text
            Case "EDITAR"
                sfr = "&sfr=NUEVO%20PRODUCTO&id=" + Bt.CommandArgument
            Case "PRODUCTO"
                If valctr = True Then
                    add_ftecnica()
                End If
                sfr = "&sfr=NUEVO PRODUCTO&id=" + fr.reque("id")
            Case "FOTOS"
                If valctr = True Then
                    add_ftecnica()
                End If
                sfr = "&sfr=FOTOS&id=" + fr.reque("id")
            Case "AGREGAR FOTO"
                dspi.Addimagen("PRODUCTOID=" + fr.reque("id"), _fr.FindControl("FlFOTO"))
                sfr = "&sfr=FOTOS&id=" + fr.reque("id")
            Case "VER"
                sfr = "&sfr=FOTOS&id=" + Bt.CommandName
            Case "AGREGAR INVENTARIO"
                sfr = "&sfr=ADD_PRODUCTO&id=" + fr.reque("id")
                dspd.insertardb(fr.reque("id") + ",'" + fr.HOY_FR + "','" + frp.FR_CONTROL("DrBODEGAS") + "'," + frp.FR_CONTROL("TxCANTIDAD") + "," + frp.FR_CONTROL("TxCANTIDAD"))
                fr.redir("?" + fr.urlac)
        End Select
        If fr.urlac.Contains("&ct=") = True Then
            sfr = "&ct=" + fr.reque("ct") + sfr
        End If
        fr.redir("?fr=INVENTARIO" + sfr)

    End Sub
    Private Sub add_ftecnica()
        Dim ftec As String = Nothing
        For Each ROW As DataRow In dspa.Carga_tablas("formulario='INVENTARIO' and criterio='" + fr.reque("pl") + "'").Rows
            ftec += "<br>" + ROW.Item("valor") + ":" + fr.FR_CONTROL("Tx" + ROW.Item("valor").ToString.ToUpper)
        Next
        dspi.actualizardb("descripcion='" + ftec + "'", "kproducto=" + fr.reque("id"))
    End Sub
    Private Sub sel_grp()
        fr.redir("?fr=INVENTARIO&sfr=PLANTILLA&pl=" + frp.FR_CONTROL("GrPL"))
    End Sub
    Private Sub nueva_plantilla()
        FRPN.Controls.Add(fr_agregar)
        frp.FORMULARIO_GR("NUEVA PLANTILLA", "GrPL", "VALOR-K,plantilla;valor-BT", Nothing, "parametros", "FORMULARIO='INVENTARIO' AND CRITERIO='PLANTILLA'", AddressOf sel_grp)
    End Sub
    Private Sub nuevo_iplantilla()
        FRPN.Controls.Add(fr_agregar)
        frp.FORMULARIO_GR("NUEVA ITEM PLANTILLA " + fr.reque("pl"), "GrIP", "ITEM;valor", Nothing, "parametros", "FORMULARIO='INVENTARIO' AND CRITERIO='" + fr.reque("pl") + "'")
    End Sub
    Private Sub nueva_bodega()
        FRPN.Controls.Add(fr_agregar)
        frp.FORMULARIO_GR("NUEVA BODEGA", "GrBD", "BODEGA;valor", Nothing, "parametros", "FORMULARIO='INVENTARIO' AND CRITERIO='BODEGA'")
    End Sub
    Private Sub carga_clases()
        For Each row As DataRow In dspa.Carga_tablas("formulario='cliente' and criterio='LLANTA INTERES'").Rows
            Dim BtN As New Button
            BtN.Text = row.Item("VALOR")
            BtN.Width = Unit.Percentage(100)
            Pn3.Controls.Add(BtN)
        Next
    End Sub

#End Region

End Class
