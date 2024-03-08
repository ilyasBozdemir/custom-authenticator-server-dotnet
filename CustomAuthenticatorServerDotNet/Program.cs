using CustomAuthenticatorServerDotNet;


string secret = SecretGenerator.GenerateSecret();
var counter = OTPManager.GetCurrentCounter();
string otp = OTPManager.GenerateOTP(secret, counter);
bool authenticationSuccess = false;
string userInput;
bool isValid = false;
OTPUriBuilder uriBuilder = new OTPUriBuilder();
DateTime nextOTPUpdateTime = DateTime.UtcNow.AddSeconds(OTPManager.IntervalLength);


ConsoleHelper.WriteColoredLine("=== Custom Authenticator Server ===\n".ToUpper(), ConsoleColors.Title);
ConsoleHelper.WriteColored("Generated Secret: ", ConsoleColors.Default);
ConsoleHelper.WriteColored(secret + "\n", ConsoleColors.Info);
ConsoleHelper.WriteColored("OTP Generated Date :", ConsoleColors.Default);
ConsoleHelper.WriteColored(OTPManager.GetCurrentTime() + "\n", ConsoleColors.Info);
ConsoleHelper.WriteColored("OTP Expired Date :", ConsoleColors.Default);
ConsoleHelper.WriteColored(OTPManager.GetExpirationTime() + "\n", ConsoleColors.Info);

ConsoleHelper.WriteColored("Generated OTP :", ConsoleColors.Default);
ConsoleHelper.WriteColored(otp + "\n", ConsoleColors.Info);

while (!authenticationSuccess)
{
    if (DateTime.UtcNow >= nextOTPUpdateTime)
    {
        counter = OTPManager.GetCurrentCounter();
        otp = OTPManager.GenerateOTP(secret, counter);

        ConsoleHelper.WriteColored("New OTP Generated Date :", ConsoleColors.Default);
        ConsoleHelper.WriteColored(OTPManager.GetCurrentTime() + "\n", ConsoleColors.Info);

        ConsoleHelper.WriteColored("New OTP Expired Date :", ConsoleColors.Default);
        ConsoleHelper.WriteColored(OTPManager.GetExpirationTime() + "\n", ConsoleColors.Info);

        ConsoleHelper.WriteColored("Generated OTP :", ConsoleColors.Default);
        ConsoleHelper.WriteColored(otp + "\n", ConsoleColors.Default);
        nextOTPUpdateTime = DateTime.UtcNow.AddSeconds(OTPManager.IntervalLength);

   
    }

    do
    {
        ConsoleHelper.WriteColored("Enter the OTP from your authenticator app:", ConsoleColors.Default);
        Console.ForegroundColor = ConsoleColors.Prompt;

        userInput = Console.ReadLine();
        isValid = CheckInput(userInput);
   
    }
    while (!isValid);

    Console.WriteLine();


    if (OTPManager.ValidateOTP(secret, userInput, out int remainingSeconds))
    {
        Console.ForegroundColor = ConsoleColors.Success;
        Console.WriteLine("Authentication successful!");
        authenticationSuccess = true;
        RecoveryCodeGenerator recoveryCodeGenerator = new RecoveryCodeGenerator();

        var list = recoveryCodeGenerator.GenerateRecoveryCodes();
        Console.ForegroundColor = ConsoleColors.Default;
        var qrcodeUri = uriBuilder.GenerateQrCodeUri(secret, "İlyas Bozdemir", "bozdemir.ib70@gmail.com");

        (string sharedKey, string title, string email) = uriBuilder.DecodeQrCodeUri(qrcodeUri);

        Console.WriteLine($"qrcodeUri:\"{qrcodeUri}\"\n");
        ConsoleHelper.WriteColoredLine("bu uri ile qrcode olusturup 30 sn de bir üretilen kod ile dogrulama yapabilirsiniz.", ConsoleColors.Info);
        ConsoleHelper.WriteColoredLine("sadece çalışma mantığı ele alınmıştır.", ConsoleColors.Info);
        ConsoleHelper.WriteColoredLine("dogrulama yaparken secret keyin static olmasına dikkat edin bu uygulama her run edilince key değişmektedir.", ConsoleColors.Warning);
        Console.WriteLine($"sharedKey:{sharedKey} title:{title}  email:{email}");

        Console.WriteLine("=== Recovery Code ===\n");
        foreach (var item in list)
            ConsoleHelper.WriteColoredLine(item, ConsoleColors.Success);
        Console.WriteLine();
    }
    else
    {
        Console.ForegroundColor = ConsoleColors.Error;
        Console.WriteLine("Authentication failed!");
        Console.ForegroundColor = ConsoleColors.Default;

    }
}

static bool CheckInput(string input)
{
    if (input.Length != 6)
    {
        ConsoleHelper.WriteColored("Girdiğiniz sayı 6 karakterden oluşmalıdır.\n", ConsoleColors.Error);
        return false;
    }

    foreach (char c in input)
    {
        if (!char.IsDigit(c))
        {
            ConsoleHelper.WriteColored("Girdiğiniz sayı sadece rakamlardan oluşmalıdır.\n", ConsoleColors.Error);
            return false;
        }
    }
    return true;
}




Console.ReadLine();
