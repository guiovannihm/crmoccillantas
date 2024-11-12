Imports System.IO
Imports Classcatalogoch
Imports System.Data.SqlClient
Imports System.Net.WebRequestMethods

Public Class ClassINVENTARIOS
    Private Shadows _fr As Panel
    Private fr, frp As ClassConstructor22
    Private lg As New ClassLogin
    Private dsim As New carga_dssql("imagenes")
    Private dspi As New carga_dssql("proinv")
    Private dspa As New carga_dssql("parametros")
    Private dspd As New carga_dssql("prodis")
    Private dsvdp As New carga_dssql("v_invdis")
    Private PnBT, PnTL, Pn3 As New Panel
    Private Shadows idimg As String

    Sub New(Panelfr As Panel)
        _fr = Panelfr
        '_fr.Controls.Clear()
        fr = New ClassConstructor22(_fr)

        dsim.campostb = "kimagen-key,nombre-varchar(250),foto-image"
        dspi.campostb = "kproducto-key,referencia-varchar(250),diseno-varchar(250),marca-varchar(250),descripcion-varchar(500),precio_contado-money,precio_credito-money,bodega-varchar(250),disponible-bigint,plantilla-varchar(50)"
        dspd.campostb = "kdispo-key,kproducto-bigint,fingreso-date,bodega-varchar(250),cantidad-bigint"
        VALIDAR_INVENTARIO()

        Select Case fr.reque("fr")
            Case "INVENTARIOS", "INVENTARIO"
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
                    Case "ADD_PRODUCTO"
                        ADD_PRODUCTO()
                End Select
        End Select
    End Sub
    Private Sub VALIDAR_INVENTARIO()
        For Each ROW As DataRow In dspi.Carga_tablas().Rows
            Dim x, y As Integer
            x = dspd.valor_campo_OTROS("sum(cantidad)", "KPRODUCTO=" + ROW.Item(0).ToString)
            Dim dsim As New carga_dssql("itemmo")
            y = dsim.valor_campo_OTROS("sum(cantidad)", "kproducto=" + ROW.Item(0).ToString)
            dspi.actualizardb("disponible=" + (x - y).ToString, "kproducto=" + ROW.Item(0).ToString)
        Next
    End Sub
    Private FRPN As Panel
    Private valctr As Boolean
#Region "INVENTARIO"
    Private Sub carga_inventario()
        fr.FORMULARIO("INVENTARIO", "TxBUSCAR,BtBUSCAR", True,, lg.MODULOS)
        If lg.perfil > 1 Then
            valctr = True
            fr.FR_BOTONES("NUEVA_PLANTILLA,NUEVA_BODEGA,NUEVO_PRODUCTO",, True)
            fr.FR_CONTROL("BtNUEVO_PRODUCTO", evento:=AddressOf sel_bt) = Nothing
            fr.FR_CONTROL("BtNUEVA_PLANTILLA", evento:=AddressOf sel_bt) = Nothing
            fr.FR_CONTROL("BtNUEVA_BODEGA", evento:=AddressOf sel_bt) = Nothing
            fr.FR_CONTROL("BtGUARDAR", evento:=AddressOf sel_bt) = "INGRESO PRODUCTO"
        Else
            fr.FR_CONTROL("BtGUARDAR", valctr) = "NUEVO PRODUCTO"
        End If
        FRPN = _fr.FindControl("PnBOTONES")
        frp = New ClassConstructor22(FRPN)
        Dim TbINV As New Table : TbINV.Width = Unit.Percentage(100)
        If fr.reque("sfr") Is Nothing Then
            productos()
        Else
            FRPN.Controls.Clear()
        End If

    End Sub

    Private Sub sel_bt(sender As Object, E As EventArgs)
        Dim BtS As Button = sender
        fr.redir("?fr=INVENTARIO&sfr=" + BtS.Text)
    End Sub
