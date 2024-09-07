Imports Classcatalogoch

Public Class ClassAPP
    Private fr As ClassConstructor22
    Private dsctl As New carga_dssql("control_llamada")
    Private DSCL As New carga_dssql("clientes")
    Private Shadows kcl As String

    Sub New(fpn As Panel)
        dsctl.campostb = "kllamada-key,fecha_llamada-datetime,numero-bigint,usuario-varchar(50),hora_inicio-time(7),hora_fin-time(7),tiempot-int"
        fr = New ClassConstructor22(fpn)
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
                Dim FI As String = Now.ToString("yyyy-MM-dd") + " " + dsctl.valor_campo("HORA_INICIO", "FECHA_LLAMADA='" + Now.ToString("yyyy-MM-dd") + "' AND NUMERO=" + fr.reque("tl") + " AND TIEMPOT=0")
                Dim X As Integer = DateDiff(DateInterval.Minute, CDate(FI), Now)
                dsctl.actualizardb("HORA_FIN='" + Now.ToString("HH:mm:ss") + "',TIEMPOT=" + X.ToString, "FECHA_LLAMADA='" + Now.ToString("yyyy-MM-dd") + "' AND NUMERO=" + fr.reque("tl") + " AND TIEMPOT=0")
            End If

            If kcl Is Nothing Then
                fr.FORMULARIO("CLIENTE NUEVO", "LbTELEFONO,TxNOMBRE,TmOBSERVACION", True)
                fr.FR_CONTROL("LbTELEFONO") = fr.reque("tl")
                fr.FR_CONTROL("BtGUARDAR", evento:=AddressOf crear_cl) = Nothing

            Else
                fr.FORMULARIO(VAL_CLIENTE("ktelefono"), "LbNOMBRE,TmOBSERVACION", True)
                fr.FR_CONTROL("LbNUMERO") = VAL_CLIENTE("ktelefono")
                fr.FR_CONTROL("LbNOMBRE") = VAL_CLIENTE("NOMBRE")
            End If
        End If
    End Sub
    Private Function VAL_CLIENTE(campo As String) As String
        Return DSCL.valor_campo(campo, "kcliente=" + kcl)
    End Function

    Private Sub crear_cl()
        Dim tl, nm, ob As String
        tl = fr.FR_CONTROL("LbTELEFONO") : nm = fr.FR_CONTROL("TxNOMBRE") : ob = fr.FR_CONTROL("TmOBSERVACION")
        DSCL.insertardb(tl + ",'" + nm + "','',0,'','ACTIVO','" + fr.USERLOGUIN + "','','',0,'','PROSPECTO','" + fr.HOY_FR + "','" + ob + "','','1900-01-01','1900-01-01','NO','" + fr.HOY_FR + "',''", True)
        fr.redir("fr=cl")
    End Sub
    Private Sub actua_cl()

    End Sub


End Class
