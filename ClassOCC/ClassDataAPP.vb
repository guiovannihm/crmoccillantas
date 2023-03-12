Imports System.Web
Imports System.IO


Public Class ClassDataAPP

    Private ds As New DataSet
    Private context As HttpContext = HttpContext.Current
    Private enc As New ENCRIPTAR
    Private Shared npar As String = "\App_Data\DataAPP.xml"

    Private Sub carga_xml()
        Dim X As String = context.Server.MapPath("") + npar
        Dim fl As New IO.FileInfo(X)
        If fl.Exists = True Then
            ds.ReadXml(X)
        Else
            Dim dt As New DataTable
            dt.TableName = "ParametrosAPP"
            Dim dr As DataRow
            dr = dt.NewRow
            dt.Columns.Add(dtcol("Key"))
            dt.Columns.Add(dtcol("Valor1"))
            dt.Columns.Add(dtcol("Valor2"))
            dt.Columns.Add(dtcol("Valor3"))
            ds.Tables.Add(dt)
        End If
    End Sub

    Private Function dtcol(cnombre As String, Optional tipo As String = "System.String") As DataColumn
        dtcol = New DataColumn
        dtcol.ColumnName = cnombre
        dtcol.DataType = Type.GetType(tipo)
        Return dtcol
    End Function

    Public Sub escribir_xml()
        'ds.Parametros.AddParametrosRow(enc.enc(StrReverse(val1)), enc.enc(StrReverse(val2)), enc.enc(StrReverse(val3)))
        Dim X As String = context.Server.MapPath("")
        ds.WriteXml(X + npar)
    End Sub

    'Public Function valor_key(valor1 As String) As String
    '    carga_xml()
    '    For Each row As DataRow In ds.Parametros.Select("valor1='" + enc.enc(StrReverse(valor1)) + "'")
    '        Dim x As String = StrReverse(row.Item("valor1"))
    '        Return row.Item("key")
    '    Next
    '    Return ""
    'End Function

    Public ReadOnly Property dtparametros
        Get
            carga_xml()
            Return ds.Tables("ParametrosAPP")
        End Get
    End Property

    Public Sub addpropiedades(valor1 As String, valor2 As String, valor3 As String)
        Dim dt As DataTable = ds.Tables("ParametrosAPP")
        Dim dr As DataRow
        dr = dt.NewRow
        dr("key") = dt.Rows.Count + 1
        dr("valor1") = valor1
        dr("valor2") = valor2
        dr("valor3") = valor3
        dt.Rows.Add(dr)
        escribir_xml()
    End Sub

    Private Function dtpar() As DataTable
        dtpar = New DataTable
        dtpar.TableName = "ParametrosAPP"
        dtpar.Rows.Add(rowdt(""))
        Return dtpar
    End Function
    Private Function rowdt(campos As String) As DataRow


    End Function

End Class
