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
    Private dsict As New carga_dssql("itemct")
    Private dsfn As New carga_dssql("financiacion")
    Private dsvfn As New carga_dssql("v_cartera")
    Private dsinv As New carga_dssql("v_inv")
    Private lg As New ClassLogin
    Private INV As ClassINVENTARIOS

    Private Shadows fr As Panel
    Dim PnT As New Panel
    Private Shadows cam, ctz, mo, pf, fp, cl, EST, CR, FIL, TL, ORD, mes As String
    Sub New(PANEL As Panel, PERFIL As String)
        fr = PANEL
        fp = "&#finalp"
        'lg.APP_PARAMETROS("MULTIORDEN") = "FORMA PAGO"
        pf = PERFIL
        dsmo.campostb = "kmo-key,KCOT-bigint,fechamo-date,tipo_orden-varchar(250),valor_total-bigint,forma_pago-varchar(250),creado_por-varchar(250),cerrado_por-varchar(250),estadomo-varchar(50),factura-varchar(50),observaciones-varchar(500),fc_por-varchar(50)"
        dsimo.campostb = "kimo-key,kmo-bigint,cantidad-bigint,descripcion-varchar(1000),ref-varchar(250),dis-varchar(250),marca-varchar(250),valoru-bigint,bodega-varchar(250)"
        dsfn.campostb = "kfn-key,kmo-bigint,forma_pago-varchar(250),fecha_cuota-date,numero-bigint,valor_cuota-money,estado-varchar(50),nota-varchar(250),confirmo-varchar(50)"
        dsvfn.vistatb("v_cartera", "financiacion f", "v_multiorden m", "f.*,m.nombre as cliente,m.asesor,m.estado_multiorden as estadomo", "f.kmo=m.no_multiorden")

        CT = New ClassConstructor22(PANEL, "default.aspx", "MULTIORDEN")
        INV = New ClassINVENTARIOS(fr)
        ctz = CT.reque("ct") : mo = CT.reque("mo")
        Select Case CT.reque("fr")
            Case "MULTIORDEN"
                carga_multiorden()
            Case "MULTIORDENES"
                carga_multiordenes()
            Case "ITEMSMO"
                CARGA_IMO()
            Case "FINANCIACION"
                CARGA_FINANCIACION()
            Case "VALINV"
                CARGA_INVENTARIO
            Case "CARTERA"
                Dim CAM, CRIT As String
                CAM = "KFN-K,CLIENTE-BT,FORMA_PAGO-BT,FECHA_CUOTA-BT,NUMERO-BT,VALOR_CUOTA-BT,NOTA-BT"
                Select Case pf
                    Case "1", "3"
                        CAM += ",-CH"
                End Select
                CRIT = "FECHA_CUOTA <='" + Now.ToString("yyyy-MM-dd") + "' AND ESTADO='PENDIENTE' and estadomo<>'3 ANULADO'"
                Select Case pf
                    Case 2, 3
                        CAM += ",ASESOR-BT"
                    Case 1
                        CRIT += " AND ASESOR='" + CT.USERLOGUIN + "'"
                End Select
                CT.FORMULARIO_GR("CARTERA PENDIENTE", "GrFN", CAM, lg.MODULOS, "V_CARTERA", CRIT, AddressOf SEL_GrCP)
                Dim GrFN As GridView = fr.FindControl("GrFN")
                If GrFN.Rows.Count > 0 Then
                    Select Case pf
                        Case "1", "3"
                            CT.FR_BOTONES("MULTIORDEN,CONFIRMAR")
                            CT.FR_CONTROL("BtCONFIRMAR",,, AddressOf CLIC_BT) = "CONFIRMAR PAGO"
                    End Select
                End If

        End Select
    End Sub
#Region "INVENTARIO"

    Private Sub CARGA_INVENTARIO()
        Dim inv As New ClassINVENTARIOS(fr)
        inv.consulta_inventario(, AddressOf SEL_GRINV)
        CT.FR_BOTONES("VOLVER_MULTIORDEN")
        CT.FR_CONTROL("BtVOLVER_MULTIORDEN", evento:=AddressOf CLIC_BT) = Nothing
    End Sub
    Private Sub SEL_GRINV()
        'CT.FR_CONTROL("TxREFERENCIAS") =
    End Sub


