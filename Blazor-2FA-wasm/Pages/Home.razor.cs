using OtpNet;
using QRCoder;

namespace Blazor_2FA_wasm.Pages;

public partial class Home
{
    private byte[] _secretKey;
    private string _totpUri;
    private string _qrCodeAsBase64;
    private string _userCode;
    private bool _isValid;

    protected override void OnInitialized()
    {
        // Generate a new secret key for the user
        _secretKey = KeyGeneration.GenerateRandomKey(20);
        var base32Secret = Base32Encoding.ToString(_secretKey);


        string issuer = "Blazor-2FA";
        string userEmail = "utilisateur@example.com";

        // Google Authenticator
        // Microsoft Authenticator
        // Authy
        // 1Password
        // FreeOTP, Aegis, etc.   
        _totpUri = $"otpauth://totp/{issuer}:{userEmail}?secret={base32Secret}&issuer={issuer}&digits=6";
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
            return;

        base.OnAfterRender(firstRender);

        // Créer le QR code à partir de l'URI
        QRCodeGenerator qrGenerator = new();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(_totpUri, QRCodeGenerator.ECCLevel.Q);
        Base64QRCode qrCode = new(qrCodeData);

        // Image du WR Code en base64
        _qrCodeAsBase64 = qrCode.GetGraphic(20);

        StateHasChanged();
    }

    private void VerifyCode()
    {
        if (_secretKey is null)
            return;

        if (string.IsNullOrWhiteSpace(_userCode))
            return;

        var totp = new Totp(_secretKey);

        // a step = 30 seconds
        VerificationWindow window = VerificationWindow.RfcSpecifiedNetworkDelay; // accepts 1 stpe before ans 1 step after
        window = new VerificationWindow(2, 2); // accepts 2 step before and 2 after

        _isValid = totp.VerifyTotp(_userCode, out long timeStepMatched, window);

        Console.WriteLine(timeStepMatched);
    }

}
