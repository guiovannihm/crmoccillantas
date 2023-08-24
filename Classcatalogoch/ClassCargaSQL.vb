Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports System.Data

Public Class ClassCargaSQL
    Private con As New SqlConnection
    Private buscar As New SqlCommand
    Private data As New SqlDataAdapter
    Private lectura As SqlDataReader
    Private ds As DataSet
    Private lberror As String
    Private _ruta As String
    Private tabla As String
    Private _col_tb As String
    Private _resultado As Integer
    Private context As Web.HttpContext = Web.HttpContext.Current
    Dim comando As String

#Region "DataAPP"
    'CARGA DATOS SQL
    Private Sub carga_tbapp()

    End Sub
    Private Function ruta() As String

        Return "data Source=catalogosch.mssql.somee.com;Initial Catalog=catalogosch;Persist Security Info=True;User ID=ventaschbg_SQLLogin_1;Password=5ggaby9hk2"
    End Function


#End Region

    Public Sub New(nombre_tabla As String)
        tabla = nombre_tabla
    End Sub

    Public Function Carga_tablas(ByVal criterio As String, Optional ORDEN As String = Nothing) As DataTable
        Try
            comando = Nothing
            ds = New DataSet
            ds.DataSetName = "DsCARGA"
            con.ConnectionString = ruta()
            con.Open()
            If criterio <> Nothing Then
                comando += " where " + criterio
            End If
            If ORDEN <> Nothing Then
                comando += " order by " + ORDEN
            End If
            buscar.CommandText = "Select * from " + tabla + comando
            buscar.Connection = con
            data.SelectCommand = buscar
            Dim builder As New SqlCommandBuilder(data)
            data.Fill(ds, tabla)
            data.Update(ds, tabla)
            con.Close()
        Catch ex As Exception
            lberror = ex.Message.Replace(",", " ").Replace(".", " ").Replace("'", "")
            con.Close()
        End Try
        Return ds.Tables(tabla)
    End Function



End Class