#End Region


    Private Sub carga_multiorden()
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
                    CT.FR_BOTONES("IMPRESION,FACTURADO,EDITAR,FINANCIACION")
                Else
                    CT.FR_BOTONES("IMPRESION,FINANCIACION")
                End If
                CT.FR_CONTROL("BtIMPRESION", evento:=AddressOf CLIC_BT) = Nothing
                CT.FR_CONTROL("BtFACTURADO", evento:=AddressOf CLIC_BT) = Nothing
                CT.FR_CONTROL("BtEDITAR", evento:=AddressOf CLIC_BT) = Nothing
                CT.FR_CONTROL("BtFINANCIACION", evento:=AddressOf CLIC_BT) = Nothing
            ElseIf EST = "2 FACTURADO" Then
                If pf = 1 Then
                    CT.FR_CONTROL("LbNUMERO_FACTURA") = dsmo.valor_campo("factura", "kmo=" + mo)
                Else
                    CT.FR_CONTROL("TxNUMERO_FACTURA") = dsmo.valor_campo("factura", "kmo=" + mo)
                End If
                CT.FR_CONTROL("BtGUARDAR", False) = Nothing
                CT.FR_CONTROL("BtCANCELAR", False) = Nothing
                If lg.perfil > 1 Then
                    CT.FR_BOTONES("ACTUALIZAR_FACTURA,ANULAR_MULTIORDEN,FINANCIACION")
                    CT.FR_CONTROL("BtACTUALIZAR_FACTURA", evento:=AddressOf CLIC_BT) = Nothing
                    CT.FR_CONTROL("BtANULAR_MULTIORDEN", evento:=AddressOf CLIC_BT) = Nothing

                End If
                CT.FR_BOTONES("IMPRESION,FINANCIACION")
                CT.FR_CONTROL("BtFINANCIACION", evento:=AddressOf CLIC_BT) = Nothing
                CT.FR_CONTROL("BtIMPRESION", evento:=AddressOf CLIC_BT) = Nothing
                'End If
            ElseIf EST = "3 ANULADO" Then
                CT.FR_CONTROL("BtGUARDAR", False) = Nothing
                CT.FR_CONTROL("BtCANCELAR", False) = Nothing
            Else
                CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GMO) = "AGREGAR ITEMS"
                CT.FR_CONTROL("DrFORMA_PAGO",, db:=dsmo.dtparametros("MULTIORDEN", "FORMA PAGO")) = "VALOR=" + CT.DrPARAMETROS("MULTIORDEN", "FORMA PAGO")
                If dsfn.Carga_tablas("KMO=" + mo).Rows.Count > 0 Then
                    CT.FR_BOTONES("ENVIAR_FACTURACION,ELIMINAR_MULTIORDEN,FINANCIACION")
                    CT.FR_CONTROL("BtENVIAR_FACTURACION", evento:=AddressOf CLIC_BT) = "ENVIAR ORDEN"
                Else
                    CT.FR_BOTONES("FINANCIACION,ELIMINAR_MULTIORDEN")
                End If
                CT.FR_CONTROL("BtFINANCIACION", evento:=AddressOf CLIC_BT) = Nothing
                CT.FR_CONTROL("BtELIMINAR_MULTIORDEN", evento:=AddressOf CLIC_BT) = Nothing
            End If
        ElseIf ctz IsNot Nothing Then
            CT.FR_CONTROL("LbFECHA") = Now.ToString("yyyy-MM-dd")
            CT.FR_CONTROL("DrFORMA_PAGO",, db:=dsmo.dtparametros("MULTIORDEN", "FORMA PAGO")) = "VALOR=" + CT.DrPARAMETROS("MULTIORDEN", "FORMA PAGO")
            If val_cliente("numeroid") = "0" Or val_cliente("direccion").Length = 0 Or val_cliente("ciudad").Length = 0 Or val_cliente("email").Length = 0 Then
                CT.alerta("EL CLIEENTE NO TIENE LA INFORMACION DE FACTUACION COMPLETA 1. NOMBRES  2. APELLIDOS 3. TIPO DE DOCUMENTO 4. NÙMERO DE DOCUMENTO 5. DIRECCIÒN 6. CIUDAD 7. NÙMERO DE CELULAR 8. CORREO ELECTRÒNICO Y DEBE SER ACTUALIZADO PARA CONTINUAR")
                CT.FR_CONTROL("BtCLIENTE", col_txt:=Drawing.Color.Red) = Nothing
                'CT.redir("?fr=CLIENTE&cl=" + cl + "&ct=" + ctz)
                CT.FR_CONTROL("BtGUARDAR", False) = ""
            Else
                CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GMO) = "AGREGAR ITEMS"
            End If

        Else

        End If
        BtCLIENTE()
    End Sub
    Private Sub carga_multiordenes()
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
            If CT.SESION_GH("FILMO") Is Nothing Then
                CT.SESION_GH("FILMO") = CT.FR_CONTROL("DrESTADOMO") + "," + CT.FR_CONTROL("DrMES") + "," + CT.FR_CONTROL("DrYEAR")
            Else
                CT.FR_CONTROL("DrESTADOMO", evento:=AddressOf SEL_DR) = "=" + CT.SESION_GH("FILMO").ToString.Split(",")(0)
                CT.FR_CONTROL("DrMES", evento:=AddressOf SEL_DR) = "=" + CT.SESION_GH("FILMO").ToString.Split(",")(1)
                CT.FR_CONTROL("DrYEAR", evento:=AddressOf SEL_DR) = "=" + CT.SESION_GH("FILMO").ToString.Split(",")(2)
            End If
            FIL = "estadomo='" + CT.SESION_GH("FILMO").ToString.Split(",")(0) + "' and month(fechamo)=" + CT.SESION_GH("FILMO").ToString.Split(",")(1) + " and year(fechamo)=" + CT.SESION_GH("FILMO").ToString.Split(",")(2)
            ORD = "KMO DESC"
        Else
            CR = Nothing
            cam = "kmo-K,No;kmo-BT,CLIENTE;NOMBRE,FECHA;FECHAMO-D,FORMA_PAGO,VALOR_TOTAL-M,ESTADO;ESTADOMO,FACTURA,creado_por,facturado_por;fc_por"
            If lg.perfil = 2 Then
                'CR = " and estadomo <> '0 CREACION'"
                'CT.FILTROS_GRID("estadomo,orden,MES,YEAR")
                TL = ""
                CT.DrMES("DrMES", AddressOf SEL_DR) : CT.DrYEAR("DrYEAR", 2020, AddressOf SEL_DR)
                CT.FR_CONTROL("Drorden", evento:=AddressOf SEL_ORD) = "No.,NOMBRE(AZ),NOMBRE(ZA),FECHAMO(AZ),FECHAMO(ZA),FACTURA(AZ),FACTURA(ZA)"
                'CT.FR_CONTROL("Drestadomo",, dsmo.Carga_tablas("estadomo <> '0 CREACION'", "estadomo", "estadomo", True), AddressOf SEL_DR) = "estadomo-estadomo"
                CT.FR_CONTROL("Drestadomo", evento:=AddressOf SEL_DR) = "1 POR FACTURAR,2 FACTURADO,3 ANULADO"
                If CT.SESION_GH("FILMO") Is Nothing Then
                    CT.SESION_GH("FILMO") = CT.FR_CONTROL("DrESTADOMO") + "," + CT.FR_CONTROL("DrMES") + "," + CT.FR_CONTROL("DrYEAR")
                Else
                    CT.FR_CONTROL("DrESTADOMO", evento:=AddressOf SEL_DR) = "=" + CT.SESION_GH("FILMO").ToString.Split(",")(0)
                    CT.FR_CONTROL("DrMES", evento:=AddressOf SEL_DR) = "=" + CT.SESION_GH("FILMO").ToString.Split(",")(1)
                    CT.FR_CONTROL("DrYEAR", evento:=AddressOf SEL_DR) = "=" + CT.SESION_GH("FILMO").ToString.Split(",")(2)
                End If
                FIL = "estadomo='" + CT.SESION_GH("FILMO").ToString.Split(",")(0) + "' and month(fechamo)=" + CT.SESION_GH("FILMO").ToString.Split(",")(1) + " and year(fechamo)=" + CT.SESION_GH("FILMO").ToString.Split(",")(2)
            ElseIf lg.perfil = 3 Then
                cam = "creado_por-K,creado_por-BT,estadomo,total-SUM(valor_total)"
                CT.FORMULARIO_GR(TL, "GrMULTI", cam, lg.MODULOS, "multiorden", "estadomo='2 FACTURADO' and year(fechamo)=" + Now.Year.ToString + " and month(fechamo)=" + Now.Month.ToString, AddressOf sel_grmulti)
                Exit Sub
            End If
            'ORD = "estadomo"
            ORD = CT.FR_CONTROL("Drorden")
        End If
        CT.FORMULARIO_GR(TL, "GrMULTI", cam, lg.MODULOS + ",CARTERA", ,, AddressOf sel_grmulti, btorden:=True)
        fr.Controls.Add(PnT)
        CARGA_GrMUTI()
    End Sub
    Private Function val_cliente(campo As String) As String
        Return dscl.valor_campo(campo, "kcliente=" + cl)
    End Function

