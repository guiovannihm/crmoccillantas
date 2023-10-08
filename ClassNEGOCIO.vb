Imports Classcatalogoch
Public Class ClassCOTIZACION
    Private CT As ClassConstructor22
    Private lg As New ClassLogin

    Private dsct As New carga_dssql("cotizaciones")
    Private dscl As New carga_dssql("clientes")
    Private dssg As New carga_dssql("seguimiento")
    Private dspa As New carga_dssql("parametros")
    Private Shadows cam, cr, fil, pf, cl, ctz As String
    Private FR As Panel

    Sub New(PANEL As Panel, PERFIL As String)
        FR = PANEL
        dsct.campostb = "kcot-key,kcliente-bigint,fechan-date,tvehiculo-varchar(100),tterreno-varchar(100),posicion-varchar(100),estadon-varchar(50),usuarion-varchar(100),referencia-varchar(200),fechaseg-date,tcarga-varchar(250),encalidad-varchar(100),fpago-varchar(250),ciudaden-varchar(100),OBS-varchar(500)"
        dssg.campostb = "kseg-key,kCOT-bigint,fechas-date,tseguimiento-varchar(100),notas-text,usuarios-varchar(100),causal-varchar(100)"
        pf = PERFIL
        CT = New ClassConstructor22(PANEL, "default.aspx", "COTIZACIONES")
        lg.APP_PARAMETROS("COTIZACION") = "TIPO VEHICULO,TIPO TERRENO,POSICION,EN CALIDAD,CAUSAL"
        cr = Nothing : fil = Nothing
        Select Case CT.reque("fr")
            Case "COTIZACIONES"
                COTIZACIONES()
            Case "COTIZACION"
                cl = CT.reque("cl")
                ctz = CT.reque("ct")
                COTIZACION()
            Case "SEGUIMIENTO"
                SEGUIMIENTO()
        End Select
    End Sub
    Private Sub COTIZACION()
        cam = "BtCLIENTE,LbFECHA,TxTIPO_VEHICULO,TxREFERENCIAS,DrTIPO_TERRENO,DrPOSICION,DrFP-FORMA DE PAGO,TmOBSN,TxTC-TIPO_CARGA"
        If pf >= 2 Then
            cam += ",DrASESOR"
        End If
        If cl IsNot Nothing And ctz Is Nothing Then
            CT.FORMULARIO("NUEVA COTIZACION", cam, True,, lg.MODULOS)
            CT.FR_CONTROL("TxTIPO_VEHICULO", focus:=True) = Nothing
            CT.FR_CONTROL("LbFECHA") = Now.ToString("yyyy-MM-dd")
            CT.FR_CONTROL("DrCE") = CT.DrPARAMETROS("CLIENTE", "CIUDAD")
            CT.FR_CONTROL("DrTIPO_TERRENO") = CT.DrPARAMETROS("COTIZACION", "TIPO TERRENO")
            CT.FR_CONTROL("DrPOSICION") = CT.DrPARAMETROS("COTIZACION", "POSICION")
            'CT.FR_CONTROL("DrEC") = CT.DrPARAMETROS("COTIZACION", "EN CALIDAD")
            CT.FR_CONTROL("DrFP") = "CONTADO,CREDITO"
            CT.FR_CONTROL("DrREFERIDO") = "NO,SI"
            CT.FR_CONTROL("DrREFERIDO") = "=" + dscl.valor_campo("REFERERIDO", "KCLIENTE=" + cl)
            CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GNCOTIZACION) = "SIGUIENTE"
        ElseIf ctz IsNot Nothing Then
            Dim EST() As String = dsct.valor_campo("ESTADON", "KCOT=" + ctz).Split(" ")
            Dim CTF As Boolean = False
            If CInt(EST(0)) < 2 Then
                CTF = True
            End If
            CT.FORMULARIO("COTIZACION " + ctz, cam, CTF,, lg.MODULOS)
            cl = dsct.valor_campo("kcliente", "KCOT=" + ctz)
            CT.FR_CONTROL("LbFECHA") = dsct.valor_campo("FECHAN", "KCOT=" + ctz)
            'CT.FR_CONTROL("DrCE", CTF, dsct.dtparametros("CLIENTE", "CIUDAD")) = "VALOR=" + dsct.valor_campo("CIUDADEN", "KCOT=" + ctz)
            CT.FR_CONTROL("TxTIPO_VEHICULO", CTF, focus:=True) = dsct.valor_campo("TVEHICULO", "KCOT=" + ctz)
            CT.FR_CONTROL("DrTIPO_TERRENO", CTF, dsct.dtparametros("COTIZACION", "TIPO TERRENO")) = "VALOR=" + dsct.valor_campo("TTERRENO", "KCOT=" + ctz)
            CT.FR_CONTROL("DrPOSICION", CTF, dsct.dtparametros("COTIZACION", "POSICION")) = "VALOR=" + dsct.valor_campo("POSICION", "KCOT=" + ctz)
            CT.FR_CONTROL("TxTC", CTF) = dsct.valor_campo("TCARGA", "KCOT=" + ctz)
            CT.FR_CONTROL("DrEC", CTF, dsct.dtparametros("COTIZACION", "EN CALIDAD")) = "VALOR=" + dsct.valor_campo("ENCALIDAD", "KCOT=" + ctz)
            CT.FR_CONTROL("DrFP") = "CONTADO,CREDITO"
            CT.FR_CONTROL("DrFP", CTF) = "VALOR=" + dsct.valor_campo("FPAGO", "KCOT=" + ctz)
            CT.FR_CONTROL("TxREFERENCIAS", CTF) = dsct.valor_campo("REFERENCIA", "KCOT=" + ctz)
            CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GNCOTIZACION) = "ACTUALIZAR DATOS COTIZACION"
            CT.FR_CONTROL("DrREFERIDO") = "NO,SI"
            CT.FR_CONTROL("DrREFERIDO", CTF) = "=" + dscl.valor_campo("REFERERIDO", "KCLIENTE=" + cl)
            'CT.FR_CONTROL("TmOBSN", CTF) = ""
            CT.FR_CONTROL("TmOBSN", CTF) = dsct.valor_campo("OBS", "KCOT=" + ctz)
            If dsct.valor_campo("USUARION", "KCOT=" + ctz) = CT.USERLOGUIN And CInt(EST(0)) < 2 Then
                CT.FR_BOTONES("LLAMADA,WHATSAPP,CIERRE")
                CT.FR_CONTROL("BtLLAMADA", evento:=AddressOf BtSEGUIMIENTO) = Nothing
                CT.FR_CONTROL("BtWHATSAPP", evento:=AddressOf BtSEGUIMIENTO) = Nothing
                CT.FR_CONTROL("BtCIERRE", evento:=AddressOf BtSEGUIMIENTO) = Nothing
            ElseIf CInt(EST(0)) = 2 Then
                CT.FR_BOTONES("MULTIORDEN")
                CT.FR_CONTROL("BtMULTIORDEN", evento:=AddressOf BtSEGUIMIENTO) = Nothing
            End If
            lg.DrUSUARIO_USER(FR.FindControl("DrASESOR"), dsct.valor_campo("USUARION", "KCLIENTE=" + cl))
            CT.FORMULARIO_GR(Nothing, "GrSEG", "FECHAS-D,TSEGUIMIENTO,NOTAS,USUARIOS", Nothing, "SEGUIMIENTO", "KCOT=" + ctz,,, "KSEG DESC")
        Else
            CT.FORMULARIO("BUSCAR CLIENTE", "TnCELULAR,TnIDENTIFICACION,BtCONSULTAR")
            CT.FR_CONTROL("TnCELULAR", post:=True, evento:=AddressOf CONSULTA_CLIENTE) = 0
            CT.FR_CONTROL("TnIDENTIFICACION", post:=True, evento:=AddressOf CONSULTA_CLIENTE) = 0
            CT.FR_CONTROL("BtCONSULTAR", evento:=AddressOf CONSULTA_CLIENTE) = Nothing
        End If
        BtCLIENTE()
    End Sub

    Private Sub BtCLIENTE()
        If cl IsNot Nothing Then
            CT.FR_CONTROL("BtCLIENTE", evento:=AddressOf SEL_CL) = dscl.valor_campo("NOMBRE", "KCLIENTE=" + cl) + " - " + dscl.valor_campo("KTELEFONO", "KCLIENTE=" + cl)
            If dscl.valor_campo("USUARIOC", "KCLIENTE=" + cl) <> CT.USERLOGUIN Then
                CT.alerta("ESTE CLIENTE PERTENECE A " + dscl.valor_campo("USUARIOC", "KCLIENTE=" + cl))
            End If
        End If
    End Sub
    Private Sub CONSULTA_CLIENTE()
        Dim CEL, CED As String
        CEL = CT.FR_CONTROL("TnCELULAR") : CED = CT.FR_CONTROL("TnIDENTIFICACION")
        If CEL <> "0" And CEL IsNot Nothing Then
            cl = dscl.valor_campo("kcliente", "Ktelefono=" + CEL)
            If cl Is Nothing Then
                CT.redir("?fr=CLIENTE" + "&tel=" + CEL)
            Else
                CT.redir("?fr=COTIZACION&cl=" + cl)
            End If
        ElseIf CED <> "0" And CED IsNot Nothing Then
            cl = dscl.valor_campo("kcliente", "numeroid=" + CED)
            If cl Is Nothing Then
                CT.redir("?fr=CLIENTE" + "&cd=" + CED)
            Else
                CT.redir("?fr=COTIZACION&cl=" + cl)
            End If
        Else
            CT.redir("?fr=CLIENTE")
        End If
    End Sub
    Private Sub SEGUIMIENTO()
        ctz = CT.reque("ct")
        cl = dsct.valor_campo("kcliente", "KCOT=" + ctz)
        Dim cam, FSE As String : cam = "BtCLIENTE,BtCOTIZACION,LbFECHA,TfFECHA_PROXIMO_SEGUIMIENTO,TmDESCRIPCION-OBSERVACIONES"
        FSE = DateAdd(DateInterval.Day, 3, Now).ToString("yyyy-MM-dd")
        Select Case CT.reque("tsg")
            Case "CIERRE"
                cam += ",DrCIERRE,DrRAZON"
                'FSE = DateAdd(DateInterval.Day, 180, Now).ToString("yyyy-MM-dd")
            Case "WHATSAPP"
                CT.rewrite("window.open('https://wa.me/+57" + dscl.valor_campo("KTELEFONO", "KCLIENTE=" + cl) + "?text=.')")
        End Select
        CT.FORMULARIO("SEGUIMIENTO", cam, True,, "COTIZACIONES,CLIENTES")
        CT.FR_CONTROL("LbFECHA") = Now.ToString("yyyy-MM-dd")
        CT.FR_CONTROL("TfFECHA_PROXIMO_SEGUIMIENTO") = FSE
        CT.FR_CONTROL("TmDESCRIPCION", focus:=True) = Nothing
        CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GUARDAR_SEGUIMIENTO) = "SIGUIENTE"
        CT.FR_CONTROL("DrCIERRE", evento:=AddressOf Sel_DrRAZON, post:=True) = "2 GANADA,3 PERDIDA"
        CT.FR_CONTROL("DrRAZON", False) = CT.DrPARAMETROS("COTIZACION", "CAUSAL")
        'CT.FR_CONTROL("DrRAZON", False) = Nothing
        BtCLIENTE()
        CT.FR_CONTROL("BtCOTIZACION") = dsct.valor_campo("REFERENCIA", "KCOT=" + ctz)
    End Sub
    Private Sub Sel_DrRAZON(Sender As Object, e As EventArgs)
        Dim dr As DropDownList = Sender
        If dr.SelectedItem.Text = "3 PERDIDA" Then
            Dim DRR As DropDownList = FR.FindControl("DrRAZON")
            DRR.Enabled = True
        End If
        'If CT.FR_CONTROL("DrCIERRE") = "3 PERDIDA" Then
        '    CT.FR_CONTROL("DrRAZON", True) = CT.DrPARAMETROS("COTIZACION", "CAUSAL")
        'End If

    End Sub

    Private Sub GUARDAR_SEGUIMIENTO()
        Dim FE, TS, TD, ES, FP, CU As String
        FE = CT.FR_CONTROL("LbFECHA") : FP = CT.FR_CONTROL("TfFECHA_PROXIMO_SEGUIMIENTO", VALIDAR:=True) : TS = CT.reque("tsg") : TD = CT.FR_CONTROL("TmDESCRIPCION")
        'If FR.FindControl("DrRAZON") Is Nothing Then
        '    CU = ""
        'Else
        '    CU = CT.FR_CONTROL("DrRAZON")
        'End If

        If TS = "CIERRE" Then
            ES = CT.FR_CONTROL("DrCIERRE")
            If ES.Contains("PERDIDA") Then
                CU = CT.FR_CONTROL("DrRAZON") + " - "
            Else
                CU = ""
            End If
            FE = CT.HOY_FR
            TS += " - " + ES + " " + CU
        ElseIf TS = "" Then

        Else
            ES = "1 SEGUIMIENTO"
        End If


        If FP Is Nothing Then
            FP = Now.ToString("yyyy-MM-dd")
        End If
        If CT.validacion_ct = False Then
            If CDate(FE) >= CDate(FP) Then
                CT.alerta("LA FECHA DE PROXIMO SEGUIMIENTO NO PUEDE SER MENOR O IGUAL A HOY")
            Else
                dssg.insertardb(ctz + ",'" + FE + "','" + TS + "','" + TD + "','" + CT.USERLOGUIN + "','" + CU + "'", True)
                dsct.actualizardb("estadon='" + ES + "',FECHASEG='" + FE + "'", "KCOT=" + ctz)
                ES = CT.HOY_FR + " ACTUALIZO COTIZACION No " + ctz + "-" + ES.Replace("3 ", "").Replace("2 ", "") + " - " + CU + TD '+ Chr(10) + "-------------" + Chr(10) + dscl.valor_campo("OBSCL", "KCLIENTE=" + cl)

                dscl.actualizardb("FECHASCL='" + FP + "',OBSCL='" + ES + "'", "KCLIENTE=" + cl)
                If CT.reque("tsg") = "CIERRE" And CT.FR_CONTROL("DrCIERRE") = "2 GANADA" Then

                    CT.redir("?fr=MULTIORDEN&ct=" + ctz + "&#pfinal")
                Else
                    CT.redir("?fr=COTIZACION&ct=" + ctz)
                End If

            End If
        End If
    End Sub
    Private CRF As String
    Private Shadows US, ESCT, KCOT As String
    Private Sub COTIZACIONES()

        cr = Nothing
        If pf = 1 Or KCOT IsNot Nothing Or CT.reque("ct") IsNot Nothing Then
            If CT.reque("us") IsNot Nothing Then
                US = CT.reque("us")
            ElseIf US Is Nothing Then
                US = CT.USERLOGUIN
            End If
            cam = "KCOT-K,NUMERO;KCOT-BT,CLIENTE;NOMBRE-BT,FECHA_COTIZACION;FECHASEG-BT,FORMA_PAGO;FPAGO-BT"
            cr = " and usuarion='" + US + "' and month(fechaseg)=" + Now.Month.ToString + " and year(fechaseg)=" + Now.Year.ToString
            CT.FILTROS_GRID("estadon")
            CT.FR_CONTROL("DrESTADON",, dsct.Carga_tablas("usuarion='" + US + "'", "ESTADON", "ESTADON", True), AddressOf SEL_DR) = "ESTADON-ESTADON"
            fil = "and ESTADON='" + CT.FR_CONTROL("DrESTADON") + "'"
            CT.FORMULARIO_GR("COTIZACIONES " + CT.reque("us"), "GrCOTIZACION", cam, "NUEVO CLIENTE," + lg.MODULOS, evento:=AddressOf selGrCOTIZACION, filtros:=fil)
            CT.FR_CONTROL("DrESTADON",, dsct.Carga_tablas("usuarion='" + US + "'", "ESTADON", "ESTADON", True), AddressOf SEL_DR, post:=True) = "ESTADON-ESTADON"
            CARGA_GrCOTIZACIONN()
        ElseIf pf = 3 And ESCT IsNot Nothing Then
            cam = "KCOT-K,NUMERO;KCOT-BT,FECHA_COTIZACION;FECHASEG-BT,FORMA_PAGO;FPAGO-BT"
            cr = "usuarion='" + US + "' and month(fechaseg)=" + Now.Month.ToString + " and year(fechaseg)=" + Now.Year.ToString + " and ESTADON='" + ESCT + "'"
            'CT.FILTROS_GRID("estadon")
            CT.FR_CONTROL("DrESTADON",, dsct.Carga_tablas("usuarion='" + US + "'", "ESTADON", "ESTADON", True), AddressOf SEL_DR) = "ESTADON-ESTADON"
            fil = "and ESTADON='" + ESCT + "'"
            CT.FORMULARIO_GR("COTIZACIONES " + CT.reque("us"), "GrCOTIZACION", cam, "NUEVO CLIENTE," + lg.MODULOS, "cotizaciones", cr, evento:=AddressOf selGrCOTIZACION)
            'CARGA_GrCOTIZACIONN()
        ElseIf CT.reque("us") IsNot Nothing Then
            US = CT.reque("us")
            cam = "ESTADON-K,ESTADO;ESTADON-BT,TOTAL-COUNT(USUARION)"
            cr = "usuarion='" + US + "' and month(fechaseg)=" + Now.Month.ToString + " and year(fechaseg)=" + Now.Year.ToString
            CT.FORMULARIO_GR("RESUMEN COTIZACIONES DE " + US + " PARA EL MES DE " + MonthName(Now.Month), "GrCOTIZACION", cam, lg.MODULOS, "COTIZACIONES", cr, AddressOf selGrCOTIZACION,, "USUARION")
        Else
            'cam = "USUARION-K,ASESOR;USUARION-BT,ESTADO;ESTADON,TOTAL-COUNT(USUARION)"
            cam = "USUARION-K,ASESOR;USUARION-BT,TOTAL-COUNT(USUARION)"
            cr = "month(fechaseg)=" + Now.Month.ToString + " and year(fechaseg)=" + Now.Year.ToString
            fil = Nothing
            CT.FORMULARIO_GR("RESUMEN DE COTIZACIONES " + MonthName(Now.Month), "GrCOTIZACION", cam, lg.MODULOS, "COTIZACIONES", cr, AddressOf selGrCOTIZACION,, "USUARION")

        End If

    End Sub
    Private Sub CARGA_GrCOTIZACIONN()
        Dim dscls As New carga_dssql("CLIENTES C, COTIZACIONES N")
        CT.FR_CONTROL("GrCOTIZACION", db:=dscls.Carga_tablas("c.kcliente=n.kcliente" + cr + fil, "FECHASEG")) = Nothing
        If CT.movil() = True Then
            cam = cam.Replace("-BT", "").Replace("-K", "").Replace("-D", "")
            Dim cam1 As String = Nothing
            For Each str As String In cam.Split(",")
                If cam1 IsNot Nothing Then
                    cam1 += ","
                End If
                If str.Contains(";") = True Then
                    Dim st1() As String = str.Split(";")
                    cam1 += st1(1)
                Else
                    cam1 += str
                End If
            Next

            Dim GrC As GridView = FR.FindControl("GrCOTIZACION")
            For Each GROW As GridViewRow In GrC.Rows
                Dim scam As String = Nothing
                'cr += " and n.kcot=" + GROW.Cells(0).Text
                For Each row As DataRow In dscls.Carga_tablas("c.kcliente=n.kcliente" + cr + " and n.kcot=" + GROW.Cells(0).Text + " " + fil, , cam1).Rows
                    For x As Integer = 1 To dscls.Carga_tablas("c.kcliente=n.kcliente" + cr + " " + fil, , cam1).Columns.Count - 1
                        scam += row.Item(x).ToString + "<br>"
                    Next

                Next
                Dim LtB As New LinkButton
                LtB.Text = scam
                LtB.CommandName = "Select"
                GROW.Cells(1).Controls.Add(LtB)
                GROW.Cells(1).HorizontalAlign = HorizontalAlign.Center
                GrC.Columns(2).Visible = False
            Next
        End If
    End Sub

    Private Sub SEL_DR(sender As Object, e As EventArgs)
        Dim dr As DropDownList = sender
        Select Case dr.ID
            Case "DrESTADON"
                fil = " And ESTADON='" + CT.FR_CONTROL("DrESTADON") + "'"
                Case "DrAÑO", "DrMES"
                cr = " and year(fechaseg)=" + CT.FR_CONTROL("DrAÑO") + " and MONTH(fechaseg)=" + CT.FR_CONTROL("DrMES")
            Case "DrESTADON"
                CRF = " and estadon='" + CT.FR_CONTROL("DrESTADON") + "'"
        End Select
        CARGA_GrCOTIZACIONN()
    End Sub

    Private Sub selGrCOTIZACION()
        If pf = 1 Or KCOT IsNot Nothing Then
            CT.redir("?fr=COTIZACION&ct=" + CT.FR_CONTROL("GrCOTIZACION"))
        ElseIf US IsNot Nothing And ESCT IsNot Nothing Then
            'KCOT = CT.FR_CONTROL("GrCOTIZACION")
            CT.redir("?fr=COTIZACION&ct=" + CT.FR_CONTROL("GrCOTIZACION"))
        ElseIf US IsNot Nothing And ESCT Is Nothing Then
            ESCT = CT.FR_CONTROL("GrCOTIZACION")
            CT.redir("?fr=COTIZACIONES")
        ElseIf pf > 1 And US Is Nothing Then
            CT.redir("?fr=COTIZACIONES&us=" + CT.FR_CONTROL("GrCOTIZACION"))
        ElseIf pf > 1 Then
            US = Nothing : ESCT = Nothing : KCOT = Nothing
            CT.redir("?fr=COTIZACION&ct=" + CT.FR_CONTROL("GrCOTIZACION"))
        End If
    End Sub

    Private Sub BtSEGUIMIENTO(sender As Object, e As EventArgs)
        GNCOTIZACION()
        Dim bt As Button = sender
        If bt.Text = "MULTIORDEN" Then
            Dim dsmo As New carga_dssql("multiorden")
            Dim mo As String = dsmo.valor_campo("kmo", "kcot=" + ctz)
            If mo IsNot Nothing Then
                mo = "&mo=" + mo
            End If
            CT.redir("?fr=MULTIORDEN&ct=" + ctz + mo)
        Else
            CT.redir("?fr=SEGUIMIENTO&tsg=" + bt.Text + "&ct=" + ctz)
        End If
    End Sub
    Public Sub GNCOTIZACION()
        Dim FE, TV, TT, PO, US, RF, TC, EC, FP, CE, RE, OB As String
        If pf >= 2 Then
            US = CT.FR_CONTROL("DrASESOR")
        Else
            US = CT.USERLOGUIN
        End If
        FE = CT.FR_CONTROL("LbFECHA") : TV = CT.FR_CONTROL("TxTIPO_VEHICULO", VALIDAR:=True) : TT = CT.FR_CONTROL("DrTIPO_TERRENO") : PO = CT.FR_CONTROL("DrPOSICION") : RF = CT.FR_CONTROL("TxREFERENCIAS", VALIDAR:=True)
        TC = CT.FR_CONTROL("TxTC") : EC = CT.FR_CONTROL("DrEC") : FP = CT.FR_CONTROL("DrFP") : CE = CT.FR_CONTROL("DrCE") : RE = CT.FR_CONTROL("DrREFERENCIA") : OB = CT.FR_CONTROL("TmOBSN")
        If CT.validacion_ct = False Then
            If ctz Is Nothing Then
                dsct.insertardb(cl + ",'" + FE + "','" + TV + "','" + TT + "','" + PO + "','0 NUEVA','" + US + "','" + RF + "','" + FE + "','" + TC + "','" + EC + "','" + FP + "','" + CE + "','" + OB + "'", True)
                ctz = dsct.valor_campo_OTROS("max(KCOT)", "KCLIENTE=" + cl + " AND FECHAN='" + FE + "' AND ESTADON='0 NUEVA' AND USUARION='" + CT.USERLOGUIN + "'")
                OB = CT.HOY_FR + OB + Chr(10) + "-------------" + Chr(10) + dscl.valor_campo("obscl", "KCLIENTE=" + cl)
                dscl.actualizardb("TIPOCL='CLIENTE',FECHASCL='" + Now.ToString("yyyy-MM-dd") + "',obscl='" + OB + "',REFERERIDO='" + RE + "'", "KCLIENTE=" + cl)
                CT.redir("?fr=COTIZACION&ct=" + ctz)
            Else
                dsct.actualizardb("TVEHICULO='" + TV + "',TTERRENO='" + TT + "',POSICION='" + PO + "',REFERENCIA='" + RF + "',TCARGA='" + TC + "',FPAGO='" + FP + "',OBS='" + OB + "'", "KCOT=" + ctz)
                OB = CT.HOY_FR + " - ACTUALIZO COTIZACION No " + ctz + " - " + OB '+ Chr(10) + "-------------" + Chr(10) + dscl.valor_campo("obscl", "KCLIENTE=" + cl)
                dscl.actualizardb("TIPOCL='CLIENTE',FECHASCL='" + CT.HOY_FR + "',obscl='" + OB + "'", "KCLIENTE=" + cl)
            End If
        End If
    End Sub
    Private Sub SEL_CL()
        If dsct.valor_campo("ESTADON", "KCOT=" + ctz) = "0 NUEVA" Then
            GNCOTIZACION()
        End If
        CT.redir("?fr=CLIENTE&cl=" + cl)
    End Sub
End Class
