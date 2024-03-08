namespace CustomAuthenticatorServerDotNet;
public class RecoveryCodeGenerator
{
    public List<string> GenerateRecoveryCodes()
    {
        var recoveryCodes = new List<string>();
        var random = new Random();
        const int codeLength = 5;
        const int numberOfCodes = 16;

        for (int i = 0; i < numberOfCodes; i++)
        {
            var code1 = GenerateRandomHexCode(random, codeLength);
            var code2 = GenerateRandomHexCode(random, codeLength);
            var recoveryCode = $"{code1}-{code2}";
            recoveryCodes.Add(recoveryCode);
        }

        return recoveryCodes;
    }

    private string GenerateRandomHexCode(Random random, int length)
    {
        const string chars = "0123456789abcdef";// hex chars
        var codeChars = new char[length];

        for (int i = 0; i < length; i++)
        {
            codeChars[i] = chars[random.Next(chars.Length)];
        }

        return new string(codeChars);
    }
}
