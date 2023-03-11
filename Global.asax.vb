Imports System.Web.Mvc
Imports System.Web.Routing

Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(sender As Object, e As EventArgs)
        ' Se desencadena al iniciar la aplicación
        AreaRegistration.RegisterAllAreas()
        RouteConfig.RegisterRoutes(RouteTable.Routes)
    End Sub
End Class