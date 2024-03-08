using System.Web;

namespace CustomAuthenticatorServerDotNet;

public class OTPUriBuilder 
{
    public string GenerateQrCodeUri(string sharedKey, string title, string email)
    {
        string encodedTitle = HttpUtility.UrlEncode(title);
        string encodedEmail = HttpUtility.UrlEncode(email);


        return $"otpauth://totp/{encodedTitle}:{encodedEmail}?secret={sharedKey}&issuer={encodedTitle}";
    }
    public (string sharedKey, string title, string email) DecodeQrCodeUri(string qrCodeUri)
    {
        var uri = new Uri(qrCodeUri);

        var pathSegments = uri.AbsolutePath.Split(':');

        string decodedTitle = HttpUtility.UrlDecode(pathSegments[0]);
        string decodedEmail = HttpUtility.UrlDecode(pathSegments[1]);

        var queryParams = uri.Query.TrimStart('?').Split('&')
            .Select(param => param.Split('='))
            .ToDictionary(pair => pair[0], pair => pair[1]);

        var sharedKey = queryParams["secret"];

        return (sharedKey, decodedTitle, decodedEmail);
    }
}
