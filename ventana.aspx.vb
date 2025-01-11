Imports Classcatalogoch

Public Class ventana
    Inherits System.Web.UI.Page
    Private CT As ClassConstructor22
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CT = New ClassConstructor22(PnV)
        Try
            Dim IV As New ClassINVENTARIOS(PnV)
        Catch ex As Exception

        End Try
    End Sub

End Class