Imports Microsoft.VisualBasic
Imports System.Web.UI.WebControls
Imports System.Web.SessionState
Imports System.Data
Imports System.Net
Imports System.Web.UI
Imports System
Imports iTextSharp
Imports System.Configuration.ConfigurationManager
Imports System.Web.Security
Imports System.Drawing
Public Class ClassConstructor22

    'Private wcon As System.Web.Httpwcont = Web.Httpwcont.Current
    Private wcon As Web.HttpContext = Web.HttpContext.Current
    Private FR As Panel
    Private tb As New Table
    Private Shared _MN, id_ct, npg, filtros_gr, fil_db, campos_gr, criterio_gr, orden_gr, data_gr, gr_id, fil_dr(), FR_ATRAS As String
    Private urla2 As String = wcon.Request.Url.Segments(1).ToString
    Private tl, it_mn As String
    Private Shared evento_gr As EventHandler
    Dim pn1, Pn2 As New Panel
    Private size_fuente As Integer
    Private Shared COLOR_MN As Color
    Private Shared camtm As String
    Private Shared fr_consulta, fr_BtGUARDAR, FrSUP, MnFR, SUBMFR As Boolean
    Private gr As New GridView
    Private Shared dt_gr As DataTable
    Private Shared col_fr As Color = Color.DarkBlue

#Region "CONTROLES"
    Private _tx As TextBox
