Imports Classcatalogoch
Imports System.Configuration

Public Class ClassAPP
    Private context As Web.HttpContext = Web.HttpContext.Current
    Private fr As ClassConstructor22
    Private _fr As Panel
    Private dsctl As New carga_dssql("control_llamada")
    Private dsct As New carga_dssql("v_cotizacion")
    Private DSCL As New carga_dssql("clientes")
    Private SGC As New carga_dssql("SGCLIENTES")
    Private Shadows kcl, us, tel As String
    Private Shadows ncliente As Boolean

    Sub New(fpn As Panel)
        dsctl.campostb = "kllamada-key,fecha_llamada-datetime,numero-bigint,usuario-varchar(50),hora_inicio-time(7),hora_fin-time(7),tiempot-int"
        fr = New ClassConstructor22(fpn)
        _fr = fpn
        us = fr.reque("us")
        If fr.reque("tl") IsNot Nothing Then
            tel = fr.reque("tl").ToString.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "")
        End If
        Select Case fr.reque("fr")
            Case ""
                carga_gestion()
            Case "tr"
                carga_grill(DSCL.Carga_tablas("FECHASCL <='" + fr.HOY_FR + "' AND USUARIOC='" + fr.USERLOGUIN + "'", "FECHASCL ASC"), "fechascl,nombre", 1)
            Case "ct"
                carga_grill(dsct.Carga_tablas("estado_cotizacion='1 SEGUIMIENTO' AND asesor='" + fr.USERLOGUIN + "'", "fecha_creacion asc"), "fecha_creacion,nombre,referencia", 0)
            Case "cl"
                carga_grill(DSCL.Carga_tablas("USUARIOC='" + us + "'", "NOMBRE"), "NOMBRE,KTELEFONO,LLANTA_INTERES", 1)
            Case "mc"
                carga_llamada()
            Case "sg"
                carga_llamada()
            Case "rg"
                Dim DTI, DTF As Date
                DTI = fr.reque("hi")
                DTF = fr.reque("hf")
                'Dim cu As Integer = fr.reque("dr")
                dsctl.insertardb("'" + fr.HOY_FR + "'," + tel + ",'" + fr.USERLOGUIN + "','" + DTI.ToString("HH:mm:ss") + "','" + DTF.ToString("HH:mm:ss") + "'," + fr.reque("dr"))
                fr.redir("?us=" + fr.USERLOGUIN + "&fr=sg&tl=" + tel)
        End Select
    End Sub

    Private Sub carga_gestion()
        'Dim CRG As New ClassConstructor22(fr)
        Dim GTB As DataTable = DSCL.TABLA_INTERNA("TbGESTION", "GESTION,DIARIO,ACUMULADO")
        us = fr.reque("us")
        If US Is Nothing Then
            US = fr.USERLOGUIN
        End If
        LbTL.Text = Now.ToLongDateString.ToUpper + "<hr>"
        LbTL.Font.Size = FontUnit.XXLarge

        _fr.Controls.Add(LbTL)
        fr.FORMULARIO(Nothing, "BtTAR,BtCOT,BtCLI,BtLLA", False)
        Dim x, y As String
        x = dsct.valor_campo_OTROS("count(no_cotizacion)", "fecha_creacion='" + fr.HOY_FR + "' and asesor='" + us + "'")
        y = dsct.valor_campo_OTROS("count(no_cotizacion)", "Month(fecha_creacion)=" + Now.Month.ToString + " and year(fecha_creacion)=" + Now.Year.ToString + " and asesor='" + us + "'")
        btgestion("BtCOT", "COTIZACIONES" + Chr(10) + "HOY=" + x + " - ACUMULADO MES= " + y)

        x = DSCL.valor_campo_OTROS("count(kcliente)", "fechacre='" + fr.HOY_FR + "' and usuarioc='" + US + "'")
        y = DSCL.valor_campo_OTROS("count(kcliente)", "Month(fechacre)=" + Now.Month.ToString + " and year(fechacre)=" + Now.Year.ToString + " and usuarioc='" + US + "'")
        fr.FR_CONTROL("BtCLI") = "CLIENTES HOY=" + x + " ACUMULADO " + y
        btgestion("BtCLI", "CLIENTES" + Chr(10) + "HOY=" + x + " - ACUMULADO MES= " + y)

        x = DSCL.valor_campo_OTROS("count(kcliente)", "fechascl='" + fr.HOY_FR + "' and usuarioc='" + US + "'")
        y = DSCL.valor_campo_OTROS("count(kcliente)", "year(fechascl)=" + Now.Year.ToString + " and fechascl<='" + fr.HOY_FR + "' and usuarioc='" + US + "'")
        btgestion("BtTAR", "AGENDAMIENTO" + Chr(10) + "HOY=" + x + " - ACUMULADO MES= " + y)

        x = DSCL.valor_campo_OTROS("count(kllamada)", "fechas_llamada='" + fr.HOY_FR + "' and usuario='" + us + "'")
        y = DSCL.valor_campo_OTROS("count(kllamada)", "year(fechas_llamada)=" + Now.Year.ToString + " and fechas_llamada<='" + fr.HOY_FR + "' and usuario='" + us + "'")
        btgestion("BtLLA", "LLAMADAS" + Chr(10) + "HOY=" + x + " - ACUMULADO MES= " + y)

    End Sub
    Private Sub btgestion(nombre As String, texto As String)
        Dim BtG As Button = _fr.FindControl(nombre)
        If BtG IsNot Nothing Then
            BtG.Text = texto
            BtG.Width = Unit.Percentage(100)
            BtG.Font.Size = FontUnit.Large
            AddHandler BtG.Click, AddressOf CLIC_BtGESTION
        End If
    End Sub

    Private Sub CLIC_BtGESTION(sender As Object, e As EventArgs)
        Dim BtC As Button = sender : Dim rfr As String = Nothing
        Select Case BtC.ID
            Case "BtTAR"
                rfr = "tr"
            Case "BtCLI"
                rfr = "cl"
            Case "BtCOT"
                rfr = "ct"
            Case "BtLLA"
                rfr = "ll"
        End Select
        fr.redir("?us=" + fr.USERLOGUIN + "&fr=" + rfr)
    End Sub
    Private LbTL As New Label
    Private Sub carga_grill(tb As DataTable, campos As String, referencia As Integer)
        Dim x As Integer = 0

        For Each row As DataRow In tb.Rows
            Dim tx As String = Nothing
            Dim Bt As New Button
            Bt.Width = Unit.Percentage(100)
            Bt.Font.Size = FontUnit.Large
            Dim CP() As String = campos.Split(",")
            For y As Integer = 0 To CP.Count - 1
                If row.Item(CP(y)).ToString.Contains("00:00") Then
                    tx += CDate(row.Item(CP(y))).ToLongDateString + Chr(10)
                Else
                    tx += row.Item(CP(y)).ToString + Chr(10)
                End If
            Next
            Bt.Text = tx 'CDate(row.Item(13)).ToLongDateString + Chr(10) + row.Item("nombre").ToString '+ Chr(10) + row.Item("obscl").ToString
            Bt.CommandName = row.Item(referencia)
            Bt.ToolTip = row.Item(referencia)
            AddHandler Bt.Click, AddressOf clic_btll
            If x = 0 Then
                Bt.BackColor = Drawing.Color.Gray
                x = 1
            Else
                Bt.BackColor = Drawing.Color.White
                x = 0
            End If
            _fr.Controls.Add(Bt)
        Next
    End Sub

    Private Sub clic_btll(sender As Object, e As EventArgs)
        Dim bt As Button = sender
        Select Case fr.reque("fr")
            Case "cl", "tr"
                fr.redir("?fr=mc&tl=" + bt.CommandName)
            Case "ct"
                Dim NM As String = DSCL.valor_campo("KTELEFONO", "KCLIENTE=" + bt.CommandName)
                fr.redir("?fr=mc&tl=" + NM)
        End Select
    End Sub

    Private Sub carga_llamada()
        If fr.reque("fr") = "mc" Then
            fr.FORMULARIO("LLAMADA", "TxNOMBRE,TxTELEFONO")
        ElseIf fr.reque("fr") = "sg" Then
            fr.FORMULARIO("LLAMADA", "TxNOMBRE,TxTELEFONO,DrLLANTA_INTERES,TmSEGUIMIENTO,TfPROXIMO_SEGUIMIENTO", True)
        End If
        carga_grill(SGC.Carga_tablas("KCLIENTE=" + VAL_CLIENTE("KCLIENTE"), "fechasg desc", "TOP(3) FECHASG,COMENTARIO"), "FECHASG,COMENTARIO", 0)
        Dim nm As String = Nothing
        If VAL_CLIENTE("NOMBRE") Is Nothing Then
            nm = fr.reque("nm")
            ncliente = True
        Else
            nm = VAL_CLIENTE("NOMBRE")
            ncliente = False
        End If
        fr.FR_CONTROL("TxNOMBRE") = nm
        fr.FR_CONTROL("TxTELEFONO") = tel
        fr.FR_CONTROL("DrLLANTA_INTERES") = fr.DrPARAMETROS("CLIENTE", "LLANTA INTERES")
        fr.FR_CONTROL("BtGUARDAR", evento:=AddressOf seg_cliente) = Nothing
    End Sub
    Private Function VAL_CLIENTE(CAMPO As String) As String
        Return DSCL.valor_campo(CAMPO, "KTELEFONO=" + tel)
    End Function
    Private Sub seg_cliente()
        Dim TF, NM, TI, NI, EM, US, CI, DI, CE, TP, FS, OB, ORG, FN, FEX, RF, INT As String
        TF = tel : NM = fr.FR_CONTROL("TxNOMBRE") : TI = fr.FR_CONTROL("DrLLANTA_INTERES")
        If ncliente = True Then
            DSCL.insertardb(TF + ",'" + NM + "','',0,'','ACTIVO','" + fr.USERLOGUIN + "','','',0,'','PROSPECTO','" + Now.ToString("yyyy-MM-dd") + "','" + fr.FR_CONTROL("TmSEGUIMIENTO") + "','LLAMADA','" + fr.FR_CONTROL("TfPROXIMO_SEGUIMIENTO") + "','" + fr.HOY_FR + "','','" + fr.HOY_FR + "','" + TI + "'", True)
        Else
            DSCL.actualizardb("ktelefono=" + fr.FR_CONTROL("TxTELEFONO") + ",nombre='" + fr.FR_CONTROL("TxNOMBRE") + "',llanta_interes='" + TI + "',obscl='" + fr.FR_CONTROL("TmSEGUIMIENTO") + "',fechascl='" + fr.FR_CONTROL("TfPROXIMO_SEGUIMIENTO") + "'", "ktelefono=" + tel)
        End If
        SGC.insertardb(VAL_CLIENTE("KCLIENTE") + ",'" + Now.ToString(SGC.formato_fechal) + "','" + fr.FR_CONTROL("TmSEGUIMIENTO") + "','" + fr.USERLOGUIN + "'")
        fr.redireccion("rllamada.aspx")
    End Sub

End Class
