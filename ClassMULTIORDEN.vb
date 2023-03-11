Imports Classcatalogoch

Public Class ClassMULTIORDEN
    Private CT As ClassConstructor22
    Private dscl As New carga_dssql("clientes")
    Private dsne As New carga_dssql("negocios")
    Private dsmo As New carga_dssql("multiorden")
    Private dsimo As New carga_dssql("itemmo")
    Private lg As New ClassLogin

    Private Shared cam, ne, mo, pf, fp, cl As String
    Sub New(PANEL As Panel, PERFIL As String)
        fp = "&#finalp"
        lg.APP_PARAMETROS("MULTIORDEN") = "TIPO ORDEN,FORMA PAGO"
        pf = PERFIL
        dsmo.campostb = "kmo-key,knegocio-bigint,fechamo-date,tipo_orden-varchar(250),valor_total-bigint,forma_pago-varchar(250),creado_por-varchar(250),cerrado_por-varchar(250),estadomo-varchar(50)"
        dsimo.campostb = "kimo-key,kmo-bigint,cantidad-bigint,descripcion-varchar(1000),ref-varchar(250),dis-varchar(250),marca-varchar(250),valoru-bigint"
        CT = New ClassConstructor22(PANEL, "default.aspx", "MULTIORDEN")
        ne = CT.reque("ne") : mo = CT.reque("mo")
        Select Case CT.reque("fr")
            Case "MULTIORDEN"
                cam = "BtCLIENTE,BtNEGOCIO,LbFECHA,DrFORMA_PAGO,LbVALOR_TOTAL"
                If CT.reque("mo") IsNot Nothing Then
                    mo = CT.reque("mo")
                    ne = dsmo.valor_campo("knegocio", "kmo=" + mo)
                    cl = dsne.valor_campo("kcliente", "knegocio=" + ne)
                Else
                    mo = dsmo.valor_campo("kmo", "knegocio=" + ne)
                    cl = dsne.valor_campo("kcliente", "knegocio=" + ne)
                End If
                CT.FORMULARIO("MULTIORDEN", cam, True,, "NEGOCIOS,CLIENTES")
                If mo IsNot Nothing Then
                    CARGA_MO()
                    CT.FORMULARIO_GR(Nothing, "GrITEMS", "KIMO-K,cantidad,descripcion,ref,dis,marca,valoru", Nothing, "itemmo", "kmo=" + mo)
                    If dsmo.valor_campo("ESTADOMO", "KMO=" + mo) = "1 FACTURAR" Then
                        CT.FR_CONTROL("BtGUARDAR", False) = Nothing
                        CT.FR_CONTROL("BtCANCELAR", False) = Nothing
                        CT.FR_BOTONES("IMPRESION,FACTURADO")
                        CT.FR_CONTROL("BtIMPRESION", evento:=AddressOf CLIC_BT) = Nothing
                        CT.FR_CONTROL("BtFACTURADO", evento:=AddressOf CLIC_BT) = Nothing
                    ElseIf dsmo.valor_campo("ESTADOMO", "KMO=" + mo) = "2 FACTURADO" Then
                        CT.FR_CONTROL("BtGUARDAR", False) = Nothing
                        CT.FR_CONTROL("BtCANCELAR", False) = Nothing
                        CT.FR_BOTONES("IMPRESION")
                        CT.FR_CONTROL("BtIMPRESION", evento:=AddressOf CLIC_BT) = Nothing
                    Else
                        CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GMO) = "AGREGAR ITEMS"
                    End If
                ElseIf ne IsNot Nothing Then
                    CT.FR_CONTROL("LbFECHA") = Now.ToString("yyyy-MM-dd")
                    CT.DrPARAMETROS("DrFORMA_PAGO", "MULTIORDEN", "FORMA PAGO") = Nothing
                    CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GMO) = "AGREGAR ITEMS"
                Else

                End If
                BtCLIENTE()

            Case "MULTIORDENES"
                If pf = 1 Then
                    CT.FORMULARIO_GR("MULTIORDENES", "GrMULTI", "kmo-K,NOMBRE-BT,FECHAMO-BT,FORMA_PAGO-BT,VALOR_TOTAL-BT", "NEGOCIOS,CLIENTES", "multiorden m,negocios n,clientes c", "m.knegocio=n.knegocio and n.kcliente=c.kcliente and m.creado_por='" + CT.USERLOGUIN + "'", AddressOf sel_grmulti, "estadomo")
                Else
                    CT.FORMULARIO_GR("MULTIORDENES", "GrMULTI", "", "NEGOCIOS,CLIENTES", "multiorden", "creado_por='" + CT.USERLOGUIN + "'",, "estadomo")
                End If
            Case "ITEMSMO"
                CARGA_IMO()
        End Select
    End Sub
    Private Sub BtCLIENTE()
        If cl IsNot Nothing Then
            CT.FR_CONTROL("BtCLIENTE", evento:=AddressOf SEL_CL) = dscl.valor_campo("NOMBRE", "KCLIENTE=" + cl) + " - " + dscl.valor_campo("KTELEFONO", "KCLIENTE=" + cl)
            CT.FR_CONTROL("BtNEGOCIO", evento:=AddressOf sel_ne) = "NEGOCIO No. " + ne + " REF. (" + dsne.valor_campo("REFERENCIA", "KNEGOCIO=" + ne) + ")"
        End If
    End Sub
    Private Sub SEL_CL()
        CT.redir("?fr=CLIENTE&cl=" + cl)
    End Sub
    Private Sub sel_ne()
        CT.redir("?fr=NEGOCIO&ne=" + ne)
    End Sub
    Private Sub sel_grmulti()
        CT.redir("?fr=MULTIORDEN&mo=" + CT.FR_CONTROL("GrMULTI") + fp)
    End Sub
    Private Sub CARGA_IMO()
        mo = CT.reque("mo")
        cam = "TnCANTIDAD,TxDESCRIPCION,TxREFERENCIA,TxDISEÑO,TxMARCA,TnVALOR_UNITARIO"
        CT.FORMULARIO("ITEMS MULTIORDEN No. " + mo, cam, True,, "NEGOCIOS,CLIENTES")
        CT.FR_CONTROL("TnCANTIDAD", focus:=True) = "1"
        ''CARGA_MO()
        CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GITEMS) = "GUARDAR ITEM"
        'CT.FR_CONTROL("BtGUARDAR", evento:=AddressOf GITEMS) = "GUARDAR ITEM"
        CT.FORMULARIO_GR(Nothing, "GrITEMS", "KIMO-K,cantidad,descripcion,ref,dis,marca,valoru,-CH", Nothing, "itemmo", "kmo=" + mo)
        CT.FR_BOTONES("ELIMINAR_ITEMS,ENVIAR_FACTURACION") : CT.FR_CONTROL("BtELIMINAR_ITEMS", evento:=AddressOf CLIC_BT) = Nothing
        CT.FR_CONTROL("BtENVIAR_FACTURACION", evento:=AddressOf CLIC_BT) = "ENVIAR ORDEN"
    End Sub
    Private Sub GITEMS()
        Dim CN, DE, RE, DI, MA, VL, VT As String
        CN = CT.FR_CONTROL("TnCANTIDAD", VALIDAR:=True) : DE = CT.FR_CONTROL("TxDESCRIPCION", VALIDAR:=True) : RE = CT.FR_CONTROL("TxREFERENCIA")
        DI = CT.FR_CONTROL("TxDISEÑO", VALIDAR:=True) : MA = CT.FR_CONTROL("TxMARCA", VALIDAR:=True) : VL = CT.FR_CONTROL("TnVALOR_UNITARIO", VALIDAR:=True)
        If CT.validacion_ct = False Then
            VT = CInt(dsmo.valor_campo("VALOR_TOTAL", "KMO=" + mo)) + (CInt(VL) * CInt(CN))
            dsimo.insertardb(mo + "," + CN + ",'" + DE + "','" + RE + "','" + DI + "','" + MA + "'," + VL)
            dsmo.actualizardb("VALOR_TOTAL=" + VT, "KMO=" + mo)
            CT.redir("?fr=ITEMSMO&mo=" + mo + "&#finalp")
        End If
    End Sub
    Private Sub CARGA_MO()
        CT.FR_CONTROL("LbFECHA") = dsmo.valor_campo("FECHAMO", "KMO=" + mo)
        CT.FR_CONTROL("DrTIPO_ORDEN", False) = dsmo.valor_campo("TIPO_ORDEN", "KMO=" + mo)
        CT.FR_CONTROL("DrFORMA_PAGO", False) = dsmo.valor_campo("FORMA_PAGO", "KMO=" + mo)
        CT.FR_CONTROL("LbVALOR_TOTAL") = dsmo.valor_campo("VALOR_TOTAL", "KMO=" + mo)
    End Sub

    Private Sub CLIC_BT(SENDER As Object, E As EventArgs)
        Dim BT As Button = SENDER
        Select Case BT.Text
            Case "ELIMINAR ITEMS"
                Dim imo, vimo, vt As String
                imo = CT.FR_CONTROL("ChGrITEMS") : vimo = dsimo.valor_campo_OTROS("sum(valor * cantidad)", "kimo=" + imo)
                vt = CInt(dsmo.valor_campo("valor_total", "kmo=" + mo)) - CInt(vimo)
                dsmo.actualizardb("valor_total=" + vt, "kmo=" + mo) : dsimo.Eliminardb("kimo=" + imo)
            Case "ENVIAR FACTURACION"
                dsmo.actualizardb("estadomo='1 FACTURAR'", "KMO=" + mo)
            Case "IMPRESION"
                CT.alerta("EN PREPARACION")
            Case "FACTURADO"
                dsmo.actualizardb("estadomo='2 FACTURADO'", "KMO=" + mo)
        End Select
        CT.redir("?fr=MULTIORDEN&mo=" + mo)
    End Sub



    Private Sub GMO()
        Dim FE, TMO, VT, FP, CP, CR As String
        FE = CT.FR_CONTROL("LbFECHA") : TMO = "" : FP = CT.FR_CONTROL("DrFORMA_PAGO") : CP = CT.USERLOGUIN : CR = CT.USERLOGUIN
        If mo Is Nothing Then
            dsmo.insertardb(ne + ",'" + FE + "','" + TMO + "',0,'" + FP + "','" + CP + "','','0 CREACION'")
            mo = dsmo.valor_campo("kmo", "fechamo='" + FE + "' and knegocio=" + ne + " and creado_por='" + CP + "'")
        Else
            VT = CT.FR_CONTROL("LbVALOR_TOTAL") : FP = CT.FR_CONTROL("DrFORMA_PAGO")
        End If
        If mo IsNot Nothing Then
            CT.redir("?fr=ITEMSMO&mo=" + mo)
        End If
    End Sub
End Class
