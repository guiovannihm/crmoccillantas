Imports Classcatalogoch

Public Class ClassMULTIORDEN
    Private CT As ClassConstructor22
    Private dscl As New carga_dssql("clientes")
    Private dsct As New carga_dssql("COTIZACIONES")
    Private dsmo As New carga_dssql("multiorden")
    Private dsimo As New carga_dssql("itemmo")
    Private lg As New ClassLogin

    Private Shared cam, ctz, mo, pf, fp, cl, EST, CR, FIL, TL, ORD As String
    Sub New(PANEL As Panel, PERFIL As String)
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
                    If EST = "1 POR FACTURAR" Then
                        cam += ",TxNUMERO_FACTURA"
                    ElseIf EST = "2 FACTURADO" Then
                        cam += ",LbNUMERO_FACTURA"
                    End If
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
                CT.FORMULARIO("MULTIORDEN", cam, True,, lg.MODULOS)
                If mo IsNot Nothing Then
                    CARGA_MO()
                    CT.FORMULARIO_GR(Nothing, "GrITEMS", "KIMO-K,cantidad,descripcion,ref,dis,marca,valoru", Nothing, "itemmo", "kmo=" + mo)
                    If EST = "1 POR FACTURAR" Then
                        CT.FR_CONTROL("TmOBS") = dsmo.valor_campo("observaciones", "kmo=" + mo)
                        CT.FR_CONTROL("BtGUARDAR", False) = Nothing
                        CT.FR_CONTROL("BtCANCELAR", False) = Nothing
                        If lg.perfil > 1 Then
                            CT.FR_BOTONES("IMPRESION,FACTURADO")
                        End If
                        CT.FR_CONTROL("BtIMPRESION", evento:=AddressOf CLIC_BT) = Nothing
                        CT.FR_CONTROL("BtFACTURADO", evento:=AddressOf CLIC_BT) = Nothing
                    ElseIf EST = "2 FACTURADO" Then
                        CT.FR_CONTROL("LbNUMERO_FACTURA") = dsmo.valor_campo("factura", "kmo=" + mo)
                        CT.FR_CONTROL("BtGUARDAR", False) = Nothing
                        CT.FR_CONTROL("BtCANCELAR", False) = Nothing
                        If lg.perfil > 1 Then
                            CT.FR_BOTONES("IMPRESION")
                            CT.FR_CONTROL("BtIMPRESION", evento:=AddressOf CLIC_BT) = Nothing
                        End If
                    Else
                        CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GMO) = "AGREGAR ITEMS"
                        CT.FR_BOTONES("ENVIAR_FACTURACION")
                        CT.FR_CONTROL("BtENVIAR_FACTURACION", evento:=AddressOf CLIC_BT) = "ENVIAR ORDEN"

                    End If
                ElseIf ctz IsNot Nothing Then
                    CT.FR_CONTROL("LbFECHA") = Now.ToString("yyyy-MM-dd")
                    CT.FR_CONTROL("DrFORMA_PAGO", True, db:=dsmo.dtparametros("MULTIORDEN", "FORMA PAGO")) = "VALOR=" + CT.DrPARAMETROS("MULTIORDEN", "FORMA PAGO")
                    CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GMO) = "AGREGAR ITEMS"
                Else

                End If
                BtCLIENTE()

            Case "MULTIORDENES"
                If pf = 1 Then
                    cam = "kmo-K,No;kmo,CLIENTE;NOMBRE-BT,FECHA;FECHAMO-BT,FORMA_PAGO-BT,VALOR_TOTAL-BT,ESTADO;ESTADOMO-BT"
                    CR = " and m.creado_por='" + CT.USERLOGUIN + "' and year(fechamo)=" + Now.Year.ToString + " and month(fechamo)=" + Now.Month.ToString
                    TL = "MULTIORDENES"
                    CT.FILTROS_GRID("estadomo")
                    CT.FR_CONTROL("DrESTADOMO",, dsmo.Carga_tablas("creado_por='" + CT.USERLOGUIN + "' and year(fechamo)=" + Now.Year.ToString + " and month(fechamo)=" + Now.Month.ToString, "ESTADOMO", "ESTADOMO", True), AddressOf SEL_DR, post:=True) = "ESTADOMO-ESTADOMO"
                    FIL = " AND ESTADOMO='" + CT.FR_CONTROL("DrESTADOMO") + "'"
                Else
                    CR = Nothing
                    cam = "kmo-K,No;kmo,CLIENTE;NOMBRE-BT,FECHA;FECHAMO-BT,FORMA_PAGO-BT,VALOR_TOTAL-BT,ESTADO;ESTADOMO-BT,FACTURA-BT,creado_por-BT,facturado_por;fc_por-BT"
                    If lg.perfil = 2 Then
                        'CR = " and estadomo <> '0 CREACION'"
                        CT.FILTROS_GRID("estadomo")
                        CT.FR_CONTROL("Drestadomo",, dsmo.Carga_tablas("estadomo <> '0 CREACION'", "estadomo", "estadomo", True), AddressOf SEL_DR) = "estadomo-estadomo"
                        FIL = " and estadomo='" + CT.FR_CONTROL("DrESTADOMO") + "'"
                    ElseIf lg.perfil = 3 Then
                        CT.FILTROS_GRID("creado_por,estadomo")
                        CT.FR_CONTROL("Drcreado_por",, dsmo.Carga_tablas(, "creado_por", "creado_por", True), AddressOf SEL_DR) = "creado_por-creado_por"
                        CT.FR_CONTROL("Drestadomo",, dsmo.Carga_tablas(, "estadomo", "estadomo", True), AddressOf SEL_DR) = "estadomo-estadomo"
                        FIL = " and estadomo='" + CT.FR_CONTROL("DrESTADOMO") + "' and creado_por='" + CT.FR_CONTROL("DrCREADO_POR") + "'"
                    End If
                    ORD = "estadomo"

                End If
                CT.FORMULARIO_GR(TL, "GrMULTI", cam, lg.MODULOS, , , AddressOf sel_grmulti)
                CARGA_GrMUTI()
            Case "ITEMSMO"
                CARGA_IMO()
        End Select
    End Sub
    Private Sub SEL_DR(sender As Object, e As EventArgs)
        Dim dr As DropDownList = sender
        FIL = Nothing
        Select Case dr.ID
            Case "DrESTADOMO"
                Select Case lg.perfil
                    Case "1"
                        FIL = " AND ESTADOMO='" + CT.FR_CONTROL("DrESTADOMO") + "'"
                    Case "2"
                        FIL = " and estadomo='" + CT.FR_CONTROL("DrESTADOMO") + "'"
                    Case "3"
                        FIL = " and estadomo='" + CT.FR_CONTROL("DrESTADOMO") + "' and creado_por='" + CT.FR_CONTROL("DrCREADO_POR") + "'"
                End Select
            Case "DrCREADO_POR"
                FIL = " and estadomo='" + CT.FR_CONTROL("DrESTADOMO") + "' and creado_por='" + CT.FR_CONTROL("DrCREADO_POR") + "'"
            Case "DrAÑO"
                FIL = " and year(fechamo)=" + CT.FR_CONTROL("DrAÑO") + " and MONTH(fechamo)=" + CT.FR_CONTROL("DrMES")
            Case "Drmonth(fechamo)#"
                FIL += " and MONTH(fechamo)=" + CT.FR_CONTROL("Drmonth(fechamo)#")
        End Select
        CARGA_GrMUTI()
    End Sub

    Private Sub CARGA_GrMUTI()
        Dim DSNM As New carga_dssql("multiorden m,COTIZACIONES n,clientes c")
        CT.FR_CONTROL("GrMULTI", db:=DSNM.Carga_tablas("m.KCOT=n.KCOT and n.kcliente=c.kcliente" + CR + FIL, ORD)) = Nothing
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
        CT.redir("?fr=MULTIORDEN&mo=" + CT.FR_CONTROL("GrMULTI") + fp)
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
        If dsmo.valor_campo("FORMA_PAGO", "KMO=" + mo).Contains("0 ") Then
            CT.FR_CONTROL("DrFORMA_PAGO", True, db:=dsmo.dtparametros("MULTIORDEN", "FORMA PAGO")) = "VALOR=" + dsmo.valor_campo("FORMA_PAGO", "KMO=" + mo)
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
            Case "ELIMINAR ITEMS"
                Dim imo, vimo As String
                imo = CT.FR_CONTROL("ChGrITEMS") : vimo = dsimo.valor_campo_OTROS("sum(valor * cantidad)", "kimo=" + imo)
                dsimo.Eliminardb("kimo=" + imo)
            Case "ENVIAR ORDEN"
                dsmo.actualizardb("estadomo='1 POR FACTURAR', observaciones='" + CT.FR_CONTROL("TmOBS") + "'", "KMO=" + mo)
            Case "FACTURADO"
                EST = CT.FR_CONTROL("TxNUMERO_FACTURA")
                If EST.Length <> 0 Then
                    dsmo.actualizardb("estadomo='2 FACTURADO',factura='" + EST + "',fc_por='" + CT.USERLOGUIN + "'", "KMO=" + mo)
                Else
                    CT.alerta("SE DEBE INGRESAR EL NUMERO DE FACTURA")
                    Exit Sub
                End If
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
        Dim imp As New ClassImpresion
        imp.bCLIENTE = "Nombre:" + Chr(10) + "Nit" + Chr(10) + "Dir:" + Chr(10) + "Tel:" + Chr(10) + "Email:" + Chr(10)
        imp.aLogo = "LogoOCCILLANTAS.jpeg"
        imp.aTitulo = "MULTIORDEN No."

        imp.bTABLA_COSTOS = dsimo.Carga_tablas("kmo=" + mo,, "cantidad,descripcion,ref,dis,marca,valoru")
        imp.CDESCRIPCION = "DESCRIPCION:" + Chr(10)
        imp.cGENERAR_PDF()

    End Sub
End Class
