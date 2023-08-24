Imports System.Web.UI.WebControls
Public Class ClassESTADISTICAS
    Private DSES As New carga_dssql("estadisticas")
    Private lg As New ClassLogin
    Private ct As ClassConstructor22
    Private Shared FR As Panel
    Private Shared cam, cr, fil, mn, KES As String
    Sub New(PANEL_FR As Panel)
        FR = PANEL_FR
        DSES.campostb = "kest-key,nombre-varchar(250),tipo-varchar(10),meta-varchar(20),permisos-int,tablas-varchar(250),campos-varchar(500),condicion-varchar(500),periodo-varchar(50),grupos-varchar(500),presentacion-varchar(50),estado-varchar(10)"
        carga_fr()
    End Sub
    Private Sub carga_fr()
        ct = New ClassConstructor22(FR, "default.aspx", "ESTADISTICOS")
        Select Case lg.perfil
            Case "3"
                fr_admin()
            Case "2", "1"

        End Select
    End Sub
#Region "ESTADISTICAS"
    Private Sub fr_admin()
        FR.Controls.Clear()

        Select Case ct.reque("fr")
            Case "ESTADISTICO"
                FR_ESTADISTICO()
            Case "ESTADISTICAS"
                ct.FORMULARIO_GR("ESTADISTICAS", "GrEST", "KEST-K,NOMBRE-BT,TIPO-BT,META-BT,PERMISOS-BT", "ESTADISTICAS,NUEVA ESTADISTICA", "ESTADISTICAS",, AddressOf SEL_GrEST)
            Case "ESTADISTICA"
                fr_estadistica()
            Case "CREACIONIN"
                If TB Is Nothing Then
                    TB = "" : CP = "" : cr = ""
                End If
                FR_INFORME()
        End Select
    End Sub
    Private Sub FR_INFORME()
        KES = ct.reque("id")
        cam = Nothing
        If DSES.valor_campo("TIPO", "KEST=" + KES) = "PANTALLA" Then
            cam = "DrTABLAS,DrCONDICION,DrCRITERIO,DrPRESENTACION,DrESTADO"
        Else
            cam = "DrTABLAS,DrCONDICION,DrCRITERIO,DrPRESENTACION.DrESTADO,TxALIAS"
        End If
        ct.FORMULARIO("INFORME", cam, False)
        ct.FR_CONTROL("DrTABLAS",, DSES.LISTA_TABLAS, AddressOf SEL_DR) = "TABLE_NAME-TABLE_NAME"
        ct.FR_CONTROL("DrCONDICION") = "SUMAR,CONTAR,AGRUPAR"
        ct.FR_CONTROL("DrCRITERIO") = "HOY,MES,AÑO,FECHA,USUARIO"
        ct.FR_CONTROL("DrPRESENTACION") = "ESPECIFICO,DETALLADO,PORCENTUAL"
        ct.FR_CONTROL("DrESTADO") = "CREACION,PUBLICADO"
        ct.FR_BOTONES("LI,GR")
        ct.FR_CONTROL("BtLI", evento:=AddressOf CLIC_BT) = "LIMPIAR INFORME"
        ct.FR_CONTROL("BtGR", evento:=AddressOf CLIC_BT) = "GUARDAR INFORME"
        CARGA_CAMPOSTB()
    End Sub

    Private Sub FR_ESTADISTICO()
        cr = Nothing : cam = Nothing
        Select Case lg.perfil
            Case "3"
                cr = "tipo='pantalla'"
                mn = "NUEVA_ESTADISTICA,ESTADISTICAS,INICIO"
        End Select
        For xc As Integer = 1 To DSES.Carga_tablas(cr).Rows.Count
            If cam IsNot Nothing Then
                cam += ","
            End If
            cam += "PnEST" + xc.ToString
        Next
        If cam Is Nothing Then
            cam = "LbPN=NO HAY ESTADISTICAS PARA MOSTRAR"
        End If
        ct.FORMULARIO("PANEL ESTADISTICO", cam,,, mn)
    End Sub
    Private Sub fr_estadistica()
        ct.FORMULARIO(ct.reque("fr"), "TxNOMBRE,DrTIPO,TnMETA,DrPERIODO,DrPERMISOS", True,, "ESTADISTICAS")
        KES = ct.reque("id")
        If KES Is Nothing Then
            ct.FR_CONTROL("DrTIPO") = "PANTALLA,INFORME"
            ct.FR_CONTROL("DrPERMISOS") = "ADMIN,OPERADOR,TODOS"
            ct.FR_CONTROL("DrPERIODO") = "DIARIO,MENSUAL,RANGO,TODOS"

            ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf CLIC_BT) = "SIGUIENTE"
        Else
            ct.FR_CONTROL("TxNOMBRE") = DSES.valor_campo("NOMBRE", "KEST=" + KES)
            ct.FR_CONTROL("DrTIPO") = DSES.valor_campo("TIPO", "KEST=" + KES)
            ct.FR_CONTROL("TnMETA") = DSES.valor_campo("META", "KEST=" + KES)
            ct.FR_CONTROL("DrPERMISOS") = DSES.valor_campo("PERMISOS", "KEST=" + KES)
            ct.FR_CONTROL("DrPERIODO") = DSES.valor_campo("PERIODO", "KEST=" + KES)
            ct.FR_CONTROL("DrCRITERIO") = "HOY,MES,AÑO,FECHA,USUARIO"
            ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf CLIC_BT) = "INFORME"
        End If


    End Sub
    Private Shared ES, NM, TP, MT, TB, CP, CN, PERI, PR, GR, TEMPC, CRT, PRES, EST As String

    Private Sub SEL_DR(SENDER As Object, E As EventArgs)
        Dim DR As DropDownList = SENDER
        Select Case DR.ID
            Case "DrTABLAS"
                ct.FR_CONTROL("DrCAMPOS") = Nothing
                CARGA_CAMPOSTB()
            Case "DrCAMPOS"
                TEMPC = ct.FR_CONTROL("DrCAMPOS")
        End Select
    End Sub
    Private Sub CARGA_CAMPOSTB()
        ct.FORMULARIO_GR(Nothing, "GrCAMPOS", "COLUMNA-K,COLUMNA-BT,TIPO,-CH", Nothing, evento:=AddressOf SEL_GrCAMPOS, dt_grid:=DSES.LISTA_COLUMNAS(ct.FR_CONTROL("DrTABLAS")), ancho:=50)
    End Sub
    Private Sub SEL_GrCAMPOS()
        TEMPC = ct.FR_CONTROL("GrCAMPOS")
        GR += "" : CP += ""
        If TB.Contains(ct.FR_CONTROL("DrTABLAS")) = False Then
            If TB.Length > 0 Then
                TB += ","
            End If
            TB += ct.FR_CONTROL("DrTABLAS")
        End If
        Dim TCP As String = Nothing
        Select Case ct.FR_CONTROL("DrCONDICION")
            Case "SUMAR"
                TCP = "SUM(" + TEMPC + ")"
            Case "CONTAR"
                TCP = "count(" + TEMPC + ")"
            Case "AGRUPAR"
                If GR.Length > 0 Then
                    GR += ","
                End If
                'TCP = TEMPC
                GR += TEMPC
            Case "INFORME"
                TCP = TEMPC
        End Select
        If TCP IsNot Nothing Then
            If CP.Contains(TCP) = False Then
                If CP.Length > 0 Then
                    CP += ","
                End If
                CP += TCP
            End If
        End If
        FR.Controls.Add(GrPANTALLA())

    End Sub
    Shared CP1 As String
    Private Function GrPANTALLA() As GridView
        CP1 = ""
        GrPANTALLA = New GridView
        GrPANTALLA.ID = DSES.valor_campo("NOMBRE", "KEST=" + KES).Replace(" ", "_")
        Dim dsest As New carga_dssql(TB)
        If GR.Length > 0 And CP.Length > 0 Then
            CP1 = GR + "," + CP
        ElseIf GR.Length > 0 And CP.Length = 0 Then
            CP1 = GR
        Else
            CP1 = CP
        End If
        GrPANTALLA.ShowHeader = False
        GrPANTALLA.DataSource = dsest.Carga_tablas_especial(CP1, CRT,, GR)
        GrPANTALLA.DataBind()
        GrPANTALLA.HorizontalAlign = HorizontalAlign.Center

    End Function

    Private Sub CLIC_BT(SENDER As Object, E As EventArgs)
        Dim BT As Button = SENDER
        Select Case BT.Text
            Case "SIGUIENTE"
                NM = ct.FR_CONTROL("TxNOMBRE") : TP = ct.FR_CONTROL("DrTIPO") : MT = ct.FR_CONTROL("TnMETA") : PERI = ct.FR_CONTROL("DrPERIODO")
                PRES = ct.FR_CONTROL("DrPRESENTACION") : EST = ct.FR_CONTROL("DrESTADO")
                Select Case ct.FR_CONTROL("DrPERMISOS")
                    Case "ADMIN"
                        PR = "3"
                    Case "OPERADOR"
                        PR = "2"
                    Case "TODOS"
                        PR = "1"
                End Select
                DSES.insertardb("'" + NM + "','" + TP + "','" + MT + "','" + TB + "','" + CP + "','" + CN + "'," + PR + ",'" + PERI + "',''")
                ct.redir("?fr=CREACIONIN&id=" + DSES.valor_campo_OTROS("MAX(KEST)", Nothing))
            Case "INFORME"
                ct.redir("?fr=CREACIONIN&id=" + KES)
            Case "LIMPIAR INFORME"
                TB = "" : CP = "" : GR = "" : CRT = ""
                GrPANTALLA()
            Case "GUARDAR INFORME"
                Dim _tcr, _tts As String
                _tts = ct.FR_CONTROL("DrCRITERIO")
                _tcr = ct.FR_CONTROL("ChGrCAMPOS")
                For Each row As DataRow In DSES.LISTA_COLUMNAS(TB).Select("columna='" + _tcr + "'")
                    Dim TIPOC As String = ""
                    Select Case row.Item("TIPO")
                        Case "varchar", "text"
                            CRT = _tcr + "=-" + _tts + "-"
                        Case "int", "bigint"
                            CRT = _tcr + "=" + _tts
                        Case "date"
                            Select Case _tts
                                Case "DIA"
                                    CRT = _tcr + "=-" + _tts + "-"
                                Case "MES"
                                    CRT = "month(" + _tcr + ")=" + _tts
                                Case "AÑO"
                                    CRT = "year(" + _tcr + ")=" + _tts
                            End Select
                    End Select
                Next
                DSES.actualizardb("tablas='" + TB + "',campos='" + CP1 + "',condicion='" + CRT + "',grupos='" + GR + "',presentacion='" + PRES + "',estado='" + EST + "'", "KEST=" + KES)
        End Select
    End Sub

    Private Sub SEL_GrEST()
        ct.redir("?fr=ESTADISTICA&id=" + ct.FR_CONTROL("GrEST"))
    End Sub
