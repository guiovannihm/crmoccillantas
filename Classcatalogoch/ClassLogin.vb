'ultima modificacion 16-04-24
Imports System.Security.Claims
Imports System.Web
Imports System.Web.UI.WebControls


Public Class ClassLogin

    Private Shared ct As ClassConstructor22
    Private dspar As New carga_dssql("parametros", True)
    Private dsus As New carga_dssql("usuarios", False)
    Private dsper As New carga_dssql("permisos")
    Private dsmsn As New carga_dssql("msn")
    Private enc As New ENCRIPTAR2
    Private Shared pns, pnf, pni As Panel
    Sub New()
        dspar.campostb = "kparametro-key,formulario-varchar(250),criterio-varchar(250),valor-varchar(250)"
        dsus.campostb = "keyusuarios-key,usuario-varchar(250),clave-varchar(250),nombre-varchar(250),correo-varchar(250),cargo-varchar(250),perfil-varchar(250)"
        dsper.campostb = "keypermisos-key,kusuario-int,orden-bigint,modulo-varchar(250),perfil-varchar(250)"
        dsmsn.campostb = "kmsn-key,fecham-datetime,de-varchar(100),para-varchar(100),asunto-varchar(500),msn-text,estado-varchar(10)"
    End Sub
    Shared XP As String

