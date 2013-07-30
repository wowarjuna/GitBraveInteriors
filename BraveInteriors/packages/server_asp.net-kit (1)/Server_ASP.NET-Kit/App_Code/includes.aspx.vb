Imports Microsoft.VisualBasic
Imports MySql.Data.MySqlClient
Imports System.Data
Public Class includes
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' This Class defines the constants and functions used in other pages in the
    ' kit.  It also opens connections to the database and defines record sets for later use.  It is
    ' included at the top of every other page in the kit and is paried with the closedown scipt.
    '**************************************************************************************************
    '**************************************************************************************************
    ' Values for you to update
    '**************************************************************************************************
    Public Shared strConnectTo As String = "SIMULATOR"     '** Set to SIMULATOR for the Simulator expert system, TEST for the Test Server **
    '** and LIVE in the live environment **

    Public Shared strDatabaseUser As String = "sagepayUser" '** Change this if you created a different user name to access the database **
    Public Shared strDatabasePassword As String = "[your database user password]" '** Set the password for the above user here **

    Public Shared strVirtualDir As String = "SagePayServerKit" '** Change if you've created a Virtual Directory in IIS with a different name **

    '** IMPORTANT.  Set the strYourSiteFQDN value to the Fully Qualified Domain Name of your server. **
    '** This should start http:// or https:// and should be the name by which our servers can call back to yours **
    '** i.e. it MUST be resolvable externally, and have access granted to the Sage Pay servers **
    '** examples would be https://www.mysite.com or http://212.111.32.22/ **
    '** NOTE: You should leave the final / in place. **
    Public Shared strYourSiteFQDN As String = "http://[your web site]/"

    '** At the end of a Server transaction, the customer is redirected back to the completion page **
    '** on your site using a client-side browser redirect. On live systems, this page will always be **
    '** referenced using the strYourSiteFQDN value above.  During development and testing, however, it **
    '** is often the case that the development machine sits behind the same firewall as the server **
    '** hosting the kit, so your browser might not be able resolve external IPs or dns names. **
    '** e.g. Externally your server might have the IP 212.111.32.22, but behind the firewall it **
    '** may have the IP 192.168.0.99.  If your test machine is also on the 192.168.0.n network **
    '** it may not be able to resolve 212.111.32.22. **
    '** Set the strYourSiteInternalFQDN to the internal Fully Qualified Domain Name by which **
    '** your test machine can reach the server (in the example above you'd use http://192.168.0.99/) **
    '** If you are not on the same network as the test server, set this value to the same value **
    '** as strYourSiteFQDN above. **
    '** NOTE: You should leave the final / in place. **
    Public Shared strYourSiteInternalFQDN As String = "http://[your web site]/"

    Public Shared strVendorName As String = "[your Sage Pay Vendor Name]" '** Set this value to the Vendor Name assigned to you by Sage Pay or chosen when you applied **
    Public Shared strCurrency As String = "GBP" '** Set this to indicate the currency in which you wish to trade. You will need a merchant number in this currency **
    Public Shared strTransactionType As String = "PAYMENT" '** This can be DEFERRED or AUTHENTICATE if your Sage Pay account supports those payment types **
    Public Shared strPartnerID As String = "" '** Optional setting. If you are a Sage Pay Partner and wish to flag the transactions with your unique partner id set it here.


    '**************************************************************************************************
    ' Database Definitions for this site
    '**************************************************************************************************
    Public Shared strSagePayDSN = "SERVER=localhost; user id=" & strDatabaseUser & "; password=" & strDatabasePassword & "; database=sagepay; pooling=false;"

    '**************************************************************************************************
    ' Global Definitions for this site
    '**************************************************************************************************
    Public Shared strProtocol As String = "2.23"
    Public Shared Function SystemURL(ByVal strConnectTo As String, ByVal strType As String) As String
        Dim strSystemURL As String = ""
        If strConnectTo = "LIVE" Then
            Select Case strType
                Case "abort"
                    strSystemURL = "https://live.sagepay.com/gateway/service/abort.vsp"
                Case "authorise"
                    strSystemURL = "https://live.sagepay.com/gateway/service/authorise.vsp"
                Case "cancel"
                    strSystemURL = "https://live.sagepay.com/gateway/service/cancel.vsp"
                Case "purchase"
                    strSystemURL = "https://live.sagepay.com/gateway/service/vspserver-register.vsp"
                Case "refund"
                    strSystemURL = "https://live.sagepay.com/gateway/service/refund.vsp"
                Case "release"
                    strSystemURL = "https://live.sagepay.com/gateway/service/release.vsp"
                Case "repeat"
                    strSystemURL = "https://live.sagepay.com/gateway/service/repeat.vsp"
                Case "void"
                    strSystemURL = "https://live.sagepay.com/gateway/service/void.vsp"
                Case "3dcallback"
                    strSystemURL = "https://live.sagepay.com/gateway/service/direct3dcallback.vsp"
                Case "showpost"
                    strSystemURL = "https://test.sagepay.com/showpost/showpost.asp"
            End Select
        ElseIf strConnectTo = "TEST" Then
            Select Case strType
                Case "abort"
                    strSystemURL = "https://test.sagepay.com/gateway/service/abort.vsp"
                Case "authorise"
                    strSystemURL = "https://test.sagepay.com/gateway/service/authorise.vsp"
                Case "cancel"
                    strSystemURL = "https://test.sagepay.com/gateway/service/cancel.vsp"
                Case "purchase"
                    strSystemURL = "https://test.sagepay.com/gateway/service/vspserver-register.vsp"
                Case "refund"
                    strSystemURL = "https://test.sagepay.com/gateway/service/refund.vsp"
                Case "release"
                    strSystemURL = "https://test.sagepay.com/gateway/service/release.vsp"
                Case "repeat"
                    strSystemURL = "https://test.sagepay.com/gateway/service/repeat.vsp"
                Case "void"
                    strSystemURL = "https://test.sagepay.com/gateway/service/void.vsp"
                Case "3dcallback"
                    strSystemURL = "https://test.sagepay.com/gateway/service/direct3dcallback.vsp"
                Case "showpost"
                    strSystemURL = "https://test.sagepay.com/showpost/showpost.asp"
            End Select
        Else
            Select Case strType
                Case "abort"
                    strSystemURL = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorAbortTx"
                Case "authorise"
                    strSystemURL = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorAuthoriseTx"
                Case "cancel"
                    strSystemURL = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorCancelTx"
                Case "purchase"
                    strSystemURL = "https://test.sagepay.com/simulator/VSPServerGateway.asp?Service=VendorRegisterTx"
                Case "refund"
                    strSystemURL = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorRefundTx"
                Case "release"
                    strSystemURL = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorReleaseTx"
                Case "repeat"
                    strSystemURL = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorRepeatTx"
                Case "void"
                    strSystemURL = "https://test.sagepay.com/simulator/vspserverGateway.asp?Service=VendorVoidTx"
                Case "3dcallback"
                    strSystemURL = "https://test.sagepay.com/simulator/vspserverCallback.asp"
                Case "showpost"
                    strSystemURL = "https://test.sagepay.com/showpost/showpost.asp"
            End Select
        End If

        Return strSystemURL
    End Function
    'create database connection
    Public Shared Function oMySQLConnection() As MySqlConnection
        Dim oConn As MySqlConnection
        Try
            oConn = New MySqlConnection(strSagePayDSN)
            oConn.Open()
            oMySQLConnection = oConn
        Catch ex As Exception
            Throw ex
        End Try

    End Function
    'destroy database connection
    Public Shared Sub DestroyConnection(ByVal objConn As MySqlConnection)
        Try
            'Close the connection if object is not nothing and connectton is option
            If Not objConn Is Nothing Then
                If objConn.State = ConnectionState.Open Then
                    objConn.Close()
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    '**************************************************************************************************
    ' Useful functions for all pages in this kit
    '**************************************************************************************************

    '** Filters unwanted characters out of an input string.  Useful for tidying up FORM field inputs
    Public Shared Function cleanInput(ByVal strRawText As String, ByVal strType As String) As String

        Dim strClean As String = ""
        Dim bolHighOrder As Boolean
        Dim strCleanedText As String = ""
        Dim iCharPos As Integer = 1
        Dim chrThisChar As String = ""

        If strType = "Number" Then
            strClean = "0123456789."
            bolHighOrder = False
        ElseIf strType = "VendorTxCode" Then
            strClean = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_."
            bolHighOrder = False
        Else
            strClean = " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.,'/{}@():?-_&£$=%~<>*+""" & vbCrLf
            bolHighOrder = True
        End If

        Do While iCharPos <= Len(strRawText)
            '** Only include valid characters **
            chrThisChar = Mid(strRawText, iCharPos, 1)

            If InStr(strClean, chrThisChar) <> 0 Then
                strCleanedText = strCleanedText & chrThisChar
            ElseIf bolHighOrder Then
                '** Fix to allow accented characters and most high order bit chars which are harmless **
                If Asc(chrThisChar) >= 191 Then strCleanedText = strCleanedText & chrThisChar
            End If

            iCharPos = iCharPos + 1
        Loop

        Return Trim(strCleanedText)

    End Function

    '** Doubles up single quotes to stop breakouts from SQL strings **
    Public Shared Function SQLSafe(ByVal strRawText As String) As String

        Dim strCleanedText As String = ""
        Dim iCharPos As Integer = 1

        Do While iCharPos <= Len(strRawText)
            '** Double up single quotes, but only if they aren't already doubled **
            If Mid(strRawText, iCharPos, 1) = "'" Then
                strCleanedText = strCleanedText & "''"
                If iCharPos <> Len(strRawText) Then
                    If Mid(strRawText, iCharPos + 1, 1) = "'" Then iCharPos = iCharPos + 1
                End If
            Else
                strCleanedText = strCleanedText & Mid(strRawText, iCharPos, 1)
            End If

            iCharPos = iCharPos + 1
        Loop

        Return Trim(strCleanedText)

    End Function

    '** Counts the number of : in a string.  Used to validate the basket fields
    Public Function countColons(ByVal strSource As String) As Integer

        Dim iNumCol As Integer
        Dim iCharPos As Integer

        If strSource = "" Then
            iNumCol = 0
        Else
            iCharPos = 1
            iNumCol = 0
            Do While iCharPos <> 0
                iCharPos = InStr(iCharPos + 1, strSource, ":")
                If iCharPos <> 0 Then iNumCol = iNumCol + 1
            Loop

        End If

        Return iNumCol

    End Function

    '** Checks to ensure a Basket Field is correctly formatted **
    Public Function validateBasket(ByVal strThisBasket As String) As Boolean

        Dim iRows As String
        Dim bolValid As Boolean = False

        If Len(strThisBasket) > 0 And (InStr(strThisBasket, ":") <> 0) Then
            iRows = Left(strThisBasket, InStr(strThisBasket, ":") - 1)
            If IsNumeric(iRows) Then
                iRows = CInt(iRows)
                If countColons(strThisBasket) = ((iRows * 5) + iRows) Then bolValid = True
            End If
        End If

        Return bolValid

    End Function

    '** ASP has no inbuild URLDecode function, so here's on in case we need it **
    Public Shared Function URLDecode(ByVal strString As String) As String

        Dim lngPos As Integer
        Dim strUrlDecode As String = ""

        For lngPos = 1 To Len(strString)
            If Mid(strString, lngPos, 1) = "%" Then
                strUrlDecode = strUrlDecode & Chr("&H" & Mid(strString, lngPos + 1, 2))
                lngPos = lngPos + 2
            ElseIf Mid(strString, lngPos, 1) = "+" Then
                strUrlDecode = strUrlDecode & " "
            Else
                strUrlDecode = strUrlDecode & Mid(strString, lngPos, 1)
            End If
        Next

        Return strUrlDecode

    End Function

    '** There is a URLEncode function, but wrap it up so keep the code clean **
    Public Shared Function URLEncode(ByVal strString As String) As String

        Return HttpUtility.UrlEncode(strString, System.Text.Encoding.GetEncoding("ISO-8859-15"))

    End Function

    '** MySQL can't handle ASP format dates.  It needs values separated by - signed, so create a MySQL valid date from that passed **
    Public Function mySQLDate(ByVal dateASP As Date)

        mySQLDate = DatePart("yyyy", dateASP) & "-" & Right("00" & DatePart("m", dateASP), 2) & "-" & Right("00" & DatePart("d", dateASP), 2) & " " & Right("00" & DatePart("h", dateASP), 2) & ":" & Right("00" & DatePart("n", dateASP), 2) & ":" & Right("00" & DatePart("s", dateASP), 2)

    End Function

    '** Used to split out the fields returned from the Sage Pay systems in the response part of the POSTs **
    Public Shared Function findField(ByVal strFieldName As String, ByVal strThisResponse As String) As String

        Dim iItem As Integer
        Dim arrItems(1) As String
        Dim strFindField As String = ""

        arrItems = Split(strThisResponse, vbCrLf)
        For iItem = LBound(arrItems) To UBound(arrItems)
            If InStr(arrItems(iItem), strFieldName & "=") = 1 Then
                strFindField = Mid(arrItems(iItem), Len(strFieldName) + 2)
                Exit For
            End If
        Next

        Return strFindField

    End Function

    '** Used to prevent issue caused by NULL strings being returned
    Public Shared Function nullToStr(ByVal dbRecord) As String
        If dbRecord Is DBNull.Value Then
            Return ""
        Else
            Return dbRecord.ToString
        End If
    End Function
End Class





