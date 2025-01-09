Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports System.Data
Imports System.Configuration.ConfigurationManager
Imports System.Drawing

Public Class carga_dssql

    Private con As New SqlConnection
    Private buscar As New SqlCommand
    Private data As New SqlDataAdapter
    Private lectura As SqlDataReader
    Private ds As New DataSet
    Private lberror As String
    Private _ruta As String
    Private tabla, criteriocl As String
    Private _col_tb As String
    Private _resultado As Integer
    Private context As Web.HttpContext = Web.HttpContext.Current
    Dim comando As String
    Private Shared dataxlm As Boolean
    Private Shared npar As String = "DataAPP.xml"
    Private enc As New ENCRIPTAR

    Public Sub New(nombre_tabla As String, Optional xmlapp As Boolean = False, Optional criterio As String = Nothing)
        tabla = nombre_tabla
        criteriocl = criterio
        dataxlm = xmlapp
    End Sub

#Region "ParametrosAPP"
    Private Sub carga_xmlp(tb As String)
        Dim X As String = context.Server.MapPath("App_Data") + "\DataAPP.xml"
        Dim fl As New IO.FileInfo(X)
        If fl.Exists = True Then
            ds.ReadXml(X)
        End If
        If ds.Tables.Contains("ParametrosAPP") = False Then
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
    Private Sub escribir_xml(tb)
        Dim X As String = context.Server.MapPath("App_Data") + "\DataAPP.xml"
        Dim fl As New IO.FileInfo(X)
        If fl.Exists = False Then
            fl.Delete()
        End If
        ds.WriteXml(X)
    End Sub
    Public ReadOnly Property dtparametros(FR As String, CR As String) As DataTable
        Get
            Dim dspa As New carga_dssql("parametros")
            Return dspa.Carga_tablas("formulario='" + FR + "' and criterio='" + CR + "'", "VALOR")
        End Get
    End Property
    Public Sub addparametroXLM(valor1 As String, valor2 As String, valor3 As String)
        Dim dt As DataTable = ds.Tables("ParametrosAPP")
        Dim dr As DataRow
        dr = dt.NewRow
        dr("key") = dt.Rows.Count + 1
        dr("valor1") = enc.enc(StrReverse(valor1))
        dr("valor2") = enc.enc(StrReverse(valor2))
        dr("valor3") = enc.enc(StrReverse(valor3))
        dt.Rows.Add(dr)
        escribir_xml("DataAPP")
    End Sub
    Public Sub addparametroDB(valor1 As String, valor2 As String, valor3 As String)
        Try
            Dim DSPR As New carga_dssql("parametros")
            If DSPR.valor_campo("valor", "formulario='" + valor1 + "' and criterio='" + valor2 + "' and valor='" + valor3 + "'") Is Nothing Then
                DSPR.insertardb("'" + valor1.ToUpper + "','" + valor2.ToUpper + "','" + valor3.ToUpper + "'")
            End If
        Catch ex As Exception

        End Try
    End Sub
    Public Function valor_parametroxml(V1 As String, V2 As String) As String
        For Each row As DataRow In ds.Tables("ParametrosAPP").Select("valor'1=" + enc.enc(StrReverse(V1)) + "' and valor2='" + enc.enc(StrReverse(V2)) + "'")
            Return StrReverse(enc.desenc(row.Item("valor3")))
        Next
        Return ""
    End Function


#End Region
#Region "ManejoXML"

