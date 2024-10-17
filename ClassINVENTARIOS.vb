Imports System.IO

Public Class ClassINVENTARIOS


    Private Function GetStreamAsByteArray(ByVal stream As Stream) As Byte()
        Dim streamLength As Integer = Convert.ToInt32(stream.Length)
        Dim fileData As Byte() = New Byte(streamLength) {}

        stream.Read(fileData, 0, streamLength)
        stream.Close()

        Return fileData
    End Function

    Private Sub guardar_img()

        'Dim Imagen() As Byte = GetStreamAsByteArray(FileUpload1.PostedFile.InputStream)

    End Sub


End Class