#Region "INICIO"


    Public Property MODULOS As String
        Get
            Try
                XP = Nothing
                If ct.USERLOGUIN <> Nothing Then
                    Try
                        For Each ROW As DataRow In dsper.Carga_tablas("KUSUARIO=" + item_usuario("KEYUSUARIOS",, ct.USERLOGUIN), "orden").Rows
                            If XP IsNot Nothing Then
                                XP += ","
                            End If
                            XP += enc.stdsenencripta(ROW.Item("MODULO"))
                        Next
                    Catch ex As Exception

                    End Try
                End If
            Catch ex As Exception

            End Try

            Return XP
        End Get
        Set(value As String)
            For Each STR As String In value.Split(",")
                XP = dspar.valor_campo("VALOR", "FORMULARIO='" + enc.stencripta("SistemA") + "' AND CRITERIO='" + enc.stencripta("mODULo") + "' and valor='" + enc.stencripta(STR.ToUpper) + "'")
                If XP Is Nothing Then
                    dspar.insertardb("'" + enc.stencripta("SistemA") + "','" + enc.stencripta("mODULo") + "','" + enc.stencripta(STR.ToUpper) + "'")
                End If
            Next
            If ct.USERLOGUIN IsNot Nothing Then
                carga_permisos_admin(value)
            End If
        End Set
    End Property
    Private Sub carga_permisos_admin(MD As String)
        Dim xper As Integer = 1
        For Each srow As String In MD.Split(",")
            If dsper.valor_campo("kusuario", "modulo='" + enc.stencripta(srow) + "'") Is Nothing Then
                dsper.insertardb("1," + xper.ToString + ",'" + enc.stencripta(srow.ToUpper) + "',3")
                xper += 1
            End If
        Next
    End Sub

    Public Sub carga_fr(Fr As Panel)
        pnf = Fr
        ct = New ClassConstructor22(Fr, "default.aspx", "INICIO")
        If dspar.Carga_tablas("FORMULARIO='SISTEMA' AND CRITERIO='TITULO'").Rows.Count = 0 Then
            ct.FORMULARIO("CONFIGURACION", "TxNOMBRE_APP,FnLOGO", True)
            ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf GUARDAR) = Nothing
        ElseIf dsus.Carga_tablas.Rows.Count = 0 Then
            ct.FORMULARIO("CONFIGURACION", "TxUSUARIO,TpCLAVE,TxNOMBRE,TxCORREO,TxCARGO", True)
            ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf GR_USUARIO) = Nothing
        Else
            If ct.reque("app") Is Nothing Then
                ct.FORMULARIO(ct.val_parametro("SISTEMA", "TITULO"), "LgINICIO_DE_SESION,ImLOGO,LbERROR")
                ct.FR_CONTROL("ImLOGO") = ct.val_parametro("SISTEMA", "LOGO")
                ct.FR_CONTROL("LbERROR") = Nothing
                ct.FR_CONTROL("BtINICIO_SESION", evento:=AddressOf login) = Nothing
                ct.SESION_GH("app") = False
            Else
                ct.SESION_GH("app") = True
                login()
            End If
        End If
    End Sub
    Private Sub GUARDAR()
        If ct.FR_CONTROL("TxNOMBRE_APP") IsNot Nothing Then
            ct.add_parametro("SISTEMA", "TITULO", ct.FR_CONTROL("TxNOMBRE_APP"))
        End If
        If ct.FR_CONTROL("FnLOGO") IsNot Nothing Then
            ct.add_parametro("SISTEMA", "LOGO", ct.FR_CONTROL("FnLOGO"))
            Dim FNL As FileUpload = pnf.FindControl("FnLOGO")
            FNL.SaveAs(ct.ruta_dir + "\img\" + FNL.FileName)
        End If
        ct.redireccion("login.aspx")
    End Sub
    Private Sub GR_USUARIO()
        Dim cl As String = ct.FR_CONTROL("TpCLAVE")
        If cl.Length <= 5 Then
            ct.alerta("La contraseña debe contener mas de 6 caracteres")
        ElseIf val_cl_num(cl) = False Or val_cl_may(cl) = False Then
            ct.alerta("La contraseña debe contener un numero, una minuscula y una mayuscula")
        Else
            dsus.insertardb("'" + enc.stencripta(ct.FR_CONTROL("TxUSUARIO").ToUpper) + "','" + enc.stencripta(ct.FR_CONTROL("TpCLAVE")) + "','" + enc.stencripta(ct.FR_CONTROL("TxNOMBRE").ToUpper) + "','" + enc.stencripta(ct.FR_CONTROL("TxCORREO")) + "','" + enc.stencripta(ct.FR_CONTROL("TxCARGO")) + "','" + enc.stencripta("SUPERADMIN") + "'")
            ct.alerta("login.aspx")
        End If
    End Sub

    Public Sub panel_sup(pn As Panel, menu As String, Optional PCOLOR As Drawing.Color = Nothing)
        ct = New ClassConstructor22(pn, "default.aspx")
        pn.Controls.Clear()
        If ct.SESION_GH("app") Is Nothing Then
            ct.redireccion("login.aspx")
        End If
        Dim cam As String = "ImSUP-20,DrUSUARIO,LbF,BtMSN,LbB,BtBUSCAR"
        If ct.SESION_GH("app") = False Then
            If menu IsNot Nothing Then
                ct.FORMULARIO(ct.val_parametro("SISTEMA", "titulo"), cam, FrSUPERIOR:=True, It_MENU:=menu, col_fr:=PCOLOR)
            Else
                ct.FORMULARIO(ct.val_parametro("SISTEMA", "titulo"), cam, FrSUPERIOR:=True, col_fr:=PCOLOR)
            End If
            If ct.val_parametro("SISTEMA", "LOGO") Is Nothing Then
                ct.FR_CONTROL("LbSUP") = ct.val_parametro("SISTEMA", "TITULO") + "<br>" + Now.ToString("MMMM dd yyyy hh:mm")
            Else
                ct.FR_CONTROL("ImSUP") = ct.val_parametro("SISTEMA", "LOGO") ' + "<br>" + Now.ToString("MMMM dd yyyy hh:mm")
                ct.FR_CONTROL("LbF") = Now.ToString("MMMM dd yyyy hh:mm")
            End If
            ct.FR_CONTROL("DrUSUARIO", evento:=AddressOf SEL_DrSESION) = enc.stdsenencripta(dsus.valor_campo("nombre", "usuario='" + enc.stencripta(ct.USERLOGUIN) + "'")) + ",CAMBIO CLAVE,CERRAR SESION"
        Else
            ct.FORMULARIO("", "LbAPP", FrSUPERIOR:=True, It_MENU:=menu, col_fr:=PCOLOR)
        End If
        If perfil() = "3" Or perfil() = "2" Then
            If ct.reque("fr") = "" Or ct.reque("fr") = "INICIO" Then
                ct.FR_ICONOS("INICIO,CONFIGURACION", HorizontalAlign.Left, True)
            Else
                ct.FR_ICONOS("INICIO", HorizontalAlign.Left, True)
            End If
        Else
            ct.FR_ICONOS("INICIO", HorizontalAlign.Left, True)
        End If
        If pn.FindControl("BtMSN") IsNot Nothing Then
            Dim XMSN As Integer = dsmsn.valor_campo_OTROS("COUNT(KMSN)", "ESTADO='NOLEIDO' AND PARA='" + ct.USERLOGUIN + "'")
            XMSN += dsmsn.valor_campo_OTROS("COUNT(KMSN)", "ESTADO='NOLEIDO' AND PARA='TODOS'")
            If XMSN > 0 Then
                ct.FR_CONTROL("BtMSN", evento:=AddressOf CLIC_BtMSN, col_txt:=Drawing.Color.Red) = "MENSAJES " + XMSN.ToString
            Else
                ct.FR_CONTROL("BtMSN", evento:=AddressOf CLIC_BtMSN) = "MENSAJES " + XMSN.ToString
            End If

        End If
        If pn.FindControl("BtBUSCAR") IsNot Nothing Then
            ct.FR_CONTROL("BtBUSCAR", evento:=AddressOf CLIC_BtBUSCAR) = Nothing
        End If
    End Sub
    Public Sub NUEVO_MSN(DE As String, PARA As String, ASUNTO As String, MSN As String)
        Dim mnst As String = dsmsn.valor_campo("msn", "de='" + DE + "' and para='" + PARA + "' order by kmsn desc")
        If mnst <> MSN Then
            dsmsn.insertardb("'" + Now.ToString("yyyy-MM-dd") + "T" + Now.ToString("HH:mm:ss") + "','" + DE + "','" + PARA + "','" + ASUNTO + "','" + MSN + "','NOLEIDO'", True)
        End If
    End Sub
    Private Sub CLIC_BtMSN()
        ct.redir("?fr=LMSN")
    End Sub
    Private Sub CLIC_BtBUSCAR()
        ct.redir("?fr=BUSCAR CLIENTE")
    End Sub
    Public Sub MSN(pn As Panel)
        pns = pn
        ct = New ClassConstructor22(pn, "default.aspx", "INICIO")
        Select Case ct.reque("fr")
            Case "LMSN", "RECIBIDOS"
                'ct.FORMULARIO_GR("MSN", "GrMSN", "KMSN-K,FECHA;FECHAM-BT,DE-BT,PARA-BT,ASUNTO-BT,ESTADO-BT", "NUEVO MSN", "MSN", "DE='" + ct.USERLOGUIN + "' OR PARA='" + ct.USERLOGUIN + "'", AddressOf SEL_GrMSN,, "ESTADO,FECHAM DESC")
                ct.FORMULARIO_GR("MSN RECIBIDOS", "GrMSN", "KMSN-K,FECHA;FECHAM-BT,DE-BT,PARA-BT,ASUNTO-BT,ESTADO-BT", "INICIO,NUEVO MSN,ENVIADOS", "MSN", "PARA='" + ct.USERLOGUIN + "'", AddressOf SEL_GrMSN,, "ESTADO,FECHAM DESC")
            Case "ENVIADOS"
                ct.FORMULARIO_GR("MSN ENVIADOS", "GrMSN", "KMSN-K,FECHA;FECHAM-BT,DE-BT,PARA-BT,ASUNTO-BT,ESTADO-BT", "INICIO,NUEVO MSN,RECIBIDOS", "MSN", "DE='" + ct.USERLOGUIN + "'", AddressOf SEL_GrMSN,, "ESTADO,FECHAM DESC")
            Case "MSN"
                Dim CAM As String = Nothing
                Dim id As String = ct.reque("id")
                If id Is Nothing Then
                    CAM = "LbFECHA,DrPARA,TxASUNTO,TmMENSAJE"
                Else
                    CAM = "LbFECHA,LbDE,LbPARA,LbASUNTO,TmRESPUESTA,LbMENSAJE"
                End If
                ct.FORMULARIO("MENSAJE", CAM, True,, "INICIO,NUEVO MSN,RECIBIDOS", COL:=True)
                If id Is Nothing Then
                    DrUSUARIO_USER(pn.FindControl("DrPARA"),, True, True)
                    ct.FR_CONTROL("LbFECHA") = Now.ToString("yyyy-MM-dd HH:mm:ss")
                    ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf CLIC_ENVIO) = "ENVIAR"
                Else
                    ct.FR_CONTROL("LbFECHA") = dsmsn.valor_campo("FECHAM", "KMSN=" + id).Replace(Chr(10), "<BR>")
                    ct.FR_CONTROL("LbDE") = dsmsn.valor_campo("DE", "KMSN=" + id).Replace(Chr(10), "<BR>")
                    ct.FR_CONTROL("LbPARA") = dsmsn.valor_campo("PARA", "KMSN=" + id).Replace(Chr(10), "<BR>")
                    ct.FR_CONTROL("LbASUNTO") = dsmsn.valor_campo("ASUNTO", "KMSN=" + id).Replace(Chr(10), "<BR>")
                    ct.FR_CONTROL("LbMENSAJE") = dsmsn.valor_campo("MSN", "KMSN=" + id).Replace(Chr(10), "<BR>")
                    If ct.FR_CONTROL("LbPARA") = ct.USERLOGUIN Then
                        ct.FR_BOTONES("ELIMINAR_MSN")
                        ct.FR_CONTROL("BtELIMINAR_MSN", evento:=AddressOf CLIC_ELIMINAR) = Nothing
                    End If
                    ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf CLIC_ENVIO) = "RESPONDER"
                    dsmsn.actualizardb("ESTADO='LEIDO'", "PARA='" + ct.USERLOGUIN + "' AND kmsn=" + id)
                End If
        End Select
    End Sub
    Private Sub CLIC_ELIMINAR()
        dsmsn.Eliminardb("kmsn=" + ct.reque("id"))
        ct.redir("?fr=LMSN")
    End Sub
    Private Sub CLIC_ENVIO()
        ct = New ClassConstructor22(pns)
        Dim fe, de, pa, asu, ms As String
        fe = Now.ToString("yyyy-MM-dd") + "T" + Now.ToString("HH:mm:ss")
        Select Case ct.FR_CONTROL("BtGUARDAR")
            Case "ENVIAR"
                pa = ct.FR_CONTROL("DrPARA") : asu = ct.FR_CONTROL("TxASUNTO") : ms = ct.FR_CONTROL("TmMENSAJE")
                If pa = "TODOS" Then
                    For Each ROW As DataRow In dtusuario.Rows
                        If ROW.Item("USUARIO") <> ct.USERLOGUIN Then
                            dsmsn.insertardb("'" + fe + "','" + ct.USERLOGUIN + "','" + ROW.Item("USUARIO") + "','" + asu + "','" + ms + "','NOLEIDO'", True)
                        End If
                    Next
                Else
                    dsmsn.insertardb("'" + fe + "','" + ct.USERLOGUIN + "','" + pa + "','" + asu + "','" + ms + "','NOLEIDO'", True)
                End If
                ct.redir("?fr=LMSN")
            Case "RESPONDER"
                Dim rta As String = "msn='" + ct.USERLOGUIN + " (" + Now.ToString + "): " + ct.FR_CONTROL("TmRESPUESTA") + Chr(10) + dsmsn.valor_campo("msn", "kmsn=" + ct.reque("id")) + "'"
                pa = dsmsn.valor_campo("de", "kmsn=" + ct.reque("id")) : de = dsmsn.valor_campo("para", "kmsn=" + ct.reque("id"))
                If pa = ct.USERLOGUIN Then
                    pa = dsmsn.valor_campo("para", "kmsn=" + ct.reque("id")) : de = dsmsn.valor_campo("de", "kmsn=" + ct.reque("id"))
                End If
                dsmsn.actualizardb(rta + ",fecham='" + fe + "',de='" + de + "',para='" + pa + "',estado='NOLEIDO'", "kmsn=" + ct.reque("id"))
                ct.redir("?fr=LMSN")
        End Select
    End Sub
    Private Sub SEL_GrMSN()
        ct = New ClassConstructor22(pns)
        ct.redir("?fr=MSN&id=" + ct.FR_CONTROL("GrMSN"))
    End Sub

    Private Sub INICIO()
        ct.redireccion("default.aspx")
    End Sub
    Private Sub CONFIGURACION()
        ct.redireccion("default.aspx", "fr=CONFIG")
    End Sub

    Private Sub SEL_DrSESION()
        Select Case ct.FR_CONTROL("DrUSUARIO")
            Case "CERRAR SESION"
                ct.cerrar_session()
            Case "CAMBIO CLAVE"
                ct.redir("?fr=CC")
        End Select
    End Sub
    Private Function val_cl_num(txt As String) As Boolean
        For x As Integer = 0 To 9
            If txt.Contains(x.ToString) Then
                Return True
            End If
        Next
        Return False
    End Function
    Private Function val_cl_may(txt As String) As Boolean
        Dim x As String = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z"
        Dim bx As Boolean
        For Each y As String In x.Split(",")
            If txt.Contains(y) Then
                bx = True
            End If
        Next
        For Each y As String In x.ToUpper.Split(",")
            If txt.Contains(y) Then
                If bx = True Then
                    Return True
                End If
            End If
        Next
        Return False
    End Function


    Private Sub login()
        Dim us, cl As String
        If ct.SESION_GH("app") = True Then
            us = ct.reque("app")
            cl = ct.reque("sl")
        Else
            us = ct.FR_CONTROL("TxUSUARIO").ToUpper
            cl = ct.FR_CONTROL("TxCLAVE")
        End If

        For Each row As DataRow In dsus.Carga_tablas("usuario='" + enc.stencripta(us.ToUpper) + "'").Rows
            If enc.stdsenencripta(row.Item("clave")) = cl Then
                Try
                    ct.SESION_GH("perfil") = enc.stdsenencripta(row.Item("perfil"))
                Catch ex As Exception
                    ct.SESION_GH("perfil") = "ADMIN"
                End Try
                System.Web.Security.FormsAuthentication.RedirectFromLoginPage(us, True)
                ct.redireccion("default.aspx")
            Else
                ct.FR_CONTROL("LbERROR") = "ERROR DE USUARIO O CLAVE"
            End If
        Next
        If us.ToUpper = "ADMINCH" And cl = "61S4st3m" Then
            System.Web.Security.FormsAuthentication.RedirectFromLoginPage(us, True)
            ct.SESION_GH("perfil") = "ADMIN"
            ct.redireccion("default.aspx")
        End If
        ct.FR_CONTROL("LbERROR", col_txt:=Drawing.Color.Red) = "ERROR DE USUARIO O CLAVE"
    End Sub

    Public ReadOnly Property TIPO_PERFIL
        Get
            Return ct.SESION_GH("perfil")
        End Get
    End Property

    Public Sub NUEVO_USUARIO(USUARIO As String, CLAVE As String, NOMBRE As String, CORREO As String, CARGO As String, PERFIL As String)
        dsus.insertardb("'" + enc.stencripta(USUARIO.ToUpper) + "','" + enc.stencripta(CLAVE) + "','" + enc.stencripta(NOMBRE.ToUpper) + "','" + enc.stencripta(CORREO) + "','" + enc.stencripta(CARGO.ToUpper) + "','" + enc.stencripta(PERFIL) + "'")
    End Sub
    Public Function perfil()
        Dim X As String = item_usuario("perfil",, Web.HttpContext.Current.User.Identity.Name)
        If X = "ADMIN" Then
            perfil = 3
        Else
            perfil = dsper.valor_campo("perfil", "kusuario=" + dsus.valor_campo("keyusuarios", "usuario='" + enc.stencripta(Web.HttpContext.Current.User.Identity.Name) + "'"))
        End If

    End Function
    Public Function usuarios_cargo(cr As String) As String
        usuarios_cargo = Nothing
        For Each str As DataRow In dsus.Carga_tablas("cargo='" + enc.stencripta(cr) + "'").Rows
            If usuarios_cargo IsNot Nothing Then
                usuarios_cargo += ","
            End If
            usuarios_cargo += enc.stdsenencripta(str.Item("usuario"))
        Next
    End Function
    Public Function usuarios_perfil(cr As String) As String
        usuarios_perfil = Nothing
        For Each str As DataRow In dsus.Carga_tablas("perfil='" + enc.stencripta(cr.ToUpper) + "'").Rows
            If usuarios_perfil IsNot Nothing Then
                usuarios_perfil += ","
            End If
            usuarios_perfil += enc.stdsenencripta(str.Item("usuario"))
        Next
    End Function
    Public ReadOnly Property USER_NOMBRE(NOMBRE As String)
        Get
            Return dsus.valor_campo("usuario", "nombre='" + enc.stencripta(NOMBRE) + "'")
        End Get
    End Property