#End Region
    Public WriteOnly Property campostb(Optional tb As String = Nothing) As String
        Set(value As String)
            _col_tb = value
            If ultima_actualizacion < Now Then
                Exit Property
            End If
            If tb IsNot Nothing Then
                tabla = tb
            End If
            If dataxlm = False Then
                Dim x, y, z As Integer
                If Carga_tablas() IsNot Nothing Then
                    Dim CP() As String = value.Split(",")
                    If Carga_tablas.Columns.Count < CP.Count Then
                        actualizar_cp_tb()
                    End If
                Else
                    Try
                        con.ConnectionString = ruta()
                        con.Open()
                        buscar.Connection = con
                        data.SelectCommand = buscar
                        buscar.CommandText = campos_tb()
                        buscar.ExecuteNonQuery()
                        con.Close()
                    Catch ex1 As Exception
                        txtError(ex1)
                        lberror = ex1.Message.Replace(",", " ").Replace(".", " ").Replace("'", "")
                        con.Close()
                    End Try
                End If

                'Try
                '    x = Carga_tablas.Rows.Count
                'Catch ex As Exception
                '    Try
                '        con.ConnectionString = ruta()
                '        con.Open()
                '        buscar.Connection = con
                '        data.SelectCommand = buscar

                '        'Dim st As String = "CREATE TABLE " + tabla '+ " (" + campos_tb(_col_tb) + ")"
                '        buscar.CommandText = campos_tb()
                '        buscar.ExecuteNonQuery()
                '        'Dim builder As New SqlCommandBuilder(data)
                '        con.Close()
                '    Catch ex1 As Exception
                '        txtError(ex1)
                '        lberror = ex.Message.Replace(",", " ").Replace(".", " ").Replace("'", "")
                '        con.Close()
                '    End Try
                '    'ejecutar_comando("CREATE TABLE " + tabla + " (" + campos_tb(_col_tb) + ")")
                'End Try

                If x < 0 Then

                Else
                    'ejecutar("ALTER TABLE " + tabla + " ADD " + cp1(0) + " " + cp1[1])
                End If
            Else
                Try
                    Dim X As String = context.Server.MapPath("App_Data") + "\DataAPP.xml"
                    Dim fl As New IO.FileInfo(X)
                    If fl.Exists = True Then
                        ds.ReadXml(X)
                    End If
                    If ds.Tables.Contains(tabla) = False Then
                        Dim dt As New DataTable
                        dt.TableName = tabla
                        Dim dr As DataRow
                        dr = dt.NewRow
                        For Each str As String In _col_tb.Split(",")
                            Dim stc() As String = str.Split("-")
                            dt.Columns.Add(dtcol(stc(0)))
                        Next
                        ds.Tables.Add(dt)
                    End If
                Catch ex As Exception

                End Try

            End If
        End Set
    End Property
    Public ReadOnly Property LISTA_TABLAS(Optional criteriolt As String = Nothing) As DataTable
        Get
            Try
                'tabla = "TbLTB"
                comando = Nothing
                ds = New DataSet
                ds.DataSetName = "DsCARGA"
                con.ConnectionString = ruta()
                con.Open()
                If criteriolt IsNot Nothing Then
                    criteriolt += " AND " + criteriolt
                End If
                buscar.CommandText = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME <>'USUARIOS' AND TABLE_NAME <>'PARAMETROS' AND TABLE_NAME <>'PERMISOS' AND TABLE_NAME <>'ESTADISTICAS' AND TABLE_NAME <>'MSN'" + criteriolt
                buscar.Connection = con
                data.SelectCommand = buscar
                Dim builder As New SqlCommandBuilder(data)
                data.Fill(ds, "TbLTB")
                data.Update(ds, "TbLTB")
                con.Close()
            Catch ex As Exception
                txtError(ex)
                lberror = ex.Message.Replace(",", " ").Replace(".", " ").Replace("'", "")
                con.Close()
            End Try
            Return ds.Tables("TbLTB")
        End Get
    End Property
    Public ReadOnly Property LISTA_VISTAS(Optional criteriolt As String = Nothing) As DataTable
        Get
            Try
                'tabla = "TbLTB"
                comando = Nothing
                ds = New DataSet
                ds.DataSetName = "DsCARGA"
                con.ConnectionString = ruta()
                con.Open()
                If criteriolt IsNot Nothing Then
                    criteriolt += " AND " + criteriolt
                End If
                buscar.CommandText = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME like 'v_%'"
                buscar.Connection = con
                data.SelectCommand = buscar
                Dim builder As New SqlCommandBuilder(data)
                data.Fill(ds, "TbLTB")
                data.Update(ds, "TbLTB")
                con.Close()
            Catch ex As Exception
                txtError(ex)
                lberror = ex.Message.Replace(",", " ").Replace(".", " ").Replace("'", "")
                con.Close()
            End Try
            Return ds.Tables("TbLTB")
        End Get
    End Property
    Public ReadOnly Property LISTA_COLUMNAS(TB As String, Optional orden As Boolean = False) As DataTable
        Get
            Try
                'tabla = "TbLCB"
                comando = Nothing
                ds = New DataSet
                ds.DataSetName = "DsCARGA"
                con.ConnectionString = ruta()
                con.Open()
                Dim oc As String = Nothing
                If orden = True Then
                    oc = " order by COLUMN_NAME"
                End If
                buscar.CommandText = "SELECT COLUMN_NAME AS COLUMNA,DATA_TYPE AS TIPO FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='" + TB + "'" + oc
                buscar.Connection = con
                data.SelectCommand = buscar
                Dim builder As New SqlCommandBuilder(data)
                data.Fill(ds, "TbLTB")
                data.Update(ds, "TbLTB")
                con.Close()
            Catch ex As Exception
                txtError(ex)
                lberror = ex.Message.Replace(",", " ").Replace(".", " ").Replace("'", "")
                con.Close()
            End Try
            Return ds.Tables("TbLTB")
        End Get
    End Property
    'Public Sub vistatb(tablas As String, campos As String)
    '    con.ConnectionString = ruta()
    '    con.Open()
    '    buscar.Connection = con
    '    data.SelectCommand = buscar

    '    'Dim st As String = "CREATE TABLE " + tabla '+ " (" + campos_tb(_col_tb) + ")"
    '    buscar.CommandText = campos_tb()
    '    buscar.ExecuteNonQuery()
    '    'Dim builder As New SqlCommandBuilder(data)
    '    con.Close()
    'End Sub
    Private Function campos_tb() As String
        Dim cp, cpk As String
        cp = Nothing : cpk = Nothing
        Dim cp1(2) As String
        For Each str As String In _col_tb.Split(",")
            If str.Contains("-key") Then
                cp1 = str.Split("-")
                cpk = cp1(0)
                cp += cp1(0) + " bigint IDENTITY(1,1) PRIMARY KEY NOT NULL"
            Else
                cp1 = str.Split("-")
                cp += "," + cp1(0) + " " + cp1(1).ToUpper '+ " NOT NULL"
            End If
        Next
        Dim rs As String = "CREATE TABLE " + tabla + " (" + cp + ")"
        Return rs
    End Function
    Private Function actualizar_cp_tb() As String
        Dim cp, cpk As String
        cp = Nothing : cpk = Nothing
        Dim cp1(2) As String
        For Each str As String In _col_tb.Split(",")
            cp1 = str.Split("-")
            If Carga_tablas.Columns.Contains(cp1(0)) = False Then
                con.ConnectionString = ruta()
                con.Open()
                buscar.Connection = con
                data.SelectCommand = buscar
                buscar.CommandText = "ALTER TABLE " & tabla & " ADD " & cp1(0) & " " & cp1(1)
                buscar.ExecuteNonQuery()
                con.Close()
                If cp1(1).Contains("varchar") Then
                    actualizardb(cp1(0) + "=''", Nothing)
                ElseIf cp1(1).Contains("date") Then
                    actualizardb(cp1(0) + "='" + Now.ToString("yyyy-MM-dd") + "'", Nothing)
                ElseIf cp1(1).Contains("text") Then
                    actualizardb(cp1(0) + "=''", Nothing)
                End If
            End If


            'If str.Contains("-key") Then
            '    cp1 = str.Split("-")
            '    cpk = cp1(0)
            '    cp += cp1(0) + " bigint IDENTITY(1,1) PRIMARY KEY NOT NULL"
            'Else
            '    cp1 = str.Split("-")
            '    cp += "," + cp1(0) + " " + cp1(1).ToUpper + " NOT NULL"
            'End If
        Next
        Dim rs As String = "CREATE TABLE " + tabla + " (" + cp + ")"
        Return rs
    End Function

    Public Function ruta() As String
        Dim iplocal As String = context.Request.Url.Host 'context.Request.UserHostAddress
        If iplocal = "127.0.0.1" Then
            iplocal = "::1"
        End If
        Return ConfigurationManager.ConnectionStrings(iplocal).ToString
    End Function
    Public ReadOnly Property formato_fecha As String
        Get
            Return ConfigurationManager.AppSettings("FormatoFecha")
        End Get
    End Property
    Public ReadOnly Property ultima_actualizacion As DateTime
        Get
            Return ConfigurationManager.AppSettings("ultimaactualizacion")
        End Get
    End Property
    Public ReadOnly Property formato_fechal As String
        Get
            Return ConfigurationManager.AppSettings(context.Request.Url.Host + ":fechal")
        End Get
    End Property
    Public ReadOnly Property resultado As Integer
        Get
            Return _resultado
        End Get
    End Property
    Public ReadOnly Property datatable_gl(Optional orden As String = Nothing) As DataTable
        Get
            Return Carga_tablas(Nothing, orden)
            ' dstable
        End Get
    End Property

    Public ReadOnly Property errors As String
        Get
            Return lberror
        End Get
    End Property

    Public Function Carga_tablas(Optional criterio As String = Nothing, Optional ORDEN As String = Nothing, Optional campos As String = "*", Optional grupo As Boolean = False) As DataTable
        If dataxlm = False Then
            Try
                comando = Nothing
                ds = New DataSet
                ds.DataSetName = "DsCARGA"
                con.ConnectionString = ruta()
                con.Open()
                If criterio <> Nothing And criteriocl <> Nothing Then
                    comando += " where " + criteriocl + " AND " + criterio
                ElseIf criterio <> Nothing And criteriocl = Nothing Then
                    comando += " where " + criterio
                ElseIf criterio = Nothing And criteriocl <> Nothing Then
                    comando += " where " + criteriocl
                End If
                If grupo = True Then
                    comando += " group by " + campos
                End If
                If ORDEN <> Nothing Then
                    comando += " order by " + ORDEN
                End If
                buscar.CommandText = "Select " + campos + " from " + tabla + comando
                buscar.Connection = con
                data.SelectCommand = buscar
                Dim builder As New SqlCommandBuilder(data)
                data.Fill(ds, tabla)
                data.Update(ds, tabla)
                con.Close()
            Catch ex As Exception
                txtError(ex)
                lberror = ex.Message.Replace(",", " ").Replace(".", " ").Replace("'", "")
                con.Close()
            End Try
        Else
            'carga_xmlp(tabla)
        End If
        Return ds.Tables(tabla)
    End Function
    Public Function Carga_tb_especial(Optional campos As String = "*", Optional criterio As String = Nothing, Optional grupo As String = Nothing, Optional ORDEN As String = Nothing) As DataTable
        If dataxlm = False Then
            Try
                comando = Nothing
                ds = New DataSet
                ds.DataSetName = "DsCARGA"
                con.ConnectionString = ruta()
                con.Open()
                If criterio <> Nothing And criteriocl <> Nothing Then
                    comando += " where " + criteriocl + " AND " + criterio
                ElseIf criterio <> Nothing And criteriocl = Nothing Then
                    comando += " where " + criterio
                ElseIf criterio = Nothing And criteriocl <> Nothing Then
                    comando += " where " + criteriocl
                End If
                If grupo IsNot Nothing Then
                    comando += " group by " + grupo
                End If
                If ORDEN <> Nothing Then
                    comando += " order by " + ORDEN
                End If
                buscar.CommandText = "Select " + campos + " from " + tabla + comando
                buscar.Connection = con
                data.SelectCommand = buscar
                Dim builder As New SqlCommandBuilder(data)
                data.Fill(ds, tabla)
                data.Update(ds, tabla)
                con.Close()
            Catch ex As Exception
                txtError(ex)
                lberror = ex.Message.Replace(",", " ").Replace(".", " ").Replace("'", "")
                con.Close()
            End Try
        Else
            'carga_xmlp(tabla)
        End If
        Return ds.Tables(tabla)
    End Function
    Public ReadOnly Property TABLA_NOMBRE As String
        Get
            Return tabla
        End Get
    End Property
    Public Function Carga_tablas_especial(ByVal _select As String, _criterio As String, Optional pivot As String = Nothing, Optional _grupo As String = Nothing, Optional _orden As String = Nothing) As DataTable
        Try
            ds = New DataSet
            con.ConnectionString = ruta()
            con.Open()
            'If _criterio <> Nothing Then
            '    _criterio = " where " + _criterio
            'End If
            If _criterio <> Nothing And criteriocl <> Nothing Then
                _criterio = " where " + criteriocl + " AND " + _criterio
            ElseIf _criterio <> Nothing And criteriocl = Nothing Then
                _criterio = " where " + _criterio
            ElseIf _criterio = Nothing And criteriocl <> Nothing Then
                _criterio = " where " + criteriocl
            End If
            If _grupo <> Nothing Then
                _grupo = " group by " + _grupo
            End If
            If _orden <> Nothing Then
                _orden = " order by " + _orden
            End If
            buscar.CommandText = "Select " + _select.Replace(".", ",") + " from " + tabla + " " + pivot + _criterio + _grupo + _orden
            buscar.Connection = con
            data.SelectCommand = buscar
            Dim builder As New SqlCommandBuilder(data)
            data.Fill(ds, "especial")
            data.Update(ds, "especial")
            con.Close()
        Catch ex As Exception
            con.Close()
            lberror = ex.Message.ToString
            txtError(ex)
        End Try
        Return ds.Tables("especial")
    End Function
    Public Function Carga_tablas_reportes(dsrp As DataSet, ByVal criterio As String, tb_resultado As String) As DataTable
        Try
            con.ConnectionString = ruta()
            con.Open()
            If criterio = "" Then
                buscar.CommandText = "Select * from " + tabla
            Else
                buscar.CommandText = "Select * from " + tabla + " where " + criterio
            End If
            buscar.Connection = con
            data.SelectCommand = buscar
            Dim builder As New SqlCommandBuilder(data)
            data.Fill(dsrp, tb_resultado)
            data.Update(dsrp, tb_resultado)
            con.Close()
        Catch ex As Exception
            txtError(ex)
            lberror = ex.Message.Replace(",", " ").Replace(".", " ").Replace("'", "")
            con.Close()
        End Try
        Return ds.Tables(tb_resultado)
    End Function
    Public Sub ejecutar_comando(comando As String)
        Dim x As String = Nothing
        Try
            con.ConnectionString = ruta()
            con.Open()
            buscar.Connection = con
            data.SelectCommand = buscar
            buscar.CommandText = comando
            Dim builder As New SqlCommandBuilder(data)
            con.Close()
        Catch ex As Exception
            txtError(ex)
            lberror = ex.Message.Replace(",", " ").Replace(".", " ").Replace("'", "")
            con.Close()
        End Try
    End Sub
    Public Sub insertardb(ByVal campos As String, Optional mayusculas As Boolean = False)
        If dataxlm = False Then
            Try
                con.ConnectionString = ruta()
                con.Open()
                Dim cp As String
                If mayusculas = False Then
                    cp = campos
                Else
                    cp = campos.ToUpper
                End If
                buscar.CommandText = "Insert into " + tabla + " values(" + cp + ")"
                buscar.Connection = con
                data.InsertCommand = buscar
                buscar.ExecuteNonQuery()
                _resultado = data.UpdateBatchSize
                con.Close()
                ''seguimientodb(tabla, "Insert", campos.Replace(",", "|").Replace("'", ""), Nothing)
            Catch ex As Exception
                con.Close()
                lberror = ex.Message.ToString
                txtError(ex)
            End Try
        Else
            Dim dt As DataTable = ds.Tables(tabla)
            Dim x As Integer = 0
            Dim dr As DataRow
            Dim ct() As String = _col_tb.Split(",")
            dr = dt.NewRow
            Dim ctc() As String = ct(x).Split("-")
            x += 1
            dr(ctc(0)) = dt.Rows.Count + 1
            For Each str As String In campos.Split(",")
                ctc = ct(x).Split("-")
                dr(ctc(0)) = str.Replace("'", "")
                x += 1
            Next
            dt.Rows.Add(dr)
            escribir_xml(tabla)
        End If

    End Sub
    Public Sub insertardbMIN(ByVal campos As String)
        Try
            con.ConnectionString = ruta()
            con.Open()
            buscar.CommandText = "Insert into " + tabla + " values(" + campos + ")"
            buscar.Connection = con
            data.InsertCommand = buscar
            buscar.ExecuteNonQuery()
            _resultado = data.UpdateBatchSize
            con.Close()
            'seguimientodb(tabla, "Insert", campos.Replace(",", "|").Replace("'", ""), Nothing)
        Catch ex As Exception
            con.Close()
            lberror = ex.Message.ToString
            txtError(ex)
        End Try
    End Sub
    Private Sub seguimientodb(tabla As String, tipo As String, seguimiento As String, criterio As String)
        Try
            Dim bk As String = Nothing
            If criterio <> Nothing Then
                For Each row As DataRow In Carga_tablas(criterio).Rows
                    For x As Integer = 0 To Carga_tablas(criterio).Columns.Count - 1
                        bk += row.Item(x).ToString + "|"
                    Next
                Next
            End If
            con.ConnectionString = ruta()
            con.Open()
            Dim context As Web.HttpContext = Web.HttpContext.Current
            buscar.CommandText = "insert into seguimiento values('" + context.User.Identity.Name + "','" + Now.ToString("dd/MM/yyyy HH:mm:ss") + "','" + tabla + "','" + tipo + "','" + seguimiento + "','" + context.Request.ServerVariables("REMOTE_ADDR") + "','" + bk + "')"
            buscar.Connection = con
            data.InsertCommand = buscar
            buscar.ExecuteNonQuery()
            _resultado = data.UpdateBatchSize
            con.Close()
        Catch ex As Exception
            con.Close()
            lberror = ex.Message.ToString
            txtError(ex)
        End Try
    End Sub
    Public Sub actualizardb(ByVal campos As String, criterio As String, Optional mayusculas As Boolean = False)
        If dataxlm = False Then
            Try
                con.ConnectionString = ruta()
                con.Open()
                Dim cp As String
                If mayusculas = False Then
                    cp = campos
                Else
                    cp = campos.ToUpper
                End If
                If criterio Is Nothing Then
                    buscar.CommandText = "update " + tabla + " set " + cp
                Else
                    buscar.CommandText = "update " + tabla + " set " + cp + " where " + criterio
                End If
                buscar.Connection = con
                data.UpdateCommand = buscar
                buscar.ExecuteNonQuery()
                _resultado = data.UpdateBatchSize
                con.Close()
            Catch ex As Exception
                con.Close()
                lberror = ex.Message.ToString
                txtError(ex)
            End Try
        Else
            For Each row As DataRow In ds.Tables(tabla).Select(criterio)
                For Each str As String In campos.Split(",")
                    Dim ct() As String = str.Split("=")
                    row.Item(ct(0)) = ct(1).Replace("'", "")
                Next
            Next
        End If

    End Sub
    Public Sub act_campodb(ByVal campos As String, valor As String, criterio As String, Optional mayusculas As Boolean = False)
        'AGREGA UN DATO A LA BASE DE DATOS
        If dataxlm = False Then
            Try
                ''seguimientodb(tabla, "Update", campos.Replace(",", "|").Replace("'", ""), criterio)
                con.ConnectionString = ruta()
                con.Open()
                Dim cp As String
                If mayusculas = False Then
                    cp = campos
                Else
                    cp = campos.ToUpper
                End If
                buscar.CommandText = "update " + tabla + " set " + cp + "=" + valor + " where " + criterio
                buscar.Connection = con
                data.UpdateCommand = buscar
                buscar.ExecuteNonQuery()
                _resultado = data.UpdateBatchSize
                con.Close()
            Catch ex As Exception
                con.Close()
                lberror = ex.Message.ToString
                txtError(ex)
            End Try
        Else
            For Each row As DataRow In ds.Tables(tabla).Select(criterio)
                For Each str As String In campos.Split(",")
                    Dim ct() As String = str.Split("=")
                    row.Item(ct(0)) = ct(1).Replace("'", "")
                Next
            Next
        End If

    End Sub
    Public Sub actualizardbMIN(ByVal campos As String, criterio As String)
        'AGREGA UN DATO A LA BASE DE DATOS
        Try
            'seguimientodb(tabla, "Update", campos.Replace(",", "|").Replace("'", ""), criterio)
            con.ConnectionString = ruta()
            con.Open()
            buscar.CommandText = "update " + tabla + " set " + campos + " where " + criterio
            buscar.Connection = con
            data.UpdateCommand = buscar
            buscar.ExecuteNonQuery()
            _resultado = data.UpdateBatchSize
            con.Close()
        Catch ex As Exception
            con.Close()
            lberror = ex.Message.ToString
            txtError(ex)
        End Try
    End Sub
    Public Sub Eliminardb(criterio As String)
        If dataxlm = False Then
            Try
                'seguimientodb(tabla, "Delete", criterio.Replace(",", "|").Replace("'", ""), criterio)
                con.ConnectionString = ruta()
                con.Open()
                buscar.CommandText = "Delete from " + tabla + " where " + criterio
                buscar.Connection = con
                data.DeleteCommand = buscar
                buscar.ExecuteNonQuery()
                _resultado = data.UpdateBatchSize
                con.Close()
            Catch ex As Exception
                con.Close()
                lberror = ex.Message.ToString
                txtError(ex)
            End Try
        Else
            For Each row As DataRow In ds.Tables(tabla).Select(criterio)
                row.Delete()
            Next
        End If

    End Sub

    Public Function valor_campo(ByVal campo As String, ByVal criterio As String, Optional formato_N As String = Nothing) As String
        If dataxlm = False Then
            Try
                Dim VAL As String = Nothing
                If Carga_tablas(criterio) Is Nothing Then
                    Return Nothing
                End If
                For Each row As DataRow In Carga_tablas(criterio).Rows
                    If row.IsNull(0) = False Then
                        Select Case formato_N
                            Case Nothing
                                VAL = row.Item(campo).ToString
                            Case "N", "n"
                                VAL = FormatNumber(row.Item(campo).ToString, 0)
                            Case "d", "D"
                                VAL = FormatDateTime(row.Item(campo).ToString)
                        End Select
                        Return VAL.Replace(",", ".")
                    End If
                Next
            Catch ex As Exception
                lberror = ex.Message.ToString
                txtError(ex)
            End Try
        Else
            carga_xmlp(tabla)
            For Each row As DataRow In ds.Tables(tabla).Select(criterio)
                Select Case formato_N
                    Case Nothing
                        Return row.Item(campo)
                    Case "N", "n"
                        Return FormatNumber(row.Item(campo), 0)
                    Case "d", "D"
                        Return FormatDateTime(row.Item(campo))
                End Select

            Next
        End If
        Return Nothing
    End Function
    Public Function valor_campo_OTROS(ByVal campo As String, ByVal criterio As String, Optional grupo As String = Nothing, Optional orden As String = Nothing, Optional formato_n As String = Nothing) As String
        ds.Clear()
        Try
            For Each row As DataRow In Carga_tablas_especial(campo, criterio, grupo, orden).Rows
                If row.IsNull(0) = False Then
                    Dim VAL As String = Nothing
                    Select Case formato_n
                        Case Nothing
                            VAL = row.Item(0)
                        Case "N", "n"
                            VAL = FormatNumber(row.Item(0), 0)
                        Case "d", "D"
                            VAL = FormatDateTime(row.Item(0))
                    End Select
                    Return VAL.Replace(",", ".").Replace(".0000", "").Replace(",0000", "")
                End If
            Next
        Catch ex As Exception
            lberror = ex.Message.ToString
            txtError(ex)
        End Try
        Return Nothing
    End Function
    Public Function colum_table(Optional tb As String = Nothing, Optional columnkey As String = Nothing) As DataTable
        If tb = Nothing Then
            tb = tabla
        End If
        If columnkey <> Nothing Then
            columnkey = " and COLUMN_NAME <> '" + columnkey + "'"
        End If
        Dim r As String
        Try
            ds = New DataSet
            con.ConnectionString = ruta()
            con.Open()
            buscar.CommandText = "SELECT COLUMN_NAME AS COLUMNA,DATA_TYPE AS TIPO FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='" + tb + "'" + columnkey + " ORDER BY COLUMN_NAME"
            buscar.Connection = con
            data.SelectCommand = buscar
            Dim builder As New SqlCommandBuilder(data)
            data.Fill(ds, "tbcol")
            data.Update(ds, "tbcol")
            con.Close()
            For Each row As DataRow In ds.Tables("tbcol").Rows
                r = row.Item(0) + ";"
            Next
        Catch ex As Exception
            txtError(ex)
            lberror = ex.Message
            con.Close()
        End Try
        Return ds.Tables("tbcol")
    End Function
    Public Sub bloqueo_control(vf As Boolean, viewp As View)
        For Each CTR As Control In viewp.Controls
            If TypeOf CTR Is TextBox Then
                CType(CTR, TextBox).Enabled = vf
            ElseIf TypeOf CTR Is DropDownList Then
                CType(CTR, DropDownList).Enabled = vf
            ElseIf TypeOf CTR Is Button Then
                CType(CTR, Button).Visible = vf
            End If
            If context.Session("Movil") = True Then
                If TypeOf CTR Is TextBox Then
                    CType(CTR, TextBox).Font.Size = FontUnit.Point(20)
                ElseIf TypeOf CTR Is DropDownList Then
                    CType(CTR, DropDownList).Font.Size = FontUnit.Point(20)
                ElseIf TypeOf CTR Is Button Then
                    CType(CTR, Button).Font.Size = FontUnit.Point(20)
                End If
            End If
        Next

    End Sub
    Public Function PIVOT_2tb(campos As String, campo_pivot As String, campo_for As String, colum_resultado As String, Optional criterio As String = Nothing, Optional grupo As Boolean = False, Optional orden As String = Nothing) As DataTable
        Dim _grupo As String = Nothing
        Dim _colum_resultado As String = Nothing
        Try
            For Each cr In colum_resultado.Split(",")
                If _colum_resultado <> Nothing Then
                    _colum_resultado += ","
                End If
                _colum_resultado += "[" + cr + "]"
            Next

            ds = New DataSet
            con.ConnectionString = ruta()
            con.Open()
            If criterio <> Nothing Then
                criterio = " where " + criterio
            End If
            If grupo = True Then
                _grupo = " group by " + campos
            End If
            If orden <> Nothing Then
                orden = " order by " + orden
            End If
            Dim sp As String = "with tbpivot (" + campos + ") as (SELECT " + campos + " FROM " + tabla + criterio + _grupo + orden + ") select * from tbpivot PIVOT (" + campo_pivot + " for " + campo_for + " in (" + _colum_resultado + ")) pvt"
            buscar.CommandText = sp
            buscar.Connection = con
            data.SelectCommand = buscar
            Dim builder As New SqlCommandBuilder(data)
            data.Fill(ds, "tbpivot")
            data.Update(ds, "tbpivot")
            con.Close()
        Catch ex As Exception
            con.Close()
            lberror = ex.Message.ToString
            txtError(ex)
        End Try
        Return ds.Tables("tbpivot")

    End Function
    Public Function pivot_1tb(campos As String, campo_pivot As String, campo_for As String, colum_resultado As String, Optional criterio As String = Nothing, Optional grupo As Boolean = False, Optional orden As String = Nothing) As DataTable
        Dim _grupo As String = Nothing
        Dim _colum_resultado As String = Nothing
        Try
            For Each cr In colum_resultado.Split(",")
                If _colum_resultado <> Nothing Then
                    _colum_resultado += ","
                End If
                _colum_resultado += "[" + cr + "]"
            Next

            ds = New DataSet
            con.ConnectionString = ruta()
            con.Open()
            If criterio <> Nothing Then
                criterio = " where " + criterio
            End If
            If grupo = True Then
                _grupo = " group by " + campos
            End If
            If orden <> Nothing Then
                orden = " order by " + orden
            End If
            Dim sp As String = "select (" + campo_pivot + ") as (SELECT " + campos + " FROM " + tabla + criterio + _grupo + orden + ") select * from tbpivot PIVOT (" + campo_pivot + " for " + campo_for + " in (" + _colum_resultado + ")) pvt"
            buscar.CommandText = sp
            buscar.Connection = con
            data.SelectCommand = buscar
            Dim builder As New SqlCommandBuilder(data)
            data.Fill(ds, "tbpivot")
            data.Update(ds, "tbpivot")
            con.Close()
        Catch ex As Exception
            con.Close()
            lberror = ex.Message.ToString
            txtError(ex)
        End Try
        Return ds.Tables("tbpivot")
    End Function
    Public Sub txtError(er As Exception, Optional ote As String = Nothing)
        Try
            Dim archivo As String = context.Server.MapPath("~") + "\error.txt"
            Dim ft As New StreamWriter(archivo, True)
            If er IsNot Nothing Then
                ft.WriteLine("------------------------")
                ft.WriteLine(Now.ToString)
                ft.WriteLine("usuario:" + context.User.Identity.Name)
                ft.WriteLine("Tabla:" + tabla)
                ft.WriteLine("PG:" + context.Request.Url.AbsolutePath)
                ft.WriteLine("FR:" + context.Request.QueryString("fr"))
                ft.WriteLine(er.Source)
                ft.WriteLine(er.Message)
                ft.WriteLine(er.StackTrace)
                ft.WriteLine(er.TargetSite.Name)
                ft.WriteLine(buscar.CommandText)
                ft.Close()
            ElseIf ote IsNot Nothing Then
                ft.WriteLine("------------------------")
                ft.WriteLine(Now.ToString)
                ft.WriteLine("usuario:" + context.User.Identity.Name)
                ft.WriteLine("PG:" + context.Request.Url.AbsolutePath)
                ft.WriteLine("FR:" + context.Request.QueryString("fr"))
                ft.WriteLine(ote)
                ft.Close()
            End If

        Catch ex As Exception

        End Try

    End Sub
    Public Function DsFILTRO(DrF As DropDownList, CxF As Control, CRITERIO As String, Optional ORDEN As String = Nothing, Optional CAMPOS As String = " * ", Optional GRUPO As String = Nothing) As DataTable
        DsFILTRO = Carga_tablas_especial(CAMPOS, CRITERIO, GRUPO, ORDEN)
        Dim FILTRO As String = Nothing 'context.Session(context.Request.Url.AbsoluteUri)
        If CRITERIO.ToUpper.Contains(DrF.SelectedItem.Text) = True And FILTRO Is Nothing Then
            FILTRO = "  "
        End If

        If TypeOf CxF Is TextBox Then
            Dim TxF As TextBox = CxF
            If TxF.Text <> "" Then
                If FILTRO Is Nothing And CRITERIO IsNot Nothing Then
                    FILTRO += " AND "
                ElseIf FILTRO IsNot Nothing And CRITERIO IsNot Nothing Then
                    FILTRO += " OR "
                End If
                If DrF.SelectedItem.Value.Contains("N-") = True Then
                    FILTRO += DrF.SelectedItem.Value.Substring(2) + "=" + TxF.Text
                ElseIf DrF.SelectedItem.Value.Contains("T-") = True Then
                    FILTRO += DrF.SelectedItem.Value.Substring(2) + " like '%" + TxF.Text + "%'"
                ElseIf DrF.SelectedItem.Value.Contains("F-") = True Then
                    FILTRO += DrF.SelectedItem.Value.Substring(2) + " > '" + TxF.Text + " 00:00' and " + DrF.SelectedItem.Value.Substring(2) + " < '" + TxF.Text + " 23:59'"
                End If
            End If
        ElseIf TypeOf CxF Is DropDownList Then
            Dim DxT As DropDownList = CxF
            If DxT.SelectedItem.Text <> "TODOS" Then
                If FILTRO Is Nothing And CRITERIO IsNot Nothing Then
                    FILTRO += " AND "
                ElseIf FILTRO IsNot Nothing And CRITERIO IsNot Nothing Then
                    FILTRO += " OR "
                End If
                If DrF.SelectedItem.Value.Contains("N-") = True Then
                    FILTRO += DrF.SelectedItem.Value.Substring(2) + "=" + DxT.Text
                ElseIf DrF.SelectedItem.Value.Contains("T-") = True Then
                    FILTRO += DrF.SelectedItem.Value.Substring(2) + " like '%" + DxT.Text + "%'"
                ElseIf DrF.SelectedItem.Value.Contains("F-") = True Then
                    FILTRO += DrF.SelectedItem.Value.Substring(2) + " > '" + DxT.Text + " 00:00' and " + DrF.SelectedItem.Value.Substring(2) + " < '" + DxT.Text + " 23:59'"
                End If
            End If
        End If

        If Carga_tablas_especial(CAMPOS, CRITERIO + FILTRO, GRUPO, ORDEN).Rows.Count > 0 Then
            'context.Session(context.Request.Url.AbsoluteUri) = FILTRO
            DsFILTRO = Carga_tablas_especial(CAMPOS, CRITERIO + FILTRO, GRUPO, ORDEN)
        End If
    End Function
    Public Sub carga_dr(dr As DropDownList, criterio_dr As String, texto_dr As String)
        If dr.Items.Count = 0 Then
            dr.DataSource = Carga_tablas(criterio_dr)
            dr.DataTextField = texto_dr
            dr.DataBind()
        End If
    End Sub

    Public Sub vistatb(nombre As String, tabla1 As String, tabla2 As String, campos As String, criterio As String, Optional orden As String = Nothing, Optional grupo As String = Nothing)
        If orden IsNot Nothing Then
            orden = " ORDER BY " + orden
        End If
        If grupo IsNot Nothing Then
            grupo = " GROUP BY " + grupo
        End If
        If Carga_tablas() IsNot Nothing Then
            Dim CP() As String = campos.Split(",")
            'If Carga_tablas.Columns.Count < CP.Count Then
            con.ConnectionString = ruta()
            con.Open()
            buscar.Connection = con
            data.SelectCommand = buscar
            If tabla2 Is Nothing Then
                buscar.CommandText = "ALTER VIEW " + nombre + " AS SELECT " + campos + " FROM " + tabla1 + grupo + orden
            Else
                buscar.CommandText = "ALTER VIEW " + nombre + " AS SELECT " + campos + " FROM " + tabla1 + " LEFT OUTER JOIN " + tabla2 + " ON " + criterio
            End If

            buscar.ExecuteNonQuery()
            con.Close()
        Else
            con.ConnectionString = ruta()
            con.Open()
            buscar.Connection = con
            data.SelectCommand = buscar
            If tabla2 Is Nothing Then
                buscar.CommandText = "CREATE VIEW " + nombre + " AS SELECT " + campos + " FROM " + tabla1 + grupo + orden
            Else
                buscar.CommandText = "CREATE VIEW " + nombre + " AS SELECT " + campos + " FROM " + tabla1 + " LEFT OUTER JOIN " + tabla2 + " ON " + criterio
            End If

            buscar.ExecuteNonQuery()
            con.Close()
            'End If
        End If

    End Sub

    Public Function TABLA_INTERNA(NOMBRE As String, COLUMNAS As String) As DataTable
        TABLA_INTERNA = New DataTable : TABLA_INTERNA.TableName = NOMBRE
        For Each STR As String In COLUMNAS.Split(",")
            TABLA_INTERNA.Columns.Add(STR)
        Next
    End Function

    Public WriteOnly Property TABLA_INTERNA_DATOS(TB As DataTable) As String
        Set(value As String)

            Dim TROW As DataRow = TB.NewRow
            For X As Integer = 0 To TB.Columns.Count - 1
                TROW(X) = value.Split(",")(X)
            Next
            TB.Rows.Add(TROW)
        End Set
    End Property
    Public Sub CREAR_XML(TB As DataTable, NOMBRE_XML As String)
        Dim filePath As String = context.Server.MapPath("~") + "\tr" + NOMBRE_XML + ".xml"
        TB.WriteXml(filePath)
    End Sub