#Region "FINANCIACION"
    Private Sub SEL_GrCP()
        CT.redir("?fr=FINANCIACION&mo=" + dsfn.valor_campo("kmo", "kfn=" + CT.FR_CONTROL("GrFN")))
    End Sub
    Private Sub CARGA_FINANCIACION()
        mo = CT.reque("mo") : Dim x, y, z As Integer : Dim CONF As String = Nothing
        x = val_multiorden("VALOR_TOTAL") : y = dsfn.valor_campo_OTROS("SUM(VALOR_CUOTA)", "kmo=" + mo) : z = x - y
        CT.FORMULARIO("FINANCIACION", "LbCLIENTE,LbCELULAR",,, "MULTIORDENES,CARTERA")
        cl = dsct.valor_campo("kcliente", "kcot=" + val_multiorden("kcot"))
        CT.FR_CONTROL("LbCLIENTE") = dscl.valor_campo("nombre", "kcliente=" + cl)
        CT.FR_CONTROL("LbCELULAR") = dscl.valor_campo("ktelefono", "kcliente=" + cl)
        If val_multiorden("ESTADOMO") = "0 CREACION" Or val_multiorden("ESTADOMO") = "1 POR FACTURAR" Then
            If z > 0 Then
                CT.FORMULARIO(Nothing, "LbTOTAL_MULTIORDEN,LbSALDO,DrFORMA_PAGO,TxVALOR_A_FINANCIAR=0,DrCUOTAS,TxNOTA", True)
                CT.FR_CONTROL("LbTOTAL_MULTIORDEN") = "$" + FormatNumber(val_multiorden("VALOR_TOTAL"))
                CT.FR_CONTROL("LbSALDO") = "$" + FormatNumber(x - y).ToString
                CT.FR_CONTROL("DrFORMA_PAGO",, dsmo.dtparametros("MULTIORDEN", "FORMA PAGO")) = "VALOR-VALOR"
                CT.FR_CONTROL("DrFORMA_PAGO") = "=" + val_multiorden("FORMA_PAGO")
                CT.FR_CONTROL("DrCUOTAS") = "0,1,2,3,4"
                'CT.FR_BOTONES("MULTIORDEN")
                CT.FR_CONTROL("BtGUARDAR",,, AddressOf CLIC_BT) = "AGREGAR FINANCIACION"
                'ElseIf pf >= 2 Then
                CONF = ",CONFIRMO,-CH"
                'ElseIf CDate(dsfn.valor_campo("FECHA_CUOTA", "KMO=" + mo)) <= Now.ToShortDateString And dsfn.valor_campo("ESTADO", "KMO=" + mo) = "PENDIENTE" Then
            Else
                CONF = ",CONFIRMO,-CH"
            End If
        ElseIf val_multiorden("ESTADOMO") = "2 FACTURADO" Then
            If dsfn.valor_campo("ESTADO", "kmo=" + mo + " and estado='PENDIENTE'") IsNot Nothing Then
                CONF = ",CONFIRMO,-CH"
            End If
        End If
        CT.FORMULARIO_GR("FINANCIACION", "GrFN", "KFN-K,FORMA_PAGO,FECHA_CUOTA-D,NUMERO,VALOR_CUOTA-M,ESTADO,NOTA" + CONF, Nothing, "financiacion", "kmo=" + mo, SUBM_FR:=True)
        If pf = 1 Then
            Dim GrFN As GridView = fr.FindControl("GrFN") : Dim CFN As Boolean = False
            For Each GROW As GridViewRow In GrFN.Rows
                Try

                    If GROW.Cells(6).Text = "PAGO" Then
                        Dim ChG As CheckBox = GROW.Cells(1).FindControl("ChG")
                        ChG.Enabled = False
                    ElseIf CDate(GROW.Cells(3).Text) > Now.ToShortDateString Then
                        GROW.Cells(1).Text = ""
                    ElseIf CDate(GROW.Cells(3).Text) < Now.ToShortDateString And GROW.Cells(6).Text = "PENDIENTE" Then
                        CFN = True
                    End If
                Catch ex As Exception

                End Try
            Next
            If CFN = True Then
                CT.FR_BOTONES("MULTIORDEN,CONFIRMAR")
                CT.FR_CONTROL("BtCONFIRMAR",,, AddressOf CLIC_BT) = "CONFIRMAR PAGO"
            Else
                CT.FR_BOTONES("MULTIORDEN,LIMPIARFN")
                CT.FR_CONTROL("BtLIMPIARFN",,, AddressOf CLIC_BT) = "LIMPIAR FINANCIACION"
            End If
        End If
        CT.FR_CONTROL("BtMULTIORDEN",,, AddressOf CLIC_BT) = Nothing
    End Sub