#End Region
#Region "producto"
    Private Dr As DropDownList
    Private Lb As Label
    Private Tx As TextBox
    Private Bt As Button
    Private Sub productos()
        FRPN = _fr.FindControl("PnBOTONES")
        Dim TbINV As New Table : TbINV.Width = Unit.Percentage(100)
        Dim tb As DataTable
        If lg.perfil > 1 Then
            tb = dspi.Carga_tablas(, "disponible desc")
        Else
            tb = dspi.Carga_tablas("disponible > 0", "disponible desc")
        End If

        For Each ROW As DataRow In tb.Rows
            Dim imgf As New ImageButton
            Dim TbR As New TableRow
            Dim Pnf1, Pnf2, Pnf3 As New Panel
            imgf.ImageUrl = dsim.imagendb(dsim.valor_campo("kimagen", "nombre='productoid=" + ROW.Item(0).ToString + "'"), 200).ImageUrl
            imgf.Height = Unit.Pixel(200) : imgf.ID = "img" + ROW.Item(0).ToString
            If imgf.ImageUrl = Nothing Then
                imgf.ImageUrl = "~/img/LogoOCCILLANTAS2024.jpeg"
            End If
            imgf.PostBackUrl = "?fr=INVENTARIO&sfr=NUEVO PRODUCTO&id=" + ROW.Item(0).ToString
            Pnf1.Controls.Add(imgf)
            For x As Integer = 1 To tb.Columns.Count - 1
                Dim col As String = tb.Columns(x).ColumnName
                If col.Contains("precio") = False And col.Contains("disponible") = False And col.Contains("grupo") = False Then
                    Lb = New Label
                    Lb.Font.Size = FontUnit.Large
                    Lb.Text = "<b>" + col.ToUpper + ":" + "</b>" + ROW.Item(x).ToString.ToUpper + "</br>"
                    Pnf2.Controls.Add(Lb)
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
                Pnf3.Controls.Add(BtPRODUCTO("editar", ROW.Item(0)))
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
                Bt = New Button : Bt.CssClass = "boton"
                Bt.Text = str.Remove(0, 2) : Bt.ID = str
                AddHandler Bt.Click, AddressOf bt_agregar
                If valctr = True Then
                    PnBT.Controls.Add(Bt)
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
        fr.redir("?fr=INVENTARIO" + SFR)
    End Sub
    Private Sub nuevo_pr()
        Dim IDp As String = fr.reque("id")
        FRPN.Controls.Add(PnPR)
        FRPN.Controls.Add(fr_producto("TlPRODUCTO,DrPLANTILLA,DrGRUPO,TxREFERENCIA,TxDISEÑO,TxMARCA,TnPRECIO_CONTADO,TnPRECIO_CREDITO,BtSIGUIENTE"))
        fr.DrPARAMETROS2("DrPLANTILLA", "INVENTARIO", "PLANTILLA") = Nothing
        fr.DrPARAMETROS2("DrGRUPO", "CLIENTE", "LLANTA INTERES") = Nothing
        If IDp IsNot Nothing Then
            fr.FR_CONTROL("DrPLANTILLA") = "V=" + dspi.valor_campo("PLANTILLA", "KPRODUCTO=" + IDp)
            fr.FR_CONTROL("DrGRUPO") = "V=" + dspi.valor_campo("GRUPO", "KPRODUCTO=" + IDp)
            fr.FR_CONTROL("TxREFERENCIA") = dspi.valor_campo("REFERENCIA", "KPRODUCTO=" + IDp)
            fr.FR_CONTROL("TxDISEÑO") = dspi.valor_campo("DISENO", "KPRODUCTO=" + IDp)
            fr.FR_CONTROL("TxMARCA") = dspi.valor_campo("MARCA", "KPRODUCTO=" + IDp)
            fr.FR_CONTROL("TnPRECIO_CONTADO") = FormatNumber(dspi.valor_campo("PRECIO_CONTADO", "KPRODUCTO=" + IDp).Replace(".0000", ""), 0)
            fr.FR_CONTROL("TnPRECIO_CREDITO") = FormatNumber(dspi.valor_campo("PRECIO_CREDITO", "KPRODUCTO=" + IDp).Replace(".0000", ""), 0)
        Else

        End If
    End Sub
    Private Sub nuevo_iproducto()
        FRPN.Controls.Add(PnPR)
        Dim CPLANTILLA As String = Nothing
        For Each ROW As DataRow In dspa.Carga_tablas("formulario='INVENTARIO' and criterio='" + fr.reque("pl") + "'").Rows
            'If CPLANTILLA IsNot Nothing Then
            '    CPLANTILLA += ","
            'End If
            CPLANTILLA += ",Tx" + ROW.Item("valor")
        Next
        FRPN.Controls.Add(fr_producto("TlFICHA TECNICA,BtPRODUCTO,BtFOTOS" + CPLANTILLA))
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
        frp.FORMULARIO_GR("productos", "GrIPR", "kproducto-K,grupo-BT,referencia-BT,diseno-BT,marca-BT", Nothing, "proinv", evento:=AddressOf sel_GrIPR)
    End Sub
    Private Sub sel_GrIPR()
        fr.redir("?fr=INVENTARIO&sfr=ADD_PRODUCTO&id=" + frp.FR_CONTROL("GrIPR"))
    End Sub
    Private Sub ADD_PRODUCTO()
        Dim TlP As String = "<H1>"

        TlP += dspi.valor_campo("GRUPO", "KPRODUCTO=" + fr.reque("id")) + "<br>"
        TlP += dspi.valor_campo("REFERENCIA", "KPRODUCTO=" + fr.reque("id")) + "<br>"
        TlP += dspi.valor_campo("DISENO", "KPRODUCTO=" + fr.reque("id")) + "<br>"
        TlP += dspi.valor_campo("MARCA", "KPRODUCTO=" + fr.reque("id")) + "<br>"
        TlP += "</H1>"
        Lb = New Label
        Lb.Text = TlP
        FRPN.Controls.Add(Lb)
        FRPN.Controls.Add(fr_producto("DrBODEGA,TxCANTIDAD,BtAGREGAR"))
        frp.FR_CONTROL("BtAGREGAR", evento:=AddressOf bt_PnPR) = "AGREGAR INVENTARIO"
        frp.DrPARAMETROS2("DrBODEGA", "INVENTARIO", "BODEGA") = Nothing
        frp.FORMULARIO_GR("INGRESOS", "GrIINV", "FECHA;FINGRESO-D,BODEGA,CANTIDAD", Nothing, "v_invdis", "kproducto=" + fr.reque("id"), SUBM_FR:=True)
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
            Dim PL, RF, DS, MA, CO, CR, GR As String
            RF = fr.FR_CONTROL("TxREFERENCIA", VALIDAR:=True) : DS = fr.FR_CONTROL("TxDISEÑO", VALIDAR:=True) : MA = fr.FR_CONTROL("TxMARCA", VALIDAR:=True)
            CO = fr.FR_CONTROL("TnPRECIO_CONTADO", VALIDAR:=True) : CR = fr.FR_CONTROL("TnPRECIO_CREDITO", VALIDAR:=True) : GR = fr.FR_CONTROL("DrGRUPO") : PL = fr.FR_CONTROL("DrPLANTILLA")
            If valctr = True Then
                If fr.validacion_ct = False Then
                    sfr = "&sfr=NUEVO IPRODUCTO&pl=" + PL
                    If fr.reque("id") Is Nothing Then
                        If dspi.valor_campo("KPRODUCTO", "REFERENCIA='" + RF + "' and DISENO='" + DS + "' and MARCA='" + MA + "' and GRUPO='" + GR + "' and PLANTILLA='" + PL + "'") = Nothing Then
                            dspi.insertardb("'" + RF + "','" + DS + "','" + MA + "',''," + CO + "," + CR + ",'" + GR + "',0,'" + PL + "'", True)
                            sfr += "&id=" + dspi.valor_campo("KPRODUCTO", "REFERENCIA='" + RF + "' and DISENO='" + DS + "' and MARCA='" + MA + "' and GRUPO='" + GR + "' and PLANTILLA='" + PL + "'")
                        Else
                            fr.alerta("El producto ya existe")
                            Exit Sub
                        End If
                    Else
                        dspi.actualizardb("REFERENCIA='" + RF + "', DISENO='" + DS + "', MARCA='" + MA + "', GRUPO='" + GR + "',precio_contado=" + CO + ",precio_credito=" + CR + ", PLANTILLA='" + PL + "'", "kproducto=" + fr.reque("id"))
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
                dspd.insertardb(fr.reque("id") + ",'" + fr.HOY_FR + "','" + frp.FR_CONTROL("DrBODEGA") + "'," + frp.FR_CONTROL("TxCANTIDAD"))
        End Select
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
