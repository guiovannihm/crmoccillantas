<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/occ.Master" CodeBehind="cotizacion.aspx.vb" Inherits="occillantascrm.cotizacion" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 70%;
        }
        .auto-style2 {
            width: 50%;
        }
        .auto-style3 {
            height: 19px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="Panel2" runat="server">
        <table align="center" cellpadding="0" cellspacing="0" class="auto-style1" runat="server" id="TbCOT">
            <tr>
                <td class="auto-style3" colspan="2"></td>
            </tr>
            <tr>
                <td class="auto-style2">
                    <asp:Button ID="BtCLIENTE" runat="server" Text="Button" />
                </td>
                <td>
                    <asp:Label ID="LbFECHA" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">
                    <asp:DropDownList ID="DrTIPO_VEHICULO" runat="server" Width="97%">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="DrTIPO_TERRENO" runat="server" Width="97%">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">
                    <asp:DropDownList ID="DrPOSICION" runat="server" Width="97%">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="DrEN_CALIDAD" runat="server" Width="97%">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">
                    <asp:DropDownList ID="DrFORMA_PAGO" runat="server" Width="97%">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="DrCIUDAD_ENTREGA" runat="server" Width="97%">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">
                    <asp:TextBox ID="TxTIPO_CARGA" runat="server" Width="97%"></asp:TextBox>
                </td>
                <td rowspan="2">
                    <asp:TextBox ID="TmREFERENCIAS" runat="server" TextMode="MultiLine" Width="97%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">
                    <asp:Button ID="BtGUARDAR" runat="server" Text="Button" />
                </td>
                <td>
                    <asp:Button ID="BtCANCELAR" runat="server" Text="Button" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
