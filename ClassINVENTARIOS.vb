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
            Case "PRODUCTO"
                carga_producto()
        End Select
    End Sub
    Private Sub carga_inventario()
        fr.FORMULARIO("INVENTARIO", "TxBUSCAR,BtBUSCAR", True)
        fr.FR_BOTONES("NUEVA_PLANTILLA,NUEVA_BODEGA,NUEVO_PRODUCTO")

        If lg.perfil > 0 Then
            fr.FR_CONTROL("BtNUEVO_PRODUCTO", evento:=AddressOf nuevo_pr) = Nothing
            fr.FR_CONTROL("BtNUEVA_PLANTILLA", evento:=AddressOf nueva_plantilla) = Nothing
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
            TbROW.Cells.Add(TbCELL1)
            TbROW.Cells.Add(TbCELL2)
            TbINV.Rows.Add(TbROW)
        End If
        FRP.Controls.Add(TbINV)
        carga_clases()
    End Sub
    Dim frp As New ClassConstructor22(Pn2)
    Dim frg As New ClassConstructor22(Pn2)
    Private Sub nuevo_pr()
        Dim CPLANTILLA As String = Nothing
        For Each ROW As DataRow In dspa.Carga_tablas("formulario='INVENTARIO' and criterio='LLANTA'").Rows
            CPLANTILLA += ",Tx" + ROW.Item("valor")
        Next
        frp.FORMULARIO("NUEVO PRODUCTO", "DrTIPO,DrGRUPO,DrBODEGA,TxREFERENCIA,TxDISEÑO,TxMARCA" + CPLANTILLA, True)
    End Sub
    Private Sub nueva_plantilla()
        frp.FORMULARIO("NUEVA PLANTILLA", "TxNOMBRE", True)
        Dim GrPL As New GridView
        frp.CARGAD_GR(GrPL, dspa.Carga_tablas())
        Pn2.Controls.Add(GrPL)
    End Sub
    Private Sub nueva_bodega()
        frp.FORMULARIO("NUEVA BODEGA", "TxNOMBRE", True)
    End Sub
    Private Sub carga_clases()

        For Each row As DataRow In dspa.Carga_tablas("formulario='cliente' and criterio='LLANTA INTERES'").Rows
            Dim BtN As New Button
            BtN.Text = row.Item("VALOR")
            BtN.Width = Unit.Percentage(100)
            Pn1.Controls.Add(BtN)
        Next
    End Sub

    Private Sub carga_producto()

    End Sub
    Private Sub carga_imagen()
        fr.FORMULARIO("PRODUCTO", "TxPRODUCTO,FiPRO,ImPRODUCTO", True)
        fr.FR_CONTROL("BtGUARDAR", evento:=AddressOf gimagen) = Nothing
        Dim imgp As Image = _fr.FindControl("ImPRODUCTO")
        imgp.ImageUrl = dsim.imagendb("inventario", "producto", "").ImageUrl

        '_fr.Controls.Add(imgp)
        'fr.FR_CONTROL("ImPRODUCTO") = dsim.imagendb("inventario", "producto", "").ImageUrl.ToString
    End Sub
    Private Sub gimagen()
        dsim.Addimagen("inventario", "producto", "", _fr.FindControl("FiPRO"))
    End Sub





End Class
