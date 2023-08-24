Friend Class ENCRIPTAR

    Private patron_busqueda As String = "OopqPQcde1IJK2ijkWXYZlm3ABCD45NÑwxyzRST6HLMU7fghn8F 9bñrs0EGVatuv"
    Private Patron_encripta As String = "JhijklK1Zab34VcdWpqrsXY78OÑf gmnPQeotuv90ABCDEFñGHI56LMwxyzNRS2TU"

    Private patron_busqueda2 As String = "OopqPQcde1IJK2ijkWXYZlm3ABCD45NÑwxyzRST6HL MU7fghn8F9bñrs0EGVatuv:"
    Private Patron_encripta2 As String = "JhijJklK1ZÑab34VcdWpqrsXY78Of gmnPQe@otuv90ABCDEFGHI56LMwxyzNRS2TU"

    Private sttexto As String
    Public Property stencripta() As String
        Get
            Return sttexto
        End Get
        Set(ByVal value As String)
            sttexto = value
        End Set
    End Property
    ReadOnly Property stdsenencripta()
        Get
            Return sttexto
        End Get
    End Property
    Public Sub New(Optional encriptar As Boolean = False, Optional texto As String = Nothing, Optional nivel As Integer = 0)
        If nivel = 1 Then
            If encriptar = True Then
                stencripta = EncriptarCadena(texto)
            Else
                sttexto = DesEncriptarCadena(texto)
            End If
        ElseIf nivel = 2 Then
            If encriptar = True Then
                stencripta = EncriptarCadena2(texto)
            Else
                sttexto = DesEncriptarCadena2(texto)
            End If
        End If

    End Sub
    Public Function enc(txt As String) As String
        Return EncriptarCadena(txt)
    End Function
    Public Function desenc(txt As String) As String
        Return DesEncriptarCadena(txt)
    End Function
#Region "ENCRIPTAR"
    Private Function EncriptarCadena(ByVal cadena As String) As String
        Dim idx As Integer
        Dim result As String = ""
        If cadena <> Nothing Then
            For idx = 0 To cadena.Length - 1
                result += EncriptarCaracter(cadena.Substring(idx, 1), cadena.Length, idx)
            Next
        End If
        Return result
    End Function
    Private Function EncriptarCaracter(ByVal caracter As String,
                                                             ByVal variable As Integer,
                                                            ByVal a_indice As Integer) As String
        Dim caracterEncriptado As String, indice As Integer
        caracterEncriptado = ""
        If patron_busqueda.IndexOf(caracter) <> -1 Then
            indice = (patron_busqueda.IndexOf(caracter) + variable + a_indice) Mod patron_busqueda.Length
            Return Patron_encripta.Substring(indice, 1)
        End If
        Return caracter
    End Function
#End Region
#Region "DESENCRIPTAR"
    Private Function DesEncriptarCadena(ByVal cadena As String) As String
        Dim idx As Integer
        Dim result As String = ""
        If cadena <> Nothing Then
            For idx = 0 To cadena.Length - 1
                result += DesEncriptarCaracter(cadena.Substring(idx, 1), cadena.Length, idx)
            Next
        End If
        Return result
    End Function

    Private Function DesEncriptarCaracter(ByVal caracter As String,
                                                                      ByVal variable As Integer,
                                                                      ByVal a_indice As Integer) As String
        Dim indice As Integer
        If Patron_encripta.IndexOf(caracter) <> -1 Then
            If (Patron_encripta.IndexOf(caracter) - variable - a_indice) > 0 Then
                indice = (Patron_encripta.IndexOf(caracter) - variable - a_indice) Mod Patron_encripta.Length
            Else
                'La línea está cortada por falta de espacio
                indice = (patron_busqueda.Length) + ((Patron_encripta.IndexOf(caracter) - variable - a_indice) Mod Patron_encripta.Length)
            End If
            indice = indice Mod Patron_encripta.Length
            Return patron_busqueda.Substring(indice, 1)
        Else
            Return caracter
        End If

    End Function
#End Region

#Region "ENCRIPTAR2"
    Private Function EncriptarCadena2(ByVal cadena As String) As String
        Dim idx As Integer
        Dim result As String = ""
        If cadena <> Nothing Then
            For idx = 0 To cadena.Length - 1
                result += EncriptarCaracter2(cadena.Substring(idx, 1), cadena.Length, idx)
            Next
        End If
        Return result
    End Function
    Private Function EncriptarCaracter2(ByVal caracter As String,
                                                             ByVal variable As Integer,
                                                            ByVal a_indice As Integer) As String
        Dim caracterEncriptado As String, indice As Integer
        caracterEncriptado = ""
        If patron_busqueda2.IndexOf(caracter) <> -1 Then
            indice = (patron_busqueda2.IndexOf(caracter) + variable + a_indice) Mod patron_busqueda2.Length
            Return Patron_encripta2.Substring(indice, 1)
        End If
        Return caracter
    End Function
#End Region
#Region "DESENCRIPTAR"
    Private Function DesEncriptarCadena2(ByVal cadena As String) As String
        Dim idx As Integer
        Dim result As String = ""
        If cadena <> Nothing Then
            For idx = 0 To cadena.Length - 1
                result += DesEncriptarCaracter2(cadena.Substring(idx, 1), cadena.Length, idx)
            Next
        End If
        Return result
    End Function

    Private Function DesEncriptarCaracter2(ByVal caracter As String,
                                                                      ByVal variable As Integer,
                                                                      ByVal a_indice As Integer) As String
        Dim indice As Integer
        If Patron_encripta2.IndexOf(caracter) <> -1 Then
            If (Patron_encripta2.IndexOf(caracter) - variable - a_indice) > 0 Then
                indice = (Patron_encripta2.IndexOf(caracter) - variable - a_indice) Mod Patron_encripta2.Length
            Else
                'La línea está cortada por falta de espacio
                indice = (patron_busqueda2.Length) + ((Patron_encripta2.IndexOf(caracter) - variable - a_indice) Mod Patron_encripta2.Length)
            End If
            indice = indice Mod Patron_encripta2.Length
            Return patron_busqueda2.Substring(indice, 1)
        Else
            Return caracter
        End If

    End Function
#End Region

End Class
