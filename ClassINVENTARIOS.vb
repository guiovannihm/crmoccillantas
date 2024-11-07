Imports System.IO
Imports Classcatalogoch
Imports System.Data.SqlClient

Public Class ClassINVENTARIOS
    Private Shadows _fr As Panel
    Private fr As ClassConstructor22
    Private lg As New ClassLogin
    Private dsim As New carga_dssql("imagenes")
    Private dspi As New carga_dssql("proinv")
    Private dspa As New carga_dssql("parametros")
    Private Pn1, Pn2, Pn3 As New Panel

    Sub New(Panelfr As Panel)
        _fr = Panelfr
        fr = New ClassConstructor22(_fr)
        dsim.campostb = "kimagen-key,nombre-varchar(250),foto-image"
        dspi.campostb = "kproducto-key,referencia-varchar(250),diseno-varchar(250),marca-varchar(250),descripcion-varchar(250),precio_contado-money,precio_credito-money,grupo-varchar(250),bodega-varchar(250),disponible-bigint"
        Select Case fr.reque("fr")
            Case "INVENTARIO"
                carga_inventario()
                Select Case fr.reque("sfr")
                    Case "NUEVO PRODUCTO"
                        nuevo_pr()
                    Case "NUEVA PLANTILLA"
                        nueva_plantilla()
                End Select
        End Select
    End Sub
    Private FRPN As Panel
    Private Sub carga_inventario()
        fr.FORMULARIO("INVENTARIO", "TxBUSCAR,BtBUSCAR", True)
        fr.FR_BOTONES("NUEVA_PLANTILLA,NUEVA_BODEGA,NUEVO_PRODUCTO")

        If lg.perfil > 0 Then
            fr.FR_CONTROL("BtNUEVO_PRODUCTO", evento:=AddressOf sel_bt) = Nothing
            fr.FR_CONTROL("BtNUEVA_PLANTILLA", evento:=AddressOf sel_bt) = Nothing
            fr.FR_CONTROL("BtNUEVA_BODEGA", evento:=AddressOf nueva_bodega) = Nothing
            fr.FR_CONTROL("BtGUARDAR") = "INGRESO PRODUCTO"
        Else
            fr.FR_CONTROL("BtGUARDAR", False) = "NUEVO PRODUCTO"
        End If
        FRPN = _fr.FindControl("PnBOTONES")
        Dim TbINV As New Table : TbINV.Width = Unit.Percentage(100)
        'TbINV.BackColor = Drawing.Color.White : TbINV.BorderWidth = Unit.Pixel(1)


        If fr.movil = False Then
            'Dim TbROW As New TableRow
            'Dim TbCELL1 As New TableCell
            'TbCELL1.Width = Unit.Percentage(20)
            'TbCELL1.VerticalAlign = VerticalAlign.Top
            'TbCELL1.Controls.Add(Pn1) : TbCELL1.VerticalAlign = VerticalAlign.Middle
            'Dim TbCELL2 As New TableCell
            'TbCELL2.Controls.Add(Pn2) : TbCELL2.VerticalAlign = VerticalAlign.Top
            'Dim TbCELL3 As New TableCell
            'TbCELL3.Controls.Add(Pn3) : TbCELL3.VerticalAlign = VerticalAlign.Top
            'TbROW.Cells.Add(TbCELL1)
            'TbROW.Cells.Add(TbCELL2)
            'TbROW.Cells.Add(TbCELL3)
            'TbCELL1.BorderWidth = Unit.Pixel(1) : TbCELL2.BorderWidth = Unit.Pixel(1)
            'TbINV.Rows.Add(productos())
        End If
        productos()
    End Sub
    Dim frp As New ClassConstructor22(FRPN)
    Private Sub sel_bt(sender As Object, E As EventArgs)
        Dim BtS As Button = sender
        fr.redir("?fr=INVENTARIO&sfr=" + BtS.Text)
        Select Case BtS.Text
            Case "NUEVO PARAMETRO"

        End Select
    End Sub

