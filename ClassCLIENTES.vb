Imports Classcatalogoch
Public Class ClassCLIENTES
    Private CT As ClassConstructor22
    Private lg As New ClassLogin

    Private dscl As New carga_dssql("clientes")
    Private dsct As New carga_dssql("COTIZACIONES")
    Private Shared cam, pf, cl, fil, US, BC, CRI, ORD, TL, MES, FRO As String
    Private FR As Panel
    Sub New(PANEL As Panel, perfil As String)
        FR = PANEL
        cl = Nothing
        dscl.campostb = "kcliente-key,ktelefono-bigint,nombre-varchar(250),tidentificacion-varchar(100),numeroid-bigint,empresa-varchar(250),estadoc-varchar(50),usuarioc-varchar(100),ciudad-varchar(250),direccion-varchar(250),kclmaster-bigint,email-varchar(250),tipocl-varchar(50),fechascl-date,obscl-varchar(1000),origencl-varchar(100),fechanc-date,fechaex-date,refererido-varchar(2),fechacre-date"
        pf = perfil
        CT = New ClassConstructor22(PANEL, "default.aspx", "CLIENTES")
        lg.APP_PARAMETROS("CLIENTE") = "CIUDAD,TIPO IDENTIFICACION,PERSONA,ORIGEN"
        If MES Is Nothing Then
            MES = Now.Month
        End If
        CRI = Nothing : ORD = Nothing : TL = Nothing : fil = Nothing
        Select Case CT.reque("fr")
            Case "TAREAS"
                TAREAS()
                FRO = CT.reque("fr")
            Case "CLIENTES", "PROSPECTOS"
                CLIENTES()
                FRO = CT.reque("fr")
            Case "CLIENTE"
                CLIENTE()
            Case "CONTACTO"
                CONTACTO()
        End Select
    End Sub
    Private Sub TAREAS()

        cam = "KCLIENTE-K,TIPO;TIPOCL-BT,NOMBRE-BT,CELULAR;KTELEFONO-BT,FECHA_ULTIMO_SEG;FECHASCL-D,OBSCL"
        If pf = 1 Or CT.reque("us") IsNot Nothing Then
            Dim US As String = CT.reque("us")
            If CT.reque("us") Is Nothing Then
                US = CT.USERLOGUIN
            End If
            fil = "month(fechascl)#,year(fechascl)#"
            CRI = "USUARIOC='" + US + "' AND MONTH(FECHASCL)=" + MES + " AND YEAR(FECHASCL)=" + Now.Year.ToString
            ORD = "FECHASCL ASC"
        Else
            cam = "USUARIOC-K,ASESOR;USUARIOC-BT,TAREAS_MES-COUNT(USUARIOC),ATRASADAS;USUARIOC"
            CRI = "MONTH(FECHASCL)=" + MES + " AND YEAR(FECHASCL)=" + Now.Year.ToString '+ " AND FECHASCL < '" + CT.HOY_FR + "'"
            fil = "USUARIOC,month(fechascl)#,year(fechascl)#"
            ORD = "FECHASCL ASC,USUARIOC"
        End If
        TL = "TAREAS" + " DE " + MonthName(CInt(MES))
        CT.FORMULARIO_GR(TL, "GrTAREAS", cam, "NUEVO CLIENTE,CLIENTES," + lg.MODULOS, "CLIENTES", CRI, AddressOf SEL_CLIENTES, fil, ORD)
        Dim GrC As GridView = FR.FindControl("GrTAREAS")
        If CT.movil = False Then
            If pf = 1 Or CT.reque("us") IsNot Nothing Then
                For Each GROW As GridViewRow In GrC.Rows
                    If CDate(GROW.Cells(4).Text) < Now.ToShortDateString Then
                        GROW.Cells(1).BackColor = Drawing.Color.Red
                    ElseIf CDate(GROW.Cells(4).Text) = Now.ToShortDateString Then
                        GROW.Cells(1).BackColor = Drawing.Color.Yellow
                    ElseIf CDate(GROW.Cells(4).Text) > Now.ToShortDateString Then
                        GROW.Cells(1).BackColor = Drawing.Color.Green
                        GROW.Cells(1).ForeColor = Drawing.Color.White
                    End If
                    GROW.BorderWidth = 0
                Next
            ElseIf CT.reque("us") Is Nothing Then
                For Each GROW As GridViewRow In GrC.Rows
                    GROW.Cells(2).HorizontalAlign = HorizontalAlign.Center
                    GROW.Cells(3).HorizontalAlign = HorizontalAlign.Center
                    GROW.Cells(3).Font.Bold = True
                    GROW.Cells(3).ForeColor = Drawing.Color.White
                    GROW.Cells(3).BackColor = Drawing.Color.Red
                    GROW.Cells(3).Text = dscl.valor_campo_OTROS("COUNT(USUARIOC)", "USUARIOC='" + GROW.Cells(0).Text + "' AND FECHASCL < '" + CT.HOY_FR + "'")
                Next
            End If
        Else
            If pf = 1 Or CT.reque("us") IsNot Nothing Then
                cam = "KCLIENTE-K,TIPO;TIPOCL-BT,NOMBRE-BT,CELULAR;KTELEFONO-BT,FECHA_ULTIMO_SEG;FECHASCL-D,OBSCL"
                Dim US As String = CT.reque("us")
                If CT.reque("us") Is Nothing Then
                    US = CT.USERLOGUIN
                End If
                fil = "month(fechascl)#,year(fechascl)#"
                CRI = "USUARIOC='" + US + "' AND MONTH(FECHASCL)=" + MES + " AND YEAR(FECHASCL)=" + Now.Year.ToString
                ORD = "FECHASCL ASC"
                cam = cam.Replace("-BT", "").Replace("-K", "").Replace("-D", "")
                Dim cam1 As String = Nothing
                For Each str As String In cam.Split(",")
                    If cam1 IsNot Nothing Then
                        cam1 += ","
                    End If
                    If str.Contains(";") = True Then
                        Dim str1() As String = str.Split(";")
                        cam1 += str1(1)
                    Else
                        cam1 += str
                    End If
                Next
                Dim scam As String = Nothing
                For Each GROW As GridViewRow In GrC.Rows
                    CRI += " and kcliente=" + GROW.Cells(0).Text
                    For Each row As DataRow In dscl.Carga_tablas(CRI, ORD, cam1).Rows
                        For x As Integer = 0 To dscl.Carga_tablas(CRI, ORD, cam1).Columns.Count - 1
                            scam += row.Item(x).ToString + "<br>"
                        Next

                    Next
                    GROW.Cells(1).Text = scam
                    GROW.Cells(2).HorizontalAlign = HorizontalAlign.Center
                    GrC.Columns(1).Visible = False

                    If CDate(dscl.valor_campo("fechascl", "kcliente=" + GROW.Cells(0).Text)) < Now.ToShortDateString Then
                        GROW.Cells(2).BackColor = Drawing.Color.Red
                        GROW.Cells(2).ForeColor = Drawing.Color.White
                    ElseIf CDate(dscl.valor_campo("fechascl", "kcliente=" + GROW.Cells(0).Text)) = Now.ToShortDateString Then
                        GROW.Cells(2).BackColor = Drawing.Color.Yellow
                    ElseIf CDate(dscl.valor_campo("fechascl", "kcliente=" + GROW.Cells(0).Text)) > Now.ToShortDateString Then
                        GROW.Cells(2).BackColor = Drawing.Color.Green
                        GROW.Cells(2).ForeColor = Drawing.Color.White
                    End If
                    GROW.BorderWidth = 0
                Next

            Else

            End If

        End If
        BT_CONTROLESTR()

    End Sub
    Private Sub BT_CONTROLESTR()
        CT.FR_BOTONES("ANTES,DESPUES")
        Dim NMES As Integer = CInt(MES) - 1
        If NMES = 0 Then
            NMES = 12
        End If
        CT.FR_CONTROL("BtANTES", evento:=AddressOf CLIC_BT) = "<< - " + MonthName(NMES).ToUpper
        NMES = CInt(MES) + 1
        If NMES = 13 Then
            NMES = 1
        End If
        CT.FR_CONTROL("BtDESPUES", evento:=AddressOf CLIC_BT) = MonthName(NMES).ToUpper + " - >>"
        resumen_tareas()
    End Sub
    Private Sub resumen_tareas()
        Dim GrTR As GridView
        Dim camt, crit, grt As String
        camt = "" : crit = "" : grt = ""
        camt = "month(fechascl) as mes,count(fechascl) as pendientes"
        grt = "month(fechascl)"
        If pf = 1 Then
            crit = "USUARIOC='" + CT.USERLOGUIN + "' and fechascl < '" + CT.HOY_FR + "'"
        Else
            crit = "fechascl < '" + CT.HOY_FR + "'"
        End If
        If FR.FindControl("GrTR") Is Nothing Then
            GrTR = New GridView
            GrTR.ID = "GrTR"
            GrTR.ShowHeader = False
            GrTR.BorderStyle = BorderStyle.None
            GrTR.BackColor = Drawing.Color.Red
            GrTR.Font.Bold = True
            GrTR.HorizontalAlign = HorizontalAlign.Center
            GrTR.ForeColor = Drawing.Color.White
            FR.Controls.Add(GrTR)
        Else
            GrTR = FR.FindControl("GrTR")
        End If
        GrTR.DataSource = dscl.Carga_tablas_especial(camt, crit,, grt)
        GrTR.DataBind()
        For Each grow As GridViewRow In GrTR.Rows
            grow.Cells(0).Text = MonthName(grow.Cells(0).Text).ToUpper
            grow.Cells(1).Text = "TIENE " + grow.Cells(1).Text + " TAREAS ATRASADAS"
        Next
    End Sub
    Private Sub CLIC_BT(sender As Object, e As EventArgs)
        Dim BtC As Button = sender
        Select Case BtC.ID
            Case "BtANTES"
                MES = CInt(MES) - 1
                If MES = "0" Then
                    MES = "12"
                End If
                BtC.Text = MonthName(CInt(MES))
                FR.Controls.Clear()
            Case "BtDESPUES"
                MES = CInt(MES) + 1
                If MES = "13" Then
                    MES = "1"
                End If
                FR.Controls.Clear()
        End Select
        TAREAS()
    End Sub
    Private Sub CLIENTES()
        Dim MN As String = ""
        Dim us As String = CT.reque("us")
        If us Is Nothing Then
            us = CT.USERLOGUIN
        End If
        TL = CT.reque("fr")
        If lg.perfil = "1" Or CT.reque("us") IsNot Nothing Then

            cam = "KCLIENTE-K,NOMBRE-BT,CELULAR-BT;KTELEFONO-BT,TIPO-BT;TIDENTIFICACION-BT,NUMERO;NUMEROID-BT"
            CRI = "USUARIOC='" + us + "' AND "
            ORD = "NOMBRE"
            Select Case TL
                Case "CLIENTES"
                    CRI += "TIPOCL='CLIENTE'"
                    MN = "PROSPECTOS,"
                Case "PROSPECTOS"
                    CRI += "TIPOCL='PROSPECTO'"
                    MN = "CLIENTES,"
            End Select
        Else
            cam = "USUARIOC-K,ASESOR;USUARIOC-BT,TIPO;TIPOCL,TOTAL_" + TL + "-COUNT(USUARIOC)"
            ORD = Nothing
        End If
        CT.FORMULARIO_GR(TL + " " + us, "GrCL", cam, MN + lg.MODULOS, "CLIENTES", CRI, AddressOf SEL_CL,, ORD)
    End Sub
    Private Sub SEL_CLIENTES()
        If pf = 1 Or CT.reque("us") IsNot Nothing Then
            CT.redir("?fr=CLIENTE&cl=" + CT.FR_CONTROL("GrTAREAS"))
        Else
            CT.redir("?fr=TAREAS&us=" + CT.FR_CONTROL("GrTAREAS"))
        End If

    End Sub
    Private Sub CLIENTE()
        Dim BTE As Boolean = True
        If CT.reque("cl") IsNot Nothing Then
            cl = CT.reque("cl")
        End If
        If cl Is Nothing Then
            cam = "TnTELEFONO-CELULAR,TxNOMBRE,DrGUARDAR-NECESITA LLANTAS"
            BTE = True
            TL = "CREAR CLIENTE O PROSPECTO"
        Else
            TL = dscl.valor_campo("TIPOCL", "KCLIENTE=" + cl)
            Select Case TL
                Case "PROSPECTO"
                    cam = "TnTELEFONO-CELULAR,TxNOMBRE"
                Case "CLIENTE"
                    cam = "TnTELEFONO-CELULAR,TxNOMBRE,DrTIPO_IDENTIFICACION,TnNUMERO,TfFECHANC-FECHA NACIMIENTO,TfFECHAEX-FECHA EXPEDICION DOC,DrEMPRESA-PERSONA,TxCIUDAD-CIUDAD_RESIDENCIA,TxDIRECCION,TxCORREO_ELECTRONICO,DrORIGEN"
            End Select
            cam += ",DrREFERIDO,TfFSCL-FECHA PROXIMO SEGIMIENTO,TmOBSCL-OBSERVACIONES,BtWS"
        End If
        If pf >= 2 Then
            cam += ",DrASESOR"
        ElseIf dscl.valor_campo("USUARIOC", "KCLIENTE=" + cl) <> CT.USERLOGUIN And cl IsNot Nothing Then
            fil = " And USUARION='" + CT.USERLOGUIN + "'"
            BTE = False
        End If


        CT.FORMULARIO(TL, cam, BTE,, lg.MODULOS)
        CARGA_DCLIENTE()

    End Sub
    Private Sub CONTACTO()
        If CT.reque("cl") IsNot Nothing Then
            cl = CT.reque("cl")
        End If
        cam = "BtCLIENTE,TnTELEFONO,TxNOMBRE,DrCIUDAD,TxDIRECCION"
        If pf >= 2 Then
            cam += ",DrASESOR"
        Else
            cam += ",LbASESOR=" + CT.USERLOGUIN
        End If
        If CT.reque("ct") Is Nothing Then
            CT.FORMULARIO("CONTACTO", cam, True,, "CLIENTES,COTIZACIONES")
            CT.FR_CONTROL("BtCLIENTE", evento:=AddressOf SEL_CL) = dscl.valor_campo("NOMBRE", "KCLIENTE=" + cl) + " - " + dscl.valor_campo("KTELEFONO", "KCLIENTE=" + cl)
            CT.FR_CONTROL("DrCIUDAD") = dscl.valor_campo("CIUDAD", "KCLIENTE=" + cl)
        Else

        End If


    End Sub

    Private Sub gcontacto()
        Dim TL, NM, CI, DI As String
        TL = CT.FR_CONTROL("TnTELEFONO", VALIDAR:=True) : NM = CT.FR_CONTROL("TxNOMBRE", VALIDAR:=True) : CI = CT.FR_CONTROL("DrCIUDAD") : DI = CT.FR_CONTROL("TxDIRECCION", VALIDAR:=True)
        If CT.validacion_ct = False Then
            If CT.FR_CONTROL("BtGUARDAR") = "GUARDAR" Then
                dscl.insertardb(TL + ",'" + NM + "','',0,'','ACTIVO','" + CT.USERLOGUIN + "','" + CI + "','" + DI + "'," + cl)
            Else
                dscl.actualizardb("", "")
            End If
        End If


    End Sub
    Private Sub SEL_CL()
        If pf = 1 Or CT.reque("us") IsNot Nothing Then
            CT.redir("?fr=CLIENTE&cl=" + CT.FR_CONTROL("GrCL"))
        ElseIf CT.reque("us") Is Nothing Then
            Dim grc As GridView = FR.FindControl("GrCL")
            CT.redir("?fr=" + grc.SelectedRow.Cells(2).Text + "S&us=" + grc.SelectedRow.Cells(0).Text)
        End If

    End Sub
    Private ACT As Boolean
    Private Sub CARGA_DCLIENTE()
        US = CT.USERLOGUIN
        If cl Is Nothing Then
            CT.FR_CONTROL("TnTELEFONO") = CT.reque("tel")
            CT.FR_CONTROL("TxNOMBRE") = CT.reque("cd")
            CT.FR_CONTROL("DrTIPO_IDENTIFICACION") = CT.DrPARAMETROS(CT.reque("fr"), "TIPO IDENTIFICACION")
            CT.FR_CONTROL("DrORIGEN", , dscl.dtparametros("CLIENTE", "ORIGEN")) = "VALOR-VALOR"
            CT.FR_CONTROL("DrEMPRESA") = "NATURAL,JURUDICA"
            CT.FR_CONTROL("DrGUARDAR") = "NO,SI"
            CT.FR_CONTROL("TfFECHANC") = Now.ToString("yyyy-MM-dd")
            CT.FR_CONTROL("TfFECHAEX") = Now.ToString("yyyy-MM-dd")
            CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf gcliente) = "SIGUIENTE"
            If pf >= 2 Then
                lg.DrUSUARIO_USER(FR.FindControl("DrASESOR"))
            Else
                CT.FR_CONTROL("LbASESOR") = CT.USERLOGUIN
            End If
        Else
            If dscl.valor_campo("usuarioc", "KCLIENTE=" + cl) = CT.USERLOGUIN Or pf >= 2 Then
                ACT = True
                CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf gcliente) = "ACTUALIZAR " + TL
                CT.FR_BOTONES("NUEVO_COTIZACION")
            Else
                ACT = False
                CT.FR_BOTONES("NUEVO_COTIZACION")
            End If
            CT.FR_CONTROL("TnTELEFONO", False) = dscl.valor_campo("KTELEFONO", "KCLIENTE=" + cl)
            CT.FR_CONTROL("TxNOMBRE", ACT, focus:=True) = dscl.valor_campo("NOMBRE", "KCLIENTE=" + cl)
            CT.FR_CONTROL("DrTIPO_IDENTIFICACION", ACT, dscl.dtparametros("CLIENTE", "TIPO IDENTIFICACION")) = "VALOR=" + dscl.valor_campo("TIDENTIFICACION", "KCLIENTE=" + cl)
            CT.FR_CONTROL("TxCIUDAD", ACT) = dscl.valor_campo("CIUDAD", "KCLIENTE=" + cl)
            CT.FR_CONTROL("TnNUMERO", ACT) = dscl.valor_campo("NUMEROID", "KCLIENTE=" + cl)
            CT.FR_CONTROL("DrEMPRESA", ACT, dscl.dtparametros("CLIENTE", "PERSONA")) = "VALOR=" + dscl.valor_campo("EMPRESA", "KCLIENTE=" + cl)
            CT.FR_CONTROL("TxDIRECCION", ACT) = dscl.valor_campo("DIRECCION", "KCLIENTE=" + cl)
            CT.FR_CONTROL("DrORIGEN", ACT, dscl.dtparametros("CLIENTE", "ORIGEN")) = "VALOR=" + dscl.valor_campo("ORIGENCL", "KCLIENTE=" + cl)
            CT.FR_CONTROL("TxCORREO_ELECTRONICO", ACT) = dscl.valor_campo("EMAIL", "KCLIENTE=" + cl)
            CT.FR_CONTROL("TfFECHANC") = CDate(dscl.valor_campo("FECHANC", "KCLIENTE=" + cl)).ToString("yyyy-MM-dd")
            CT.FR_CONTROL("TfFECHAEX") = CDate(dscl.valor_campo("FECHAEX", "KCLIENTE=" + cl)).ToString("yyyy-MM-dd")
            If CT.HOY_FR(dscl.valor_campo("FECHASCL", "KCLIENTE=" + cl)) < CT.HOY_FR Then
                CT.FR_CONTROL("TfFSCL", ACT) = Now.ToString("yyyy-MM-dd")
            Else
                CT.FR_CONTROL("TfFSCL", ACT) = CDate(dscl.valor_campo("FECHASCL", "KCLIENTE=" + cl)).ToString("yyyy-MM-dd")
            End If
            CT.FR_CONTROL("TmOBSCL", ACT) = dscl.valor_campo("OBSCL", "KCLIENTE=" + cl)
            CT.FR_CONTROL("DrREFERIDO", ACT) = "NO,SI"
            CT.FR_CONTROL("DrREFERIDO", ACT) = dscl.valor_campo("REFERERIDO", "KCLIENTE=" + cl)
            CT.FR_CONTROL("BtWS", evento:=AddressOf CLI_BtWS) = "WHATSAPP"
            US = dscl.valor_campo("usuarioc", "KCLIENTE=" + cl)
            CT.FR_CONTROL("BtNUEVO_COTIZACION", evento:=AddressOf NCOTIZACION) = Nothing
            CT.FR_CONTROL("BtNUEVO_CONTACTO", evento:=AddressOf NCONTACTO) = Nothing
            CT.FR_CONTROL("BtEDITAR_CLIENTE", evento:=AddressOf BT_EDIT) = Nothing
            cam = "KCOT-K,No;KCOT-BT,FECHA_COTIZACION;FECHASEG-D,REFERENCIA,FORMA_PAGO;FPAGO,ESTADO;ESTADON"
            If pf >= 2 Then
                cam += ",ASESOR;USUARIOC"
                lg.DrUSUARIO_USER(FR.FindControl("DrASESOR"), dscl.valor_campo("USUARIOC", "KCLIENTE=" + cl))
            Else
                If dscl.valor_campo("USUARIOC", "KCLIENTE=" + cl) = CT.USERLOGUIN Then
                    CT.FORMULARIO_GR(Nothing, "GrNEG", cam, Nothing, "COTIZACIONES", "KCLIENTE=" + cl, AddressOf SEL_GrNEG,, "ESTADON")
                End If
            End If
        End If
    End Sub

    Private Sub CLI_BtWS()
        CT.rewrite("window.open('https://wa.me/+57" + dscl.valor_campo("KTELEFONO", "KCLIENTE=" + cl) + "?text=.')")
    End Sub
    Private Sub SEL_GrNEG()
        CT.redir("?fr=COTIZACION&ct=" + CT.FR_CONTROL("GrNEG"))
    End Sub
    Private Sub BT_EDIT()
        ACT = True
        CARGA_DCLIENTE()
    End Sub

    Private Sub NCONTACTO()
        CT.redir("?fr=CONTACTO&cl=" + cl)
    End Sub
    Private Sub NCOTIZACION()
        CT.redir("?fr=COTIZACION&cl=" + cl)
    End Sub
    Private Sub bus_tel()
        Dim tx1 As String = CT.FR_CONTROL("TxNOMBRE")
        If CT.FR_CONTROL("TnTELEFONO").Length = 10 Then
            cl = dscl.valor_campo("KCLIENTE", "ktelefono=" + CT.FR_CONTROL("TnTELEFONO"))
            If cl IsNot Nothing Then
                CT.redir("?fr=COTIZACION&cl=" + cl)
            Else
                CT.FR_CONTROL("TxNOMBRE") = tx1
            End If
        Else
            CT.alerta("NUMERO DE TELEFONO ")
        End If

    End Sub
    Private Sub BUS_NIDE()
        Dim tx1 As String = CT.FR_CONTROL("TxNOMBRE")
        cl = dscl.valor_campo("KCLIENTE", "NUMEROID=" + CT.FR_CONTROL("TnNUMERO"))
        If cl IsNot Nothing Then
            CT.redir("?fr=CLIENTE&cl=" + cl)
        Else
            CT.FR_CONTROL("TxNOMBRE") = tx1
            ' CT.FR_CONTROL("TxDIRECCION", focus:=True) = Nothing
        End If
        'CARGA_DCLIENTE()
    End Sub

    Public Sub gcliente()
        Dim TF, NM, TI, NI, EM, US, CI, DI, CE, TP, FS, OB, ORG, FN, FEX, RF As String
        TF = CT.FR_CONTROL("TnTELEFONO", VALIDAR:=True) : NM = CT.FR_CONTROL("TxNOMBRE", VALIDAR:=True) : TI = CT.FR_CONTROL("DrTIPO_IDENTIFICACION") : NI = CT.FR_CONTROL("TnNUMERO") : EM = CT.FR_CONTROL("DrEMPRESA")
        CI = CT.FR_CONTROL("TxCIUDAD") : DI = CT.FR_CONTROL("TxDIRECCION") : CE = CT.FR_CONTROL("TxCORREO_ELECTRONICO") : FS = CT.FR_CONTROL("TfFSCL") : OB = CT.FR_CONTROL("TmOBSCL")
        ORG = CT.FR_CONTROL("GrORIGEN") : FN = CT.FR_CONTROL("TfFECHANC") : FEX = CT.FR_CONTROL("TfFECHAEX") : US = dscl.valor_campo("USUARIOC", "KTELEFONO='" + TF + "'") : RF = CT.FR_CONTROL("DrREFERIDO")
        If TF.Length < 10 Then
            CT.alerta("EL NUMERO TELEFONICO NO PUEDE SER MENOR A 10 DIGITOS")
            Exit Sub
        End If
        For Each row As DataRow In dscl.Carga_tablas("KTELEFONO='" + TF + "'").Rows
            If cl Is Nothing Then
                CT.redir("?fr=CLIENTE&cl=" + row.Item("KCLIENTE").ToString)
            End If
        Next
        If NI Is Nothing Then
            NI = "0"
        ElseIf NI <> "0" Then
            For Each row As DataRow In dscl.Carga_tablas("NUMEROID='" + NI + "'").Rows
                If row.Item("USUARIOC") <> CT.USERLOGUIN And pf = 1 Then
                    CT.alerta("NUMERO DE CEDULA YA CREADA Y EL CLIENTE PERTENECE A " + row.Item("USUARIOC"))
                    Exit Sub
                End If
            Next
        End If


        If cl Is Nothing Then
            TP = "PROSPECTO"
        Else
            TP = dscl.valor_campo("TIPOCL", "KCLIENTE=" + cl)
        End If

        If CT.FR_CONTROL("DrASESOR") IsNot Nothing Then
            US = CT.FR_CONTROL("DrASESOR")
        Else
            US = CT.USERLOGUIN
        End If

        If TL Is Nothing And TL.Length >= 10 Then
            Exit Sub
        End If
        If FS Is Nothing Then
            FS = Now.ToString("yyyy-MM-dd")
        End If
        If OB Is Nothing Then
            OB = dscl.valor_campo("OBSCL", "KCLIENTE=" + cl)
        End If
        If CT.validacion_ct = False Then
            If CT.FR_CONTROL("BtGUARDAR") = "SIGUIENTE" Then
                dscl.insertardb(TF + ",'" + NM + "','" + TI + "'," + NI + ",'" + EM + "','ACTIVO','" + US + "','" + CI + "','" + DI + "',0,'" + CE + "','" + TP + "','" + Now.ToString("yyyy-MM-dd") + "','','" + ORG + "','" + FN + "','" + FEX + "','" + RF + "','" + CT.HOY_FR + "'", True)
                cl = dscl.valor_campo("KCLIENTE", "KTELEFONO=" + TF)
                dscl.addparametroDB("CLIENTE", "CIUDAD", CI)
                If CT.FR_CONTROL("DrGUARDAR") = "SI" And cl IsNot Nothing Then
                    CT.redir("?fr=COTIZACION&cl=" + cl)
                ElseIf cl IsNot Nothing Then
                    CT.redir("?fr=CLIENTE&cl=" + cl)
                End If
            Else
                'OB += Chr(10) + "-------------" + Chr(10) + dscl.valor_campo("obscl", "KCLIENTE=" + cl)
                dscl.actualizardb("NOMBRE='" + NM + "',tidentificacion='" + TI + "',numeroid=" + NI + ",ciudad='" + CI + "',direccion='" + DI + "',usuarioc='" + US + "',email='" + CE + "',fechascl='" + FS + "',obscl='" + OB + "',ORIGENCL='" + ORG + "',FECHANC='" + FN + "',FECHAEX='" + FEX + "',REFERERIDO='" + RF + "'", "kcliente=" + cl, True)
                CT.redir("?fr=CLIENTE&cl=" + cl)
            End If
        End If
    End Sub
End Class