#End Region
#Region "RESULTADOS"
    Public Sub PANEL_USUARIO()
        Dim PnUS As New Panel
        PnUS.BackColor = Drawing.Color.DarkRed
        For Each ROW As DataRow In DSES.Carga_tablas("permisos <=" + lg.perfil + " and tipo='pantalla' and estado ='PUBLICADO'").Rows
            Dim LB As New Label : LB.ForeColor = Drawing.Color.White : LB.Text = ROW.Item("NOMBRE")
            PnUS.Controls.Add(LB)
            PnUS.Controls.Add(GrEUS(ROW.Item("tablas"), ROW.Item("campos"), ROW.Item("condicion"), ROW.Item("grupos")))
        Next
        FR.Controls.Add(PnUS)
    End Sub
    Private Function GrEUS(_tb As String, _cp As String, _cri As String, _gr As String) As GridView
        GrEUS = New GridView
        Dim tbp As New carga_dssql(_tb)
        If _cri.Contains("-USUARIO-") = True Then
            _cri = _cri.Replace("-USUARIO-", "'" + ct.USERLOGUIN + "'")
        ElseIf _cri.Contains("-DIA-") = True Then
            _cri = _cri.Replace("-DIA-", "'" + Now.ToString("yyyy-MM-dd") + "'")
        ElseIf _cri.Contains("MES") = True Then
            _cri = _cri.Replace("MES", Now.Month.ToString)
        ElseIf _cri.Contains("AÑO") = True Then
            _cri = _cri.Replace("AÑO", Now.Year.ToString)
        End If
        GrEUS.ShowHeader = False
        GrEUS.Width = Unit.Percentage(50)
        GrEUS.ForeColor = Drawing.Color.White
        GrEUS.DataSource = tbp.Carga_tablas_especial(_cp, _cri,, _gr)
        GrEUS.DataBind()
        For Each GROW As GridViewRow In GrEUS.Rows
            GROW.Cells(0).Width = Unit.Percentage(10)
            GROW.Cells(1).Width = Unit.Percentage(90)
            Dim LbU As New Label : LbU.Text = " - " : LbU.BackColor = Drawing.Color.YellowGreen : LbU.Width = Unit.Percentage(GROW.Cells(2).Text)
            Dim LbG As New Label : LbG.Text = " - " : LbG.BackColor = Drawing.Color.BlueViolet : LbG.Width = Unit.Percentage(100)
            GROW.Cells(1).Controls.Add(LbU)
            GROW.Cells(1).Controls.Add(LbG)
        Next

    End Function

#End Region

End Class
