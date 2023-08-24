Imports System.Web.UI.WebControls

Public Class ClassPARAMETROS

    Private Shared _ID, _FR, _CR, _VT As String
    Private DSPR As New carga_dssql("parametros")
    Sub New(FORMULARIO As String)
        _FR = FORMULARIO

    End Sub

    Private Sub CARGA_DATOS()

    End Sub
    Public ReadOnly Property VALOR As String
        Get

        End Get
    End Property
    Public Property ID As String
        Get
            Return _ID
        End Get
        Set(value As String)
            _ID = value
        End Set
    End Property

    Public ReadOnly Pr As PANEL


End Class
