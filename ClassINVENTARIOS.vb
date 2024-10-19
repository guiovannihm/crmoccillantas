Imports System.IO
Imports Classcatalogoch

Public Class ClassINVENTARIOS
    Private Shadows _fr As Panel
    Private fr As ClassConstructor22

    Sub New(Panelfr As Panel)
        _fr = Panelfr
        fr = New ClassConstructor22(_fr)
        Select Case fr.reque("fr")
            Case "producto"

        End Select
    End Sub


    Private Function GetStreamAsByteArray(ByVal stream As Stream) As Byte()
        Dim streamLength As Integer = Convert.ToInt32(stream.Length)
        Dim fileData As Byte() = New Byte(streamLength) {}

        stream.Read(fileData, 0, streamLength)
        stream.Close()

        Return fileData
    End Function

    Private Sub Guardar_img()

        'Dim Imagen() As Byte = GetStreamAsByteArray(FileUpload1.PostedFile.InputStream)

    End Sub


End Class
