Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Single
'Imports System.Web.UI.WebControls
Imports System.Text
Imports System.Threading.Tasks
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports System.Web
Imports System.Data

Public Class ClassImpresion
    Private context As Web.HttpContext = Web.HttpContext.Current
    Dim DOC As Document
    Dim Writer As PdfWriter
    Dim _tituloFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.ITALIC, 12, iTextSharp.text.Font.BOLD, Color.BLUE)
    Dim _tlnegrillaFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 15, iTextSharp.text.Font.BOLDITALIC, Color.BLACK)
    Dim _negrillaFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 9, iTextSharp.text.Font.BOLDITALIC, Color.BLACK)
    Dim _tablaFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 9, iTextSharp.text.Font.BOLD, Color.WHITE)
    Dim _itemFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 9, iTextSharp.text.Font.NORMAL, Color.BLACK)
    Dim _standardFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 11, iTextSharp.text.Font.NORMAL, Color.BLACK)

    Shared tl, enc, lg, hm, cl, ds, fir, ifir As String
    Dim dsti As DataTable



    Public WriteOnly Property aTitulo As String
        Set(value As String)
            tl = value
        End Set
    End Property
    Public WriteOnly Property aLogo As String
        Set(value As String)
            lg = context.Server.MapPath("\img\" + value)
        End Set
    End Property
    Public WriteOnly Property aHojamem As String
        Set(value As String)
            hm = context.Server.MapPath(value)
        End Set
    End Property
    Public WriteOnly Property aEncabezado As String
        Set(value As String)
            enc = value
        End Set
    End Property

    Public WriteOnly Property bCLIENTE As String
        Set(value As String)
            cl = value
        End Set
    End Property

    Public WriteOnly Property bTABLA_COSTOS As DataTable
        Set(value As DataTable)
            dsti = value
        End Set
    End Property

    Public WriteOnly Property CDESCRIPCION As String
        Set(value As String)
            ds = value
        End Set
    End Property
    Public WriteOnly Property bFIRMA As String
        Set(value As String)
            fir = value
        End Set
    End Property
    Public WriteOnly Property bIMH_FIRMA As String
        Set(value As String)
            ifir = value
        End Set
    End Property
    Public Sub cGENERAR_PDF(Optional nfile As String = "documento.pdf")

        DOC = New Document(PageSize.LETTER, 50, 50, 50, 50)
        Try
            Writer = PdfWriter.GetInstance(DOC, New FileStream(context.Server.MapPath(nfile), FileMode.Create))
        Catch ex As Exception

        End Try
        DOC.Open()
        If lg IsNot Nothing Then
            Dim oImagen As iTextSharp.text.Image
            oImagen = iTextSharp.text.Image.GetInstance(lg)
            oImagen.SetAbsolutePosition(20, 670)
            oImagen.ScalePercent(15)                 'Ajuste porcentual de la imagen
            DOC.Add(oImagen)                    'Se agrega la imagen al documento
        End If
        If tl IsNot Nothing Then
            Dim tbenc As PdfPTable = New PdfPTable(1)
            tbenc.WidthPercentage = 100
            If lg IsNot Nothing Then
                tbenc.WidthPercentage = 70
            End If
            Dim enc1 As String = Nothing
            Dim clclien As PdfPCell = New PdfPCell(New Phrase(tl, _tlnegrillaFont))
            clclien.BorderWidth = 0
            clclien.BorderWidthTop = 0.75F
            clclien.BorderWidthBottom = 0.75F
            clclien.HorizontalAlignment = Element.ALIGN_RIGHT

            tbenc.HorizontalAlignment = Element.ALIGN_RIGHT
            tbenc.AddCell(clclien)
            DOC.Header = New HeaderFooter(New Phrase(enc1, _tituloFont), False)
            DOC.Add(tbenc)
        End If

        If enc IsNot Nothing Then
            Dim tbenc As PdfPTable = New PdfPTable(1)
            tbenc.WidthPercentage = 100
            If lg IsNot Nothing Then
                tbenc.WidthPercentage = 70
            End If
            Dim enc1 As String = Nothing
            For Each str As String In enc.Split(",")
                enc1 += str + Chr(10)
            Next
            'enc1 += Chr(10) + Chr(10)
            Dim clclien As PdfPCell = New PdfPCell(New Phrase(enc1, _tituloFont))
            clclien.BorderWidth = 0
            'clclien.BorderColorBottom = Color.BLUE
            clclien.BorderWidthTop = 0.75F
            clclien.BorderWidthBottom = 0.75F
            clclien.HorizontalAlignment = Element.ALIGN_LEFT

            tbenc.HorizontalAlignment = Element.ALIGN_RIGHT
            tbenc.AddCell(clclien)
            DOC.Header = New HeaderFooter(New Phrase(enc1, _tituloFont), False)
            DOC.Add(tbenc)
        End If

        If cl IsNot Nothing Then
            Dim tbc As PdfPTable = New PdfPTable(1)
            tbc.WidthPercentage = 100
            tbc.HorizontalAlignment = Element.ALIGN_LEFT
            Dim enc1 As String = Nothing
            cl = Chr(10) + cl
            For Each str As String In cl.Split(";")
                enc1 += str + Chr(10)
            Next
            enc1 += Chr(10) + Chr(10)
            Dim clclien As PdfPCell = New PdfPCell(New Phrase(enc1, _standardFont))
            clclien.BorderWidth = 0
            tbc.AddCell(clclien)
            'DOC.NewPage()
            DOC.Add(Chunk.NEWLINE)
            DOC.Add(tbc)
        End If
        If dsti IsNot Nothing Then
            Dim tbc As PdfPTable = New PdfPTable(dsti.Columns.Count)
            tbc.WidthPercentage = 100

            For Each col As DataColumn In dsti.Columns
                Dim clclien As PdfPCell = New PdfPCell(New Phrase(col.ColumnName.ToUpper, _negrillaFont))
                clclien.HorizontalAlignment = Element.ALIGN_CENTER
                tbc.AddCell(clclien)
            Next
            For Each row As DataRow In dsti.Rows
                For X As Integer = 0 To (dsti.Columns.Count - 1)
                    Dim clclien As PdfPCell = New PdfPCell(New Phrase(row.Item(X), _standardFont))
                    Dim Y As String = row.Item(X).GetType.Name
                    tbc.AddCell(clclien)
                Next

            Next


            DOC.Add(tbc)

            'If dsti.Rows.Count > 1 Then
            '    DOC.Add(Chunk.NEXTPAGE)
            '    'tbsuperior()
            '    'DOC.Close()
            'End If
        End If
        If ds IsNot Nothing Then
            Dim tbd As PdfPTable = New PdfPTable(1)
            tbd.WidthPercentage = 100
            ds = Chr(10) + Chr(10) + ds
            Dim clclien As PdfPCell = New PdfPCell(New Phrase(ds, _standardFont))
            clclien.BorderWidth = 0
            tbd.AddCell(clclien)
            DOC.Add(tbd)
        End If

        If ifir IsNot Nothing Then
            Dim tbif As PdfPTable = New PdfPTable(1)
            Dim clclien As New PdfPCell
            Dim img As Image = Image.GetInstance(ifir)
            img.Alignment = Element.ALIGN_LEFT
            Dim percentage As System.Single = 0.0F
            percentage = 700 / img.Width
            img.ScalePercent(35.0F, 80.25F)
            clclien.AddElement(img)
            clclien.BorderWidth = 0
            tbif.AddCell(clclien)
            DOC.Add(tbif)
        Else
            fir = Chr(10) + Chr(10) + Chr(10) + Chr(10) + fir
        End If
        If fir IsNot Nothing Then
            Dim tbf As PdfPTable = New PdfPTable(1)
            tbf.WidthPercentage = 100

            Dim clclien As PdfPCell = New PdfPCell(New Phrase(Chr(10) + Chr(10) + fir, _standardFont))
            clclien.BorderWidth = 0
            tbf.AddCell(clclien)
            DOC.Add(tbf)
        End If




        'tbsuperior()

        DOC.Close()
        Try
            Writer.Close()
        Catch ex As Exception

        End Try
        'tbsuperior()
        lg = Nothing : enc = Nothing : cl = Nothing : dsti = Nothing : ds = Nothing : ifir = Nothing
    End Sub

    Private Sub tbsuperior()
        'DOC = New Document(PageSize.LETTER, 50, 50, 50, 50)
        Dim ipg As Integer = Writer.CurrentPageNumber

        Dim oImagen As iTextSharp.text.Image
        Dim cbPie As PdfContentByte
        Dim cbEncabezado As PdfContentByte
        'Writer = PdfWriter.GetInstance(DOC, New FileStream(context.Server.MapPath("documento.pdf"), FileMode.Create))
        '-----------------------------------------------------------------------------------------
        ' DEFINICIÓN DEL OBJETO PdfContentByte PARA EL ENCABEZADO
        '-----------------------------------------------------------------------------------------
        cbEncabezado = Writer.DirectContent
        With cbEncabezado
            .BeginText()
            .SetFontAndSize(FontFactory.GetFont(FontFactory.HELVETICA_BOLD, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL).BaseFont, 8)
            '.SetColorFill(iTextSharp.text.BaseColor.BLACK)
            .ShowTextAligned(PdfContentByte.ALIGN_CENTER, "XXXXXX", 290, 760, 0)
            .ShowTextAligned(PdfContentByte.ALIGN_CENTER, "XXXXXX", 290, 750, 0)
            .ShowTextAligned(PdfContentByte.ALIGN_CENTER, "XXXXXX", 290, 740, 0)
            .EndText()
        End With
        '-----------------------------------------------------------------------------------------
        ' DEFINICIÓN DEL OBJETO PdfContentByte PARA EL PIE DE PAGINA
        '-----------------------------------------------------------------------------------------
        cbPie = Writer.DirectContent
        cbPie.BeginText()
        cbPie.SetFontAndSize(FontFactory.GetFont(FontFactory.HELVETICA, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL).BaseFont, 10)
        'cbPie.SetColorFill(iTextSharp.text.BaseColor.BLACK)
        cbPie.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Página: " & Writer.PageNumber, 540, 25, 0)
        cbPie.EndText()
        '-----------------------------------------------------------------------------------------
        ' LOGOS DEL DOCUMENTO
        '-----------------------------------------------------------------------------------------
        oImagen = iTextSharp.text.Image.GetInstance(lg)
        oImagen.SetAbsolutePosition(28, 737)
        oImagen.ScalePercent(20)                 'Ajuste porcentual de la imagen
        DOC.Add(oImagen)                    'Se agrega la imagen al documento

        oImagen = iTextSharp.text.Image.GetInstance(lg)
        oImagen.SetAbsolutePosition(480, 737)
        oImagen.ScalePercent(16)
        DOC.Add(oImagen)
    End Sub
    Private Function tbsuperior1() As PdfPTable
        Dim tbs As PdfPTable = New PdfPTable(2)
        tbs.WidthPercentage = 100
        tbs.DefaultCell.Border = 0
        DOC = New Document()
        Try
            Writer = PdfWriter.GetInstance(DOC, New FileStream(context.Server.MapPath("documento.pdf"), FileMode.Create))

        Catch ex As Exception

        End Try

        If tl IsNot Nothing Then
            DOC.Open()
            tbs.HorizontalAlignment = Element.ALIGN_LEFT
            Dim clclien As PdfPCell = New PdfPCell(New Phrase(tl, _tituloFont))
            clclien.BorderWidth = 0
            clclien.BorderColorBottom = Color.BLUE
            clclien.BorderWidthBottom = 0.75F
            tbs.AddCell(clclien)
        ElseIf lg IsNot Nothing Then
            'Writer = PdfWriter.GetInstance(DOC, New FileStream(context.Server.MapPath("documento.pdf"), FileMode.Create))
            DOC.Open()
            Dim clclien As New PdfPCell
            Dim img As Image = Image.GetInstance(lg)
            img.Alignment = Element.ALIGN_LEFT
            Dim percentage As System.Single = 0.0F
            percentage = 700 / img.Width
            img.ScalePercent(15.0F, 50.25F)
            clclien.AddElement(img)
            clclien.BorderWidth = 0
            clclien.BorderColorBottom = Color.BLUE
            clclien.BorderWidthBottom = 0.75F
            tbs.AddCell(clclien)
        ElseIf hm IsNot Nothing Then

            'Writer = PdfWriter.GetInstance(DOC, New FileStream(context.Server.MapPath("documento.pdf"), FileMode.Create))
            DOC.Open()
            Dim clclien As New PdfPCell
            Dim img As Image = Image.GetInstance(hm)
            img.Alignment = Element.ALIGN_CENTER
            Dim percentage As System.Single = 0.0F
            percentage = 700 / img.Width
            img.ScalePercent(125.0F, 125.0F)
            img.SetAbsolutePosition(0, 0)
            'Writer.Add(img)
            DOC.Add(img)
        Else
            'Writer = PdfWriter.GetInstance(DOC, New FileStream(context.Server.MapPath("documento.pdf"), FileMode.Create))
            DOC.Open()
        End If
        DOC.Close()
        Writer.Close()
        Return tbs
    End Function
End Class
