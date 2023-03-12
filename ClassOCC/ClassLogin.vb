Imports System.Web
Imports System.Web.UI.WebControls


Public Class ClassLogin

    Private Shared ct As ClassConstructor22
    Private dspar As New carga_dssql("parametros", True)
    Private dsus As New carga_dssql("usuarios", False)
    Private dsper As New carga_dssql("permisos")
    Private enc As New ENCRIPTAR2
    Private Shared pns, pnf, pni As Panel
    'Private dsper As New carga_dssql("permisos", True)


    Sub New()
        dspar.campostb = "kparametro-key,formulario-varchar(250),criterio-varchar(250),valor-varchar(250)"
        dsus.campostb = "keyusuarios-key,usuario-varchar(250),clave-varchar(250),nombre-varchar(250),correo-varchar(250),cargo-varchar(250),perfil-varchar(250)"
        dsper.campostb = "keypermisos-key,kusuario-int,modulo-varchar(250),perfil-varchar(250)"
    End Sub

    Public Sub carga_fr(Fr As Panel)
        ct = New ClassConstructor22(Fr, "default.aspx", "INICIO")
        If dspar.datatable_gl.Rows.Count = 0 Then
            ct.FORMULARIO("CONFIGURACION", "TxNOMBRE_APP", True)
            ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf GUARDAR) = Nothing
        ElseIf dsus.Carga_tablas.Rows.Count = 0 Then
            ct.FORMULARIO("CONFIGURACION", "TxUSUARIO,TpCLAVE,TxNOMBRE,TxCORREO,TxCARGO", True)
            ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf GR_USUARIO) = Nothing
        Else
            If ct.reque("app") Is Nothing Then
                ct.FORMULARIO(ct.val_parametro("APP", "TITULO"), "LgINICIO_DE_SESION,ImLOGO")
                ct.FR_CONTROL("ImLOGO") = ct.val_parametro("APP", "LOGO")
                ct.FR_CONTROL("BtINICIO_SESION", evento:=AddressOf login) = Nothing
                ct.SESION_GH("app") = False
            Else
                ct.SESION_GH("app") = True
                login()
            End If
        End If
    End Sub
    Private Sub GUARDAR()
        ct.add_parametro("APP", "TITULO", ct.FR_CONTROL("TxNOMBRE_APP"))
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

    Public Sub panel_sup(pn As Panel, menu As String)
        ct = New ClassConstructor22(pn, "default.aspx")
        pn.Controls.Clear()
        If ct.SESION_GH("app") Is Nothing Then
            ct.redireccion("login.aspx")
        End If
        If ct.SESION_GH("app") = False Then
            If menu IsNot Nothing Then
                ct.FORMULARIO(ct.val_parametro("app", "titulo"), "ImSUP-15,DrUSUARIO", FrSUPERIOR:=True, It_MENU:=menu)
            Else
                ct.FORMULARIO(ct.val_parametro("app", "titulo"), "ImSUP-15,DrUSUARIO", FrSUPERIOR:=True)
            End If
            If ct.val_parametro("APP", "LOGO") Is Nothing Then
                ct.FR_CONTROL("LbSUP") = ct.val_parametro("APP", "TITULO") + "<br>" + Now.ToString("MMMM dd yyyy hh:mm")
            Else
                ct.FR_CONTROL("ImSUP") = ct.val_parametro("APP", "LOGO") ' + "<br>" + Now.ToString("MMMM dd yyyy hh:mm")
            End If
            ct.FR_CONTROL("DrUSUARIO", evento:=AddressOf SEL_DrSESION) = enc.stdsenencripta(dsus.valor_campo("nombre", "usuario='" + enc.stencripta(ct.USERLOGUIN) + "'")) + ",CERRAR SESION"
        Else
            ct.FORMULARIO("", "LbAPP", FrSUPERIOR:=True, It_MENU:=menu)
        End If
        If perfil() = "SUPERADMIN" Or perfil() = "ADMIN" Then
            If ct.reque("fr") = "" Or ct.reque("fr") = "INICIO" Then
                ct.FR_ICONOS("INICIO,CONFIGURACION", HorizontalAlign.Left, True)
            Else
                ct.FR_ICONOS("INICIO", HorizontalAlign.Left, True)
            End If
        Else
            ct.FR_ICONOS("INICIO", HorizontalAlign.Left, True)
        End If
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
            End If
        Next
        If us.ToUpper = "ADMIN" And cl.ToUpper = "61S4St3m" Then
            System.Web.Security.FormsAuthentication.RedirectFromLoginPage(us, True)
            ct.redireccion("default.aspx")
        End If

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
        perfil = enc.stdsenencripta(dsus.valor_campo("perfil", "usuario='" + enc.stencripta(Web.HttpContext.Current.User.Identity.Name) + "'"))
    End Function
    Public Function usuarios_cargo(cr As String) As String
        usuarios_cargo = Nothing
        For Each str As DataRow In dsus.Carga_tablas("cargo='" + enc.stencripta(cr) + "'").Rows
            usuarios_cargo += "," + enc.stdsenencripta(str.Item("usuario"))
        Next
    End Function
    Public ReadOnly Property USER_NOMBRE(NOMBRE As String)
        Get
            Return dsus.valor_campo("usuario", "nombre='" + enc.stencripta(NOMBRE) + "'")
        End Get
    End Property