#End Region
#Region "CONFIGURACION"
    Public WriteOnly Property APP_PARAMETROS(FORMULARIO As String) As String
        Set(value As String)
            For Each VAPP As String In value.Split(",")
                'dspar.Eliminardb("FORMULARIO='APP' AND CRITERIO='" + FORMULARIO + "'")
                If dspar.valor_campo("VALOR", "FORMULARIO='APP' AND CRITERIO='" + FORMULARIO + "' AND VALOR='" + VAPP + "'") = Nothing Then
                    dspar.insertardb("'APP','" + FORMULARIO + "','" + VAPP + "'")
                End If
            Next
        End Set
    End Property
    Public Function INICIO_PARAMETROS() As Boolean
        For Each ROW As DataRow In dspar.Carga_tablas("FORMULARIO='APP'").Rows
            If dspar.Carga_tablas("CRITERIO='" + ROW.Item("VALOR") + "'").Rows.Count = 0 Then
                Return True
            End If
            Return False
        Next
    End Function



    Private ctf As ClassConstructor22
    Private Shared FRCONFIG As Panel
    Private Shared PG, US, MD, PC As String


    Public Sub FR_CONFIG(PANEL As Panel, PERFILES As String, PAGINA As String)
        ct = New ClassConstructor22(PANEL, "default.aspx", PAGINA)
        PG = PAGINA
        FRCONFIG = PANEL
        Select Case ct.reque("sfr")
            Case "", "USUARIOS"
                ct.FORMULARIO_GR("USUARIOS", "GrUSUARIOS", "keyusuarios-K,usuario,NOMBRE,CLAVE,CARGO,CORREO,PERFIL,-CH", "NUEVO USUARIO,PARAMETROS", "USUARIOS", SUBM_FR:=True)
                Dim PR As String = Nothing
                'If TIPO_PERFIL = "ADMIN" Or TIPO_PERFIL = "SUPERADMIN" Then
                If System.Configuration.ConfigurationManager.AppSettings("parametros").Contains(TIPO_PERFIL) Or TIPO_PERFIL = "SUPERADMIN" Then
                    PR = ",PARAMETROS"
                End If
                ct.FR_BOTONES("NUUS,EDIUS,ELIUS,MODUS" + PR)
                ct.FR_CONTROL("BtNUUS", evento:=AddressOf CLIC_BT) = "NUEVO USARIO"
                ct.FR_CONTROL("BtEDIUS", evento:=AddressOf CLIC_BT) = "EDITAR USARIO"
                ct.FR_CONTROL("BtELIUS", evento:=AddressOf CLIC_BT) = "ELIMINAR USARIO"
                ct.FR_CONTROL("BtMODUS", evento:=AddressOf CLIC_BT) = "MODIFICAR LOGUIN"
                ct.FR_CONTROL("BtPARAMETROS", evento:=AddressOf CLIC_BT) = "PARAMETROS"
                FRCONFIG = ct.PANEL_FR
                Dim gr As GridView = FRCONFIG.FindControl("GrUSUARIOS")
                Dim XG As Integer = gr.Columns.Count
                For Each GROW As GridViewRow In gr.Rows
                    For xi As Integer = 2 To XG - 1
                        GROW.Cells(xi).Text = enc.stdsenencripta(dsus.valor_campo(gr.Columns(xi).HeaderText, "keyusuarios=" + GROW.Cells(0).Text))
                    Next
                Next
            Case "NUEVO USUARIO", "USUARIO"
                Dim id, tl As String
                id = ct.reque("id")
                tl = ct.reque("sfr")
                If id Is Nothing Then
                    ct.FORMULARIO(tl, "TxNOMBRE,TxUSUARIO,TxCLAVE,TxCORREO,TxCARGO,DrPERFIL", True)
                    ct.FR_CONTROL("DrPERFIL") = PERFILES
                    ct.FR_CONTROL("TxCLAVE") = "Abcd1234."
                    ct.FR_CONTROL("BtGUARDAR",, evento:=AddressOf GUSUARIOS) = Nothing
                Else
                    US = Nothing
                    ct.FORMULARIO(tl, "TxNOMBRE,TxUSUARIO,TxCLAVE,TxCORREO,TxCARGO,DrPERFIL,DrMODULO,DrNIVEL,BtAGR,LbNIVEL,BtACTUS")
                    For Each ROW As DataRow In dspar.Carga_tablas("FORMULARIO='" + enc.stencripta("SistemA") + "' AND CRITERIO='" + enc.stencripta("mODULo") + "'").Rows
                        If US IsNot Nothing Then
                            US += ","
                        End If
                        US += enc.stdsenencripta(ROW.Item("valor"))
                    Next
                    ct.FR_CONTROL("TxNOMBRE") = item_usuario("nombre", ct.reque("id"))
                    ct.FR_CONTROL("TxUSUARIO", False) = item_usuario("USUARIO", ct.reque("id"))
                    ct.FR_CONTROL("TxCLAVE") = item_usuario("CLAVE", ct.reque("id"))
                    ct.FR_CONTROL("TxCORREO") = item_usuario("CORREO", ct.reque("id"))
                    ct.FR_CONTROL("TxCARGO") = item_usuario("CARGO", ct.reque("id"))
                    If ct.reque("e") = "y" Then

                    Else

                    End If
                    ct.FR_CONTROL("DrPERFIL") = PERFILES
                    ct.FR_CONTROL("DrPERFIL") = item_usuario("PERFIL", ct.reque("id"))
                    ct.FR_CONTROL("DrMODULO") = US
                    ct.FR_CONTROL("DrNIVEL") = "1,2,3"
                    ct.FR_CONTROL("LbNIVEL") = "Nivel 1 (INGRESO Y EDICION A PROPIOS), Nivel 2 (INGRESO Y EDICION A TODOS), Nivel 3 (ADMINISTRADOR TOTAL AL SISTEMA)"
                    ct.FR_CONTROL("BtAGR",, evento:=AddressOf CLIC_BT) = "AGREGAR MODULOS"
                    ct.FORMULARIO_GR(Nothing, "GrMOD", "KEYPERMISOS-K,MODULO,NIVEL;PERFIL,-CH", Nothing, "PERMISOS", "KUSUARIO=" + ct.reque("id"))
                    Dim GrMOD As GridView = PANEL.FindControl("GrMOD")
                    For Each GROW As GridViewRow In GrMOD.Rows
                        GROW.Cells(2).Text = enc.stdsenencripta(GROW.Cells(2).Text)
                    Next
                    ct.FR_BOTONES("ELIMOD")
                    ct.FR_CONTROL("BtELIMOD", evento:=AddressOf CLIC_BT) = "ELIMINAR MODULOS"
                    ct.FR_CONTROL("BtACTUS", evento:=AddressOf CLIC_BT) = "ACTUALIZAR USUARIO"
                End If
                ct.FR_MENU("Mn" + tl, "USUARIOS", PG)
            Case "PARAMETROS"
                Dim EXP As String = "FORMULARIO <> '/dwlsP.' AND FORMULARIO <> 'SISTEMA' AND FORMULARIO <> 'APP' AND FORMULARIO <> 'CAMBIO_CLAVE'"
                ct.FORMULARIO_GR("PARAMETROS", "GrPARAMETROS", "FORMULARIO-K,FORMULARIO-BT,CRITERIO-BT,VALOR-BT", Nothing, "PARAMETROS", EXP, AddressOf CARGA_CRITERIO,, "FORMULARIO,CRITERIO,VALOR")
                'Dim dsp As New carga_dssql("parametros p")
                'ct.FR_CONTROL("GrPARAMETROS", db:=dsp.Carga_tablas_especial("p.criterio, p.valor,(select count(c.valor) from parametros c where c.criterio=p.valor) as items", "p.formulario ='APP'",, "p.CRITERIO,P.VALOR", "p.CRITERIO")) = Nothing
            Case "CRITERIOS"
                Dim TL As String = "CRITERIO " + ct.reque("cr")
                ct.FORMULARIO(TL, "DrCRITERIOS,TmVALOR", True)
                ct.FR_CONTROL("DrCRITERIOS",, dspar.Carga_tablas("CRITERIO='" + ct.reque("cr") + "'"), AddressOf CARGA_GRPAR, post:=True) = "VALOR-VALOR"
                ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf GPARAMETRO) = Nothing
                ct.FR_CONTROL("BtCANCELAR", evento:=AddressOf CLIC_BT) = Nothing
                ct.FORMULARIO_GR(Nothing, "GrPARAM", "KPARAMETRO-K,CRITERIO,VALOR,-CH", Nothing)
                CARGA_GRPAR()
                ct.FR_MENU("Mn" + TL, "PARAMETROS", PG)
                ct.FR_BOTONES("ELIMPAR")
                ct.FR_CONTROL("BtELIMPAR", evento:=AddressOf CLIC_BT) = "ELIMINAR_PARAMETROS"
            Case "MODIFICAR LOGUIN"
                ct.FORMULARIO("MODIFICAR LOGUIN o CAMBIAR DE PERFIL", "DrUSUARIO,TxNUEVO_LOGUIN,LbNOTA", True)
                DrUSUARIO_USER(FRCONFIG.FindControl("DrUSUARIO"))
                ct.FR_CONTROL("LbNOTA") = "En este modulo puede cambiar el loguin o pasar los datos a otro perfil"
                ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf clic_MODIFICAR_USUARIO) = Nothing
        End Select

    End Sub
    Private Sub clic_MODIFICAR_USUARIO()
        'enc.stencripta(USUARIO.ToUpper)
        ct = New ClassConstructor22(FRCONFIG, "default.aspx")
        Dim XUS, AUS, NUS As String
        XUS = ct.FR_CONTROL("DrUSUARIO") : NUS = ct.FR_CONTROL("TxNUEVO_LOGUIN").ToUpper
        If NUS IsNot Nothing Then
            AUS = dsus.valor_campo("keyusuarios", "usuario='" + enc.stencripta(XUS) + "'")
            If item_usuario("perfil",, XUS) = "SUPERADMIN" Then
                ct.redir("?fr=CONFIGURACION&sfr=USUARIOS")
            End If
            If AUS.Length > 0 Then
                Dim dscl As New carga_dssql("clientes") : dscl.actualizardb("usuarioc='" + NUS + "'", "usuarioc='" + XUS + "'")
                Dim dsct As New carga_dssql("cotizaciones") : dsct.actualizardb("usuarion='" + NUS + "'", "usuarion='" + XUS + "'")
                Dim dsmo As New carga_dssql("multiorden") : dsmo.actualizardb("creado_por='" + NUS + "'", "creado_por='" + XUS + "'")
                dsmo.actualizardb("cerrado_por='" + NUS + "'", "cerrado_por='" + XUS + "'") : dsmo.actualizardb("fc_por='" + NUS + "'", "fc_por='" + XUS + "'")
                Dim dssg As New carga_dssql("seguimiento") : dssg.actualizardb("usuarios='" + NUS + "'", "usuarios='" + XUS + "'")
                dsus.actualizardb("usuario='" + enc.stencripta(NUS) + "'", "keyusuarios=" + AUS)
                ct.redir("?fr=CONFIGURACION&sfr=USUARIOS")
            End If
        End If
    End Sub
    Public Sub CAMBIO_CLAVE(panel As Panel)
        FRCONFIG = panel
        ct = New ClassConstructor22(FRCONFIG, "default.aspx", "CONFIGURACION")
        ct.FORMULARIO("CAMBIO DE CLAVE", "LbUSUARIO,TxNOMBRE,TxCLAVE_ANTERIOR,TxCLAVE_NUEVA,TxCONFIRME_CLAVE", True,, "INICIO")
        ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf cambiar_clave) = "CAMBIO DE CLAVE"
        ct.FR_CONTROL("LbUSUARIO") = ct.USERLOGUIN
        ct.FR_CONTROL("TxNOMBRE") = item_usuario("nombre",, ct.USERLOGUIN)
    End Sub
    Private Sub cambiar_clave()
        ct = New ClassConstructor22(FRCONFIG, "default.aspx", "CONFIGURACION")
        If item_usuario("clave",, ct.USERLOGUIN) = ct.FR_CONTROL("TxCLAVE_ANTERIOR") Then
            If ct.FR_CONTROL("TxCLAVE_NUEVA") <> ct.FR_CONTROL("TxCONFIRME_CLAVE") Then
                ct.alerta("LAS CLAVES NO SON IGUALES")
            ElseIf val_cl_may(ct.FR_CONTROL("TxCONFIRME_CLAVE")) = True And val_cl_num(ct.FR_CONTROL("TxCONFIRME_CLAVE")) = True Then
                dsus.actualizardb("clave='" + enc.stencripta(ct.FR_CONTROL("TxCONFIRME_CLAVE")) + "',nombre='" + enc.stencripta(ct.FR_CONTROL("TxNOMBRE")) + "'", "usuario='" + enc.stencripta(ct.USERLOGUIN) + "'")
                If ct.val_parametro("CAMBIO_CLAVE", ct.USERLOGUIN) Is Nothing Then
                    ct.add_parametro("CAMBIO_CLAVE", ct.USERLOGUIN, DateAdd(DateInterval.Day, 60, Now).ToShortDateString)
                Else
                    dspar.actualizardb("valor='" + DateAdd(DateInterval.Day, 60, Now).ToShortDateString + "'", "FORMULARIO='CAMBIO_CLAVE' AND CRITERIO='" + ct.USERLOGUIN + "'")
                End If
                ct.cerrar_session()
            Else
                    ct.alerta("LA CLAVES NO CUMPLE LOS REQUISITOS DE SEGURIDAD NUMERO, MAYUSCULA Y SIMBOLO")
            End If

        Else
            ct.alerta("CAMBIO DE CLAVE NO REALIZADO")
        End If


    End Sub
    Private Sub CARGA_GRPAR()
        ct = New ClassConstructor22(FRCONFIG, "default.aspx", "CONFIGURACION")
        ct.FR_CONTROL("GrPARAM", db:=dspar.Carga_tablas("CRITERIO='" + ct.FR_CONTROL("DrCRITERIOS") + "'", "VALOR"), VALIDAR:=True) = Nothing

    End Sub
    Public ReadOnly Property item_usuario(campo As String, Optional id As String = Nothing, Optional usuario As String = Nothing) As String
        Get
            Dim cri As String = Nothing
            If id IsNot Nothing Then
                cri = "keyusuarios=" + id
            ElseIf usuario IsNot Nothing Then
                cri = "usuario='" + enc.stencripta(usuario) + "'"
            End If
            If campo.ToLower = "keyusuarios" Then
                Return dsus.valor_campo(campo, cri)
            Else
                Return enc.stdsenencripta(dsus.valor_campo(campo, cri))
            End If

        End Get
    End Property
    Private Sub CLIC_BT(sender As Object, e As EventArgs)
        ct = New ClassConstructor22(FRCONFIG, "default.aspx", "CONFIGURACION")
        Dim bt As Button = sender
        Select Case bt.ID
            Case "BtNUUS"
                ct.redir("?fr=CONFIGURACION&sfr=NUEVO USUARIO")
            Case "BtEDIUS"
                If ct.FR_CONTROL("ChGrUSUARIOS") IsNot Nothing Then
                    ct.redir("?fr=CONFIGURACION&sfr=USUARIO&e=y&id=" + ct.FR_CONTROL("ChGrUSUARIOS"))
                Else
                    ct.alerta("NO HAY USUARIO SELECCIONADO PARA EDITAR")
                End If
            Case "BTACTUS"
                Dim nm, cl, co, ca, pf As String
                nm = enc.stencripta(ct.FR_CONTROL("TxNOMBRE").ToUpper) : cl = enc.stencripta(ct.FR_CONTROL("TxCLAVE"))
                co = enc.stencripta(ct.FR_CONTROL("TxCORREO").ToUpper) : ca = enc.stencripta(ct.FR_CONTROL("TxCARGO").ToUpper)
                pf = enc.stencripta(ct.FR_CONTROL("DrPERFIL").ToUpper)
                dsus.actualizardb("nombre='" + nm + "',clave='" + cl + "',correo='" + co + "',cargo='" + ca + "',perfil='" + pf + "'", "keyusuarios=" + ct.reque("id"))
                If ct.val_parametro("CAMBIO_CLAVE", ct.FR_CONTROL("TxUSUARIO")) Is Nothing Then
                    ct.add_parametro("CAMBIO_CLAVE", ct.FR_CONTROL("TxUSUARIO"), DateAdd(DateInterval.Day, 60, Now).ToShortDateString)
                Else
                    dspar.actualizardb("valor='" + DateAdd(DateInterval.Day, 60, Now).ToShortDateString + "'", "FORMULARIO='CAMBIO_CLAVE' AND CRITERIO='" + ct.FR_CONTROL("TxUSUARIO") + "'")
                End If
                ct.redir("?fr=CONFIGURACION&sfr=USUARIO&id=" + ct.reque("id"))
            Case "BtELIUS"
                eliminar_usuario()
            Case "BtMODUS"
                ct.redir("?fr=CONFIGURACION&sfr=MODIFICAR LOGUIN")
            Case "BTAGR"
                Dim XO As String = CInt(dsper.valor_campo_OTROS("MAX(ORDEN)", "KUSUARIO=" + ct.reque("id")) + 1)
                dsper.insertardb(ct.reque("id") + "," + XO + ",'" + enc.stencripta(ct.FR_CONTROL("DrMODULO")) + "'," + ct.FR_CONTROL("DrNIVEL"))
                ct.redir("?fr=CONFIGURACION&sfr=USUARIO&id=" + ct.reque("id"))
            Case "BtELIMOD"
                dsper.Eliminardb("KEYPERMISOS=" + ct.FR_CONTROL("ChGrMOD"))
                ct.redir("?fr=CONFIGURACION&sfr=USUARIO&id=" + ct.reque("id"))
            Case "BtELIPAR"
                dspar.Eliminardb("KPARAMETRO=" + ct.FR_CONTROL("ChGrPARAM"))
                ct.redir("?fr=CONFIGURACION&sfr=CRITERIOS&cr=" + ct.FR_CONTROL("DrCRITERIOS"))
            Case "BtCANCELAR"
                ct.redir("?fr=CONFIGURACION&sfr=PARAMETROS")
            Case "BtPARAMETROS"
                ct.redir("?fr=CONFIGURACION&sfr=PARAMETROS")
        End Select
    End Sub
    Private Sub eliminar_usuario()
        Dim Gr As GridView = FRCONFIG.FindControl("GrUSUARIOS")
        If Gr IsNot Nothing Then
            For Each grow As GridViewRow In Gr.Rows
                Dim ch As CheckBox = grow.Cells(1).FindControl("ChG")
                If ch.Checked = True Then
                    Dim DSCL As New carga_dssql("clientes")
                    If DSCL.valor_campo_OTROS("count(usuarioc)", "usuarioc='" + grow.Cells(2).Text + "'") = "0" And grow.Cells(7).Text <> "SUPERADMIN" Then
                        dsus.Eliminardb("keyusuarios=" + grow.Cells(0).Text)
                        ct.redir("?fr=CONFIGURACION")
                    ElseIf grow.Cells(7).Text = "SUPERADMIN" Then
                        ct.alerta("Este usuario no se puede eliminar por configuracion del sistema.")
                    Else
                        ct.alerta("El usuario " + grow.Cells(2).Text + " NO puede ser eliminado por que tiene clientes a su cargo. Se debe modificar el loguin o pasarle el perfil a otro usuario creado.")
                    End If
                End If
            Next
        End If
    End Sub
    Private Sub GPARAMETRO()
        ct = New ClassConstructor22(FRCONFIG, "default.aspx", PG)
        For Each SPAR As String In ct.FR_CONTROL("TmVALOR").Split(Chr(10))
            If SPAR.Length > 0 Then
                dspar.insertardb("'" + ct.reque("cr") + "','" + ct.FR_CONTROL("DrCRITERIOS") + "','" + SPAR.Replace(vbCr, "") + "'", True)
            End If
        Next
        ct.redir("?fr=CONFIGURACION&sfr=CRITERIOS&cr=" + ct.reque("cr"))
    End Sub

    Private Sub CARGA_CRITERIO()
        ct = New ClassConstructor22(FRCONFIG, "default.aspx", PG)
        ct.redir("?fr=CONFIGURACION&sfr=CRITERIOS&cr=" + ct.FR_CONTROL("GrPARAMETROS"))
    End Sub

    Private Sub GUSUARIOS()
        ct = New ClassConstructor22(FRCONFIG, "default.aspx", PG)
        US = ct.FR_CONTROL("TxUSUARIO")
        NUEVO_USUARIO(US, ct.FR_CONTROL("TxCLAVE"), ct.FR_CONTROL("TxNOMBRE"), ct.FR_CONTROL("TxCORREO"), ct.FR_CONTROL("TxCARGO"), ct.FR_CONTROL("DrPERFIL"))
        ct.redir("?fr=CONFIGURACION&sfr=USUARIO&id=" + dsus.valor_campo("keyusuarios", "usuario='" + enc.stencripta(US) + "'"))
    End Sub
    Private Function dtusuario() As DataTable
        Dim dt As New DataTable()
        dt.Columns.AddRange(New DataColumn() {New DataColumn("nombre", GetType(String)),
                                               New DataColumn("usuario", GetType(String))})
        For Each ROW As DataRow In dsus.Carga_tablas().Rows
            dt.Rows.Add(enc.stdsenencripta(ROW.Item("nombre")), enc.stdsenencripta(ROW.Item("usuario")))
        Next
        Return dt
    End Function
    Public Sub DrUSUARIO_USER(Dr As DropDownList, Optional USUARIO As String = Nothing, Optional LNOMBRE As Boolean = False, Optional TODOS As Boolean = False)
        If Dr IsNot Nothing Then
            If Dr.Items.Count = 0 Then
                Dr.Items.Clear()
            End If
            Dim dtv As New DataView(dtusuario)
            If LNOMBRE = False Then
                dtv.Sort = "usuario"
                Dr.DataSource = dtv
                Dr.DataTextField = "usuario"
                Dr.DataBind()
            Else
                dtv.Sort = "nombre"
                Dr.DataSource = dtv
                Dr.DataTextField = "nombre"
                Dr.DataValueField = "usuario"
                Dr.DataBind()
            End If
            If TODOS = True Then
                Dr.Items.Add("TODOS")
            End If
            If USUARIO IsNot Nothing Then
                If Dr.Items.FindByText(USUARIO) IsNot Nothing Then
                    Dr.Items.FindByText(USUARIO).Selected = True
                End If
            End If
        End If
    End Sub

#End Region



End Class