#End Region

    Private Function urla() As String
        Return wcon.Request.Url.Segments(wcon.Request.Url.Segments.Count - 1).ToString
    End Function
    Public Property SESION_GH(NOMBRE As String) As String
        Get
            Return wcon.Session(NOMBRE)
        End Get
        Set(value As String)
            wcon.Session(NOMBRE) = value
        End Set
    End Property

    Public Function reque(rq As String) As String
        Return wcon.Request.QueryString(rq)
    End Function
    Public Sub redir(complemento_url As String)
        wcon.Response.Redirect(urla() + complemento_url)
    End Sub
    Public Sub rewrite(script As String)
        wcon.Response.Write("<script>" + script + "</script>")
    End Sub
    Public Function formato_fecha_sql(fecha As Date) As String
        Return fecha.ToString(Configuration.ConfigurationManager.AppSettings(""))
    End Function

    Public ReadOnly Property PANEL_FR
        Get
            Return FR
        End Get
    End Property

    Public Sub tb_inicio(ITEMS As String, Optional PG_RETORNO As String = Nothing)
        If PG_RETORNO IsNot Nothing Then
            npg = PG_RETORNO
        End If
        FR.BackColor = Color.White
        Dim tbi As New Table
        Dim tbr As TableRow
        tbi.Width = Unit.Percentage(100)
        Dim X, y, Z As Integer
        If movil() = True Then
            y = 1
        Else
            y = 3
        End If
        For Each ST As String In ITEMS.Split(",")
            If X = 0 Then
                tbr = New TableRow
            End If
            If X <= y Then
                Dim tbc As New TableCell
                tbc.Width = Unit.Percentage(25)
                tbc.HorizontalAlign = HorizontalAlign.Center
                tbc.VerticalAlign = VerticalAlign.Middle
                Dim BTI As New Button
                BTI.Text = ST.ToUpper
                BTI.Width = Unit.Percentage(100)
                BTI.Height = Unit.Pixel(100)
                BTI.Font.Size = size_fuente
                BTI.BackColor = COLOR_INICIO(Z)
                BTI.CssClass = "cursorbt"
                AddHandler BTI.Click, AddressOf BTI_Click
                tbc.Controls.Add(BTI)
                tbr.Cells.Add(tbc)
                X += 1
                Z += 1
            End If
            If X = y Then
                X = 0
            End If
            If Z = 4 Then
                Z = 0
            End If
            tbi.Rows.Add(tbr)
        Next


        FR.Controls.Add(tbi)

    End Sub

    Protected Sub BTI_Click(sender As Object, e As EventArgs)
        Dim BTI As Button = sender
        COLOR_MN = BTI.BackColor
        wcon.Response.Redirect("~/" + urla + "?fr=" + BTI.Text)
    End Sub

    Private Function COLOR_INICIO(PS As Integer) As Color
        Select Case PS
            Case 0
                Return Color.LightBlue
            Case 1
                Return Color.PaleVioletRed
            Case 2
                Return Color.Yellow
            Case 3
                Return Color.LightGreen
        End Select
    End Function

    Sub New(Optional Fr_pagina As Panel = Nothing, Optional pg_retorno As String = Nothing, Optional pagina_atras As String = Nothing)
        npg = pg_retorno
        If Fr_pagina IsNot Nothing Then
            FR = Fr_pagina
            'FR.ScrollBars = ScrollBars.Vertical
        End If
        If pagina_atras Is Nothing Then
            FR_ATRAS = "INICIO"
        Else
            FR_ATRAS = pagina_atras
        End If

        'FR.BackColor = System.Drawing.Color.DarkBlue
    End Sub
    Public WriteOnly Property COLOR_FR As Drawing.Color
        Set(value As Color)
            col_fr = value
        End Set
    End Property

    Public Sub redireccion(url As String, Optional qs As String = Nothing)
        Dim _qs As String = Nothing
        If qs <> Nothing Then
            If qs.Contains("&") = True Then
                _qs = "?"
                For Each st As String In qs.Split("&")
                    _qs += st
                Next
            Else
                _qs = "?" + qs
            End If
        End If
        wcon.Response.Redirect(url + _qs)
    End Sub
    Public Function val_parametro(formulario As String, criterio As String) As String
        Try
            Dim dspar As New carga_dssql("parametros")
            For Each row As DataRow In dspar.Carga_tablas("formulario='" + formulario + "' and criterio='" + criterio + "'").Rows
                Return row.Item("valor")
            Next
        Catch ex As Exception

        End Try
        Return Nothing
    End Function


    Public Sub add_parametro(formulario As String, criterio As String, valor As String)
        Dim dspar As New carga_dssql("parametros")
        dspar.insertardb("'" + formulario + "','" + criterio + "','" + valor + "'", True)
    End Sub

    Public Sub FORMULARIO(titulo As String, campos As String, Optional BtGUARDAR As Boolean = False, Optional FrSUPERIOR As Boolean = False, Optional It_MENU As String = Nothing, Optional col_fr As Color = Nothing, Optional SUBFR_MENU As Boolean = False)
        fr_consulta = BtGUARDAR
        fr_BtGUARDAR = BtGUARDAR
        SUBMFR = SUBFR_MENU
        FrSUP = FrSUPERIOR
        val_control = False
        If FrSUP = True Then
            FR.BackColor = COLOR_MN
            it_mn = It_MENU
        ElseIf MnFR = True Then
            it_mn = It_MENU
        Else
            it_mn += "," + It_MENU
        End If


        If titulo IsNot Nothing Then
            tl = titulo.ToUpper
        Else
            tl = Nothing
        End If

        CARGA_FR(campos)
    End Sub
    Public Sub fr_nuevo_parametro()
        tl = "NUEVO " + reque("c").ToUpper
        fr_BtGUARDAR = True
        FrSUP = False
        it_mn = reque("f")
        CARGA_FR("LbFORMULARIO=" + reque("f") + ",LbCRITERIO=" + reque("c") + ",TxVALOR")
        FR_CONTROL("BtGUARDAR", evento:=AddressOf guardar_parametro) = Nothing
    End Sub
    Private Sub guardar_parametro()
        Dim dsp As New carga_dssql("parametros")
        dsp.insertardb("'" + reque("f") + "','" + reque("c") + "','" + FR_CONTROL("TxVALOR") + "'", True)
        redir("?fr=" + reque("f"))
    End Sub
    Public WriteOnly Property MENU_FR
        Set(value)
            MnFR = value
        End Set
    End Property

    Private Sub carga_menu(Optional sup As Boolean = False)
        movil()
        If it_mn IsNot Nothing Then
            Dim IdMN As String
            If sup = True Then
                IdMN = "MnPRINCIPAL"
            Else
                IdMN = "Mn" + tl
            End If
            FR.Controls.Add(MN(IdMN))
            Dim men As Menu
            Try
                men = FR.FindControl("Mn" + tl)
            Catch ex As Exception

            End Try
            If men IsNot Nothing Then
                men.Items.Clear()
                men.Orientation = Orientation.Horizontal
                men.Width = Unit.Percentage(100)
                men.StaticMenuItemStyle.ForeColor = Color.White
                men.StaticMenuItemStyle.Font.Size = FontUnit.Point(size_fuente)
                men.StaticMenuItemStyle.BorderColor = Color.White
                men.StaticMenuItemStyle.BorderWidth = Unit.Point(2)
                men.StaticMenuItemStyle.BackColor = Color.DarkBlue
                If men.Items.Count = 0 Then
                    For Each STR As String In it_mn.Split(",")
                        Dim MI As New MenuItem
                        MI.Text = STR.Replace("_", " ").ToUpper
                        men.Items.Add(MI)
                    Next
                End If
                AddHandler men.MenuItemClick, AddressOf clic_mn
            End If
        End If
    End Sub

    Public Sub CARGAD_GR(grid As GridView, dt_grid As DataTable)
        grid.DataSource = dt_grid
        grid.DataBind()
    End Sub
    Public Sub FR_BOTONES(NombreBT As String, Optional ALINEACION As HorizontalAlign = HorizontalAlign.Center, Optional SUPERIOR As Boolean = False)
        Dim PNB As New Panel
        PNB.HorizontalAlign = ALINEACION
        For Each NBT As String In NombreBT.Split(",")
            Dim CT As WebControl = FR.FindControl("Bt" + NBT)
            If CT Is Nothing Then
                PNB.Controls.Add(Bt(NBT))
            End If
        Next
        If SUPERIOR = False Then
            FR.Controls.Add(PNB)
        Else
            FR.Controls.AddAt(0, PNB)
        End If

    End Sub

    Public Sub FR_ICONOS(NombreBT As String, Optional ALINEACION As HorizontalAlign = HorizontalAlign.Center, Optional SUPERIOR As Boolean = False)
        Dim PNB As Panel = FR.FindControl("PNB")
        If PNB Is Nothing Then
            PNB = New Panel
        End If
        PNB.HorizontalAlign = ALINEACION
        For Each NBT As String In NombreBT.Split(",")
            Dim CT As WebControl = FR.FindControl("Bi" + NBT)
            If CT Is Nothing Then
                PNB.Controls.Add(Bi(NBT))
            End If
        Next
        If SUPERIOR = False Then
            FR.Controls.Add(PNB)
        Else
            FR.Controls.AddAt(0, PNB)
        End If

    End Sub

    Public Sub FORMULARIO_GR(Titulo As String, id As String, titulo_campos As String, Item_mn As String, Optional db As String = Nothing, Optional criterio As String = Nothing, Optional evento As EventHandler = Nothing, Optional filtros As String = Nothing, Optional orden As String = Nothing, Optional dt_grid As DataTable = Nothing, Optional SUBM_FR As Boolean = False)
        tl = Titulo
        SUBMFR = SUBM_FR
        data_gr = db
        Dim ds As New carga_dssql(db)
        dt_gr = dt_grid
        campos_gr = titulo_campos
        criterio_gr = criterio
        orden_gr = orden
        evento_gr = evento
        'FR.Height = Unit.Pixel(400)
        FR.ScrollBars = ScrollBars.Auto
        fil_db = Nothing
        If gr_id IsNot id And filtros Is Nothing Then
            gr.ID = id
        Else
            gr.ID = id
        End If
        'gr.BackColor = COLOR_MN
        gr.Width = Unit.Percentage(100)
        gr.Columns.Clear()
        If Titulo IsNot Nothing Then
            FR.Controls.Add(Ti("Ti" + id, Titulo.ToUpper + "<hr>"))
            it_mn += "," + Item_mn
            carga_menu()
        End If
        If db IsNot Nothing Then
            Try
                If filtros IsNot Nothing And ds.Carga_tablas(criterio).Rows.Count > 0 Then
                    filtros_gr = filtros.Replace("-T", "").Replace("-t", "").Replace("-Z", "").Replace("-z", "")
                    FR.Controls.Add(PNF(filtros, ds.Carga_tablas(criterio, filtros_gr, filtros_gr)))
                    If fil_db Is Nothing Then
                        sel_drfiltro()
                    ElseIf fil_dr IsNot Nothing Then
                        For x As Integer = 0 To fil_dr.Count
                            Dim fil() As String = filtros_gr.Split(",")
                            Dim dr As DropDownList = FR.FindControl("Dr" + fil(x))
                            dr.SelectedIndex = -1
                            dr.Items.FindByText(fil_dr(x)).Selected = True
                        Next
                    End If
                Else
                    filtros_gr = Nothing
                End If

            Catch ex As Exception

            End Try
        ElseIf dt_grid IsNot Nothing And filtros IsNot Nothing Then
            filtros_gr = filtros.Replace("-T", "").Replace("-t", "").Replace("-Z", "").Replace("-z", "")
            FR.Controls.Add(PNF(filtros, dt_grid))
            If fil_db Is Nothing Then
                sel_drfiltro()
            ElseIf fil_dr IsNot Nothing Then
                For x As Integer = 0 To fil_dr.Count
                    Dim fil() As String = filtros_gr.Split(",")
                    Dim dr As DropDownList = FR.FindControl("Dr" + fil(x))
                    dr.SelectedIndex = -1
                    dr.Items.FindByText(fil_dr(x)).Selected = True
                Next
            End If

        End If


        If movil() = False Then
            Dim CH As Boolean
            If campos_gr Is Nothing Then
                gr.AutoGenerateColumns = True
            Else

                For Each str As String In campos_gr.Split(",")
                    If str.Contains("-TM") = True Then
                        gr.Columns.Add(grtem(str))
                    ElseIf str.Contains("-BT") Then
                        If str.Contains(";") Then
                            Dim strx() As String = str.Split(";")
                            gr.Columns.Add(grboton(strx(0).ToUpper, strx(1)))
                        Else
                            gr.Columns.Add(grboton(str.ToUpper.Replace("-BT", ""), str))
                        End If
                    ElseIf str.Contains("-CH") Then
                        CH = True
                        gr.Columns.Insert(1, grtem(str))
                    ElseIf str.Contains("-SUM(") Or str.Contains("-COUNT(") Then
                        Dim strx() As String = str.Split("-")
                        gr.Columns.Add(gritem(strx(0).ToUpper, strx(0)))
                    Else
                        If str.Contains(";") Then
                            Dim strx() As String = str.Split(";")
                            gr.Columns.Add(gritem(strx(0).ToUpper, strx(1)))
                        Else
                            gr.Columns.Add(gritem(str.ToUpper.Replace("-BT", ""), str))
                        End If

                    End If

                Next
            End If
            If criterio_gr IsNot Nothing Then
                'carga_gr()
            End If

            carga_gr()
            If CH = True Then
                For Each GROW As GridViewRow In gr.Rows
                    Dim CHG As New CheckBox
                    CHG.ID = "ChG"
                    GROW.Cells(1).Controls.Add(CHG)
                    GROW.HorizontalAlign = HorizontalAlign.Center
                Next
            End If
            FR.Controls.Add(gr)
        Else
            FR.Height = Unit.Pixel(1100)
            camtm = Nothing
            gr.ShowHeader = False
            gr.Font.Size = FontUnit.Point(size_fuente)
            For Each cam As String In campos_gr.Split(",")
                Dim stdb As String = Nothing
                If cam.Contains(";") Then
                    Dim scam() As String = cam.Split(";")
                    stdb = scam(1).Replace("-BT", "").Replace("-N", "").Replace("-M", "").Replace("-D", "")

                Else

                    stdb = cam.Replace("-BT", "").Replace("-N", "").Replace("-M", "").Replace("-D", "")
                End If
                Dim xgr As Integer = 0
                If stdb.Contains("-K") Then
                    If stdb.Contains(".") Then
                        Dim STT() As String = stdb.Split(".")
                        stdb = STT(1)
                    End If

                    gr.Columns.Add(gritem(stdb, stdb))
                Else
                    If camtm IsNot Nothing Then
                        camtm += ","
                    End If
                    camtm += stdb
                    If xgr = 1 Then
                        Exit For
                    End If
                    xgr += 1
                End If
            Next
            gr.Columns.Add(grtem(""))
            If campos_gr.Contains("-BT") Then
                gr.Columns.Add(grtem(""))
            ElseIf campos_gr.Contains("-CH") Then
                Dim stdb() As String = campos_gr.Split(",")
                gr.Columns.Insert(0, gritem("-CH", stdb(0).Replace("-k", "")))
                gr.Columns(0).ItemStyle.Font.Size = FontUnit.Point(0)
            End If
            'gr.Columns.Add(grboton("VER", ""))
            carga_gr(campos_gr)
            FR.HorizontalAlign = HorizontalAlign.Center
            FR.Controls.Add(gr)
        End If
    End Sub
    Private Sub carga_gr(Optional campos As String = Nothing)
        Dim ds As New carga_dssql(data_gr)
        Dim ctm, ctgrup As String
        campos = campos_gr.Replace("-K", "").Replace("-BT", "").Replace("-N", "").Replace("-M", "").Replace("-D", "")
        If campos IsNot Nothing Then
            For Each str As String In campos.Split(",")
                If str.Contains("-SUM(") Or str.Contains("-COUNT(") Then
                    Dim STR2() As String = str.Split("-")
                    ctm += "," + STR2(1) + " AS " + STR2(0)
                Else
                    If ctm IsNot Nothing Then
                        ctm += ","
                        ctgrup += ","
                    End If
                    ctm += str
                    ctgrup += str
                End If

            Next
        End If

        If data_gr IsNot Nothing Then
            'If campos.Contains("-SUM(") Then
            '    dt_gr = ds.Carga_tablas_especial(ctm, criterio_gr)
            If criterio_gr IsNot Nothing Then
                If campos.Contains("-SUM(") Or campos.Contains("-COUNT(") Then
                    dt_gr = ds.Carga_tablas_especial(ctm, criterio_gr + fil_db,, ctgrup)
                Else
                    dt_gr = ds.Carga_tablas(criterio_gr + fil_db, orden_gr)
                End If
            ElseIf fil_db IsNot Nothing Then
                Dim FILX As String = fil_db.Substring(5)
                If campos.Contains("-SUM(") Or campos.Contains("-COUNT(") Then
                    dt_gr = ds.Carga_tablas_especial(ctm, FILX,, ctgrup, orden_gr)
                Else
                    dt_gr = ds.Carga_tablas(FILX, orden_gr)
                End If

            Else
                If campos.Contains("-SUM(") Or campos.Contains("-COUNT(") Then
                    dt_gr = ds.Carga_tablas_especial(ctm, criterio_gr,, ctgrup, orden_gr)
                Else
                    dt_gr = ds.Carga_tablas(Nothing, orden_gr)
                End If

            End If

            Else
            dt_gr = dt_gr
        End If

        gr.Font.Size = FontUnit.Point(size_fuente)
        gr.AutoGenerateColumns = False
        If criterio_gr Is Nothing And fil_db IsNot Nothing Then
            Dim fr As String = Nothing
            If gr.Rows.Count = 0 Then
                fr = fil_db.Substring(5)
            End If
            gr.DataSource = dt_gr '.Select(fr)
        Else
            gr.DataSource = dt_gr
        End If

        gr.DataBind()
        'filtros_gr = Nothing
        Try
            If movil() = True And camtm IsNot Nothing Then
                gr.BorderColor = Color.White
                gr.HorizontalAlign = HorizontalAlign.Right
                Dim xgr As Integer = 0
                For Each row As DataRow In dt_gr.Rows
                    Dim cps As String = "<hr>"
                    Dim bt As New LinkButton
                    bt.ID = "BtGR"
                    'bt.Text = "VER"
                    bt.CommandName = "Select"
                    Dim LbT As New Label
                    LbT.Font.Size = FontUnit.Point(size_fuente)
                    For Each str As String In camtm.ToUpper.Split(",")
                        If str.Length > 0 Then
                            If str.Contains("FECHA") Then
                                'cps += str + ": <b>" + FormatDateTime(row.Item(str).ToString, DateFormat.ShortDate) + "</b><br>"
                                cps += "<b>" + FormatDateTime(row.Item(str).ToString, DateFormat.ShortDate) + "</b><br>"
                            ElseIf str.Contains("VALOR") Or str.Contains("TOTAL") Then
                                'cps += str + ": <b>" + FormatCurrency(row.Item(str).ToString, 0) + "</b><br>"
                                cps += "<b>" + FormatCurrency(row.Item(str).ToString, 0) + "</b><br>"
                            ElseIf str.Contains("-CH") Then
                                Dim ChB As New CheckBox
                                ChB.ID = "ChG"
                                gr.Rows(xgr).Cells(1).Controls.Add(ChB)
                                gr.Rows(xgr).Cells(1).HorizontalAlign = HorizontalAlign.Center
                            Else
                                cps += "<b>" + row.Item(str).ToString + "</b><br>"
                                'cps += str + ": <b>" + row.Item(str).ToString + "</b><br>"
                            End If
                        End If
                    Next
                    cps += "<hr>"
                    LbT.Text = cps
                    bt.Text = cps
                    If gr.Columns.Count = 1 Then
                        gr.Rows(xgr).Cells(0).Controls.Add(LbT)
                    ElseIf gr.Columns.Count = 2 Then
                        If camtm.ToUpper.Contains("-BT") Then
                            'gr.Rows(xgr).Cells(0).Controls.Add(LbT)
                            gr.Rows(xgr).Cells(1).Controls.Add(bt)
                        ElseIf camtm.ToUpper.Contains("-CH") Then
                            gr.Rows(xgr).Cells(1).Controls.Add(LbT)
                        Else
                            gr.Rows(xgr).Cells(1).Controls.Add(LbT)
                        End If
                    ElseIf gr.Columns.Count = 3 Then
                        LbT.Text = cps
                        'gr.Rows(xgr).Cells(1).Controls.Add(LbT)
                        gr.Rows(xgr).Cells(2).Controls.Add(bt)
                    End If
                    xgr += 1

                Next
            End If
        Catch ex As Exception

        End Try
        If evento_gr IsNot Nothing Then
            Try
                AddHandler gr.SelectedIndexChanged, evento_gr
            Catch ex As Exception

            End Try
        End If
    End Sub
    Private Function db_gr(nombre As String, Optional f_db As String = Nothing) As DataTable
        Dim ds As New carga_dssql(nombre)
        Return ds.Carga_tablas(f_db)
    End Function
    Private Function PNF(filtros As String, db As DataTable) As Panel
        'If wcon.Session("PNF" + wcon.Request.QueryString("fr")) Is Nothing Then
        PNF = New Panel
            PNF.ID = "PnFILTRO"
            movil()

            For Each fl As String In filtros.Split(",")
                fl = fl.ToUpper
                If FR.FindControl("Dr" + fl.Replace("-T", "").Replace("-Z", "")) Is Nothing Then

                    Dim Dr As DropDownList = control_fr("Dr" + fl.Replace("-T", "").Replace("-Z", ""), 15)
                    If movil() = True Then
                        Dr.Width = Unit.Percentage(50)
                    End If

                    Dim viw As New DataView(db)
                    If fl.Contains("-Z") Then
                        fl = fl.Replace("-Z", "")
                        viw.Sort = fl + " DESC"
                    Else
                        viw.Sort = fl.Replace("-T", "")
                    End If

                    For Each row As DataRowView In viw
                        If Dr.Items.FindByText(row.Item(fl.Replace("-T", ""))) Is Nothing Then
                            Dr.Items.Add(row.Item(fl.Replace("-T", "")))
                        End If
                    Next
                    If fl.Contains("-T") Then
                        Dr.Items.Insert(0, "TODOS")
                        fl = fl.Replace("-T", "")
                    Else
                        Dr.Items.Add("TODOS")
                    End If
                    If Dr IsNot Nothing Then
                        Dr.AutoPostBack = True
                        AddHandler Dr.SelectedIndexChanged, AddressOf sel_drfiltro
                    End If

                    Dr.Items.Add("Todos")
                    PNF.Controls.Add(Dr)
                End If

            Next
        'wcon.Session("PNF" + wcon.Request.QueryString("FR")) = PNF
        'Else
        '    PNF = wcon.Session("PNF" + wcon.Request.QueryString("FR"))
        ''    sel_drfiltro()
        ''carga_gr()
        'End If
    End Function
    Private Sub sel_drfiltro()
        Dim pnf As Panel = FR.FindControl("PnFILTRO")

        If fil_db IsNot Nothing Then
            If wcon.Session("fil_db" + wcon.Request.QueryString("FR")) = fil_db Then
                fil_db = Nothing
            Else
                fil_db = wcon.Session("fil_db" + wcon.Request.QueryString("FR"))
                pnf = wcon.Session("PNF" + wcon.Request.QueryString("FR"))
            End If
        ElseIf fil_db Is Nothing And wcon.Session("fil_db" + wcon.Request.QueryString("FR")) IsNot Nothing Then
            pnf = wcon.Session("PNF" + wcon.Request.QueryString("FR"))
            fil_db = wcon.Session("fil_db" + wcon.Request.QueryString("FR"))
        End If

        Dim x As Integer = 0
        If fil_db Is Nothing Then
            For Each sfil As String In filtros_gr.Split(",")
                If FR_CONTROL("Dr" + sfil.ToUpper) IsNot "TODOS" Then
                    fil_db += " and " + sfil + " = '" + FR_CONTROL("Dr" + sfil.ToUpper) + "'"
                    'fil_dr(x) = FR_CONTROL("Dr" + sfil.ToUpper)
                    x += 1
                End If
            Next
            wcon.Session("fil_db" + wcon.Request.QueryString("FR")) = fil_db
        Else
            Dim drfil As String = fil_db.Replace(" and ", ":").Replace(" = ", ";").Replace("'", "")
            For Each sdrf As String In drfil.Split(":")
                If sdrf.Length > 0 Then
                    Dim st() = sdrf.Split(";")
                    Dim drf As DropDownList = FR.FindControl("Dr" + st(0))
                    drf.SelectedIndex = -1
                    Try
                        drf.Items.FindByText(st(1)).Selected = True
                    Catch ex As Exception
                        drf.SelectedIndex = 0
                        fil_db = " AND " + st(0) + "='" + drf.SelectedItem.Text + "'"
                        wcon.Session("fil_db" + wcon.Request.QueryString("FR")) = fil_db
                    End Try
                End If
            Next
        End If

        carga_gr()
    End Sub
    Public Shared val_control As Boolean
    Public ReadOnly Property validacion_ct As Boolean
        Get
            Return val_control
        End Get
    End Property


    Public Property FR_CONTROL(NOMBRE As String, Optional activo As Boolean = True, Optional db As DataTable = Nothing, Optional evento As EventHandler = Nothing, Optional col_txt As Color = Nothing, Optional post As Boolean = False, Optional focus As Boolean = False, Optional VALIDAR As Boolean = False) As String
        Get

            Dim nombrect As String = NOMBRE.Remove(2)
            id_ct = NOMBRE
            Select Case nombrect
                Case "Tx", "Tn", "Tf", "Tp", "Tm"
                    Dim NCT() As String = Nothing
                    If NOMBRE.Contains("=") Then
                        NCT = NOMBRE.Split("=")
                        NOMBRE = NCT(0)
                    End If
                    Dim TxC As TextBox = FR.FindControl(NOMBRE)
                    TxC.Enabled = activo
                    val_control = False
                    TxC.BorderWidth = 3
                    If NOMBRE.Contains("=") Then
                        TxC.Text = NCT(1)
                    End If
                    If VALIDAR = True And TxC.Text.Length = 0 Then
                        alerta("EL CAMPO " + NOMBRE.Remove(0, 2) + " ES REQUERIDO")
                        TxC.BorderColor = Color.Red
                        val_control = True
                    ElseIf VALIDAR = True And nombrect = "Tn" And TxC.Text = "0" Then
                        alerta("EL CAMPO " + NOMBRE.Remove(0, 2) + " ES REQUERIDO")
                        TxC.BorderColor = Color.Red
                        val_control = True
                    Else
                        TxC.BorderColor = Color.Brown
                        TxC.BorderWidth = 1
                        If nombrect = "Tn" Then
                            Return TxC.Text.Replace(",", ".").Replace(".", "")
                        Else
                            Return TxC.Text.Replace(",", ";")
                        End If
                    End If
                Case "Dr"
                    Dim DrC As DropDownList = FR.FindControl(NOMBRE)
                    If DrC IsNot Nothing Then
                        DrC.Enabled = activo
                        If DrC.Items.Count = 0 Then
                            Return Nothing
                        Else
                            Return DrC.SelectedItem.Value
                        End If

                    End If
                    Return Nothing
                Case "Lb"
                    Dim LbC As Label = FR.FindControl(NOMBRE)
                    LbC.ForeColor = col_txt
                    Return LbC.Text

                Case "Gr"
                    Dim Gr As GridView = FR.FindControl(NOMBRE)
                    If Gr IsNot Nothing Then
                        Return Gr.SelectedRow.Cells(0).Text
                    End If
                Case "Bt"
                    Dim Bt As Button = FR.FindControl(NOMBRE)
                    If Bt IsNot Nothing Then
                        Return Bt.Text
                    End If
                Case "Bi"
                Case "Im"
                    'IMG(NOMBRE)

            End Select
            If NOMBRE.Contains("ItGr") Then
                Dim Gr As GridView = FR.FindControl(NOMBRE.Replace("It", ""))
                Return Gr.Rows.Count
            End If
            If NOMBRE.Contains("ChGr") Then
                Dim GR As GridView = FR.FindControl(NOMBRE.Replace("Ch", ""))
                For Each GROW As GridViewRow In GR.Rows
                    Dim CH As CheckBox = GROW.Cells(1).FindControl("ChG")
                    If CH.Checked = True Then
                        Return GROW.Cells(0).Text
                    End If
                Next
            End If

            Return Nothing
        End Get
        Set(value As String)
            Dim nombrect As String = NOMBRE.Remove(2)
            id_ct = NOMBRE
            Select Case nombrect
                Case "Lb"
                    Dim LbC As Label = FR.FindControl(NOMBRE)
                    Try
                        LbC.ForeColor = col_txt
                    Catch ex As Exception

                    End Try
                    LbC.Text = value
                Case "Tx", "Tn", "Tf", "Tp", "Tm"
                    Dim TxC As TextBox = FR.FindControl(NOMBRE)
                    TxC.ReadOnly = False
                    TxC.Enabled = activo
                    TxC.AutoPostBack = post
                    TxC.Text = value
                    If post = True And evento IsNot Nothing Then
                        AddHandler TxC.TextChanged, evento
                    End If
                    If focus = True Then
                        TxC.Focus()
                    End If
                Case "Dr"
                    Dim DrC As DropDownList = FR.FindControl(NOMBRE.Replace("-N", ""))
                    If DrC IsNot Nothing Then
                        DrC.Enabled = activo
                        DrC.AutoPostBack = post
                        DrC.Items.Clear()
                        Dim NITEM As Boolean
                        If NOMBRE.Contains("-N") Then
                            NOMBRE = NOMBRE.Replace("-N", "")
                            NITEM = True
                        End If
                        If value = Nothing Then
                            DrC.Items.Clear()
                            If NITEM = True Then
                                Dim dsp As New carga_dssql("parametros")
                                For Each row As DataRow In dsp.Carga_tablas("formulario='" + reque("fr") + "' and criterio='" + NOMBRE.Replace("-N", "").Replace("Dr", "") + "'", "valor").Rows
                                    DrC.Items.Add(row.Item("valor"))
                                Next
                                If DrC.Items.Count = 0 Then
                                    DrC.Items.Add(NOMBRE.Replace("-N", ""))
                                End If
                                DrC.Items.Add("NUEVO ITEM")
                                DrC.AutoPostBack = True
                                AddHandler DrC.SelectedIndexChanged, AddressOf SEL_DRNUEVOITEM
                            End If
                            Exit Property
                        End If
                        If DrC.Items.Count = 0 Then
                            If db Is Nothing Then
                                For Each str As String In value.Split(",")
                                    If str.Contains("-") Then
                                        Dim vstr() As String = str.Split("-")
                                        DrC.Items.Add(New ListItem(vstr(0), vstr(1)))
                                    Else
                                        DrC.Items.Add(New ListItem(str))
                                    End If
                                Next
                            Else
                                DrC.DataSource = db
                                If value.Contains("-") Then
                                    Dim vstr() As String = value.Split("-")
                                    DrC.DataTextField = vstr(0)
                                    DrC.DataValueField = vstr(1)
                                    DrC.DataBind()
                                ElseIf value.Contains("=") Then
                                    Dim vstr() As String = value.Split("=")
                                    DrC.DataTextField = vstr(0)
                                    DrC.DataBind()
                                    DrC.SelectedIndex = -1
                                    DrC.Items.FindByText(vstr(1)).Selected = True
                                Else
                                    DrC.DataTextField = value
                                End If
                            End If
                        Else
                            DrC.SelectedIndex = -1
                            DrC.Items.FindByText(value).Selected = True
                        End If
                        If NITEM = True Then
                            DrC.Items.Add("NUEVO ITEM")
                            DrC.AutoPostBack = True
                            AddHandler DrC.SelectedIndexChanged, AddressOf SEL_DRNUEVOITEM
                        End If
                        If evento IsNot Nothing Then
                            DrC.AutoPostBack = True
                            AddHandler DrC.SelectedIndexChanged, evento
                        End If
                    End If
                Case "Bt"
                    Dim BtC As Button = FR.FindControl(NOMBRE)
                    If BtC IsNot Nothing Then
                        BtC.Visible = activo
                        If evento IsNot Nothing Then
                            AddHandler BtC.Click, evento
                        End If
                        If value IsNot Nothing Then
                            BtC.Text = value
                        End If
                    End If
                Case "Ti"
                    Dim LtC As Literal = FR.FindControl(NOMBRE)
                    LtC.Text = "<h1>" + value + "</h1>"
                Case "Mn"
                    Dim Mn As Menu = FR.FindControl(NOMBRE)
                    Mn.StaticMenuItemStyle.ForeColor = col_fr
                    Mn.StaticMenuItemStyle.BorderColor = Color.White
                    Mn.StaticMenuItemStyle.BorderWidth = Unit.Point(1)
                    Mn.StaticMenuItemStyle.BorderStyle = BorderStyle.Solid
                    Mn.Font.Size = FontUnit.Point(size_fuente)
                    'Mn.BorderStyle = BorderStyle.Solid
                    If movil() = False Then
                        Mn.Orientation = Orientation.Horizontal
                    End If
                    _MN = value
                    Dim mni As MenuItem
                    If movil() = True Then
                        If Mn.Items.Count = 0 Then
                            mni = New MenuItem
                            mni.Text = "MENU"
                            mni.Value = _MN
                            'For Each mnite As String In _MN.Split(",")
                            '    Dim mni2 As New MenuItem
                            '    mni2.Text = mnite.ToUpper
                            '    mni.ChildItems.Add(mni2)
                            'Next
                            Mn.Items.Add(mni)
                        End If
                    Else
                        If Mn.Items.Count = 0 Then
                            For Each mnite As String In _MN.Split(",")
                                mni = New MenuItem
                                mni.Text = mnite.ToUpper
                                Mn.Items.Add(mni)
                            Next
                        End If
                    End If
                    AddHandler Mn.MenuItemClick, AddressOf clic_mn
                Case "Fl"
                    Dim fl As FileUpload = FR.FindControl(NOMBRE)
                Case "Bi"
                Case "Im"
                    Dim imgp As WebControls.Image = FR.FindControl(NOMBRE)
                    imgp.ImageUrl = "~/img/" + value
            End Select
        End Set
    End Property
    Private Sub SEL_DRNUEVOITEM(sender As Object, e As EventArgs)
        Dim dr As DropDownList = sender
        If dr.SelectedItem.Text = "NUEVO ITEM" Then
            redir("?fr=NUEVO PARAMETRO&f=" + reque("fr") + "&c=" + dr.ID.Replace("Dr", ""))
        End If
    End Sub
    Public Function VALORES_CONTROL(CONTROLES As String) As String()
        Dim VTXY As String = Nothing
        For Each CT As String In CONTROLES.Split(",")
            VTXY += FR_CONTROL(CT) + ","
        Next
        Return VTXY.Split(",")
    End Function
    Private Sub clic_mn(sender As Object, e As MenuEventArgs)
        Dim mn As Menu = sender
        If mn.SelectedItem.Text <> "MENU" And SUBMFR = False Then
            wcon.Response.Redirect(urla() + "?fr=" + mn.SelectedItem.Text)
        ElseIf mn.SelectedItem.Text <> "MENU" And SUBMFR = True Then
            wcon.Response.Redirect(npg + "?fr=" + FR_ATRAS + "&sm=" + mn.SelectedItem.Text)
        Else
            For Each STR As String In mn.SelectedItem.Value.Split(",")
                Dim MI As New MenuItem
                MI.Text = STR.ToUpper
                mn.Items.Add(MI)
            Next
            mn.Orientation = Orientation.Vertical
        End If
    End Sub
    Private Sub clic_mn2()
        Dim mn As Menu = FR.FindControl(id_ct)
        If mn.SelectedItem.Text <> "MENU" Then
            wcon.Response.Redirect(npg + "?fr=" + mn.SelectedItem.Text)
        Else
            For Each STR As String In mn.SelectedItem.Value.Split(",")
                Dim MI As New MenuItem
                MI.Text = STR.ToUpper
                mn.Items.Add(MI)
            Next
            mn.Orientation = Orientation.Vertical
        End If
    End Sub
    Public Property val_session(nombre As String)
        Get
            Return wcon.Session(nombre.ToLower)
        End Get
        Set(value)
            wcon.Session(nombre.ToLower) = value
        End Set
    End Property
    Public ReadOnly Property USERLOGUIN As String
        Get
            'Return wcon.User.Identity.Name
            Return Web.HttpContext.Current.User.Identity.Name
        End Get
    End Property
    Private Shared vpara As String
    Public Property DrPARAMETROS(nombre_DR As String, FORMULARIO As String, CRITERIO As String, Optional activo As Boolean = True)
        Get
            vpara = CRITERIO
            Dim DR As DropDownList = FR.FindControl(nombre_DR)
            DR.Items.Clear()
            Dim dspa As New carga_dssql("parametros")
            For Each row As DataRow In dspa.Carga_tablas("formulario='" + FORMULARIO + "' AND criterio='" + CRITERIO + "'", "VALOR").Rows
                DR.Items.Add(row.Item("valor"))
            Next
            If DR.Items.Count = 0 Then
                DR.Items.Insert(0, "SELECCIONAR " + CRITERIO)
            End If
            Return Nothing
        End Get
        Set(value)
            vpara = CRITERIO
            Dim DR As DropDownList = FR.FindControl(nombre_DR)
            DR.Items.Clear()
            Dim dspa As New carga_dssql("parametros")
            For Each row As DataRow In dspa.Carga_tablas("formulario='" + FORMULARIO + "' AND criterio='" + CRITERIO + "'", "VALOR").Rows
                DR.Items.Add(row.Item("valor"))
            Next
            If DR.Items.Count = 0 Then
                DR.Items.Insert(0, "SELECCIONAR " + CRITERIO)
            End If
            If value IsNot Nothing Then
                DR.Items.FindByText(value).Selected = True
            End If
            DR.Enabled = activo
        End Set
    End Property

    Public Sub FR_MENU(MnID As String, ITEMS As String, FORM As String)
        Dim MnFR As Menu = FR.FindControl(MnID)
        MnFR.Items.Clear()
        MnFR.Orientation = Orientation.Horizontal
        MnFR.Width = Unit.Percentage(100)
        MnFR.StaticMenuItemStyle.ForeColor = Color.White
        MnFR.StaticMenuItemStyle.Font.Size = FontUnit.Point(size_fuente)
        MnFR.StaticMenuItemStyle.BorderColor = Color.White
        MnFR.StaticMenuItemStyle.BorderWidth = Unit.Point(2)
        MnFR.StaticMenuItemStyle.BackColor = Color.DarkBlue
        If MnFR IsNot Nothing Then
            For Each ITM As String In ITEMS.Split(",")
                Dim MiT As New MenuItem
                MiT.Text = ITM
                MiT.NavigateUrl = npg + "?fr=" + FORM + "&sfr=" + ITM
                MnFR.Items.Add(MiT)
            Next
        End If
    End Sub



    Private Sub SEL_DRPA()
        Dim PNP As New Panel
        PNP.Controls.Add(Lb("Lb1", "VALOR"))
        PNP.Controls.Add(Tx("TxVALOR_PARAMETRO"))
        PNP.Controls.Add(Bt("PARAMETRO", AddressOf Guardar_DrPA))
        PNP.Width = 500
        PNP.HorizontalAlign = HorizontalAlign.Center
        FR.Controls.Add(PNP)

    End Sub

    Private Sub Guardar_DrPA()
        Dim dspa As New carga_dssql("parametros")
        dspa.insertardb("'" + reque("fr") + "','" + vpara + "','" + FR_CONTROL("TxVALOR_PARAMETRO") + "'")
        redir("?fr=" + reque("fr"))
    End Sub



    Public Sub cerrar_session()
        System.Web.Security.FormsAuthentication.SignOut()
        wcon.Response.Redirect("~/login.aspx")
    End Sub
    Private Function movil() As Boolean
        Dim userAgent As String = wcon.Request.UserAgent.ToLower
        Try
            If userAgent.Contains("iphone") = True Or userAgent.Contains("android") = True Or userAgent.Contains("ppc") = True Or userAgent.Contains("windows ce") = True Or userAgent.Contains("blackberry") = True Or userAgent.Contains("opera mini") = True Or userAgent.Contains("mobile") = True Or userAgent.Contains("palm") = True Or userAgent.Contains("portable") = True Then
                'wcon.Session("movil") = True

                If wcon.Session("app") = False Then
                    size_fuente = 35
                Else
                    size_fuente = 12
                End If
                Return True
            Else
                'wcon.Session("movil") = False
                size_fuente = 12
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try

    End Function
    Private Sub CARGA_FR(cp As String)
        movil()
        Dim x, y As Integer
        x = 0 : y = 0
        pn1.ID = "Pn1" + cp
        Pn2.ID = "Pn2" + cp
        tb.Width = Unit.Percentage(100)
        'FR.BackColor = COLOR_MN

        If FrSUP = True Then
            tb.Rows.Add(ct_fila_tabla(pn1, Pn2))
            If movil() = False Then
                x = 1
            End If

            For Each str As String In cp.Split(",")
                If x = 0 Then
                    pn1.Controls.Add(control_fr(str, 100))
                    pn1.Width = Unit.Percentage(100)
                    pn1.HorizontalAlign = HorizontalAlign.Center
                ElseIf x = 1 Then
                    pn1.Controls.Add(control_fr(str, 100))
                    pn1.Width = Unit.Percentage(100)
                    pn1.HorizontalAlign = HorizontalAlign.Center
                    x = 2
                ElseIf x = 2 Then
                    Pn2.Controls.Add(control_fr(str, 100))
                    Pn2.Width = Unit.Percentage(100)
                    Pn2.HorizontalAlign = HorizontalAlign.Center
                    x = 1
                End If
            Next
            FR.Controls.Add(tb)
            carga_menu(True)
        Else
            If tl IsNot Nothing Then
                Dim lbt As New Label
                If tl.Contains("-") = True Then

                End If
                If tl.Contains("=") = True Then
                    Dim _tl As String() = tl.Split("=")
                    lbt.Text = _tl(0) + "<hr size='5px' color='blue'/>"
                Else
                    lbt.Text = tl + "<hr size='5px' color='blue'/>"
                End If

                lbt.Font.Italic = True
                lbt.Font.Size = FontUnit.Point(size_fuente * 2)
                tb.Rows.Add(ct_fila_tabla(lbt))
            Else
                tl = Nothing
            End If

            tb.Width = Unit.Percentage(80)
            tb.BackColor = System.Drawing.Color.White
            tb.HorizontalAlign = HorizontalAlign.Center
            If movil() = True Then
                tb.Rows.Add(ct_fila_tabla(pn1))
            Else
                tb.Rows.Add(ct_fila_tabla(pn1, Pn2))
                x = 1
            End If
            carga_menu()
            For Each str As String In cp.Split(",")
                Dim lt As New Label
                Dim lt1 As New Literal
                Dim _tl, _tl2 As String()
                If str.Contains("-") = True Then
                    _tl = str.Split("-")
                    _tl2 = str.Split("=")
                    str = _tl(0)
                    If _tl(1).Contains("=") Then
                        str += "=" + _tl2(1)
                    End If
                End If
                If str.Contains("=") = True Then
                    _tl = str.Split("=")
                    lt.Text = " - " + _tl(0).Substring(2).Replace("_", " ") + " :"
                ElseIf str.Contains("Bt") Then
                    lt.Text = ""
                ElseIf _tl IsNot Nothing Then
                    lt.Text = " - " + _tl(1).Replace("_", " ") + " :"
                Else
                    lt.Text = " - " + str.Substring(2).Replace("_", " ") + " :"
                End If
                _tl = Nothing
                lt.Font.Size = FontUnit.Point(size_fuente)
                lt.ForeColor = Color.Gray
                lt1.Text = "<hr>"
                Dim tx As New TextBox
                tx.BorderStyle = BorderStyle.None
                tx.Width = Unit.Percentage(95)
                If x = 0 Then
                    pn1.Controls.Add(lt)
                    pn1.Controls.Add(control_fr(str))
                    pn1.Controls.Add(lt1)
                ElseIf x = 1 Then
                    pn1.Controls.Add(lt)
                    pn1.Controls.Add(control_fr(str))
                    pn1.Controls.Add(lt1)
                    x = 2
                ElseIf x = 2 Then
                    Pn2.Controls.Add(lt)
                    Pn2.Controls.Add(control_fr(str))
                    Pn2.Controls.Add(lt1)
                    x = 1
                End If
            Next
            If fr_BtGUARDAR = True Then
                Dim pnct As New Panel
                pnct.HorizontalAlign = HorizontalAlign.Center
                pnct.Controls.Add(Bt("guardar"))
                pnct.Controls.Add(Bt("cancelar", AddressOf clic_cancelar))
                tb.Rows.Add(ct_fila_tabla(pnct))
            End If
            FR.Controls.Add(tb)

        End If

    End Sub

    Private Sub clic_cancelar()
        If FR_ATRAS <> Nothing Then
            redir("?fr=" + FR_ATRAS.ToUpper)
        Else
            redireccion(urla)
        End If

    End Sub
    Public Function ct_fila_tabla(ct1 As Control, Optional ct2 As Control = Nothing, Optional nrow As Integer = 2, Optional alineacion As HorizontalAlign = HorizontalAlign.Left) As TableRow
        ct_fila_tabla = New TableRow
        Dim tcnombre As New TableCell
        Dim tccontrol As New TableCell
        If movil() = False Then
            tcnombre.Width = Unit.Percentage(50)
            tccontrol.Width = Unit.Percentage(50)
        End If

        If ct2 IsNot Nothing Then
            tcnombre.Controls.Add(ct1)
            tccontrol.Controls.Add(ct2)
            ct_fila_tabla.Cells.Add(tcnombre)
            ct_fila_tabla.Cells.Add(tccontrol)
        Else
            tcnombre.ColumnSpan = 2
            tcnombre.Controls.Add(ct1)
            ct_fila_tabla.Cells.Add(tcnombre)
        End If
        ct_fila_tabla.VerticalAlign = VerticalAlign.Top
        'ct_fila_tabla.HorizontalAlign = HorizontalAlign.Center
    End Function

    Public Sub alerta(msn As String)
        wcon.Response.Write("<script>window.alert('" + msn + "')</script>")
    End Sub
