Imports Classcatalogoch

Public Class rllamada
    Inherits System.Web.UI.Page
    Private fr As ClassConstructor22
    Private dsctl As New carga_dssql("control_llamada")
    Private DSCL As New carga_dssql("clientes")
    Private Shadows kcl As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        dsctl.campostb = "kllamada-key,fecha_llamada-datetime,numero-bigint,usuario-varchar(50),hora_inicio-time(7),hora_fin-time(7),tiempot-int"
        fr = New ClassConstructor22(Panel1)
        Select Case fr.reque("fr")
            Case ""
                carga_tr()
            Case "ct"
                control_ll()
            Case "cl"
                carga_cl()
        End Select
    End Sub
    Private Sub carga_tr()
        fr.FORMULARIO_GR("TAREAS", "TAR", "NOMBRE,TELEFONO;KTELEFONO,FSEGUIMIENTO;FECHASCL,OBSCL", Nothing, "CLIENTES", "FECHASCL <='" + fr.HOY_FR + "' AND USUARIOC='" + fr.USERLOGUIN + "'")
    End Sub
    Private Sub carga_cl()
        fr.FORMULARIO_GR("CLIENTES", "CLI", "NOMBRE,TELEFONO;KTELEFONO,FSEGUIMIENTO;FECHASCL,OBSCL", Nothing, "CLIENTES", "USUARIOC='" + fr.USERLOGUIN + "'",,, "NOMBRE")
    End Sub
    Private Sub control_ll()
        If fr.reque("tl") Is Nothing Then
            fr.FORMULARIO_GR("CONTROL LLAMADAS", "CTRLL", "KLLAMADA-K,FECHA_LLAMADA-D,NUMERO-N", Nothing, "CONTROL_LLAMADA", "USUARIO='" + fr.USERLOGUIN + "' and FECHA_LLAMADA='" + Now.ToString("yyyy-MM-dd") + "'",,, "KLLAMADA DESC")
        Else
            kcl = DSCL.valor_campo("kcliente", "ktelefono=" + fr.reque("tl"))
            If fr.reque("es") = "INICIO" Then
                dsctl.insertardb("'" + Now.ToString("yyyy-MM-dd") + "'," + fr.reque("tl") + ",'" + fr.USERLOGUIN + "','" + Now.ToString("HH:mm:ss") + "','" + Now.ToString("HH:mm:ss") + "',0")
            ElseIf fr.reque("es") = "FIN" Then
                'dsctl.actualizardb("")
            End If

            If kcl Is Nothing Then
                fr.FORMULARIO("CLIENTE NUEVO", "LbTELEFONO,TxNOMBRE,TmOBSERVCACION", True)
                fr.FR_CONTROL("LbTELEFONO") = fr.reque("tl")
            Else
                fr.FORMULARIO(VAL_CLIENTE("ktelefono"), "LbNOMBRE,TmOBSERVCACION", True)
                fr.FR_CONTROL("LbNUMERO") = VAL_CLIENTE("ktelefono")
                fr.FR_CONTROL("LbNOMBRE") = VAL_CLIENTE("NOMBRE")
            End If
        End If
    End Sub
    Private Function VAL_CLIENTE(campo As String) As String
        Return DSCL.valor_campo(campo, "kcliente=" + kcl)
    End Function


End Class