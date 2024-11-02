Imports System.IO
Imports Classcatalogoch
Imports System.Data.SqlClient

Public Class ClassINVENTARIOS
    Private Shadows _fr As Panel
    Private fr As ClassConstructor22
    Private dsim As New carga_dssql("imagenes")

    Sub New(Panelfr As Panel)
        _fr = Panelfr
        fr = New ClassConstructor22(_fr)
        dsim.campostb = "kimagen-key,nombre-varchar(250),foto-image"
        Select Case fr.reque("fr")
            Case "producto"
                carga_producto()
        End Select
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
