Public Class ClassNEGOCIO
    Private CT As Classcatalogoch.ClassConstructor22
    Private lg As New Classcatalogoch.ClassLogin

    Private dsne As New Classcatalogoch.carga_dssql("negocios")
    Private dscl As New Classcatalogoch.carga_dssql("clientes")
    Private dssg As New Classcatalogoch.carga_dssql("seguimiento")
    Private Shared cam, pf, cl, ne As String
    Private FR As Panel

    Sub New(PANEL As Panel, PERFIL As String)
        dsne.campostb = "knegocio-key,kcliente-bigint,fechan-date,nvehiculos-bigint,tvehiculo-varchar(100),tterreno-varchar(100),posicion-varchar(100),estadon-varchar(50),usuarion-varchar(100),referencia-varchar(200),fechaseg-date,tcarga-varchar(250),encalidad-varchar(100),fpago-varchar(250),ciudaden-varchar(100)"
        dssg.campostb = "kseg-key,knegocio-bigint,fechas-date,tseguimiento-varchar(100),notas-text,usuarios-varchar(100)"
        pf = PERFIL
        CT = New Classcatalogoch.ClassConstructor22(PANEL, "default.aspx", "NEGOCIOS")
        lg.APP_PARAMETROS("NEGOCIO") = "TIPO VEHICULO,TIPO TERRENO,POSICION,EN CALIDAD"
        Select Case CT.reque("fr")
            Case "NEGOCIOS"
                NEGOCIOS()
            Case "NEGOCIO"
                cl = CT.reque("cl")
                ne = CT.reque("ne")
                negocio()
            Case "SEGUIMIENTO"
                SEGUIMIENTO()
        End Select
    End Sub
    Private Sub negocio()
        cam = "BtCLIENTE,LbFECHA,TnNUMERO_VEHICULOS-NO,DrTIPO_VEHICULOS,DrTIPO_TERRENO,TxTC-TIPO_CARGA,DrEC-EN_CALIDAD,TmREFERENCIAS,DrPOSICION,DrFP-FORMA DE PAGO,DrCE-CIUDAD DE ENTREGA"
        If pf = 2 Then
            cam += ",DrASESOR"
        End If
        If cl IsNot Nothing Then
            CT.FORMULARIO("NUEVO NEGOCIO", cam, True,, "NEGOCIOS,CLIENTES")
            CT.FR_CONTROL("TnNUMERO_VEHICULOS", focus:=True) = ""
            CT.FR_CONTROL("LbFECHA") = Now.ToString("yyyy-MM-dd")
            CT.DrPARAMETROS("DrTIPO_VEHICULOS", "NEGOCIO", "TIPO VEHICULO") = Nothing
            CT.DrPARAMETROS("DrTIPO_TERRENO", "NEGOCIO", "TIPO TERRENO") = Nothing
            CT.DrPARAMETROS("DrPOSICION", "NEGOCIO", "POSICION") = Nothing
            CT.DrPARAMETROS("DrEC", "NEGOCIO", "EN CALIDAD") = Nothing
            CT.DrPARAMETROS("DrCE", "CLIENTE", "CIUDAD") = Nothing
            CT.FR_CONTROL("DrFP") = "CONTADO,CREDITO"
            CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GNNEGOCIO) = Nothing
        ElseIf ne IsNot Nothing Then
            CT.FORMULARIO("NEGOCIO " + ne, cam, False,, "NEGOCIOS,CLIENTES")
            cl = dsne.valor_campo("kcliente", "knegocio=" + ne)
            CT.FR_CONTROL("LbFECHA") = dsne.valor_campo("FECHAN", "knegocio=" + ne)
            CT.FR_CONTROL("TnNUMERO_VEHICULOS", False) = dsne.valor_campo("NVEHICULOS", "knegocio=" + ne)
            CT.DrPARAMETROS("DrTIPO_VEHICULOS", "NEGOCIO", "TIPO VEHICULO", False) = dsne.valor_campo("TVEHICULO", "knegocio=" + ne)
            CT.DrPARAMETROS("DrTIPO_TERRENO", "NEGOCIO", "TIPO TERRENO", False) = dsne.valor_campo("TTERRENO", "knegocio=" + ne)
            CT.DrPARAMETROS("DrPOSICION", "NEGOCIO", "POSICION", False) = dsne.valor_campo("POSICION", "knegocio=" + ne)
            CT.FR_CONTROL("TxTC", False) = dsne.valor_campo("TCARGA", "knegocio=" + ne)
            CT.FR_CONTROL("DrEC", False) = dsne.valor_campo("ECALIDAD", "knegocio=" + ne)
            CT.FR_CONTROL("DrFP", False) = dsne.valor_campo("FPAGO", "knegocio=" + ne)
            CT.FR_CONTROL("DrCE", False) = dsne.valor_campo("CIUDADEN", "knegocio=" + ne)
            CT.FR_CONTROL("TmREFERENCIAS", False) = dsne.valor_campo("REFERENCIA", "knegocio=" + ne)
            CT.FR_CONTROL("GrGUARDAR", False) = Nothing
            Dim EST() As String = dsne.valor_campo("ESTADON", "knegocio=" + ne).Split(" ")
            If dsne.valor_campo("USUARION", "knegocio=" + ne) = CT.USERLOGUIN And CInt(EST(0)) < 2 Then
                CT.FR_BOTONES("LLAMADA,WHATSAPP,CIERRE")
                CT.FR_CONTROL("BtLLAMADA", evento:=AddressOf BtSEGUIMIENTO) = Nothing
                CT.FR_CONTROL("BtWHATSAPP", evento:=AddressOf BtSEGUIMIENTO) = Nothing
                CT.FR_CONTROL("BtCIERRE", evento:=AddressOf BtSEGUIMIENTO) = Nothing
            ElseIf CInt(EST(0)) = 2 Then
                CT.FR_BOTONES("MULTIORDEN")
                CT.FR_CONTROL("BtMULTIORDEN", evento:=AddressOf BtSEGUIMIENTO) = Nothing
            End If
            CT.FORMULARIO_GR(Nothing, "GrSEG", "FECHAS-D,TSEGUIMIENTO,NOTAS,USUARIOS", Nothing, "SEGUIMIENTO", "KNEGOCIO=" + ne,,, "KSEG DESC")
        Else
            CT.FORMULARIO("BUSCAR CLIENTE", "TnCELULAR=,TnIDENTIFICACION=,BtCONSULTAR")
            CT.FR_CONTROL("BtCONSULTAR", evento:=AddressOf CONSULTA_CLIENTE) = Nothing


        End If
        BtCLIENTE()

    End Sub

    Private Sub BtCLIENTE()
        If cl IsNot Nothing Then
            CT.FR_CONTROL("BtCLIENTE", evento:=AddressOf SEL_CL) = dscl.valor_campo("NOMBRE", "KCLIENTE=" + cl) + " - " + dscl.valor_campo("KTELEFONO", "KCLIENTE=" + cl)
        End If
    End Sub
    Private Sub CONSULTA_CLIENTE()
        Dim CEL, CED As String
        CEL = CT.FR_CONTROL("TnCELULAR") : CED = CT.FR_CONTROL("TnIDENTIFICACION")
        If CEL <> "0" And CEL IsNot Nothing Then
            cl = dscl.valor_campo("kcliente", "Ktelefono=" + CEL)
            CT.redir("?fr=NEGOCIO&cl=" + cl)
        ElseIf CED <> "0" And CED IsNot Nothing Then
            cl = dscl.valor_campo("kcliente", "numeroid=" + CED)
            CT.redir("?fr=NEGOCIO&cl=" + cl)
        Else
            CT.redir("?fr=CLIENTE")
        End If
    End Sub
    Private Sub SEGUIMIENTO()
        ne = CT.reque("ne")
        cl = dsne.valor_campo("kcliente", "knegocio=" + ne)
        Dim cam, FSE As String : cam = "BtCLIENTE,BtNEGOCIO,LbFECHA,TfFECHA_PROXIMO_SEGUIMIENTO,TmDESCRIPCION-OBSERVACIONES"
        FSE = DateAdd(DateInterval.Day, 3, Now).ToString("yyyy-MM-dd")
        Select Case CT.reque("tsg")
            Case "CIERRE"
                cam += ",DrCIERRE"
                FSE = Now.ToString("yyyy-MM-dd")
        End Select
        CT.FORMULARIO("SEGUIMIENTO", cam, True,, "NEGOCIOS,CLIENTES")
        CT.FR_CONTROL("LbFECHA") = Now.ToString("yyyy-MM-dd")
        CT.FR_CONTROL("TfFECHA_PROXIMO_SEGUIMIENTO") = FSE
        CT.FR_CONTROL("TmDESCRIPCION", focus:=True) = Nothing
        CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GUARDAR_SEGUIMIENTO) = Nothing
        CT.FR_CONTROL("DrCIERRE") = "2 GANADA,3 PERDIDA"
        BtCLIENTE()
        CT.FR_CONTROL("BtNEGOCIO") = dsne.valor_campo("DESCRIPCION", "KNEGOCIO=" + ne)
    End Sub
    Private Sub GUARDAR_SEGUIMIENTO()
        Dim FE, TS, TD, ES, FP As String
        FE = CT.FR_CONTROL("LbFECHA") : FP = CT.FR_CONTROL("TfFECHA_PROXIMO_SEGUIMIENTO", VALIDAR:=True) : TS = CT.reque("tsg") : TD = CT.FR_CONTROL("TmDESCRIPCION")
        If TS = "CIERRE" Then
            ES = CT.FR_CONTROL("DrCIERRE")
        Else
            ES = "1 SEGUIMIENTO"
        End If
        If FP Is Nothing Then
            FP = Now.ToString("yyyy-MM-dd")
        End If
        If CT.validacion_ct = False Then
            If CDate(FE) > CDate(FP) Then
                CT.alerta("LA FECHA DE PROXIMO SEGUIMIENTO NO PUEDE SER MENOR O IGUAL A HOY")
            Else
                dssg.insertardb(ne + ",'" + FE + "','" + TS + "','" + TD + "','" + CT.USERLOGUIN + "'", True)
                dsne.actualizardb("estadon='" + ES + "',FECHASEG='" + FP + "'", "knegocio=" + ne)
                CT.redir("?fr=NEGOCIO&ne=" + ne)
            End If
        End If


    End Sub
    Private Sub NEGOCIOS()
        Select Case pf
            Case "2"
                CT.FORMULARIO_GR("PANEL DE NEGOCIOS", "GrCONTROL", "ASESOR-SUM(USUARION),TIPO VEHICULO-SUM(TVEHICULO)", "NEGOCIO,CLIENTES", "clientes c,negocios n", "c.kcliente=n.kcliente",, "estadon")
            Case "1"
                CT.FORMULARIO_GR("NEGOCIOS", "GrNEGOCIO", "KNEGOCIO-K,NOMBRE-BT,FECHAN-BT,NVEHICULOS-BT,TVEHICULO-BT", "NEGOCIO,CLIENTES", "CLIENTES C,NEGOCIOS N", "c.kcliente=n.kcliente and usuarion='" + CT.USERLOGUIN + "'", AddressOf selGrNEGOCIO, "ESTADON")
        End Select
    End Sub
    Private Sub selGrNEGOCIO()
        CT.redir("?fr=NEGOCIO&ne=" + CT.FR_CONTROL("GrNEGOCIO"))
    End Sub

    Private Sub BtSEGUIMIENTO(sender As Object, e As EventArgs)
        Dim bt As Button = sender
        If bt.Text = "MULTIORDEN" Then
            CT.redir("?fr=MULTIORDEN&ne=" + ne)
        Else
            CT.redir("?fr=SEGUIMIENTO&tsg=" + bt.Text + "&ne=" + ne)
        End If

    End Sub
    Private Sub GNNEGOCIO()
        Dim FE, NV, TV, TT, PO, US, RF, TC, EC, FP, CE As String
        If pf = 2 Then
            US = CT.FR_CONTROL("DrASESOR")
        Else
            US = CT.USERLOGUIN
        End If
        FE = CT.FR_CONTROL("LbFECHA") : NV = CT.FR_CONTROL("TnNUMERO_VEHICULOS", VALIDAR:=True) : TV = CT.FR_CONTROL("DrTIPO_VEHICULOS") : TT = CT.FR_CONTROL("DrTIPO_TERRENO") : PO = CT.FR_CONTROL("DrPOSICION") : RF = CT.FR_CONTROL("TmREFERENCIAS", VALIDAR:=True)
        TC = CT.FR_CONTROL("TxTC") : EC = CT.FR_CONTROL("DrEC") : FP = CT.FR_CONTROL("DrFP") : CE = CT.FR_CONTROL("DrCE")
        If CT.validacion_ct = False Then
            dsne.insertardb(cl + ",'" + FE + "'," + NV + ",'" + TV + "','" + TT + "','" + PO + "','0 NUEVA','" + US + "','" + RF + "','" + FE + "','" + TC + "','" + EC + "','" + FP + "','" + CE + "'", True)
            CT.redir("?fr=NEGOCIO&ne=" + dsne.valor_campo("KNEGOCIO", "KCLIENTE=" + cl + " AND FECHAN='" + FE + "' AND ESTADON='0 NUEVA' AND USUARION='" + CT.USERLOGUIN + "'"))
        End If



    End Sub
    Private Sub SEL_CL()
        CT.redir("?fr=CLIENTE&cl=" + cl)
    End Sub
End Class
