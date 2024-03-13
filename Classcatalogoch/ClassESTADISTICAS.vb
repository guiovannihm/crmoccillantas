Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports System.Collections.Generic
Imports System.Linq
Imports System.IO


Public Class ClassESTADISTICAS
    Private DSES As New carga_dssql("estadisticas")
    Private lg As New ClassLogin
    Private ct As ClassConstructor22
    Private Shared FR As Panel
    Private Shared cam, cr, fil, mn, KES As String
    Private context As Web.HttpContext = Web.HttpContext.Current
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
            Case "ESTADISTICO", "ESTADISTICAS"
                ct.FORMULARIO_GR("ESTADISTICAS", "GrEST", "KEST-K,NOMBRE-BT", "ESTADISTICAS,NUEVA ESTADISTICA", "ESTADISTICAS",, AddressOf SEL_GrEST)
            Case "ESTADISTICA"
                fr_estadistica()
            Case "INFORME"
                FR_INFORME()
            Case "CREACIONIN"
                If TB Is Nothing Then
                    TB = "" : CP = "" : cr = ""
                End If
                FR_CREACION()
        End Select
    End Sub
    Private Sub FR_CREACION()
        KES = ct.reque("id")
        FR.Controls.Clear()
        If ct.reque("tb") Is Nothing Then
            cam = Nothing
            cam = "DrTABLAS,BtCAMPOS,LiTABLAS,BtDELCM,DrFILTRO,BtFILTRO,LiFILTRO"

            ct.FORMULARIO(DSES.valor_campo("NOMBRE", "KEST=" + KES), cam)
            ct.FR_CONTROL("DrTABLAS",, DSES.LISTA_VISTAS) = "TABLE_NAME-TABLE_NAME"
            ct.FR_CONTROL("DrFILTRO") = DSES.valor_campo("CAMPOS", "KEST=" + KES).Replace("-", ",")
            ct.FR_CONTROL("BtCAMPOS", evento:=AddressOf clic_btCR) = "AGREGAR TABLA"
            ct.FR_CONTROL("BtDELCM", evento:=AddressOf clic_btCR) = "ELIMINAR TABLA"
            ct.FR_CONTROL("BtFILTRO", evento:=AddressOf clic_btCR) = "AGREGAR FILTRO"
            ct.FR_CONTROL("LiTABLAS") = DSES.valor_campo("TABLAS", "KEST=" + KES).Replace("-", ",")
            ct.FR_CONTROL("LiFILTRO") = DSES.valor_campo("condicion", "KEST=" + KES).Replace("-", ",")
            ct.FR_BOTONES("ESTADISTICA") : ct.FR_CONTROL("BtESTADISTICA", evento:=AddressOf clic_btCR) = "VOLVER ESTADISTICA"
            'VER_INFORME()
        ElseIf ct.reque("tb") IsNot Nothing Then
            ct.FORMULARIO_GR("CAMPOS " + ct.reque("tb"), "GrCMP", "COLUMNA,-CH", Nothing, dt_grid:=DSES.LISTA_COLUMNAS(ct.reque("tb")))
            ct.FR_BOTONES("ACAMPOS")
            ct.FR_CONTROL("BtACAMPOS", evento:=AddressOf clic_btCR) = "AGREGAR CAMPOS"
            SEL_CAMPOS()

        End If

        'ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf clic_verinf) = "VER INFORME"
        'ct.FR_CONTROL("TmFROM") = DSES.valor_campo("TABLAS", "KEST=" + KES)
        'ct.FR_CONTROL("TmSELECT") = DSES.valor_campo("CAMPOS", "KEST=" + KES)
        'ct.FR_CONTROL("TmWHERE") = DSES.valor_campo("CONDICION", "KEST=" + KES)
        'ct.FR_CONTROL("TmGROUP") = DSES.valor_campo("GRUPOS", "KEST=" + KES)
        'VER_INFORME()
    End Sub
    Private Sub SEL_CAMPOS()
        Dim cmp As String = DSES.valor_campo("campos", "kest=" + ct.reque("id"))
        Dim GrC As GridView = FR.FindControl("GrCMP")

        For Each GROW As GridViewRow In GrC.Rows
            If cmp.Contains(ct.reque("tb") + "." + GROW.Cells(0).Text) Then
                Dim Ch As CheckBox = GROW.Cells(1).FindControl("ChG")
                Ch.Checked = True
            End If
        Next
    End Sub
    Private Sub clic_btCR(sender As Object, e As EventArgs)
        Dim bt As Button = sender
        Select Case bt.ID
            Case "BTCAMPOS"

                ct.redir("?fr=CREACIONIN&id=" + ct.reque("id") + "&tb=" + ct.FR_CONTROL("DrTABLAS"))
            Case "BtACAMPOS"
                Dim XCMP As Integer = 0
                Dim cmp As String = DSES.valor_campo("campos", "kest=" + ct.reque("id"))
                Dim GrC As GridView = FR.FindControl("GrCMP")
                XCMP = cmp.Length
                For Each GROW As GridViewRow In GrC.Rows
                    Dim Ch As CheckBox = GROW.Cells(1).FindControl("ChG")
                    If Ch.Checked = True Then
                        If cmp.Contains(ct.reque("tb") + "." + GROW.Cells(0).Text) = False Then
                            If cmp.Length > 0 Then
                                cmp += "-"
                            End If
                            cmp += ct.reque("tb") + "." + GROW.Cells(0).Text
                        End If
                    Else
                        If cmp.Contains(ct.reque("tb") + "." + GROW.Cells(0).Text) = True Then
                            If cmp.Length > 0 Then
                                cmp = cmp.Replace("-" + ct.reque("tb") + "." + GROW.Cells(0).Text, "")
                                If cmp.Contains(ct.reque("tb") + "." + GROW.Cells(0).Text) = True Then
                                    cmp = cmp.Replace(ct.reque("tb") + "." + GROW.Cells(0).Text + "-", "")
                                End If
                                If cmp.Contains(ct.reque("tb") + "." + GROW.Cells(0).Text) = True Then
                                    cmp = cmp.Replace(ct.reque("tb") + "." + GROW.Cells(0).Text, "")
                                End If
                            Else
                                cmp = cmp.Replace(ct.reque("tb") + "." + GROW.Cells(0).Text, "")
                            End If
                        End If
                    End If
                    If cmp.Contains("--") Then
                        cmp = cmp.Replace("--", "-")
                    End If
                Next
                If cmp.Length > XCMP Then
                    DSES.actualizardb("campos='" + cmp + "'", "kest=" + ct.reque("id"))
                    Dim tbs As String = DSES.valor_campo("tablas", "kest=" + ct.reque("id"))
                    If tbs.Contains(ct.reque("tb")) = False Then
                        If tbs.Length > 0 Then
                            tbs += "-"
                        End If
                        tbs += ct.reque("tb")
                    End If
                    DSES.actualizardb("tablas='" + tbs + "'", "kest=" + ct.reque("id"))
                End If

                ct.redir("?fr=CREACIONIN&id=" + ct.reque("id"))
            Case "BTDELCM"
                Dim TB As String = DSES.valor_campo("TABLAS", "kest=" + ct.reque("id"))
                Dim TBX As String = ct.FR_CONTROL("LiTABLAS")
                If TB.Contains("-" + TBX) Then
                    TB = TB.Replace("-" + TBX, "")
                ElseIf TB.Contains(TBX + "-") Then
                    TB = TB.Replace(TBX + "-", "")
                ElseIf TB.Contains(TBX) Then
                    TB = TB.Replace(TBX, "")
                End If
                DSES.actualizardb("TABLAS='" + TB + "'", "kest=" + ct.reque("id"))
                Dim CM As String = DSES.valor_campo("CAMPOS", "kest=" + ct.reque("id"))
                For Each str As String In CM.Split("-")
                    If str.Contains(TBX) Then
                        If CM.Contains("-" + str) Then
                            CM = CM.Replace("-" + str, "")
                        ElseIf CM.Contains(STR + "-") Then
                            CM = CM.Replace(str + "-", "")
                        ElseIf CM.Contains(STR) Then
                            CM = CM.Replace(str, "")
                        End If
                    End If
                Next
                DSES.actualizardb("CAMPOS='" + CM + "'", "kest=" + ct.reque("id"))
                ct.redir("?fr=CREACIONIN&id=" + ct.reque("id"))
            Case "BTFILTRO"
                Dim cd1 As String = DSES.valor_campo("CONDICION", "kest=" + ct.reque("id"))
                If cd1.Contains(ct.FR_CONTROL("DrFILTRO")) = False Then
                    If cd1.Length > 0 Then
                        cd1 += "-"
                    End If
                    cd1 += ct.FR_CONTROL("DrFILTRO")
                End If
                DSES.actualizardb("CONDICION='" + cd1 + "'", "kest=" + ct.reque("id"))
            Case "BtESTADISTICA"
                ct.redir("?fr=ESTADISTICA&id=" + ct.reque("id"))
            Case "BtVINFORME"

            Case "BtEXPORTAR"
                delimitado_data()
        End Select
    End Sub

    Private Sub clic_verinf()
        DSES.actualizardb("TABLAS='" + ct.FR_CONTROL("TmFROM") + "',CAMPOS='" + ct.FR_CONTROL("TmSELECT") + "',CONDICION='" + ct.FR_CONTROL("TmWHERE") + "',GRUPOS='" + ct.FR_CONTROL("TmGROUP") + "'", "KEST=" + KES)
        VER_INFORME()
    End Sub
    Private Sub VER_INFORME()
        Dim tb, cm, tbx(), cmx() As String
        tb = DSES.valor_campo("tablas", "kest=" + ct.reque("id")).Replace("-", ",")
        cm = DSES.valor_campo("campos", "kest=" + ct.reque("id")).Replace("-", ",")
        If tb.Contains(",") Then

        Else
            Dim CTR As String = ""
            For Each STR As String In val_estadistica("CONDICION").Split("-")

                If STR.Contains("fecha") Then
                    If ct.FR_CONTROL("TfDESDE").Length > 0 And ct.FR_CONTROL("TfHASTA").Length > 0 Then
                        If CTR.Length > 0 Then
                            CTR += " and "
                        End If
                        CTR += STR + " BETWEEN '" + ct.FR_CONTROL("TfDESDE") + "' AND '" + ct.FR_CONTROL("TfHASTA") + "'"
                    End If
                Else
                    If ct.FR_CONTROL("Dr" + STR.Split(".")(1).ToUpper) <> "TODOS" Then
                        If CTR.Length > 0 Then
                            CTR += " and "
                        End If
                        CTR += STR + "='" + ct.FR_CONTROL("Dr" + STR.Split(".")(1).ToUpper) + "'"
                    End If
                End If
            Next
            ct.FORMULARIO_GR(Nothing, "GrINF", cm.Replace(tb + ".", ""), Nothing, tb, CTR)
        End If
        'DSES.vistatb(, ))
    End Sub
    Private Sub VER_INFORME2()
        Dim CAMP, CAMPGR, COND, ESTADO As String
        CAMP = DSES.valor_campo("CAMPOS", "KEST=" + KES)
        ESTADO = DSES.valor_campo("ESTADO", "KEST=" + KES)
        If ESTADO = "CREACION" Then
            Select Case DSES.valor_campo("PERIODO", "KEST=" + KES)
                Case "DIARIO"
                    COND = DSES.valor_campo("CONDICION", "KEST=" + KES).Replace("HOY", Now.ToString("yyyy-MM-dd"))
                Case "MENSUAL"
                    COND = DSES.valor_campo("CONDICION", "KEST=" + KES).Replace("MES", Now.Month.ToString)
                Case "RANGO"

                Case "TODOS"
            End Select
        ElseIf ESTADO = "PUBLICADO" Then
            Select Case DSES.valor_campo("PERIODO", "KEST=" + KES)
                Case "DIARIO"
                    COND = DSES.valor_campo("CONDICION", "KEST=" + KES).Replace("HOY", Now.ToString("yyyy-MM-dd"))
                Case "MENSUAL"
                    COND = DSES.valor_campo("CONDICION", "KEST=" + KES).Replace("MES", Now.Month.ToString)
                Case "RANGO"

                Case "TODOS"
            End Select
        End If

        Dim DSINF As New carga_dssql(ct.FR_CONTROL("TmFROM"),, COND)
        If CAMP.Contains("AS") Then
            For Each SCAM As String In CAMP.Split(".")
                Dim SCAM1() As String = SCAM.Split(" AS ")
                If CAMPGR IsNot Nothing Then
                    CAMPGR += ","
                End If
                If SCAM1.Length = 1 Then
                    CAMPGR += SCAM1(0)
                Else
                    CAMPGR += SCAM1(2)
                End If
            Next
        Else
            For Each SCAM As String In CAMP.Split(".")
                If SCAM.Contains("(") = False Then
                    If CAMPGR IsNot Nothing Then
                        CAMPGR += ","
                    End If
                    CAMPGR += SCAM
                End If
            Next
        End If

        Dim TB As DataTable = DSINF.Carga_tablas_especial(CAMP.Replace(".", ","), Nothing,, DSES.valor_campo("GRUPOS", "KEST=" + KES))
        ct.FORMULARIO_GR(Nothing, "GrINFORME", CAMPGR, Nothing, Nothing,,,,, TB)

    End Sub

    Private Sub FR_INFORME()
        Dim CMP As String = ""
        If val_estadistica("CONDICION").Contains("fecha") Then
            CMP += "TfDESDE,TfHASTA"
        End If
        For Each STR As String In val_estadistica("CONDICION").Split("-")
            If STR.Contains("fecha") = False Then
                If CMP.Length > 0 Then
                    CMP += ","
                End If
                CMP += "Dr" + STR.Split(".")(1).ToUpper
            End If
        Next
        ct.FORMULARIO(val_estadistica("nombre"), CMP)
        For Each STR As String In val_estadistica("CONDICION").Split("-")
            If STR.Contains("fecha") = False Then
                Dim YTB, YCM As String
                YTB = STR.Split(".")(0).ToUpper : YCM = STR.Split(".")(1).ToUpper
                Dim DSFI As New carga_dssql(YTB)
                ct.FR_CONTROL("Dr" + YCM,, DSFI.Carga_tablas_especial(YCM, Nothing,, YCM), VAL_ADD:="TODOS") = YCM + "-" + YCM
            End If
        Next
        ct.FR_BOTONES("VER_INFORME,EXPORTAR,VOLVER_ESTADISTICA")
        ct.FR_CONTROL("BtVER_INFORME", evento:=AddressOf clic_bTINF) = Nothing
        ct.FR_CONTROL("BtEXPORTAR", evento:=AddressOf clic_bTINF) = Nothing
        ct.FR_CONTROL("BtVOLVER_ESTADISTICA", evento:=AddressOf clic_bTINF) = Nothing
    End Sub
    Private Sub clic_bTINF(sender As Object, e As EventArgs)
        Dim Bt As Button = sender
        Select Case Bt.ID
            Case "BtVER_INFORME"
                VER_INFORME()
            Case "BtEXPORTAR"
                delimitado_data()
            Case "BtVOLVER_ESTADISTICA"
                ct.redir("?fr=ESTADISTICA&id=" + ct.reque("id"))
        End Select
    End Sub

    Private Function val_estadistica(campo As String) As String
        Return DSES.valor_campo(campo, "kest=" + ct.reque("id"))
    End Function
    Private Sub FR_ESTADISTICO()
        cr = Nothing : cam = Nothing
        Select Case lg.perfil
            Case "3"
                'cr = "tipo='pantalla'"
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
        'ct.FORMULARIO("PANEL ESTADISTICO", cam,,, mn)

    End Sub
    Private Sub fr_estadistica()

        KES = ct.reque("id")
        If KES Is Nothing Then
            ct.FORMULARIO(ct.reque("fr"), "TxNOMBRE", True,, "ESTADISTICAS")
            'ct.FR_CONTROL("DrTIPO") = "PANTALLA,INFORME"
            'ct.FR_CONTROL("DrPERMISOS") = "ADMIN,OPERADOR,TODOS"
            'ct.FR_CONTROL("DrPERIODO") = "DIARIO,MENSUAL,RANGO,TODOS"
            ct.FR_CONTROL("BtGUARDAR", evento:=AddressOf CLIC_BT) = "SIGUIENTE"
        ElseIf DSES.valor_campo("estado", "kest=" + KES) = "PUBLICADO" Then
            VER_INFORME()
        Else
            ct.FORMULARIO(ct.reque("fr"), "TxNOMBRE",,, "ESTADISTICAS")
            ct.FR_BOTONES("INFORME,EDICION")
            ct.FR_CONTROL("TxNOMBRE") = DSES.valor_campo("NOMBRE", "KEST=" + KES)
            'ct.FR_CONTROL("DrTIPO") = DSES.valor_campo("TIPO", "KEST=" + KES)
            'ct.FR_CONTROL("TnMETA") = DSES.valor_campo("META", "KEST=" + KES)
            'ct.FR_CONTROL("DrPERMISOS") = DSES.valor_campo("PERMISOS", "KEST=" + KES)
            'ct.FR_CONTROL("DrPERIODO") = DSES.valor_campo("PERIODO", "KEST=" + KES)
            'ct.FR_CONTROL("DrCRITERIO") = "HOY,MES,AÑO,FECHA,USUARIO"
            ct.FR_CONTROL("BtINFORME", evento:=AddressOf CLIC_BT) = "INFORME"
            ct.FR_CONTROL("BtEDICION", evento:=AddressOf CLIC_BT) = "EDICION"
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
                NM = ct.FR_CONTROL("TxNOMBRE") : TP = "" : MT = "0" : PERI = ""
                PRES = "" : EST = ""
                Select Case ct.FR_CONTROL("DrPERMISOS")
                    Case "ADMIN"
                        PR = "3"
                    Case "OPERADOR"
                        PR = "2"
                    Case "TODOS"
                        PR = "1"
                End Select
                DSES.insertardb("'" + NM + "','','','','','',1,'','','',''", True)
                ct.redir("?fr=CREACIONIN&id=" + DSES.valor_campo_OTROS("MAX(KEST)", Nothing))
            Case "INFORME"
                ct.redir("?fr=INFORME&id=" + ct.reque("id"))
            Case "EDICION"
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
    Private Sub exportar_excel()
        Dim Grv_main As GridView = FR.FindControl("GrINF")
        context.Response.Clear()
        context.Response.Buffer = True
        context.Response.ClearContent()
        context.Response.ClearHeaders()
        context.Response.Charset = ""
        Dim FileName As String = "RepoToolMaint" + DateTime.Now + ".xls"
        Dim strwritter As StringWriter = New StringWriter
        Dim htmltextwrtter As HtmlTextWriter = New HtmlTextWriter(strwritter)
        context.Response.Cache.SetCacheability(Web.HttpCacheability.NoCache)
        context.Response.ContentType = "application/vnd.ms-excel"
        context.Response.AddHeader("Content-Disposition", "attachment:filename=" + FileName)
        Grv_main.GridLines = GridLines.Both
        Grv_main.HeaderStyle.Font.Bold = True
        Grv_main.RenderControl(htmltextwrtter)
        context.Response.Write(strwritter.ToString())
        context.Response.End()
    End Sub
    Public Sub delimitado_data()
        Try
            Dim archivo As String = context.Server.MapPath("~") + "\rep" + KES + ".xls"
            If File.Exists(archivo) Then
                File.Delete(archivo)
            End If
            'Dim ft As New StreamWriter(archivo, False)
            Dim Grv_main As GridView = FR.FindControl("GrINF")
            Dim tb As String = DSES.valor_campo("tablas", "kest=" + ct.reque("id"))
            Dim cm As String = DSES.valor_campo("campos", "kest=" + ct.reque("id")).Replace("-", ",")
            Dim dsrp As carga_dssql
            Dim dt As DataTable
            If tb.Contains(",") Then

            Else
                Dim CTR As String = ""
                For Each STR As String In val_estadistica("CONDICION").Split("-")

                    If STR.Contains("fecha") Then
                        If ct.FR_CONTROL("TfDESDE").Length > 0 And ct.FR_CONTROL("TfHASTA").Length > 0 Then
                            If CTR.Length > 0 Then
                                CTR += " and "
                            End If
                            CTR += STR + " BETWEEN '" + ct.FR_CONTROL("TfDESDE") + "' AND '" + ct.FR_CONTROL("TfHASTA") + "'"
                        End If
                    Else
                        If ct.FR_CONTROL("Dr" + STR.Split(".")(1).ToUpper) <> "TODOS" Then
                            If CTR.Length > 0 Then
                                CTR += " and "
                            End If
                            CTR += STR + "='" + ct.FR_CONTROL("Dr" + STR.Split(".")(1).ToUpper) + "'"
                        End If
                    End If
                Next
                dsrp = New carga_dssql(tb)
                dt = dsrp.Carga_tablas(CTR,, cm)
                dt.WriteXml(archivo)
                ct.rewrite("window.open('rep" + ct.reque("id") + ".xls')")
            End If

            Dim x As Integer : Dim ln As String

            'For Each grow As DataRow In dt.Rows
            '    x = dt.Columns.Count
            '    ln = ""
            '    For y As Integer = 0 To x - 1
            '        If ln.Length > 0 Then
            '            ln += ","
            '        End If
            '        ln += grow.Item(y).ToString.Replace("-", "").Replace(";", ".")
            '    Next
            '    ft.WriteLine(ln)

            'Next
            'ft.Close()
        Catch ex As Exception
            Dim es As String = ex.Message
            ct.alerta(es)
        End Try

    End Sub


    Public Sub delimitado_grilla()
        Try
            Dim archivo As String = context.Server.MapPath("~") + "\rep" + KES + ".csv"
            Dim ft As New StreamWriter(archivo, True)
            Dim Grv_main As GridView = FR.FindControl("GrINF")
            Dim x As Integer : Dim ln As String
            For Each grow As GridViewRow In Grv_main.Rows
                x = grow.Cells.Count
                ln = ""
                For y As Integer = 0 To x - 1
                    If ln.Length > 0 Then
                        ln += ","
                    End If
                    ln += grow.Cells(y).Text.Replace("-", "").Replace(";", ".")
                Next
                ft.WriteLine(ln)

            Next
            ft.Close()
        Catch ex As Exception
            Dim es As String = ex.Message
            ct.alerta(es)
        End Try

    End Sub
    Public Sub VerifyRenderingInServerForm(control As Control)

    End Sub



