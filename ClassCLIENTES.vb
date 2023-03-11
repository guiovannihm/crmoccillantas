Public Class ClassCLIENTES
    Private CT As Classcatalogoch.ClassConstructor22
    Private lg As New Classcatalogoch.ClassLogin

    Private dscl As New Classcatalogoch.carga_dssql("clientes")
    Private dsne As New Classcatalogoch.carga_dssql("negocios")
    Private Shared cam, pf, cl, fil As String
    Private FR As Panel
    Sub New(PANEL As Panel, perfil As String)
        cl = Nothing
        dscl.campostb = "kcliente-key,ktelefono-bigint,nombre-varchar(250),tidentificacion-varchar(100),numeroid-bigint,empresa-varchar(250),estadoc-varchar(50),usuarioc-varchar(100),ciudad-varchar(250),direccion-varchar(250),kclmaster-bigint,email-varchar(250)"
        pf = perfil
        CT = New Classcatalogoch.ClassConstructor22(PANEL, "default.aspx", "CLIENTES")
        Select Case CT.reque("fr")
            Case "CLIENTES"
                CLIENTES()
            Case "CLIENTE"
                lg.APP_PARAMETROS("CLIENTE") = "CIUDAD,TIPO IDENTIFICACION"
                CLIENTE()
            Case "CONTACTO"
                CONTACTO()
        End Select
    End Sub
    Private Sub CLIENTES()
        Dim CRI As String
        If pf = 1 Then
            CRI = "USUARIOC='" + CT.USERLOGUIN + "'"
        End If
        CT.FORMULARIO_GR("CLIENTES", "GrCLIENTE", "KCLIENTE-K,NOMBRE-BT,KTELEFONO-BT,CIUDAD-BT", "CLIENTE,NEGOCIOS", "CLIENTES", CRI, AddressOf SEL_CLIENTES,, "NOMBRE")
    End Sub
    Private Sub SEL_CLIENTES()
        CT.redir("?fr=CLIENTE&cl=" + CT.FR_CONTROL("GrCLIENTE"))
    End Sub
    Private Sub CLIENTE()
        If CT.reque("cl") IsNot Nothing Then
            cl = CT.reque("cl")
        End If
        cam = "TnTELEFONO-CELULAR,TxNOMBRE,DrTIPO_IDENTIFICACION,TnNUMERO,DrEMPRESA-PERSONA,DrCIUDAD-CIUDAD_RESIDENCIA,TxDIRECCION,TxCORREO_ELECTRONICO"
        If pf = 2 Then
            cam += ",DrASESOR"

        Else
            cam += ",LbASESOR=" + CT.USERLOGUIN
            fil = " AND USUARION='" + CT.USERLOGUIN + "'"
        End If
        CT.FORMULARIO("CLIENTE", cam, True,, "CLIENTES,NEGOCIOS,NEGOCIO")
        CARGA_DCLIENTE()
        CT.FORMULARIO_GR(Nothing, "GrNEG", "N.KNEGOCIO-K,No_NEGOCIO;KNEGOCIO-BT,FECHA_NEGOCIO;FECHASEG-D,TOTAL_NEGOCIO;VALOR_TOTAL-M,FORMA_PAGO", Nothing, "NEGOCIOS N,MULTIORDEN M", "N.KNEGOCIO=M.KNEGOCIO AND N.kcliente=" + cl + fil, AddressOf SEL_GrNEG)
    End Sub
    Private Sub SEL_GrNEG()
        CT.redir("?fr=NEGOCIO&ne=" + CT.FR_CONTROL("GrNEG"))
    End Sub
    Private Sub CONTACTO()
        If CT.reque("cl") IsNot Nothing Then
            cl = CT.reque("cl")
        End If
        cam = "BtCLIENTE,TnTELEFONO,TxNOMBRE,DrCIUDAD,TxDIRECCION"
        If pf = 2 Then
            cam += ",DrASESOR"
        Else
            cam += ",LbASESOR=" + CT.USERLOGUIN
        End If
        If CT.reque("ct") Is Nothing Then
            CT.FORMULARIO("CONTACTO", cam, True,, "CLIENTES,NEGOCIOS")
            CT.FR_CONTROL("BtCLIENTE", evento:=AddressOf SEL_CL) = dscl.valor_campo("NOMBRE", "KCLIENTE=" + cl) + " - " + dscl.valor_campo("KTELEFONO", "KCLIENTE=" + cl)
            CT.DrPARAMETROS("DrCIUDAD", "CLIENTE", "CIUDAD") = dscl.valor_campo("CIUDAD", "KCLIENTE=" + cl)
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
        If cl Is Nothing Then
            CT.FR_CONTROL("TnTELEFONO",,, AddressOf bus_tel,, True) = Nothing
            CT.FR_CONTROL("TnNUMERO",,, AddressOf BUS_NIDE,, True) = Nothing
            CT.DrPARAMETROS("DrTIPO_IDENTIFICACION", CT.reque("fr"), "TIPO IDENTIFICACION") = Nothing
            CT.DrPARAMETROS("DrCIUDAD", CT.reque("fr"), "CIUDAD") = Nothing
            CT.FR_CONTROL("DrEMPRESA") = "NATURAL,JURUDICA"
            CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf gcliente) = Nothing
        Else
            CT.FR_CONTROL("TnTELEFONO", False) = dscl.valor_campo("KTELEFONO", "KCLIENTE=" + cl)
            CT.FR_CONTROL("TxNOMBRE", ACT, focus:=True) = dscl.valor_campo("NOMBRE", "KCLIENTE=" + cl)
            CT.DrPARAMETROS("DrTIPO_IDENTIFICACION", CT.reque("fr"), "TIPO IDENTIFICACION", ACT) = dscl.valor_campo("TIDENTIFICACION", "KCLIENTE=" + cl)
            CT.DrPARAMETROS("DrCIUDAD", CT.reque("fr"), "CIUDAD", ACT) = dscl.valor_campo("CIUDAD", "KCLIENTE=" + cl)
            CT.FR_CONTROL("TnNUMERO", ACT) = dscl.valor_campo("NUMEROID", "KCLIENTE=" + cl)
            CT.FR_CONTROL("DrEMPRESA", ACT) = dscl.valor_campo("EMPRESA", "KCLIENTE=" + cl)
            CT.FR_CONTROL("TxDIRECCION", ACT) = dscl.valor_campo("DIRECCION", "KCLIENTE=" + cl)
            CT.FR_CONTROL("TxCORREO_ELECTRONICO", ACT) = dscl.valor_campo("EMAIL", "KCLIENTE=" + cl)
            If pf = 2 Then
                CT.FR_CONTROL("DrASESOR", ACT) = dscl.valor_campo("usuarioc", "KCLIENTE=" + cl)
            Else
                CT.FR_CONTROL("LbASESOR") = dscl.valor_campo("usuarioc", "KCLIENTE=" + cl)
            End If
            CT.FR_CONTROL("BtGUARDAR", ACT, evento:=AddressOf gcliente) = "ACTUALIZAR"
            CT.FR_CONTROL("BtCANCELAR", ACT) = Nothing
            If dscl.valor_campo("usuarioc", "KCLIENTE=" + cl) = CT.USERLOGUIN Or pf = 2 Then
                CT.FR_BOTONES("NUEVO_NEGOCIO,NUEVO_CONTACTO,EDITAR_CLIENTE")
            Else
                CT.FR_BOTONES("NUEVO_NEGOCIO,NUEVO_CONTACTO")
            End If
            CT.FR_CONTROL("BtNUEVO_NEGOCIO", evento:=AddressOf NNEGOCIO) = Nothing
            CT.FR_CONTROL("BtNUEVO_CONTACTO", evento:=AddressOf NCONTACTO) = Nothing
            CT.FR_CONTROL("BtEDITAR_CLIENTE", evento:=AddressOf BT_EDIT) = Nothing
        End If
    End Sub
    Private Sub BT_EDIT()
        ACT = True
        CARGA_DCLIENTE()
    End Sub

    Private Sub NCONTACTO()
        CT.redir("?fr=CONTACTO&cl=" + cl)
    End Sub
    Private Sub NNEGOCIO()
        CT.redir("?fr=NEGOCIO&cl=" + cl)
    End Sub
    Private Sub bus_tel()
        Dim tx1 As String = CT.FR_CONTROL("TxNOMBRE")
        If CT.FR_CONTROL("TnTELEFONO").Length = 10 Then
            cl = dscl.valor_campo("KCLIENTE", "ktelefono=" + CT.FR_CONTROL("TnTELEFONO"))
            If cl IsNot Nothing Then
                CT.redir("?fr=NEGOCIO&cl=" + cl)
            Else
                CT.FR_CONTROL("TxNOMBRE", focus:=True) = tx1
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
        Dim TL, NM, TI, NI, EM, US, CI, DI, CE As String
        TL = CT.FR_CONTROL("TnTELEFONO", VALIDAR:=True) : NM = CT.FR_CONTROL("TxNOMBRE", VALIDAR:=True) : TI = CT.FR_CONTROL("DrTIPO_IDENTIFICACION") : NI = CT.FR_CONTROL("TnNUMERO", VALIDAR:=True) : EM = CT.FR_CONTROL("DrEMPRESA")
        CI = CT.FR_CONTROL("DrCIUDAD") : DI = CT.FR_CONTROL("TxDIRECCION") : CE = CT.FR_CONTROL("TxCORREO_ELECTRONICO")
        If CT.FR_CONTROL("DrASESOR") IsNot Nothing Then
            US = CT.FR_CONTROL("DrASESOR")
        Else
            US = CT.USERLOGUIN
        End If
        If CT.validacion_ct = False Then
            If CT.FR_CONTROL("BtGUARDAR") = "GUARDAR" Then
                dscl.insertardb(TL + ",'" + NM + "','" + TI + "'," + NI + ",'" + EM + "','ACTIVO','" + US + "','" + CI + "','" + DI + "',0,'" + CE + "'", True)
                cl = dscl.valor_campo("KCLIENTE", "KTELEFONO=" + TL)
            Else
                dscl.actualizardb("NOMBRE='" + NM + "',tidentificacion='" + TI + "',numeroid=" + NI + ",ciudad='" + CI + "',direccion='" + DI + "',usuarioc='" + US + "',email='" + CE + "'", "kcliente=" + cl, True)
            End If
            CT.redir("?fr=CLIENTE&cl=" + cl)
        End If

    End Sub


End Class
