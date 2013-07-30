Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Security.Cryptography
Imports System.Text.Encoding
Imports System.IO


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
    Public Shared strConnectTo As String = "SIMULATOR"    '** Set to SIMULATOR for the Sage Pay Simulator expert system, TEST for the Test Server **
    '** and LIVE in the live environment **
    Public Shared strVirtualDir As String = "SagePayFormKit" '** Change if you've created a Virtual Directory in IIS with a different name **

    '** IMPORTANT.  Set the strYourSiteFQDN value to the Fully Qualified Domain Name of your server. **
    '** This should start http:// or https:// and should be the name by which our servers can call back to yours **
    '** i.e. it MUST be resolvable externally, and have access granted to the Sage Pay servers **
    '** examples would be https://www.mysite.com or http://212.111.32.22/ **
    '** NOTE: You should leave the final / in place. **
    Public Shared strYourSiteFQDN As String = "http://[your web site]/"

    '** At the end of a Sage Pay Server transaction, the customer is redirected back to the completion page **
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

    Public Shared strVendorName As String = "[your Sage Pay Vendor Name]" '** Set this value to the Vendor Name assigned to you by Sage Pay or chosen when you applied **
    Public Shared strEncryptionPassword As String = "[your Encryption Password]" ' ** Set this value to the XOR Encryption password assigned to you by Sage Pay **
    Public Shared strCurrency As String = "GBP" '** Set this to indicate the currency in which you wish to trade. You will need a merchant number in this currency **
    Public Shared strVendorEMail As String = "[your e-mail address]" ' ** Set this to the mail address which will receive order confirmations and failures **
    Public Shared strTransactionType As String = "PAYMENT" '** This can be DEFERRED or AUTHENTICATE if your Sage Pay account supports those payment types **
    Public Shared strPartnerID As String = "" '** Optional setting. If you are a Sage Pay Partner and wish to flag the transactions with your unique partner id set it here.
    Public Shared iSendEMail As Integer = 1  '** Optional setting. 0 = Do not send either customer or vendor e-mails, 1 = Send customer and vendor e-mails if address(es) are provided(DEFAULT). 
    '** 2 = Send Vendor Email but not Customer Email. If you do not supply this field, 1 is assumed and e-mails are sent if addresses are provided.

    Public Shared strEncryptionType = "AES"  '** Encryption type should be left set to AES unless you are experiencing problems and have been told by SagePay support to change it - XOR is the only other acceptable value **

    '**************************************************************************************************
    ' Global Definitions for this site
    '**************************************************************************************************
    Public Shared strProtocol As String = "2.23"
    Public Shared arrBase64EncMap(64) As String
    Public Shared arrBase64DecMap(127) As Integer
    Public Shared arrProducts(3, 2) As String
    Public Shared strNewLine As String = "<P>" & vbCrLf
    Const BASE_64_MAP_INIT = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"

    Public Shared Function setCart()

        '** Set up a simple array of items for sale.  In your site this list would **
        '** be more complex, possible cart driven, but this is provided to illustrate **
        '** a simple way of filling a shopping basket **
        arrProducts(1, 1) = "Shaolin Soccer"
        arrProducts(1, 2) = "9.95"
        arrProducts(2, 1) = "Batman - The Dark Knight"
        arrProducts(2, 2) = "10.99"
        arrProducts(3, 1) = "IronMan"
        arrProducts(3, 2) = "8.75"

    End Function

    Public Shared Function SystemURL(ByVal strConnectTo As String) As String

        Dim strSystemURL As String = ""
        If strConnectTo = "LIVE" Then
            strSystemURL = "https://live.sagepay.com/gateway/service/vspform-register.vsp"
        ElseIf strConnectTo = "TEST" Then
            strSystemURL = "https://test.sagepay.com/gateway/service/vspform-register.vsp"
        ElseIf strConnectTo = "SIMULATOR" Then
            strSystemURL = "https://test.sagepay.com/simulator/vspformgateway.asp"
        Else
            strSystemURL = "https://test.sagepay.com/showpost/showpost.asp"
        End If

        Return strSystemURL

    End Function


    '**************************************************************************************************
    ' Useful functions for all pages in this kit
    '**************************************************************************************************


    '** Filters unwanted characters out of an input string based on type.  Useful for tidying up FORM field inputs
    Public Shared Function cleanInput(ByVal strRawText As String, _
                                      Optional ByVal filterType As CleanInputFilterType = CleanInputFilterType.WidestAllowableCharacterRange) As String

        Dim strAllowableChars As String
        Dim bAllowAccentedChars As Boolean = False
        Dim strCleaned As String = ""

        If filterType = CleanInputFilterType.Alphabetic _
        Or filterType = CleanInputFilterType.AlphabeticAndAccented Then

            strAllowableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz"
            If filterType = CleanInputFilterType.AlphabeticAndAccented Then bAllowAccentedChars = True
            strCleaned = cleanInput(strRawText, strAllowableChars, bAllowAccentedChars)

        ElseIf filterType = CleanInputFilterType.AlphaNumeric _
            Or filterType = CleanInputFilterType.AlphaNumericAndAccented Then

            strAllowableChars = "0123456789 ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
            If filterType = CleanInputFilterType.AlphaNumericAndAccented Then bAllowAccentedChars = True
            strCleaned = cleanInput(strRawText, strAllowableChars, bAllowAccentedChars)

        ElseIf filterType = CleanInputFilterType.Numeric Then
            strAllowableChars = "0123456789 .,"
            strCleaned = cleanInput(strRawText, strAllowableChars, False)

        Else ' WidestAllowableCharacterRange
            strAllowableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 .,'/\{}@():?-_&£$=%~*+""" & vbCrLf
            strCleaned = cleanInput(strRawText, strAllowableChars, True)
        End If

        Return strCleaned
    End Function

    '** Filters unwanted characters out of an input string based on an allowable character set.  Useful for tidying up FORM field inputs
    Public Shared Function cleanInput(ByVal strRawText As String, _
                                      ByVal strAllowableChars As String, _
                                      ByVal blnAllowAccentedChars As Boolean) As String
        Dim iCharPos As Integer = 1
        Dim chrThisChar As String = ""
        Dim strCleanedText As String = ""

        'Compare each character based on list of acceptable characters
        Do While iCharPos <= Len(strRawText)
            '** Only include valid characters **
            chrThisChar = Mid(strRawText, iCharPos, 1)

            If InStr(strAllowableChars, chrThisChar) <> 0 Then
                strCleanedText = strCleanedText & chrThisChar
            ElseIf blnAllowAccentedChars Then
                '** Allow accented characters and most high order bit chars which are harmless **
                If Asc(chrThisChar) >= 191 Then strCleanedText = strCleanedText & chrThisChar
            End If

            iCharPos = iCharPos + 1
        Loop

        Return strCleanedText
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

        Return HttpUtility.UrlEncode(strString)

    End Function

    '** Base 64 encoding routine.  Used to ensure the encrypted "crypt" field **
    '** can be safely transmitted over HTTP **
    Public Shared Function base64Encode(ByVal strPlain As String) As String
        Dim iLoop As Integer
        Dim iBy3 As Integer
        Dim strReturn As String
        Dim iIndex As Integer
        Dim iFirst As Integer
        Dim iSecond As Integer
        Dim iiThird As Integer
        If strPlain.Length = 0 Then
            base64Encode = ""
            Exit Function
        End If

        '** Set up Base64 Encoding and Decoding Maps for when we need them ** 
        For iLoop = 0 To Len(BASE_64_MAP_INIT) - 1
            arrBase64EncMap(iLoop) = Mid(BASE_64_MAP_INIT, iLoop + 1, 1)
        Next
        For iLoop = 0 To Len(BASE_64_MAP_INIT) - 1
            arrBase64DecMap(Asc(arrBase64EncMap(iLoop))) = iLoop
        Next

        '** Work out rounded down multiple of 3 bytes length for the unencoded text **
        iBy3 = (strPlain.Length \ 3) * 3
        strReturn = ""

        '** For each 3x8 byte chars, covert them to 4x6 byte representations in the Base64 map **
        iIndex = 1
        Do While iIndex <= iBy3
            iFirst = Asc(Mid(strPlain, iIndex + 0, 1))
            iSecond = Asc(Mid(strPlain, iIndex + 1, 1))
            iiThird = Asc(Mid(strPlain, iIndex + 2, 1))
            strReturn = strReturn & arrBase64EncMap((iFirst \ 4) And 63)
            strReturn = strReturn & arrBase64EncMap(((iFirst * 16) And 48) + ((iSecond \ 16) And 15))
            strReturn = strReturn & arrBase64EncMap(((iSecond * 4) And 60) + ((iiThird \ 64) And 3))
            strReturn = strReturn & arrBase64EncMap(iiThird And 63)
            iIndex = iIndex + 3
        Loop

        '** Handle any trailing characters not in groups of 3 **
        '** Extend to multiple of 3 characters using = signs as per RFC **
        If iBy3 < strPlain.Length Then
            iFirst = Asc(Mid(strPlain, iIndex + 0, 1))
            strReturn = strReturn & arrBase64EncMap((iFirst \ 4) And 63)
            If (strPlain.Length Mod 3) = 2 Then
                iSecond = Asc(Mid(strPlain, iIndex + 1, 1))
                strReturn = strReturn & arrBase64EncMap(((iFirst * 16) And 48) + ((iSecond \ 16) And 15))
                strReturn = strReturn & arrBase64EncMap((iSecond * 4) And 60)
            Else
                strReturn = strReturn & arrBase64EncMap((iFirst * 16) And 48)
                strReturn = strReturn & "="
            End If
            strReturn = strReturn & "="
        End If

        '** Return the encoded result string **
        base64Encode = strReturn
    End Function
    Public Shared Function base64Decode(ByVal strEncoded As String) As String
        Dim iRealLength As Integer
        Dim strReturn As String
        Dim iBy4 As Integer
        Dim iIndex As Integer
        Dim iFirst As Integer
        Dim iSecond As Integer
        Dim iThird As Integer
        Dim iFourth As Integer
        If Len(strEncoded) = 0 Then
            base64Decode = ""
            Exit Function
        End If

        '** Base 64 encoded strings are right padded to 3 character multiples using = signs **
        '** Work out the actual length of data without the padding here **
        iRealLength = Len(strEncoded)
        Do While Mid(strEncoded, iRealLength, 1) = "="
            iRealLength = iRealLength - 1
        Loop

        '** Non standard extension to Base 64 decode to allow for + sign to space character substitution by **
        '** some web servers. Base 64 expects a +, not a space, so convert vack to + if space is found **
        Do While InStr(strEncoded, " ") <> 0
            strEncoded = Left(strEncoded, InStr(strEncoded, " ") - 1) & "+" & Mid(strEncoded, InStr(strEncoded, " ") + 1)
        Loop

        strReturn = ""
        '** Convert the base 64 4x6 byte values into 3x8 byte real values by reading 4 chars at a time **
        iBy4 = (iRealLength \ 4) * 4
        iIndex = 1
        Do While iIndex <= iBy4
            iFirst = arrBase64DecMap(Asc(Mid(strEncoded, iIndex + 0, 1)))
            iSecond = arrBase64DecMap(Asc(Mid(strEncoded, iIndex + 1, 1)))
            iThird = arrBase64DecMap(Asc(Mid(strEncoded, iIndex + 2, 1)))
            iFourth = arrBase64DecMap(Asc(Mid(strEncoded, iIndex + 3, 1)))
            strReturn = strReturn & Chr(((iFirst * 4) And 255) + ((iSecond \ 16) And 3))
            strReturn = strReturn & Chr(((iSecond * 16) And 255) + ((iThird \ 4) And 15))
            strReturn = strReturn & Chr(((iThird * 64) And 255) + (iFourth And 63))
            iIndex = iIndex + 4
        Loop

        '** For non multiples of 4 characters, handle the = padding **
        If iIndex < iRealLength Then
            iFirst = arrBase64DecMap(Asc(Mid(strEncoded, iIndex + 0, 1)))
            iSecond = arrBase64DecMap(Asc(Mid(strEncoded, iIndex + 1, 1)))
            strReturn = strReturn & Chr(((iFirst * 4) And 255) + ((iSecond \ 16) And 3))
            If iRealLength Mod 4 = 3 Then
                iThird = arrBase64DecMap(Asc(Mid(strEncoded, iIndex + 2, 1)))
                strReturn = strReturn & Chr(((iSecond * 16) And 255) + ((iThird \ 4) And 15))
            End If
        End If

        base64Decode = strReturn
    End Function

    ' ** The SimpleXor encryption algorithm. **
    ' ** NOTE: This is a placeholder really.  Future releases of Sage Pay Form will use AES **
    ' ** This simple function and the Base64 will deter script kiddies and prevent the "View Source" type tampering **
    Public Shared Function simpleXor(ByVal strIn As String, ByVal strKey As String) As String
        Dim iInIndex As Integer
        Dim iKeyIndex As Integer
        Dim strReturn As String
        If Len(strIn) = 0 Or Len(strKey) = 0 Then
            simpleXor = ""
            Exit Function
        End If

        iInIndex = 1
        iKeyIndex = 1
        strReturn = ""

        '** Step through the plain text source XORing the character at each point with the next character in the key **
        '** Loop through the key characters as necessary **
        Do While iInIndex <= Len(strIn)
            strReturn = strReturn & Chr(Asc(Mid(strIn, iInIndex, 1)) Xor Asc(Mid(strKey, iKeyIndex, 1)))
            iInIndex = iInIndex + 1
            If iKeyIndex = Len(strKey) Then iKeyIndex = 0
            iKeyIndex = iKeyIndex + 1
        Loop

        simpleXor = strReturn
    End Function

    '** Wrapper function do encrypt an encode based on strEncryptionType setting **
    Public Shared Function EncryptAndEncode(ByVal strIn As String) As String
        If strEncryptionType = "XOR" Then
            '** XOR encryption with Base64 encoding **
            EncryptAndEncode = base64Encode(simpleXor(strIn, strEncryptionPassword))
        Else
            '** AES encryption, CBC blocking with PKCS5 padding then HEX encoding - DEFAULT **
            EncryptAndEncode = "@" & byteArrayToHexString(aesEncrypt(strIn))
        End If
    End Function

    '** Wrapper function do decode then decrypt based on header of the encrypted field **
    Public Shared Function DecodeAndDecrypt(ByVal strIn As String) As String
        If Left(strIn, 1) = "@" Then
            '** HEX decoding then AES decryption, CBC blocking with PKCS5 padding - DEFAULT **
            DecodeAndDecrypt = aesDecrypt(hexStringToBytes(strIn.Substring(1)))
        Else
            '** Base 64 decoding plus XOR decryption **
            DecodeAndDecrypt = simpleXor(base64Decode(strIn), strEncryptionPassword)
        End If
    End Function

    ''' <summary>
    ''' Encrypts input string using Rijndael (AES) algorithm with CBC blocking and PKCS7 padding.
    ''' </summary>
    ''' <param name="inputText">text string to encrypt </param>
    ''' <returns>Encrypted text in Byte array</returns>
    ''' <remarks>The key and IV are the same, and use strEncryptionPassword.</remarks>
    Private Shared Function aesEncrypt(ByVal inputText As String) As Byte()

        Dim AES As New RijndaelManaged
        Dim outBytes() As Byte

        'set the mode, padding and block size for the key
        AES.Padding = PaddingMode.PKCS7
        AES.Mode = CipherMode.CBC
        AES.KeySize = 128
        AES.BlockSize = 128

        'convert key and plain text input into byte arrays
        Dim keyAndIvBytes() As Byte = UTF8.GetBytes(strEncryptionPassword)
        Dim inputBytes() As Byte = UTF8.GetBytes(inputText)

        'create streams and encryptor object
        Dim memoryStream As New MemoryStream()
        Dim cryptoStream As New CryptoStream(memoryStream, AES.CreateEncryptor(keyAndIvBytes, keyAndIvBytes), CryptoStreamMode.Write)

        'perform encryption
        cryptoStream.Write(inputBytes, 0, inputBytes.Length)
        cryptoStream.FlushFinalBlock()

        'get encrypted stream into byte array
        outBytes = memoryStream.ToArray

        'close streams
        memoryStream.Close()
        cryptoStream.Close()
        AES.Clear()

        Return outBytes

    End Function

    ''' <summary>
    ''' Decrypts input string from Rijndael (AES) algorithm with CBC blocking and PKCS7 padding.
    ''' </summary>
    ''' <param name="inputBytes">Encrypted binary array to decrypt</param>
    ''' <returns>string of Decrypted data</returns>
    ''' <remarks>The key and IV are the same, and use strEncryptionPassword.</remarks>
    Private Shared Function aesDecrypt(ByVal inputBytes() As Byte) As String

        Dim AES As New RijndaelManaged
        Dim keyAndIvBytes() As Byte = UTF8.GetBytes(strEncryptionPassword)
        Dim outputBytes(inputBytes.Length) As Byte

        'set the mode, padding and block size
        AES.Padding = PaddingMode.PKCS7
        AES.Mode = CipherMode.CBC
        AES.KeySize = 128
        AES.BlockSize = 128

        'create streams and decryptor object
        Dim memoryStream = New MemoryStream(inputBytes)
        Dim cryptoStream = New CryptoStream(memoryStream, AES.CreateDecryptor(keyAndIvBytes, keyAndIvBytes), CryptoStreamMode.Read)

        'perform decryption
        cryptoStream.Read(outputBytes, 0, outputBytes.Length)

        'close streams
        memoryStream.Close()
        cryptoStream.Close()
        AES.Clear()

        'convert decryted data into string, assuming original text was UTF-8 encoded
        Return UTF8.GetString(outputBytes)

    End Function

    ''' <summary>
    ''' Converts a string of characters representing hexadecimal values into an array of bytes
    ''' </summary>
    ''' <param name="strInput">A hexadecimal string of characters to convert to binary.</param>
    ''' <returns>A byte array</returns>
    Public Shared Function hexStringToBytes(ByVal strInput As String) As Byte()

        Dim numBytes As Integer = (strInput.Length / 2)
        Dim bytes(numBytes - 1) As Byte

        For x As Integer = 0 To numBytes - 1
            bytes(x) = System.Convert.ToByte(strInput.Substring(x * 2, 2), 16)
        Next

        Return bytes

    End Function

    ''' <summary>
    ''' Converts an array of bytes into a hexadecimal string representation.
    ''' </summary>
    ''' <param name="ba">Array of bytes to convert</param>
    ''' <returns>String of hexadecimal values.</returns>
    Public Shared Function byteArrayToHexString(ByVal ba As Byte()) As String

        Return BitConverter.ToString(ba).Replace("-", "")

    End Function

    '** The getToken function. **
    '** NOTE: A function of convenience that extracts the value from the "name=value&name2=value2..." Sage Pay Form reply string **
    '** Does not use the split function because returned values can contain & or =, so it looks for known names and **
    '** removes those, leaving only the field selected ** 
    Public Shared Function getToken(ByVal strList As String, ByVal strRequired As String) As String
        Dim arrTokens(16) As String
        Dim strTokenValue As String
        Dim iIndex As Integer

        arrTokens.SetValue("Status", 0)
        arrTokens.SetValue("StatusDetail", 1)
        arrTokens.SetValue("VendorTxCode", 2)
        arrTokens.SetValue("VPSTxId", 3)
        arrTokens.SetValue("TxAuthNo", 4)
        arrTokens.SetValue("Amount", 5)
        arrTokens.SetValue("AVSCV2", 6)
        arrTokens.SetValue("AddressResult", 7)
        arrTokens.SetValue("PostCodeResult", 8)
        arrTokens.SetValue("CV2Result", 9)
        arrTokens.SetValue("GiftAid", 10)
        arrTokens.SetValue("3DSecureStatus", 11)
        arrTokens.SetValue("CAVV", 12)
        arrTokens.SetValue("CardType", 13)
        arrTokens.SetValue("Last4Digits", 14)
        arrTokens.SetValue("PayerStatus", 15)
        arrTokens.SetValue("AddressStatus", 16)

        '** If the toekn we're after isn't in the list, return nothing **
        If InStr(strList, strRequired + "=") = 0 Then
            getToken = ""
            Exit Function
        Else
            '** The token is present, so ignore everything before it in the list **    
            strTokenValue = Mid(strList, InStr(strList, strRequired) + Len(strRequired) + 1)

            '** Strip off all remaining tokens if they are present **
            iIndex = LBound(arrTokens)
            Do While iIndex <= UBound(arrTokens)
                If arrTokens(iIndex) <> strRequired Then
                    If InStr(strTokenValue, "&" + arrTokens(iIndex)) <> 0 Then
                        strTokenValue = Left(strTokenValue, InStr(strTokenValue, "&" + arrTokens(iIndex)) - 1)
                    End If
                End If
                iIndex = iIndex + 1
            Loop

            getToken = strTokenValue
        End If

    End Function

    '** Inspects and validates user input for a name field. Returns TRUE if input value is valid as a name field.
    '   Parameter "strInputValue" is the field value to validate.
    '   Parameter "returnedResult" sets a result to a value from enum FieldValidationResult.
    Public Shared Function isValidNameField(ByVal strInputValue As String, _
            Optional ByRef returnedResult As FieldValidationResult = FieldValidationResult.Invalid) As Boolean

        Dim strAllowableChars As String = " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-.'&\"
        strInputValue = Trim(strInputValue)
        returnedResult = validateString(strInputValue, strAllowableChars, True, True, 20)
        If returnedResult = FieldValidationResult.Valid Then
            Return True
        Else
            Return False
        End If
    End Function

    '** Inspects and validates user input for an Address field.
    '   Parameter "isRequired" specifies whether "strInputValue" must have a non-null and non-empty value.
    Public Shared Function isValidAddressField(ByVal strInputValue As String, _
            ByVal isRequired As Boolean, _
            Optional ByRef returnedResult As FieldValidationResult = FieldValidationResult.Invalid) As Boolean

        Dim strAllowableChars As String = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-.',/\()&:+" & vbCr & vbLf
        strInputValue = Trim(strInputValue)
        returnedResult = validateString(strInputValue, strAllowableChars, True, isRequired, 100)
        If returnedResult = FieldValidationResult.Valid Then
            Return True
        Else
            Return False
        End If
    End Function

    '** Inspects and validates user input for a City field.
    Public Shared Function isValidCityField(ByVal strInputValue As String, _
            Optional ByRef returnedResult As FieldValidationResult = FieldValidationResult.Invalid) As Boolean

        Dim strAllowableChars As String = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-.',/\()&:+" & vbCr & vbLf
        strInputValue = Trim(strInputValue)
        returnedResult = validateString(strInputValue, strAllowableChars, True, True, 40)
        If returnedResult = FieldValidationResult.Valid Then
            Return True
        Else
            Return False
        End If
    End Function

    '** Inspects and validates user input for a Postcode/zip field. 
    Public Shared Function isValidPostcodeField(ByVal strInputValue As String, _
            Optional ByRef returnedResult As FieldValidationResult = FieldValidationResult.Invalid) As Boolean

        Dim strAllowableChars As String = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-"
        strInputValue = Trim(strInputValue)
        returnedResult = validateString(strInputValue, strAllowableChars, False, True, 10)
        If returnedResult = FieldValidationResult.Valid Then
            Return True
        Else
            Return False
        End If
    End Function

    '** Inspects and validates user input for an email field. 
    Public Shared Function isValidEmailField(ByVal strInputValue As String, _
            Optional ByRef returnedResult As FieldValidationResult = FieldValidationResult.Invalid) As Boolean

        'The allowable e-mail address format accepted by the SagePay gateway must be RFC 5321/5322 compliant (see RFC 3696)  
        Dim sEmailRegXPattern = "^[a-z0-9\xC0-\xFF!#$%&amp;'*+/=?^_`{|}~\p{L}\p{M}*-]+(?:\.[a-z0-9\xC0-\xFF!#$%&amp;'*+/=?^_`{|}~\p{L}\p{M}*-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[a-z]{2,3}|com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|at|coop|travel)$"
        strInputValue = Trim(strInputValue)
        returnedResult = validateString(strInputValue, sEmailRegXPattern, False)
        If returnedResult = FieldValidationResult.Valid Then
            Return True
        Else
            Return False
        End If
    End Function

    '** Inspects and validates user input for a phone field. 
    Public Shared Function isValidPhoneField(ByVal strInputValue As String, _
            Optional ByRef returnedResult As FieldValidationResult = FieldValidationResult.Invalid) As Boolean

        Dim strAllowableChars As String = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-()+"
        strInputValue = Trim(strInputValue)
        returnedResult = validateString(strInputValue, strAllowableChars, False, False, 20)
        If returnedResult = FieldValidationResult.Valid Then
            Return True
        Else
            Return False
        End If
    End Function

    '** A generic function used to inspect and validate a string from user input.
    '   Parameter "strInputValue" is the value to perform validation on.
    '   Parameter "strAllowableChars" is a string of characters allowable in "strInputValue" if its to be deemed valid.
    '   Parameter "allowAccentedChars" determines if "strInputValue" can contain Accented or High-order characters.
    '   Parameter "isRequired" specifies whether "strInputValue" must have a non-null and non-empty value.
    '   Parameter "intMaxLength" specifies the maximum allowable length of "strInputValue".
    '   Parameter "intMinLength" specifies the miniumum allowable length of "strInputValue".
    '   Returns a result from enum FieldValidationResult. 
    Private Shared Function validateString(ByVal strInputValue As String, _
                                           ByVal strAllowableChars As String, _
                                           ByVal allowAccentedChars As Boolean, _
                                           ByVal isRequired As Boolean, _
                                           Optional ByVal intMaxLength As Integer = -1, _
                                           Optional ByVal intMinLength As Integer = 0) As FieldValidationResult

        If isRequired = True And String.IsNullOrEmpty(strInputValue) = True Then

            Return FieldValidationResult.Invalid_RequiredInputValueMissing

        ElseIf (intMaxLength <> -1) And (strInputValue.Length > intMaxLength) Then

            Return FieldValidationResult.Invalid_MaximumLengthExceeded

        ElseIf strInputValue <> cleanInput(strInputValue, strAllowableChars, allowAccentedChars) Then

            Return FieldValidationResult.Invalid_BadCharacters

        ElseIf (isRequired = True) And (strInputValue.Length < intMinLength) Then

            Return FieldValidationResult.Invalid_MinimumLengthNotMet

        ElseIf (isRequired = False) And (String.IsNullOrEmpty(strInputValue) = False) And (strInputValue.Length < intMinLength) Then

            Return FieldValidationResult.Invalid_MinimumLengthNotMet
        Else
            Return FieldValidationResult.Valid
        End If
    End Function

    '** A generic function to inspect and validate a string from user input based on a Regular Expression pattern.
    '   Parameter "strInputValue" is the value to perform validation on.
    '   Parameter "strRegExPattern" is a Regular Expression string pattern used to validate against "strInputValue".
    '   Parameter "isRequired" specifies whether "strInputValue" must have a non-null and non-empty value.
    '   Returns a result from enum FieldValidationResult. 
    Private Shared Function validateString(ByVal strInputValue As String, _
                                           ByVal strRegExPattern As String, _
                                           ByVal isRequired As Boolean) As FieldValidationResult

        If isRequired = True And String.IsNullOrEmpty(strInputValue) = True Then

            Return FieldValidationResult.Invalid_RequiredInputValueMissing

        ElseIf (String.IsNullOrEmpty(strInputValue) = False) And (Regex.IsMatch(strInputValue.toLower, strRegExPattern) = False) Then

            Return FieldValidationResult.Invalid_BadFormat
        Else
            Return FieldValidationResult.Valid
        End If
    End Function

    '** Maps a FieldValidationResult value to a string representing a user friendly validation error message.
    '   Parameter "strFieldLabelName" is the display name of the form field to use in the returned message.
    Public Shared Function getValidationMessage(ByVal validationCode As FieldValidationResult, _
                                                ByVal strFieldLabelName As String) As String
        Dim strReturn As String = ""

        Select Case validationCode

            Case FieldValidationResult.Invalid_BadCharacters
                strReturn = "Please correct " & strFieldLabelName & " as it contains disallowed characters."

            Case FieldValidationResult.Invalid_BadFormat
                strReturn = "Please correct " & strFieldLabelName & " as the format is invalid."

            Case FieldValidationResult.Invalid_MinimumLengthNotMet
                strReturn = "Please correct " & strFieldLabelName & " as the value is not long enough."

            Case FieldValidationResult.Invalid_MaximumLengthExceeded
                strReturn = "Please correct " & strFieldLabelName & " as the value is too long."

            Case FieldValidationResult.Invalid_RequiredInputValueMissing
                strReturn = "Please enter a value for " & strFieldLabelName & " where requested below."

            Case FieldValidationResult.Invalid_RequiredInputValueNotSelected
                strReturn = "Please select a value for " & strFieldLabelName & " where requested below."
        End Select

        Return strReturn
    End Function

    '** Defines filter types used for a parameter in the cleanInput() function.
    Public Enum CleanInputFilterType
        Alphabetic
        AlphabeticAndAccented
        AlphaNumeric
        AlphaNumericAndAccented
        Numeric
        WidestAllowableCharacterRange
    End Enum

    '** Defines a set of values used as outcomes in field validation functions such as isValidAddressField.
    Public Enum FieldValidationResult
        Valid
        Invalid
        Invalid_BadCharacters
        Invalid_BadFormat
        Invalid_MaximumLengthExceeded
        Invalid_MinimumLengthNotMet
        Invalid_RequiredInputValueMissing
        Invalid_RequiredInputValueNotSelected
    End Enum

End Class

