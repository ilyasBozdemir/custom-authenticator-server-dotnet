using CustomAuthenticatorServerDotNet;

Console.ForegroundColor = ConsoleColors.Title;
Console.WriteLine("=== Custom Authenticator Server ===".ToUpper());
Console.WriteLine();
Console.ForegroundColor = ConsoleColors.Default;
string secret = SecretGenerator.GenerateSecret();
Console.Write("Generated Secret: ");
Console.ForegroundColor = ConsoleColors.Info;
Console.Write(secret);
Console.ForegroundColor = ConsoleColors.Default;
var counter = OTPManager.GetCurrentCounter();
string otp = OTPManager.GenerateOTP(secret, counter);
Console.WriteLine();
Console.Write("OTP Generated Date : ");
Console.ForegroundColor = ConsoleColors.Info;
Console.Write(OTPManager.GetCurrentTime());
Console.ForegroundColor = ConsoleColors.Default;
Console.WriteLine();
Console.Write("OTP Expired Date : ");
Console.ForegroundColor = ConsoleColors.Info;
Console.Write(OTPManager.GetExpirationTime());
Console.ForegroundColor = ConsoleColors.Default;
Console.WriteLine();
Console.Write("Generated OTP: ");
Console.ForegroundColor = ConsoleColors.Info;
Console.Write(otp);
Console.ForegroundColor = ConsoleColors.Default;
bool authenticationSuccess = false;
DateTime nextOTPUpdateTime = DateTime.UtcNow.AddSeconds(OTPManager.IntervalLength);
while (!authenticationSuccess)
{
    if (DateTime.UtcNow >= nextOTPUpdateTime)
    {
        counter = OTPManager.GetCurrentCounter();
        otp = OTPManager.GenerateOTP(secret, counter);

        Console.Write("New OTP Generated Date : ");
        Console.ForegroundColor = ConsoleColors.Info;
        Console.Write(OTPManager.GetCurrentTime());
        Console.ForegroundColor = ConsoleColors.Default;
        Console.WriteLine();
        Console.Write("OTP Expired Date : ");
        Console.ForegroundColor = ConsoleColors.Info;
        Console.Write(OTPManager.GetExpirationTime());
        Console.ForegroundColor = ConsoleColors.Default;
        Console.WriteLine();
        Console.Write("Generated OTP: ");
        Console.ForegroundColor = ConsoleColors.Info;
        Console.Write(otp);
        Console.ForegroundColor = ConsoleColors.Default;
        nextOTPUpdateTime = DateTime.UtcNow.AddSeconds(OTPManager.IntervalLength);

    }
    Console.WriteLine();
    Console.Write("Enter the OTP from your authenticator app:");
    Console.ForegroundColor = ConsoleColors.Prompt;
    string userInput = Console.ReadLine();
    Console.ForegroundColor = ConsoleColors.Default;
    Console.WriteLine();

    if (OTPManager.ValidateOTP(secret, userInput, out int remainingSeconds))
    {
        Console.ForegroundColor = ConsoleColors.Success;
        Console.WriteLine("Authentication successful!");
        authenticationSuccess = true;
        Console.ForegroundColor = ConsoleColors.Default;
    }
    else
    {
        Console.ForegroundColor = ConsoleColors.Error;
        Console.WriteLine("Authentication failed! Remaining seconds: " + remainingSeconds);
        Console.ForegroundColor = ConsoleColors.Default;

    }
}

Console.ReadLine();