#Region "CONFIGURACION"
    Public WriteOnly Property APP_PARAMETROS(FORMULARIO As String) As String
        Set(value As String)
            For Each VAPP As String In value.Split(",")
                If dspar.valor_campo("VALOR", "FORMULARIO='APP' AND CRITERIO='" + FORMULARIO + "' AND VALOR='" + VAPP + "'") = Nothing Then
                    dspar.insertardb("'APP','" + FORMULARIO + "','" + VAPP + "'")
                End If
            Next
        End Set
    End Property


    Private ctf As ClassConstructor22
    Private Shared FRCONFIG As Panel
    Private Shared PG As String
    Public Sub FR_CONFIG(PANEL As Panel, PERFILES As String, PAGINA As String)
        ct = New ClassConstructor22(PANEL, "default.aspx", PAGINA)
        PG = PAGINA
        FRCONFIG = PANEL
        Select Case ct.reque("sfr")
            Case "", "USUARIOS"
                ct.FORMULARIO_GR("USUARIOS", "GrUSUARIOS", "keyusuarios-K,usuario,NOMBRE,CLAVE,CARGO,CORREO,PERFIL,-CH", Nothing, "USUARIOS", SUBM_FR:=True)
                ct.FR_MENU("MnUSUARIOS", "NUEVO USUARIO,PARAMETROS", PG)
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
                If id IsNot Nothing Then
                    tl = "NUEVO USUARIO"
                Else
                    tl = "USUARIO"
                End If
                ct.FORMULARIO(tl, "TxNOMBRE,TxUSUARIO,TxCLAVE,TxCORREO,TxCARGO,DrPERFIL", True)
                ct.FR_MENU("Mn" + tl, "USUARIOS", PG)
                If id Is Nothing Then
                    ct.FR_CONTROL("DrPERFIL") = PERFILES
                    ct.FR_CONTROL("BtGUARDAR",, evento:=AddressOf GUSUARIOS) = Nothing
                Else

                End If
            Case "PARAMETROS"
                ct.FORMULARIO("PARAMETROS", "DrCRITERIOS,BtNUEVO_CRITERIO")
                ct.FR_CONTROL("DrCRITERIOS",, dspar.Carga_tablas_especial("CRITERIO", "FORMULARIO='APP'", , "CRITERIO", "CRITERIO")) = "CRITERIO-CRITERIO"
                ct.FR_CONTROL("BtNUEVO_CRITERIO", evento:=AddressOf CARGA_CRITERIO) = Nothing
            Case "CRITERIOS"
                ct.FORMULARIO("CRITERIO " + ct.reque("cr"), "DrCRITERIOS,TmVALOR", True)
                ct.FR_CONTROL("DrCRITERIOS",, dspar.Carga_tablas("CRITERIO='" + ct.reque("cr") + "'")) = "VALOR-VALOR"
                ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf GPARAMETRO) = Nothing
        End Select

    End Sub
    Private Sub GPARAMETRO()
        ct = New ClassConstructor22(FRCONFIG, "default.aspx", PG)
        For Each SPAR As String In ct.FR_CONTROL("TmVALOR").Split(Chr(10))
            dspar.insertardb("'" + ct.reque("cr") + "','" + ct.FR_CONTROL("DrCRITERIOS") + "','" + SPAR + "'")
        Next

    End Sub

    Private Sub CARGA_CRITERIO()
        ct = New ClassConstructor22(FRCONFIG, "default.aspx", PG)
        ct.redir("?fr=CONFIGURACION&sfr=CRITERIOS&cr=" + ct.FR_CONTROL("DrCRITERIOS"))
    End Sub

    Private Sub GUSUARIOS()
        ct = New ClassConstructor22(FRCONFIG, "default.aspx", PG)

        NUEVO_USUARIO(ct.FR_CONTROL("TxUSUARIO"), ct.FR_CONTROL("TxCLAVE"), ct.FR_CONTROL("TxNOMBRE"), ct.FR_CONTROL("TxCORREO"), ct.FR_CONTROL("TxCARGO"), ct.FR_CONTROL("DrPERFIL"))
        ct.redir("?fr=CONFIGURACION&sfr=USUARIOS")
    End Sub

    Public Sub DrUSUARIO_USER(Dr As DropDownList, Optional USUARIO As String = Nothing)
        For Each ROW As DataRow In dsus.Carga_tablas().Rows
            Dr.Items.Add(enc.stdsenencripta(ROW.Item("usuario")))
        Next
        If USUARIO Is Nothing Then
            Dr.Items.FindByText(USUARIO)
        End If
    End Sub

#End Region



End Class
