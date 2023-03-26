Imports Classcatalogoch
Public Class ClassCLIENTES
    Private CT As ClassConstructor22
    Private lg As New ClassLogin

    Private dscl As New carga_dssql("clientes")
    Private dsct As New carga_dssql("COTIZACIONES")
    Private Shared cam, pf, cl, fil, US, BC, CRI, TL As String
    Private FR As Panel
    Sub New(PANEL As Panel, perfil As String)
        FR = PANEL
        cl = Nothing
        dscl.campostb = "kcliente-key,ktelefono-bigint,nombre-varchar(250),tidentificacion-varchar(100),numeroid-bigint,empresa-varchar(250),estadoc-varchar(50),usuarioc-varchar(100),ciudad-varchar(250),direccion-varchar(250),kclmaster-bigint,email-varchar(250),tipocl-varchar(50),fechascl-date,obscl-varchar(500),origencl-varchar(100),fechanc-date,fechaex-date,refererido-varchar(2)"
        pf = perfil
        CT = New ClassConstructor22(PANEL, "default.aspx", "CLIENTES")
        lg.APP_PARAMETROS("CLIENTE") = "CIUDAD,TIPO IDENTIFICACION,PERSONA,ORIGEN"
        Select Case CT.reque("fr")
            Case "CLIENTES"
                CLIENTES()
            Case "CLIENTE"
                CLIENTE()
            Case "CONTACTO"
                CONTACTO()
        End Select
    End Sub
    Private Sub CLIENTES()
        CRI = Nothing
        cam = "NOMBRE-BT,IDENTIFICACION;NUMEROID-BT,CELULAR;KTELEFONO-BT,CIUDAD-BT,FECHA_ULTIMO_SEG;FECHASCL-D"

        If pf = 1 Then
            fil = "TIPOCL,month(fechascl)#,year(fechascl)#"
            CRI = "USUARIOC='" + CT.USERLOGUIN + "'"
        Else
            cam += ",ASESOR;USUARIOC-BT"
            fil = "USUARIOC,TIPOCL,month(fechascl)#,year(fechascl)#"
        End If
        If CT.FILTROS_GRID(fil) = False Then
            CT.FR_CONTROL("DrTIPOCL",, dscl.Carga_tablas(CRI, "TIPOCL", "TIPOCL", True), AddressOf CT.sel_drfiltro, post:=True) = "TIPOCL-TIPOCL"
            CT.DrMES("Drmonth(fechascl)#", Nothing)
            CT.DrYEAR("Dryear(fechascl)#", 2023, Nothing)
            If pf > 1 Then
                CT.FR_CONTROL("DrUSUARIOC",, dscl.Carga_tablas(CRI, "USUARIOC", "USUARIOC", True), AddressOf CT.sel_drfiltro, post:=True) = "USUARIOC-USUARIOC"
            End If
        End If
        CT.FORMULARIO_GR("CLIENTES", "GrCLIENTE", "KCLIENTE-K," + cam, "NUEVO CLIENTE," + lg.MODULOS, "CLIENTES", CRI, AddressOf SEL_CLIENTES, , "FECHASCL DESC")
    End Sub
    Private Sub SEL_CLIENTES()
        CT.redir("?fr=CLIENTE&cl=" + CT.FR_CONTROL("GrCLIENTE"))
    End Sub
    Private Sub CLIENTE()
        If CT.reque("cl") IsNot Nothing Then
            cl = CT.reque("cl")
        End If
        cam = "TnTELEFONO-CELULAR,TxNOMBRE,DrTIPO_IDENTIFICACION,TnNUMERO,TfFECHANC-FECHA NACIMIENTO,TfFECHAEX-FECHA EXPEDICION DOC,DrEMPRESA-PERSONA,TxCIUDAD-CIUDAD_RESIDENCIA,TxDIRECCION,TxCORREO_ELECTRONICO,DrORIGEN"
        Dim BTE As Boolean = True
        If pf >= 2 Then
            cam += ",DrASESOR"
        ElseIf dscl.valor_campo("USUARIOC", "KCLIENTE=" + cl) <> CT.USERLOGUIN Then
            cam += ",LbASESOR=" + CT.USERLOGUIN
            fil = " And USUARION='" + CT.USERLOGUIN + "'"
            BTE = False
        End If

        If cl Is Nothing Then
            cam += ",DrGUARDAR-NECESITA LLANTAS"
            BTE = True
            TL = "CREAR CLIENTE"
        Else
            TL = dscl.valor_campo("TIPOCL", "KCLIENTE=" + cl)
            cam += ",DrREFERIDO,TfFSCL-FECHA PROXIMO SEGIMIENTO,TmOBSCL-OBSERVACIONES,BtWS"
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
        CT.redir("?fr=CLIENTE&cl=" + cl)
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
                CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf gcliente) = "ACTUALIZAR CLIENTE"
                CT.FR_BOTONES("NUEVO_COTIZACION,NUEVO_CONTACTO")
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
            CT.FR_CONTROL("TfFSCL", ACT) = CDate(dscl.valor_campo("FECHASCL", "KCLIENTE=" + cl)).ToString("yyyy-MM-dd")
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
                lg.DrUSUARIO_USER(FR.FindControl("DrASESOR"), US)
            Else
                CT.FR_CONTROL("LbASESOR") = US
                If US = CT.USERLOGUIN Then
                    CT.FORMULARIO_GR(Nothing, "GrNEG", cam, Nothing, "COTIZACIONES", "USUARION='" + US + "' AND KCLIENTE=" + cl, AddressOf SEL_GrNEG,, "ESTADON")
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
        For Each row As DataRow In dscl.Carga_tablas("KTELEFONO='" + TF + "'").Rows
            If cl Is Nothing Then
                CT.redir("?fr=CLIENTE&cl=" + row.Item("KCLIENTE").ToString)
            End If
        Next
        If NI Is Nothing Then
            NI = "0"
        ElseIf NI <> "0" Then
            For Each row As DataRow In dscl.Carga_tablas("NUMEROID='" + NI + "'").Rows
                If row.Item("USUARIOC") <> CT.USERLOGUIN Then
                    CT.alerta("NUMERO DE CEDULA YA CREADA Y EL CLIENTE PERTENECE A " + row.Item("USUARIOC"))
                    Exit Sub
                End If
            Next
        End If


        If CT.FR_CONTROL("DrGUARDAR") = "SI" Then
            TP = "CLIENTE"
        ElseIf CT.FR_CONTROL("DrGUARDAR") = "NO" Then
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
                dscl.insertardb(TF + ",'" + NM + "','" + TI + "'," + NI + ",'" + EM + "','ACTIVO','" + US + "','" + CI + "','" + DI + "',0,'" + CE + "','" + TP + "','" + Now.ToString("yyyy-MM-dd") + "','','" + ORG + "','" + FN + "','" + FEX + "','" + RF + "'", True)
                cl = dscl.valor_campo("KCLIENTE", "KTELEFONO=" + TF)
                dscl.addparametroDB("CLIENTE", "CIUDAD", CI)
                If TP = "CLIENTE" And cl IsNot Nothing Then
                        CT.redir("?fr=COTIZACION&cl=" + cl)
                    ElseIf cl IsNot Nothing Then
                        CT.redir("?fr=CLIENTE&cl=" + cl)
                    End If
                Else
                dscl.actualizardb("NOMBRE='" + NM + "',tidentificacion='" + TI + "',numeroid=" + NI + ",ciudad='" + CI + "',direccion='" + DI + "',usuarioc='" + US + "',email='" + CE + "',fechascl='" + FS + "',obscl='" + OB + "',ORIGENCL='" + ORG + "',FECHANC='" + FN + "',FECHAEX='" + FEX + "',REFERERIDO='" + RF + "'", "kcliente=" + cl, True)
                CT.redir("?fr=CLIENTE&cl=" + cl)
            End If

        End If

    End Sub


End Class
