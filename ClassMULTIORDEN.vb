Imports Classcatalogoch
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class ClassMULTIORDEN
    Private CT As ClassConstructor22
    Private dscl As New carga_dssql("clientes")
    Private dsct As New carga_dssql("COTIZACIONES")
    Private dsmo As New carga_dssql("multiorden")
    Private dsimo As New carga_dssql("itemmo")
    Private lg As New ClassLogin
    Private Shadows fr As Panel
    Dim PnT As New Panel
    Private Shadows cam, ctz, mo, pf, fp, cl, EST, CR, FIL, TL, ORD, mes As String
    Sub New(PANEL As Panel, PERFIL As String)
        fr = PANEL
        fp = "&#finalp"
        lg.APP_PARAMETROS("MULTIORDEN") = "FORMA PAGO"
        pf = PERFIL
        dsmo.campostb = "kmo-key,KCOT-bigint,fechamo-date,tipo_orden-varchar(250),valor_total-bigint,forma_pago-varchar(250),creado_por-varchar(250),cerrado_por-varchar(250),estadomo-varchar(50),factura-varchar(50),observaciones-varchar(500),fc_por-varchar(50)"
        dsimo.campostb = "kimo-key,kmo-bigint,cantidad-bigint,descripcion-varchar(1000),ref-varchar(250),dis-varchar(250),marca-varchar(250),valoru-bigint"
        CT = New ClassConstructor22(PANEL, "default.aspx", "MULTIORDEN")
        ctz = CT.reque("ct") : mo = CT.reque("mo")
        Select Case CT.reque("fr")
            Case "MULTIORDEN"
                EST = dsmo.valor_campo("ESTADOMO", "KMO=" + mo)
                cam = "BtCLIENTE,BtCOTIZACION,LbFECHA,DrFORMA_PAGO,LbVALOR_TOTAL,TmOBS-OBSERVACIONES"
                If lg.perfil = 2 Then
                    cam = "LbCLIENTE,BtCOTIZACION,LbFECHA,DrFORMA_PAGO,LbVALOR_TOTAL,TmOBS-OBSERVACIONES"
                End If
                If lg.perfil > 1 Then
                    cam += ",TxNUMERO_FACTURA"
                Else
                    If EST = "2 FACTURADO" Then
                        cam += ",LbNUMERO_FACTURA"
                    End If
                End If
                If CT.reque("mo") IsNot Nothing Then
                    mo = CT.reque("mo")
                    ctz = dsmo.valor_campo("KCOT", "kmo=" + mo)
                    cl = dsct.valor_campo("kcliente", "KCOT=" + ctz)
                Else
                    mo = dsmo.valor_campo("kmo", "KCOT=" + ctz)
                    cl = dsct.valor_campo("kcliente", "KCOT=" + ctz)
                End If
                CT.FORMULARIO("MULTIORDEN " + mo, cam, True,, lg.MODULOS)
                If mo IsNot Nothing Then
                    CARGA_MO()
                    CT.FORMULARIO_GR(Nothing, "GrITEMS", "KIMO-K,cantidad,descripcion,ref,dis,marca,valoru", Nothing, "itemmo", "kmo=" + mo, btorden:=True)
                    If EST = "1 POR FACTURAR" Then
                        CT.FR_CONTROL("TmOBS") = dsmo.valor_campo("observaciones", "kmo=" + mo)
                        CT.FR_CONTROL("BtGUARDAR", False) = Nothing
                        CT.FR_CONTROL("BtCANCELAR", False) = Nothing
                        If lg.perfil > 1 Then
                            CT.FR_BOTONES("IMPRESION,FACTURADO,EDITAR")
                        Else
                            CT.FR_BOTONES("IMPRESION")
                        End If
                        CT.FR_CONTROL("BtIMPRESION", evento:=AddressOf CLIC_BT) = Nothing
                        CT.FR_CONTROL("BtFACTURADO", evento:=AddressOf CLIC_BT) = Nothing
                        CT.FR_CONTROL("BtEDITAR", evento:=AddressOf CLIC_BT) = Nothing
                    ElseIf EST = "2 FACTURADO" Then
                        If pf = 1 Then
                            CT.FR_CONTROL("LbNUMERO_FACTURA") = dsmo.valor_campo("factura", "kmo=" + mo)
                        Else
                            CT.FR_CONTROL("TxNUMERO_FACTURA") = dsmo.valor_campo("factura", "kmo=" + mo)
                        End If
                        CT.FR_CONTROL("BtGUARDAR", False) = Nothing
                        CT.FR_CONTROL("BtCANCELAR", False) = Nothing
                        If lg.perfil > 1 Then
                            CT.FR_BOTONES("ACTUALIZAR_FACTURA,ANULAR_MULTIORDEN")
                            CT.FR_CONTROL("BtACTUALIZAR_FACTURA", evento:=AddressOf CLIC_BT) = Nothing
                            CT.FR_CONTROL("BtANULAR_MULTIORDEN", evento:=AddressOf CLIC_BT) = Nothing
                        End If
                        CT.FR_BOTONES("IMPRESION")
                        CT.FR_CONTROL("BtIMPRESION", evento:=AddressOf CLIC_BT) = Nothing
                        'End If
                    ElseIf EST = "3 ANULADO" Then
                        CT.FR_CONTROL("BtGUARDAR", False) = Nothing
                        CT.FR_CONTROL("BtCANCELAR", False) = Nothing
                    Else
                        CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GMO) = "AGREGAR ITEMS"
                        CT.FR_BOTONES("ENVIAR_FACTURACION,ELIMINAR_MULTIORDEN")
                        CT.FR_CONTROL("BtENVIAR_FACTURACION", evento:=AddressOf CLIC_BT) = "ENVIAR ORDEN"
                        CT.FR_CONTROL("BtELIMINAR_MULTIORDEN", evento:=AddressOf CLIC_BT) = Nothing
                    End If
                ElseIf ctz IsNot Nothing Then
                    CT.FR_CONTROL("LbFECHA") = Now.ToString("yyyy-MM-dd")
                    CT.FR_CONTROL("DrFORMA_PAGO",, db:=dsmo.dtparametros("MULTIORDEN", "FORMA PAGO")) = "VALOR=" + CT.DrPARAMETROS("MULTIORDEN", "FORMA PAGO")
                    CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GMO) = "AGREGAR ITEMS"
                Else

                End If
                BtCLIENTE()

            Case "MULTIORDENES"
                fr.Controls.Clear()

                If CT.reque("US") IsNot Nothing Then
                    CT.SESION_GH("USMO") = CT.reque("US")
                ElseIf pf = 1 Then
                    CT.SESION_GH("USMO") = CT.USERLOGUIN
                End If
                CT.FILTROS_GRID("estadomo,MES,YEAR")
                CT.DrMES("DrMES", AddressOf SEL_DR) : CT.DrYEAR("DrYEAR", 2023, AddressOf SEL_DR)
                If pf = 1 Or CT.reque("US") IsNot Nothing Then
                    cam = "kmo-K,No;kmo-BT,CLIENTE;NOMBRE,FECHA;FECHAMO-D,FORMA_PAGO,VALOR_TOTAL-M,ESTADO;ESTADOMO,FACTURA"
                    CR = "m.creado_por='" + CT.SESION_GH("USMO") + "' and "
                    TL = "MULTIORDENES " + CT.SESION_GH("USMO")
                    If CT.SESION_GH("mes") Is Nothing Then
                        CT.SESION_GH("mes") = CT.FR_CONTROL("DrMES")
                    Else

                    End If
                    CT.FR_CONTROL("Drestadomo", evento:=AddressOf SEL_DR) = "0 CREACION,1 POR FACTURAR,2 FACTURADO"
                    'CT.FR_CONTROL("Drestadomo",, dsmo.Carga_tablas("creado_por='" + CT.USERLOGUIN + "' and year(fechamo)=" + CT.FR_CONTROL("DrYEAR") + " and month(fechamo)=" + CT.SESION_GH("mes"), "ESTADOMO", "ESTADOMO", True), AddressOf SEL_DR) = "ESTADOMO-ESTADOMO"
                    'CT.FR_CONTROL("DrESTADOMO",, dsmo.Carga_tablas("estadomo <> '0 CREACION' and MONTH(fechamo)=" + CT.FR_CONTROL("DrMES") + " and YEAR(fechamo)=" + CT.FR_CONTROL("DrYEAR"), "estadomo", "estadomo", True), AddressOf SEL_DR) = "estadomo-estadomo"
                    FIL = "ESTADOMO='" + CT.FR_CONTROL("DrESTADOMO") + "'"
                    ORD = "KMO DESC"
                Else
                    CR = Nothing
                    cam = "kmo-K,No;kmo-BT,CLIENTE;NOMBRE,FECHA;FECHAMO-D,FORMA_PAGO,VALOR_TOTAL-M,ESTADO;ESTADOMO,FACTURA,creado_por,facturado_por;fc_por"
                    If lg.perfil = 2 Then
                        'CR = " and estadomo <> '0 CREACION'"
                        'CT.FILTROS_GRID("estadomo,orden,MES,YEAR")
                        CT.DrMES("DrMES", AddressOf SEL_DR) : CT.DrYEAR("DrYEAR", 2020, AddressOf SEL_DR)
                        CT.FR_CONTROL("Drorden", evento:=AddressOf SEL_ORD) = "No.,NOMBRE(AZ),NOMBRE(ZA),FECHAMO(AZ),FECHAMO(ZA),FACTURA(AZ),FACTURA(ZA)"
                        'CT.FR_CONTROL("Drestadomo",, dsmo.Carga_tablas("estadomo <> '0 CREACION'", "estadomo", "estadomo", True), AddressOf SEL_DR) = "estadomo-estadomo"
                        CT.FR_CONTROL("Drestadomo", evento:=AddressOf SEL_DR) = "1 POR FACTURAR,2 FACTURADO,3 ANULADO"
                        FIL = "estadomo='" + CT.FR_CONTROL("DrESTADOMO") + "' and month(fechamo)=" + CT.FR_CONTROL("DrMES") + " and year(fechamo)=" + CT.FR_CONTROL("DrYEAR")
                    ElseIf lg.perfil = 3 Then
                        cam = "creado_por-K,creado_por-BT,estadomo,total-SUM(valor_total)"
                        CT.FORMULARIO_GR(TL, "GrMULTI", cam, lg.MODULOS, "multiorden", "estadomo='2 FACTURADO' and year(fechamo)=" + Now.Year.ToString + " and month(fechamo)=" + Now.Month.ToString, AddressOf sel_grmulti)
                        Exit Sub
                    End If
                    'ORD = "estadomo"
                    ORD = CT.FR_CONTROL("Drorden")
                End If
                CT.FORMULARIO_GR(TL, "GrMULTI", cam, lg.MODULOS, ,, AddressOf sel_grmulti, btorden:=True)
                fr.Controls.Add(PnT)
                CARGA_GrMUTI()
            Case "ITEMSMO"
                CARGA_IMO()
        End Select
    End Sub
    Private Sub SEL_ORD()
        If CT.FR_CONTROL("Drorden").Contains("(AZ)") Then
            ORD = CT.FR_CONTROL("Drorden").Replace("(AZ)", "")
        ElseIf CT.FR_CONTROL("Drorden").Contains("(ZA)") Then
            ORD = CT.FR_CONTROL("Drorden").Replace("(ZA)", "") + " DESC"
        End If

        CARGA_GrMUTI()
    End Sub
    Private Sub SEL_DR(sender As Object, e As EventArgs)
        Dim dr As DropDownList = sender
        FIL = Nothing
        CT.SESION_GH("mes") = CT.FR_CONTROL("DrMES")
        Select Case lg.perfil
            Case "1", "2"
                FIL = "ESTADOMO='" + CT.FR_CONTROL("Drestadomo") + "'"
                If CT.FR_CONTROL("DrMES") = Now.Month.ToString Then
                    FIL += " and MONTH(fechamo)=" + CT.FR_CONTROL("DrMES") + " and YEAR(fechamo)=" + CT.FR_CONTROL("DrYEAR")
                ElseIf lg.perfil = 1 Then
                    FIL = "ESTADOMO='" + CT.FR_CONTROL("Drestadomo") + "' and MONTH(fechamo)=" + CT.FR_CONTROL("DrMES") + " and YEAR(fechamo)=" + CT.FR_CONTROL("DrYEAR")
                ElseIf lg.perfil = 2 Then
                    FIL += " and MONTH(fechamo)=" + CT.FR_CONTROL("DrMES") + " and YEAR(fechamo)=" + CT.FR_CONTROL("DrYEAR")
                End If
            Case "3"
                FIL = "ESTADOMO='" + CT.FR_CONTROL("Drestadomo") + "' and MONTH(fechamo)=" + CT.FR_CONTROL("DrMES") + " and YEAR(fechamo)=" + CT.FR_CONTROL("DrYEAR")
        End Select
        CARGA_GrMUTI()
    End Sub
    Private Sub CARGA_GrMUTI()
        Dim DSNM As New carga_dssql("multiorden m,COTIZACIONES n,clientes c")
        If ORD = "No." Then
            ORD = "KMO DESC"
        End If
        CT.FR_CONTROL("GrMULTI", db:=DSNM.Carga_tablas("m.KCOT=n.KCOT and n.kcliente=c.kcliente and " + CR + FIL, ORD)) = Nothing
        PnT.Controls.Clear()
        Dim LbT As New Label
        ORD = Nothing
        LbT.Font.Bold = True
        LbT.Font.Size = 30
        Dim ST As String = "TOTAL " + CT.FR_CONTROL("Drestadomo").Remove(0, 2)
        ST += " " + MonthName(CT.FR_CONTROL("DrMES")).ToUpper + " " + CT.FR_CONTROL("DrYEAR")
        LbT.Text = ST + " $ " + FormatNumber(DSNM.valor_campo_OTROS("SUM(valor_total)", "m.KCOT=n.KCOT and n.kcliente=c.kcliente and " + CR + FIL, ORD), 0)
        PnT.Controls.Add(LbT)
        FIL = Nothing
    End Sub
    Private Sub BtCLIENTE()
        If cl IsNot Nothing Then
            Dim xCL As String
            If lg.perfil = 2 Then
                xCL = dscl.valor_campo("NOMBRE", "KCLIENTE=" + cl) + "<BR>"
                xCL += dscl.valor_campo("TIDENTIFICACION", "KCLIENTE=" + cl) + ": " + dscl.valor_campo("NUMEROID", "KCLIENTE=" + cl) + "<BR>"
                xCL += "TEL: " + dscl.valor_campo("KTELEFONO", "KCLIENTE=" + cl) + "<BR>"
                xCL += dscl.valor_campo("DIRECCION", "KCLIENTE=" + cl) + " - " + dscl.valor_campo("CIUDAD", "KCLIENTE=" + cl) + "<BR>"
                xCL += dscl.valor_campo("EMAIL", "KCLIENTE=" + cl)
                CT.FR_CONTROL("LbCLIENTE") = xCL
            Else
                xCL = dscl.valor_campo("NOMBRE", "KCLIENTE=" + cl) + " - " + dscl.valor_campo("KTELEFONO", "KCLIENTE=" + cl)
                CT.FR_CONTROL("BtCLIENTE", evento:=AddressOf SEL_CL) = xCL
            End If


            CT.FR_CONTROL("BtCOTIZACION", evento:=AddressOf sel_ne) = "COTIZACION No. " + ctz + " REF. (" + dsct.valor_campo("REFERENCIA", "KCOT=" + ctz) + ")"
        End If
    End Sub
    Private Sub SEL_CL()
        CT.redir("?fr=CLIENTE&cl=" + cl)
    End Sub
    Private Sub sel_ne()
        CT.redir("?fr=COTIZACION&ct=" + ctz)
    End Sub
    Private Sub sel_grmulti()
        If lg.perfil = 3 Then
            CT.redir("?fr=MULTIORDENES&US=" + CT.FR_CONTROL("GrMULTI") + fp)
        Else
            CT.redir("?fr=MULTIORDEN&mo=" + CT.FR_CONTROL("GrMULTI") + fp)
        End If

    End Sub
    Private Sub CARGA_IMO()
        mo = CT.reque("mo")
        cam = "TnCANTIDAD,TxDESCRIPCION,TxREFERENCIA,TxDISEÑO,TxMARCA,TnVALOR_UNITARIO"
        CT.FORMULARIO("ITEMS MULTIORDEN No. " + mo, cam, True,, "COTIZACIONES,CLIENTES")
        CT.FR_CONTROL("TnCANTIDAD", focus:=True) = "1"
        CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GITEMS) = "GUARDAR ITEM"
        CT.FORMULARIO_GR(Nothing, "GrITEMS", "KIMO-K,cantidad,descripcion,ref,dis,marca,valoru,-CH", Nothing, "itemmo", "kmo=" + mo)
        CT.FR_BOTONES("ELIMINAR_ITEMS,VOLVER_MULTIORDEN") : CT.FR_CONTROL("BtELIMINAR_ITEMS", evento:=AddressOf CLIC_BT) = Nothing
        CT.FR_CONTROL("BtVOLVER_MULTIORDEN", evento:=AddressOf CLIC_BT) = Nothing

    End Sub
    Private Sub GITEMS()
        Dim CN, DE, RE, DI, MA, VL As String
        CN = CT.FR_CONTROL("TnCANTIDAD", VALIDAR:=True) : DE = CT.FR_CONTROL("TxDESCRIPCION", VALIDAR:=True) : RE = CT.FR_CONTROL("TxREFERENCIA")
        DI = CT.FR_CONTROL("TxDISEÑO", VALIDAR:=True) : MA = CT.FR_CONTROL("TxMARCA", VALIDAR:=True) : VL = CT.FR_CONTROL("TnVALOR_UNITARIO", VALIDAR:=True)
        If CT.validacion_ct = False Then
            dsimo.insertardb(mo + "," + CN + ",'" + DE + "','" + RE + "','" + DI + "','" + MA + "'," + VL)
            CT.redir("?fr=MULTIORDEN&mo=" + mo)
        End If
    End Sub
    Private Sub CARGA_MO()
        CT.FR_CONTROL("LbFECHA") = dsmo.valor_campo("FECHAMO", "KMO=" + mo)
        CT.FR_CONTROL("DrTIPO_ORDEN", False) = dsmo.valor_campo("TIPO_ORDEN", "KMO=" + mo)

        If dsmo.valor_campo("estadomo", "KMO=" + mo).Contains("0 ") Then
            CT.FR_CONTROL("DrFORMA_PAGO", True, db:=dsmo.dtparametros("MULTIORDEN", "FORMA PAGO")) = "VALOR=" + dsmo.valor_campo("FORMA_PAGO", "KMO=" + mo)
            CT.FR_CONTROL("DrFORMA_PAGO") = dsmo.valor_campo("FORMA_PAGO", "KMO=" + mo)
        Else
            CT.FR_CONTROL("DrFORMA_PAGO", False) = dsmo.valor_campo("FORMA_PAGO", "KMO=" + mo)
        End If
        dsmo.actualizardb("valor_total=" + dsimo.valor_campo_OTROS("sum(cantidad * valoru)", "KMO=" + mo), "kmo=" + mo)
        CT.FR_CONTROL("LbVALOR_TOTAL") = FormatNumber(dsmo.valor_campo("VALOR_TOTAL", "KMO=" + mo))
        CT.FR_CONTROL("TmOBS") = dsmo.valor_campo("OBSERVACIONES", "kmo=" + mo)
    End Sub
    Private Sub CLIC_BT(SENDER As Object, E As EventArgs)
        Dim BT As Button = SENDER
        Select Case BT.Text
            Case "ANULAR MULTIORDEN"
                dsmo.actualizardb("ESTADOMO='3 ANULADO',fc_por='" + CT.USERLOGUIN + "'", "KMO=" + mo)
                CT.redir("?fr=MULTIORDENES")
            Case "ELIMINAR MULTIORDEN"
                dsmo.Eliminardb("KMO=" + mo)
                CT.redir("?fr=MULTIORDENES")
            Case "ACTUALIZAR FACTURA"
                EST = CT.FR_CONTROL("TxNUMERO_FACTURA")
                dsmo.actualizardb("factura='" + EST + "',fc_por='" + CT.USERLOGUIN + "'", "KMO=" + mo)
                CT.redir("?fr=MULTIORDENES")
            Case "ELIMINAR ITEMS"
                Dim imo, vimo As String
                imo = CT.FR_CONTROL("ChGrITEMS") : vimo = dsimo.valor_campo_OTROS("sum(valor * cantidad)", "kimo=" + imo)
                dsimo.Eliminardb("kimo=" + imo)
            Case "ENVIAR ORDEN"
                dsmo.actualizardb("fechamo='" + CT.HOY_FR + "',estadomo='1 POR FACTURAR', observaciones='" + CT.FR_CONTROL("TmOBS") + "',forma_pago='" + CT.FR_CONTROL("DrFORMA_PAGO") + "'", "KMO=" + mo)
            Case "FACTURADO"
                EST = CT.FR_CONTROL("TxNUMERO_FACTURA")
                If EST.Length <> 0 Then
                    dsmo.actualizardb("estadomo='2 FACTURADO',factura='" + EST + "',fc_por='" + CT.USERLOGUIN + "'", "KMO=" + mo)
                Else
                    CT.alerta("SE DEBE INGRESAR EL NUMERO DE FACTURA")
                    Exit Sub
                End If
            Case "EDITAR"
                dsmo.actualizardb("estadomo='0 CREACION'", "KMO=" + mo)
            Case "IMPRESION"
                impresion()
        End Select
        CT.redir("?fr=MULTIORDEN&mo=" + mo)
    End Sub
    Private Sub GMO()
        Dim FE, TMO, OB, FP, CP, CR As String
        FE = CT.FR_CONTROL("LbFECHA") : TMO = "" : FP = CT.FR_CONTROL("DrFORMA_PAGO") : CP = CT.USERLOGUIN : CR = CT.USERLOGUIN
        If mo Is Nothing Then
            dsmo.insertardb(ctz + ",'" + FE + "','" + TMO + "',0,'" + FP + "','" + CP + "','','0 CREACION','','',''")
            mo = dsmo.valor_campo("kmo", "fechamo='" + FE + "' and KCOT=" + ctz + " and creado_por='" + CP + "'")
        Else
            OB = CT.FR_CONTROL("TmOBS") : FP = CT.FR_CONTROL("DrFORMA_PAGO")
            dsmo.actualizardb("FORMA_PAGO='" + FP + "',OBSERVACIONES='" + OB + "'", "KMO=" + mo)
        End If
        If mo IsNot Nothing Then
            CT.redir("?fr=ITEMSMO&mo=" + mo)
        End If
    End Sub
    Private Sub impresion()
        Dim FE, TMO, OB, FP, CP, CR, ES, FT, FTP, ENC, VT As String
        FE = CT.FR_CONTROL("LbFECHA") : TMO = "" : FP = CT.FR_CONTROL("DrFORMA_PAGO") : CP = dsmo.valor_campo("CREADO_POR", "KMO=" + mo) : CR = dsmo.valor_campo("CERRADO_POR", "KMO=" + mo)
        ES = dsmo.valor_campo("ESTADOMO", "KMO=" + mo) : FT = dsmo.valor_campo("FACTURA", "KMO=" + mo) : OB = dsmo.valor_campo("OBSERVACIONES", "KMO=" + mo) : FTP = dsmo.valor_campo("FC_POR", "KMO=" + mo)
        VT = dsmo.valor_campo("VALOR_TOTAL", "KMO=" + mo)
        Dim imp As New ClassImpresion

        ENC = "FECHA: " + CDate(FE).ToShortDateString + Chr(10) + "FORMA DE PAGO: " + FP + Chr(10) + "CREADA POR: " + CP + Chr(10)
        If ES = "2 FACTURADO" Then
            ENC += "FACTURA No. " + FT + Chr(10) + "FACTURADO POR: " + FTP
        Else
            ENC += ES
        End If
        imp.aEncabezado = ENC
        imp.aLogo = "LogoOCCILLANTAS.jpeg"
        imp.aTitulo = "MULTIORDEN No." + mo
        imp.bTABLA_COSTOS = dsimo.Carga_tablas("kmo=" + mo,, "cantidad,descripcion,ref,dis,marca,valoru")

        Dim CN, TI, ID, DI, TL, EM As String
        CN = dscl.valor_campo("NOMBRE", "KCLIENTE=" + cl) : TI = dscl.valor_campo("tidentificacion", "KCLIENTE=" + cl) : ID = dscl.valor_campo("NUMEROID", "KCLIENTE=" + cl)
        DI = dscl.valor_campo("DIRECCION", "KCLIENTE=" + cl) : TL = dscl.valor_campo("KTELEFONO", "KCLIENTE=" + cl) : EM = dscl.valor_campo("email", "KCLIENTE=" + cl)
        imp.bCLIENTE = "DATOS DEL CLIENTE:" + Chr(10) + "Nombre: " + CN + Chr(10) + TI + ": " + ID + Chr(10) + "Dir: " + DI + Chr(10) + "Tel:" + TL + Chr(10) + "Email:" + EM + Chr(10)
        imp.CDESCRIPCION = "VALOR TOTAL MULTIORDEN: $" + FormatNumber(VT, 0) + Chr(10) + Chr(10) + "DESCRIPCION:" + OB + Chr(10)
        imp.bFIRMA = "IMPRESO EL " + Now.ToLongDateString.ToUpper + " POR " + CT.USERLOGUIN

        imp.cGENERAR_PDF()
        CT.redireccion("~/documento.pdf")
    End Sub
#Region "CARTERA"

#End Region

End Class