#End Region
#Region "RESULTADOS"
    Public Sub PANEL_USUARIO()
        Dim PnUS As New Panel
        PnUS.BackColor = Drawing.Color.DarkRed
        For Each ROW As DataRow In DSES.Carga_tablas("permisos <=" + lg.perfil + " and tipo='pantalla' and estado ='PUBLICADO'").Rows
            Dim LB As New Label : LB.ForeColor = Drawing.Color.White : LB.Text = ROW.Item("NOMBRE")
            PnUS.Controls.Add(LB)
            PnUS.Controls.Add(GrEUS(ROW.Item("tablas"), ROW.Item("campos"), ROW.Item("condicion"), ROW.Item("grupos"), ROW.Item("PRESENTACION")))
        Next
        FR.Controls.Add(PnUS)
    End Sub
    Private Function GrEUS(_tb As String, _cp As String, _cri As String, _gr As String, _EST As String) As GridView
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
        Select Case _EST
            Case "PORCENTAJE"
            Case "VALOR"
            Case ""
        End Select
        'Try
        '    For Each GROW As GridViewRow In GrEUS.Rows
        '        GROW.Cells(0).Width = Unit.Percentage(10)
        '        GROW.Cells(1).Width = Unit.Percentage(90)
        '        Dim LbU As New Label : Dim LbG As New Label
        '        Try
        '            LbU.Text = " - " : LbU.BackColor = Drawing.Color.YellowGreen : LbU.Width = Unit.Percentage(GROW.Cells(2).Text)
        '            LbG.Text = " - " : LbG.BackColor = Drawing.Color.BlueViolet : LbG.Width = Unit.Percentage(100)
        '        Catch ex As Exception

        '        End Try
        '        GROW.Cells(1).Controls.Add(LbU)
        '        GROW.Cells(1).Controls.Add(LbG)
        '    Next
        'Catch ex As Exception

        'End Try


    End Function

#End Region

End Class
