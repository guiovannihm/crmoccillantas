Imports Classcatalogoch

Public Class ClassAPP
    Private cr As New ClassConstructor22
    Private dsctl As New carga_dssql("control_llamada")

    Sub New(fr As Panel)
        dsctl.campostb = "kllamada-key,fecha_llamada-datetime,numero-bigint,usuario-varchar(50)"
        Select Case cr.reque("fr")
            Case "usmovil"
                cr.redireccion("rllamada.aspx")
            Case "clapp"

        End Select
    End Sub
    Private Sub carga_cliente()

    End Sub

End Class