#Region "producto"
    Private Dr As DropDownList
    Private Lb As Label
    Private Tx As TextBox

    Private Sub lista_producto()

    End Sub
    Private Sub productos()
        FRPN = _fr.FindControl("PnBOTONES")
        Dim TbINV As New Table : TbINV.Width = Unit.Percentage(100)
        Dim tb As DataTable = dspi.Carga_tablas("disponible > 0", "disponible desc")
        For Each ROW As DataRow In tb.Rows
            Dim imgf As New Image
            Dim TbR As New TableRow
            Dim Pnf1, Pnf2, Pnf3 As New Panel
            'carga_imagen("producto=" + ROW.Item(0).ToString, imgf)
            If imgf.ImageUrl = Nothing Then
                imgf.ImageUrl = "~/img/LogoOCCILLANTAS2024.jpeg"
            End If
            Pnf1.Controls.Add(imgf)

            For x As Integer = 1 To tb.Columns.Count - 1
                Dim col As String = tb.Columns(x).ColumnName
                If col.Contains("precio") = False And col.Contains("disponible") = False Then
                    Lb = New Label
                    Lb.Font.Size = FontUnit.Large
                    Lb.Text = "<b>" + col.ToUpper + ":" + "</b>" + ROW.Item(x).ToString.ToUpper + "</br>"
                    Pnf2.Controls.Add(Lb)
                ElseIf col.Contains("disponible") Then
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
                'Pnf1.BorderWidth = Unit.Pixel(1) : Pnf2.BorderWidth = Unit.Pixel(1) : Pnf3.BorderWidth = Unit.Pixel(1)
                'Pnf1.Width = Unit.Percentage(40) : Pnf2.Width = Unit.Percentage(40) : Pnf3.Width = Unit.Percentage(40)

            Next
            Dim TbC1, TbC2, TbC3 As New TableCell
            TbC1.BorderWidth = Unit.Pixel(1) : TbC2.BorderWidth = Unit.Pixel(1) : TbC3.BorderWidth = Unit.Pixel(1)
            TbC1.Controls.Add(Pnf1) : TbC2.Controls.Add(Pnf2) : TbC3.Controls.Add(Pnf3)
            TbR.Cells.Add(TbC1) : TbR.Cells.Add(TbC2) : TbR.Cells.Add(TbC3)
            TbR.BackColor = Drawing.Color.White
            'Pn2.Width = Unit.Percentage(100)
            'Pn1.Controls.Add(Pnf1) : Pn2.Controls.Add(Pnf2) : Pn2.Controls.Add(Pnf3)
            TbINV.Rows.Add(TbR)
            TbINV.Width = Unit.Percentage(100) : TbINV.BorderWidth = Unit.Pixel(1)
            FRPN.Controls.Add(TbINV)
        Next

    End Sub
    Private Function fr_producto(campos As String) As Panel
        fr_producto = New Panel
        Dim Tb As New Table : Tb.Width = Unit.Percentage(100)

        For Each str As String In campos.Split(",")
            Dim tbr As New TableRow
            Dim tbc1, tbc2 As New TableCell
            Lb = New Label
            Lb.Text = str.Remove(0, 2)
            tbc1.Controls.Add(Lb)
            If str.Contains("Tx") Then
                Tx = New TextBox
                tbc2.Controls.Add(Tx)
            ElseIf str.Contains("Dr") Then
                Dr = New DropDownList
                tbc2.Controls.Add(Dr)
            End If
            tbr.Cells.Add(tbc1) : tbr.Cells.Add(tbc2)
            Tb.Rows.Add(tbr)
        Next
        fr_producto.Controls.Add(Tb)
    End Function
    Private Sub nuevo_pr()

        Dim CPLANTILLA As String = Nothing
        For Each ROW As DataRow In dspa.Carga_tablas("formulario='INVENTARIO' and criterio='LLANTA'").Rows
            CPLANTILLA += ",Tx" + ROW.Item("valor")
        Next
        Pn2.Controls.Add(fr_producto("DrPLANTILLA,DrGRUPO,DrBODEGA,TxREFERENCIA,TxDISEÑO,TxMARCA"))
    End Sub

#End Region

#Region "PARAMETROS"
    Private Shared fcriterio, ccampo As String
    Private Function fr_agregar() As Panel
        fr_agregar = New Panel
        Dim Tx As New TextBox
        Tx.Width = Unit.Percentage(90)
        Dim BtA As New Button
        BtA.Text = "AGREGAR"
        BtA.CssClass = "boton"
        Dim BtE As New Button
        BtE.Text = "ELIMINAR"
        BtE.CssClass = "boton"
        fr_agregar.Controls.Add(Tx)
        fr_agregar.Controls.Add(BtA)
        fr_agregar.Controls.Add(BtE)
    End Function
    Private Sub sel_grp()
        fcriterio = dspa.valor_campo("valor", "kparametro=" + frp.FR_CONTROL("GrPL"))
        Pn2.Controls.Clear()
        Pn2.Controls.Add(fr_agregar)
        frp.FORMULARIO_GR("PLANTILLA", "GrPL", "plantilla;valor-BT", Nothing, "parametros", "FORMULARIO='INVENTARIO' AND CRITERIO='" + fcriterio + "'")
    End Sub
    Private Sub nueva_plantilla()
        FRPN.Controls.Add(fr_agregar)
        frp.FORMULARIO_GR("NUEVA PLANTILLA", "GrPL", "kparametro-K,plantilla;valor-BT", Nothing, "parametros", "FORMULARIO='INVENTARIO' AND CRITERIO='PLANTILLA'", AddressOf sel_grp)
    End Sub
    Private Sub nueva_bodega()
        Pn2.Controls.Add(fr_agregar)
        frp.FORMULARIO_GR("NUEVA BODEGA", "GrBD", "BODEGA;valor-BT", Nothing, "parametros", "FORMULARIO='INVENTARIO' AND CRITERIO='BODEGA'")
    End Sub
    Private Sub carga_clases()
        For Each row As DataRow In dspa.Carga_tablas("formulario='cliente' and criterio='LLANTA INTERES'").Rows
            Dim BtN As New Button
            BtN.Text = row.Item("VALOR")
            BtN.Width = Unit.Percentage(100)
            Pn2.Controls.Add(BtN)
        Next
    End Sub

#End Region


    Private Sub carga_imagen(nombre As String, imgp As Image)
        'fr.FORMULARIO("PRODUCTO", "TxPRODUCTO,FiPRO,ImPRODUCTO", True)
        'fr.FR_CONTROL("BtGUARDAR", evento:=AddressOf gimagen) = Nothing
        imgp.ImageUrl = dsim.imagendb(nombre).ImageUrl

    End Sub
    Private Sub gimagen()
        dsim.Addimagen("inventario", "producto", "", _fr.FindControl("FiPRO"))
    End Sub





End Class