#Region "CONTROLES"
    Private Function control_fr(nombre As String, Optional ancho As Integer = 98) As Control

        Dim namect, ali As String
        ali = Nothing
        If nombre.Contains("-") = True Then
            Dim NC() As String = nombre.Split("-")
            namect = NC(0).Remove(2)
        Else
            namect = nombre.Remove(2)
        End If
        Select Case namect
            Case "Tx"
                Return Tx(nombre, ancho:=ancho)
            Case "Tn"
                Return Tx(nombre, TextBoxMode.Number, ancho, "0")
            Case "Tp"
                Return Tx(nombre, TextBoxMode.Password, ancho)
            Case "Tf"
                Return Tx(nombre, TextBoxMode.Date, ancho, Now.ToShortDateString)
            Case "Tm"
                Return Tx(nombre, TextBoxMode.MultiLine, ancho)
            Case "Dr"
                Return DR(nombre, ancho:=ancho)
            Case "Fn"
                If fr_consulta = True Then
                    Return Fn(nombre)
                Else
                    Return Tx(nombre)
                End If
            Case "Bt"
                Return Bt(nombre)
            Case "Lg"
                Return LgLOGIN()
            Case "Im"
                Return IMG(nombre)
            Case "Lb"
                Return Lb(nombre, ancho:=98)
            Case "Ti"
                Return Ti(nombre)
            Case "Mn"
                Return MN(nombre)
            Case "Pn"
                Dim PnAu As New Panel
                PnAu.ID = nombre
                Return PnAu
        End Select
        Return pn1
    End Function
    Private Function MN(ID As String) As Panel
        MN = New Panel
        MN.ID = "P" + ID
        Dim LT As New Literal
        LT.Text = "<hr>"
        MN.Controls.Add(LT)
        Dim MENU As New Menu
        MENU.ID = ID
        MN.Controls.Add(MENU)

    End Function
    Private Function Lb(id As String, Optional texto As String = Nothing, Optional ancho As Integer = 0) As Label
        Lb = New Label
        If id.Contains("=") = True Then
            Dim _ID As String() = id.Split("=")
            Lb.Text = _ID(1)
            Lb.ID = _ID(0)
        Else
            Lb.ID = id
        End If
        Lb.Font.Size = FontUnit.Point(size_fuente)
        If texto IsNot Nothing Then
            Lb.Text = texto
        End If
        If ancho > 0 Then
            Lb.Width = Unit.Percentage(ancho)
        End If
    End Function
    Private Function Ti(ID As String, Optional texto As String = Nothing, Optional tamano As String = "1") As Literal
        Ti = New Literal
        Ti.ID = ID

        If texto IsNot Nothing Then
            Ti.Text = "<h" + tamano + ">" + texto + "</h" + tamano + ">"
        Else
            If ID.Contains("-") = True Then
                Dim _tx() As String = ID.Split("-")
                tamano = _tx(1)
            End If
            Ti.Text = "<h" + tamano + ">" + ID.Substring(2).Replace("-" + tamano, "").Replace("_", " ") + "</h" + tamano + ">"
        End If
    End Function
    Private Function Tx(ID As String, Optional TipoTX As TextBoxMode = TextBoxMode.SingleLine, Optional ancho As Integer = 98, Optional texto As String = Nothing, Optional aliastx As String = Nothing) As TextBox
        Tx = New TextBox
        If ID.Contains("=") = True Then
            Dim _ID As String() = ID.Split("=")
            Tx.Text = _ID(1)
            Tx.ID = _ID(0)
        Else
            Tx.ID = ID
        End If
        If TipoTX = TextBoxMode.MultiLine Then
            Tx.Height = 77
        End If
        Tx.TextMode = TipoTX
        Tx.Font.Size = FontUnit.Point(size_fuente)
        Tx.BorderStyle = BorderStyle.Ridge
        Tx.Width = Unit.Percentage(ancho)
        If texto IsNot Nothing Then
            Tx.Text = texto
        End If
    End Function
    Private Function DR(ID As String, Optional ancho As Integer = 98) As DropDownList
        DR = New DropDownList
        DR.ID = ID
        DR.Font.Size = FontUnit.Point(size_fuente)
        DR.BorderStyle = BorderStyle.None
        DR.Width = Unit.Percentage(ancho)
    End Function
    Private Function Fn(ID As String) As FileUpload
        Fn = New FileUpload
        Fn.ID = ID
        Fn.Font.Size = FontUnit.Point(size_fuente)
        Fn.BorderStyle = BorderStyle.None
        Fn.Width = Unit.Percentage(98)
    End Function
    Private Function LgLOGIN() As Panel
        LgLOGIN = New Panel
        fr_consulta = False
        LgLOGIN.BorderStyle = BorderStyle.Ridge
        LgLOGIN.Controls.Add(Lb("usuario :"))
        LgLOGIN.Controls.Add(Tx("TxUSUARIO"))
        LgLOGIN.Controls.Add(Lb("clave :"))
        LgLOGIN.Controls.Add(Tx("TxCLAVE", TextBoxMode.Password))
        LgLOGIN.Controls.Add(Bt("INICIO_SESION"))

    End Function
    Private Function Bi(Texto As String, Optional url As String = Nothing) As ImageButton
        Bi = New ImageButton
        Bi.ID = "Bi" + Texto.ToUpper
        Bi.ImageUrl = "~/img/" + Bi.ID + ".png"
        'Bi.AlternateText = Texto.ToUpper
        Bi.ToolTip = Texto.ToUpper
        Bi.Width = 30
        Bi.Height = 30
        If url Is Nothing Then
            Bi.PostBackUrl = npg + "?fr=" + Texto
        Else
            Bi.PostBackUrl = url
        End If
    End Function
    Private Function Bt(Texto As String, Optional evento As EventHandler = Nothing) As Button
        Bt = New Button
        If Texto.Contains("Bt") = True Then
            Bt.ID = Texto.ToUpper
        Else
            Bt.ID = "Bt" + Texto.ToUpper
        End If

        Bt.Font.Size = FontUnit.Point(size_fuente)
        Bt.BackColor = col_fr
        Bt.BorderColor = Color.White
        Bt.ForeColor = Color.White
        Bt.BorderWidth = Unit.Point(5)
        Bt.BorderStyle = BorderStyle.Solid
        Bt.Style.Add("hover", "cursor: pointer")
        If Texto.Contains("Bt") = True Then
            Bt.Text = Texto.Substring(2).Replace("_", " ").ToUpper
        Else
            Bt.Text = Texto.Replace("_", " ").ToUpper
        End If
        If evento IsNot Nothing Then
            AddHandler Bt.Click, evento
        End If

        If movil() = True Then
            Bt.Width = Unit.Percentage(50)
        End If
        'If evento IsNot Nothing Then
        '    AddHandler Bt.Click, evento
        'End If
    End Function
    Private Function IMG(NOMBRE_ANCHO As String, Optional IMAGEN As String = Nothing) As WebControls.Image
        IMG = New WebControls.Image
        Dim id, wh As String
        If NOMBRE_ANCHO.Contains("-") Then
            Dim tim() As String = NOMBRE_ANCHO.Split("-")
            id = tim(0)
            wh = tim(1)
        Else
            id = NOMBRE_ANCHO
            wh = 100
        End If
        IMG.ID = id
        IMG.Width = Unit.Percentage(wh)
        If IMAGEN IsNot Nothing Then
            IMG.ImageUrl = "~/img/" + IMAGEN
        End If
    End Function

    Private Function Sp(ID As String) As Panel
        Sp = New Panel
        Sp.ID = ID
        Sp.Controls.Add(Ti(tl))
    End Function

