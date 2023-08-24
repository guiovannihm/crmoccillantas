Friend Class ENCRIPTAR2

    Private patron_busqueda As String = "QAZu9ytrEDCI@UTGB0poi/HÑPYewOLK.q8ñl7jhgfJMN$6WSs2kd1X#bxn5VFRzm4vc3a"
    Private Patron_encripta As String = "OWjgh1P$Q29Zzm.xl4UR5eirYT6AdkfÑSvMañsIE@3qp78L#ncbDXN/0CwouBVJtyGHKF"
    Private sttexto As String

    Public ReadOnly Property stencripta(txt As String, Optional str As Boolean = False) As String
        Get
            Dim xmil As String = Now.Millisecond.ToString
            If str = True Then
                Try
                    Return EncriptarCadena(txt)
                Catch ex As Exception
                    Return ""
                End Try
            Else
                Try
                    Return EncriptarCadena(txt)
                Catch ex As Exception
                    Return ""
                End Try
            End If
        End Get
    End Property
    Public ReadOnly Property stdsenencripta(txt As String)
        Get
            Try
                Return DesEncriptarCadena(txt)
            Catch ex As Exception
                Return Nothing
            End Try

        End Get

    End Property
    'Public Sub New(ByVal encriptar As Boolean, ByVal texto As String, Optional nivel As Integer = 1)
    '    If nivel = 1 Then
    '        If encriptar = True Then
    '            stencripta = EncriptarCadena(texto)
    '        Else
    '            sttexto = DesEncriptarCadena(texto)
    '        End If
    '    ElseIf nivel = 2 Then
    '        If encriptar = True Then
    '            stencripta = EncriptarCadena(texto)
    '        Else
    '            sttexto = DesEncriptarCadena(texto)
    '        End If
    '    End If

    'End Sub

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


End Class