#End Region


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
        CT.SESION_GH("FILMO") = CT.FR_CONTROL("DrESTADOMO") + "," + CT.FR_CONTROL("DrMES") + "," + CT.FR_CONTROL("DrYEAR")
        Select Case lg.perfil
            Case "1", "2"
                FIL = "ESTADOMO='" + CT.SESION_GH("FILMO").ToString.Split(",")(0) + "'"
                If CT.FR_CONTROL("DrMES") = Now.Month.ToString Then
                    FIL += " and MONTH(fechamo)=" + CT.SESION_GH("FILMO").ToString.Split(",")(1) + " and YEAR(fechamo)=" + CT.SESION_GH("FILMO").ToString.Split(",")(2)
                ElseIf lg.perfil = 1 Then
                    FIL = "ESTADOMO='" + CT.SESION_GH("FILMO").ToString.Split(",")(0) + "' and MONTH(fechamo)=" + CT.SESION_GH("FILMO").ToString.Split(",")(1) + " and YEAR(fechamo)=" + CT.SESION_GH("FILMO").ToString.Split(",")(2)
                ElseIf lg.perfil = 2 Then
                    FIL += " and MONTH(fechamo)=" + CT.SESION_GH("FILMO").ToString.Split(",")(1) + " and YEAR(fechamo)=" + CT.SESION_GH("FILMO").ToString.Split(",")(2)
                End If
            Case "3"
                FIL = "ESTADOMO='" + CT.SESION_GH("FILMO").ToString.Split(",")(0) + "' and MONTH(fechamo)=" + CT.SESION_GH("FILMO").ToString.Split(",")(1) + " and YEAR(fechamo)=" + CT.SESION_GH("FILMO").ToString.Split(",")(2)
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
        If CT.FR_CONTROL("Drestadomo") IsNot Nothing Then
            Dim ST As String = "TOTAL " + CT.FR_CONTROL("Drestadomo").Remove(0, 2)
            ST += " " + MonthName(CT.FR_CONTROL("DrMES")).ToUpper + " " + CT.FR_CONTROL("DrYEAR")
            LbT.Text = ST + " $ " + FormatNumber(DSNM.valor_campo_OTROS("SUM(valor_total)", "m.KCOT=n.KCOT and n.kcliente=c.kcliente and " + CR + FIL, ORD), 0)
            PnT.Controls.Add(LbT)
        End If

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
        Dim ret As String = Nothing
        If CT.reque("ct") IsNot Nothing Then
            ret = "&ct=" + ctz
        ElseIf CT.reque("mo") IsNot Nothing Then
            ret = "&mo=" + mo
        End If
        CT.redir("?fr=CLIENTE&cl=" + cl + ret)
    End Sub
    Private Sub sel_ne()
        CT.redir("?fr=COTIZACION&ct=" + ctz)
    End Sub
    Private Sub sel_grmulti()
        If lg.perfil = 3 Then
            If CT.reque("US") Is Nothing Then
                CT.redir("?fr=MULTIORDENES&US=" + CT.FR_CONTROL("GrMULTI") + fp)
            Else
                CT.redir("?fr=MULTIORDEN&mo=" + CT.FR_CONTROL("GrMULTI") + fp)
            End If
        Else
            CT.redir("?fr=MULTIORDEN&mo=" + CT.FR_CONTROL("GrMULTI") + fp)
        End If

    End Sub
    Private Sub CARGA_IMO()
        mo = CT.reque("mo")
        Dim idct, ref As String : idct = val_multiorden("kcot")


        Dim ict, imo As Integer
        ict = dsict.Carga_tablas("kcot=" + idct).Rows.Count : imo = dsimo.Carga_tablas("kmo=" + mo).Rows.Count
        ref = dsct.valor_campo("referencia", "kcot=" + idct)
        If dsict.Carga_tablas("kcot=" + idct).Rows.Count > 0 And dsimo.Carga_tablas("kmo=" + mo).Rows.Count = 0 Then
            ref = Nothing
            For Each row As DataRow In dsict.Carga_tablas("kcot=" + idct).Rows
                Dim VAL_INV As Integer = INV.disponibilidad(row.Item("referencia"))
                If ref IsNot Nothing Then
                    ref += " and "
                End If
                ref += "referencia='" + row.Item("referencia") + "'"
                'dsimo.insertardb(mo + "," + row.Item("cantidad").ToString + ",'" + row.Item("referencia") + "','" + row.Item("medida") + "','" + row.Item("diseño") + "','" + row.Item("marca") + "'," + row.Item("precio_u").ToString.Replace(",0000", "") + ",''")
            Next
            INV.consulta_inventario(ref)
        ElseIf INV.disponibilidad(dsct.valor_campo("referencia", "kcot=" + idct)) > 0 Then
            INV.consulta_inventario("referencia='" + ref + "'")
        End If
        If CT.reque("rf") IsNot Nothing Then
            ref = CT.reque("rf")
            cam = "TnCANTIDAD,TxDESCRIPCION,TxREFERENCIA,TxDISEÑO,TxMARCA,TnVALOR_UNITARIO"
            CT.FORMULARIO("ITEMS MULTIORDEN No. " + mo, cam, True,, "COTIZACIONES,CLIENTES")
            CT.FR_CONTROL("TnCANTIDAD", focus:=True) = "1"
            CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GITEMS) = "GUARDAR ITEM"
            CT.FR_CONTROL("TxREFERENCIA", False) = dsinv.valor_campo("referencia", "REFERENCIA='" + ref + "'")
            CT.FR_CONTROL("TxDESCRIPCION", False) = dsinv.valor_campo("grupo", "REFERENCIA='" + ref + "'")
            CT.FR_CONTROL("TxDISEÑO", False) = dsinv.valor_campo("diseno", "REFERENCIA='" + ref + "'")
            CT.FR_CONTROL("TxMARCA", False) = dsinv.valor_campo("marca", "REFERENCIA='" + ref + "'")
            CT.FR_CONTROL("TnVALOR_UNITARIO") = dsinv.valor_campo("precio_contado", "REFERENCIA='" + ref + "'").Replace(".0000", "")
        End If

        CT.FORMULARIO_GR(Nothing, "GrITEMS", "KIMO-K,cantidad,descripcion,ref,dis,marca,valoru,bodega,-CH", Nothing, "itemmo", "kmo=" + mo)
        CT.FR_BOTONES("ELIMINAR_ITEMS,VALIDAR_INVENTARIO,VOLVER_MULTIORDEN") : CT.FR_CONTROL("BtELIMINAR_ITEMS", evento:=AddressOf CLIC_BT) = Nothing
        CT.FR_CONTROL("BtVOLVER_MULTIORDEN", evento:=AddressOf CLIC_BT) = Nothing
        CT.FR_CONTROL("BtVALIDAR_INVENTARIO", evento:=AddressOf CLIC_BT) = Nothing

    End Sub
    Function val_multiorden(campo As String, Optional idmo As String = Nothing) As String
        If idmo Is Nothing Then
            idmo = CT.reque("mo")
        End If
        Return dsmo.valor_campo_OTROS(campo, "kmo=" + idmo)
    End Function

    Private Sub GITEMS()
        Dim CN, DE, RE, DI, MA, VL, PI, BD As String
        CN = CT.FR_CONTROL("TnCANTIDAD", VALIDAR:=True) : DE = CT.FR_CONTROL("TxDESCRIPCION", VALIDAR:=True) : RE = CT.FR_CONTROL("TxREFERENCIA")
        DI = CT.FR_CONTROL("TxDISEÑO", VALIDAR:=True) : MA = CT.FR_CONTROL("TxMARCA", VALIDAR:=True) : VL = CT.FR_CONTROL("TnVALOR_UNITARIO", VALIDAR:=True)
        BD = CT.reque("bd")
        If PI Is Nothing Then
            PI = "0"
        Else
            Dim xp As Integer = dsinv.valor_campo("precio_contado", "KDISPO=" + PI).Replace(".0000", "")
            If CN > dsinv.valor_campo("DISPONIBLEB", "KDISPO=" + PI) Then
                CT.alerta("La cantidad es mayor al disponible del inventario")
                Exit Sub
            ElseIf VL < xp Then
                CT.alerta("El valor es menor al valor de contado")
                Exit Sub
            End If
        End If
        If CT.validacion_ct = False Then
            dsimo.insertardb(mo + "," + CN + ",'" + DE + "','" + RE + "','" + DI + "','" + MA + "'," + VL + ",'" + BD + "'")
            CT.redir("?fr=ITEMSMO&mo=" + mo)
        End If
    End Sub
    Private Sub CARGA_MO()
        CT.FR_CONTROL("LbFECHA") = dsmo.valor_campo("FECHAMO", "KMO=" + mo)
        CT.FR_CONTROL("DrTIPO_ORDEN", False) = dsmo.valor_campo("TIPO_ORDEN", "KMO=" + mo)

        If dsmo.valor_campo("estadomo", "KMO=" + mo).Contains("0 ") Then
            CT.FR_CONTROL("DrFORMA_PAGO",, db:=dsmo.dtparametros("MULTIORDEN", "FORMA PAGO")) = "valor-valor"
            CT.FR_CONTROL("DrFORMA_PAGO") = "=" + dsmo.valor_campo("FORMA_PAGO", "KMO=" + mo)
            'CT.FR_CONTROL("DrFORMA_PAGO", True, db:=dsmo.dtparametros("MULTIORDEN", "FORMA PAGO")) = "VALOR=" + dsmo.valor_campo("FORMA_PAGO", "KMO=" + mo)
            'CT.FR_CONTROL("DrFORMA_PAGO") = 
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
                Dim OBS As String = CT.FR_CONTROL("TmOBS")
                If OBS.Length < 20 Then
                    CT.alerta("NO HAY OBSERVACION DE ENTREGA DILIGENCIE PARA CONTINUAR")
                    Exit Sub
                End If
                dsmo.actualizardb("fechamo='" + CT.HOY_FR + "',estadomo='1 POR FACTURAR', observaciones='" + OBS + "',forma_pago='" + CT.FR_CONTROL("DrFORMA_PAGO") + "'", "KMO=" + mo)
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
                Exit Sub
            Case "FINANCIACION"
                CT.redir("?fr=FINANCIACION&mo=" + mo)
            Case "AGREGAR FINANCIACION"
                Dim FP, FC, NC, VC, ES, NT, XNC As String
                FP = CT.FR_CONTROL("DrFORMA_PAGO") : FC = CDate(val_multiorden("FECHAMO")).ToString("yyyy-MM-dd")
                NC = CT.FR_CONTROL("DrCUOTAS") : VC = CT.FR_CONTROL("TxVALOR_A_FINANCIAR") : ES = "PENDIENTE" : NT = CT.FR_CONTROL("TxNOTA")
                If NC = 0 Then
                    XNC = -1
                Else
                    XNC = 0
                End If
                If VC = "" Then
                    CT.FR_CONTROL("LbERROR", col_txt:=Drawing.Color.Red) = "<BR>EL CAMPO VALOR A FINANCIAR NO PUEDE ESTAR EN BLANCO"
                    Exit Sub
                ElseIf CInt(VC) = 0 Then
                    CT.FR_CONTROL("LbERROR", col_txt:=Drawing.Color.Red) = "<BR>EL CAMPO VALOR A FINANCIAR DEBE SER MAYOR A 0"
                    Exit Sub
                End If
                If NT = "" Then
                    CT.FR_CONTROL("LbERROR", col_txt:=Drawing.Color.Red) = "<BR>EL CAMPO NOTA NO PUEDE ESTAR EN BLANCO"
                    Exit Sub
                End If
                If CInt(VC) <= CInt(CT.FR_CONTROL("LbSALDO")) Then
                    For XC As Integer = XNC To CInt(NC) - 1
                        Dim DVC As String = VC
                        If NC <> "0" Then
                            DVC = FormatNumber(CInt(VC) / CInt(NC), 0).Replace(".", "")
                            FC = DateAdd(DateInterval.Day, 30, CDate(FC)).ToString("yyyy-MM-dd")
                        End If
                        dsfn.insertardb(mo + ",'" + FP + "','" + FC + "'," + (XC + 1).ToString + "," + DVC + ",'" + ES + "','" + NT + "',''")
                    Next
                    CT.redir("?fr=FINANCIACION&mo=" + mo)
                Else
                    CT.FR_CONTROL("LbERROR", col_txt:=Drawing.Color.Red) = "<BR>El valor a financiar es mayor al saldo de la multiorden".ToUpper
                    Exit Sub
                End If
            Case "CONFIRMAR PAGO"
                Dim GrFN As GridView = fr.FindControl("GrFN")
                For Each GROW As GridViewRow In GrFN.Rows
                    Dim ChG As CheckBox = GROW.Cells(1).FindControl("ChG")
                    If ChG IsNot Nothing Then
                        If ChG.Checked = True Then
                            dsfn.actualizardb("ESTADO='PAGO',CONFIRMO='" + CT.USERLOGUIN + "'", "KFN=" + GROW.Cells(0).Text)
                        End If
                    End If

                Next
                Select Case CT.reque("fr")
                    Case "CARTERA"
                        CT.redir("?fr=CARTERA")
                    Case "FINANCIACION"
                        CT.redir("?fr=FINANCIACION&mo=" + CT.reque("mo"))
                End Select

            Case "LIMPIAR FINANCIACION"
                dsfn.Eliminardb("KMO=" + mo + " AND ESTADO='PENDIENTE'")
            Case "VALIDAR INVENTARIO"
                CT.redir("?fr=VALINV&mo=" + CT.reque("mo"))
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
            If OB.Length < 20 Then
                CT.alerta("NO HAY OBSERVACION DE ENTREGA DILIGENCIE PARA CONTINUAR")
                Exit Sub
            End If
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

        imp.cGENERAR_PDF("multiorden.pdf")
        CT.redireccion("multiorden.pdf",, True)

    End Sub
#Region "CARTERA"

#End Region

End Class
