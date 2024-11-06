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
                'Pn2.Controls.Clear()
                Select Case fr.reque("sfr")
                    Case "NUEVO PRODUCTO"
                        nuevo_pr()
                    Case "NUEVA PLANTILLA"
                        nueva_plantilla()
                End Select
        End Select
    End Sub
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
        Dim FRP As Panel = _fr.FindControl("PnBOTONES")
        Dim TbINV As New Table : TbINV.Width = Unit.Percentage(100)
        Dim TbROW As New TableRow
        If fr.movil = False Then
            Dim TbCELL1 As New TableCell
            TbCELL1.Width = Unit.Percentage(20)
            TbCELL1.VerticalAlign = VerticalAlign.Top
            TbCELL1.Controls.Add(Pn1)
            Dim TbCELL2 As New TableCell
            TbCELL2.Controls.Add(Pn2)
            Dim TbCELL3 As New TableCell
            TbCELL3.Controls.Add(Pn3)
            TbROW.Cells.Add(TbCELL1)
            TbROW.Cells.Add(TbCELL2)
            TbROW.Cells.Add(TbCELL3)
            TbINV.Rows.Add(TbROW)
        End If
        FRP.Controls.Add(TbINV)
        productos()
    End Sub
    Dim frp As New ClassConstructor22(Pn2)
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
        For Each ROW As DataRow In dspi.Carga_tablas("disponible > 0").Rows

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
        Pn2.Controls.Add(fr_agregar)
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


    Private Sub carga_imagen()
        fr.FORMULARIO("PRODUCTO", "TxPRODUCTO,FiPRO,ImPRODUCTO", True)
        fr.FR_CONTROL("BtGUARDAR", evento:=AddressOf gimagen) = Nothing
        Dim imgp As Image = _fr.FindControl("ImPRODUCTO")
        imgp.ImageUrl = dsim.imagendb("inventario", "producto", "").ImageUrl

    End Sub
    Private Sub gimagen()
        dsim.Addimagen("inventario", "producto", "", _fr.FindControl("FiPRO"))
    End Sub





End Class