#Region "CARGA_TbIMAGEN"
    Public Sub Addimagen(nombre As String, FileUpload1 As FileUpload)
        campostb("imagenes") = "kimg-key,formulario-varchar(250),criterio-varchar(250),nombre-varchar(250),img-image"
        If FileUpload1.HasFile = False Then
            Exit Sub
        End If
        Dim photo() As Byte = GetStreamAsByteArray(FileUpload1.PostedFile.InputStream)

        Using connection As SqlConnection = New SqlConnection(ruta)

            Dim command As SqlCommand = New SqlCommand(
              "INSERT INTO imagenes (nombre, foto) " &
              "Values(@nombre,@Photo)", connection)

            'command.Parameters.Add("@formulario",
            'SqlDbType.NVarChar, 20).Value = formulario
            'command.Parameters.Add("@criterio",
            'SqlDbType.NVarChar, 20).Value = criterio
            command.Parameters.Add("@nombre",
              SqlDbType.NVarChar, 20).Value = nombre
            command.Parameters.Add("@Photo",
              SqlDbType.Image, photo.Length).Value = photo

            connection.Open()
            command.ExecuteNonQuery()

        End Using
    End Sub
    Private Function redimencionar(imgo As Drawing.Image, alto As Integer) As Drawing.Image
        Dim Radio = CDbl(alto) / imgo.Height
        Dim Nuevoalto = CType((imgo.Width * Radio), Integer)
        Dim Nuevoancho = CType((imgo.Height * Radio), Integer)
        Dim Nimg = New Bitmap(Nuevoancho, Nuevoalto)
        Dim g = Graphics.FromImage(Nimg)
        g.DrawImage(imgo, 0, 0, Nuevoancho, Nuevoalto)
        Return Nimg
    End Function
    Private Function GetStreamAsByteArray(ByVal stream As Stream) As Byte()
        Dim streamLength As Integer = Convert.ToInt32(stream.Length)
        Dim fileData As Byte() = New Byte(streamLength) {}

        stream.Read(fileData, 0, streamLength)
        stream.Close()

        Return fileData
    End Function
    Public Function imagendb(idimagen As String, Optional imtam As Integer = 300) As WebControls.Image
        imagendb = New WebControls.Image
        Dim bytBLOBData() As Byte
        Try
            For Each row As DataRow In Carga_tablas("kimagen='" + idimagen + "'").Rows
                bytBLOBData = row.Item("foto")
            Next
            Dim stmBLOBData As New MemoryStream(bytBLOBData)
            If imtam > 0 Then
                'imagendb.Width = Unit.Pixel(imtam) ':
                imagendb.Height = Unit.Pixel(imtam)
            End If
            imagendb.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(bytBLOBData)

        Catch ex As Exception
            imagendb.ImageUrl = Nothing
        End Try
        'Me.PictureBox1.Image = Image.FromStream(stmBLOBData)
    End Function


    Public Shared Function GetPhoto(filePath As String) As Byte()
        Dim stream As FileStream = New FileStream(
           filePath, FileMode.Open, FileAccess.Read)
        Dim reader As BinaryReader = New BinaryReader(stream)

        Dim photo() As Byte = reader.ReadBytes(stream.Length)

        reader.Close()
        stream.Close()

        Return photo
    End Function
#End Region




End Class