#Region "grilla"
    Private Function gritem(titulo As String, campo As String) As BoundField
        gritem = New BoundField
        If titulo.Contains(".") Then
            Dim STT() As String = titulo.Split(".")
            titulo = STT(1)
        End If
        If campo.Contains(".") Then
            Dim STT() As String = campo.Split(".")
            campo = STT(1)
        End If
        gritem.HeaderText = titulo.Replace("-K", "").Replace("-D", "").Replace("-N", "").Replace("-M", "")
        gritem.HeaderStyle.Font.Size = FontUnit.Point(size_fuente)
        gritem.HeaderStyle.BackColor = col_fr
        gritem.HeaderStyle.BorderStyle = BorderStyle.Outset
        gritem.HeaderStyle.ForeColor = Color.White
        gritem.HeaderStyle.Font.Bold = True
        gritem.ItemStyle.Font.Size = FontUnit.Point(size_fuente)
        Dim cp As String
        If titulo.Contains("-CT") Then
            gritem.ItemStyle.Font.Size = FontUnit.Point(0)
            gritem.HeaderStyle.Font.Size = FontUnit.Point(0)
            gritem.ItemStyle.Width = Unit.Percentage(0)
        ElseIf campo IsNot Nothing Then
            If campo.Contains("-K") Or campo.Contains("-Ch") Then
                gritem.ItemStyle.Font.Size = FontUnit.Point(0)
                gritem.HeaderText = campo.Replace("-K", "").Replace("-D", "").Replace("-N", "").Replace("-M", "")
                gritem.HeaderStyle.Font.Size = FontUnit.Point(0)
                gritem.ItemStyle.Width = Unit.Percentage(0)
            ElseIf campo.Contains("-") Then
                gritem.ItemStyle.HorizontalAlign = HorizontalAlign.Center
                Dim fstr() As String = campo.Split("-")
                gritem.DataFormatString = forma(fstr(1).Replace("-K", "").Replace("-D", "").Replace("-N", "").Replace("-M", ""))
            End If
            gritem.DataField = campo.Replace("-K", "").Replace("-D", "").Replace("-N", "").Replace("-M", "")

        End If
    End Function
    Private Function forma(fr As String) As String
        Select Case fr
            Case "M"
                Return "{0:c0}"
            Case "N"
                Return "{0:n0}"
            Case "D", "d"
                Return "{0:d}"
        End Select
        Return ""
    End Function
    Private Function grboton(titulo As String, campo As String) As ButtonField
        grboton = New ButtonField
        grboton.HeaderText = titulo.ToUpper
        grboton.HeaderStyle.BackColor = col_fr
        grboton.HeaderStyle.BorderStyle = BorderStyle.Outset
        grboton.HeaderStyle.ForeColor = Color.White
        grboton.ButtonType = ButtonType.Link
        grboton.Text = titulo
        grboton.CommandName = "Select"
        grboton.ItemStyle.Font.Size = FontUnit.Point(size_fuente)
        grboton.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        If campo.Contains("-") Then
            grboton.ItemStyle.HorizontalAlign = HorizontalAlign.Center
            Dim fstr() As String = campo.Split("-")
            grboton.DataTextFormatString = forma(fstr(1))
        End If
        grboton.DataTextField = campo.Replace("-BT", "").Replace("-D", "").Replace("-N", "").Replace("-M", "")
    End Function
    Private Function grtem(titulo As String) As TemplateField
        grtem = New TemplateField
        grtem.ItemStyle.Font.Size = FontUnit.Point(size_fuente)
        grtem.HeaderText = titulo.Replace("-TM", "").Replace("-CH", "").ToUpper
        grtem.HeaderStyle.BackColor = col_fr
        grtem.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        grtem.HeaderStyle.BorderStyle = BorderStyle.Outset
        grtem.HeaderStyle.ForeColor = Color.White
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
#End Region

#End Region




End Class
