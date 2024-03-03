namespace CustomAuthenticatorServerDotNet;
public class SecretGenerator
{
    public static string GenerateSecret(int byteLength = 20)
    {
        byte[] secretBytes = new byte[byteLength];
        Random rand = new Random();
        rand.NextBytes(secretBytes);
        return Convert.ToBase64String(secretBytes);
    }
}