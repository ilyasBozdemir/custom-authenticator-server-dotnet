using System.Security.Cryptography;
using System.Timers;


namespace CustomAuthenticatorServerDotNet;

public class OTPManager
{
    private static System.Timers.Timer _timer;
    private static ulong _startTime;
    private static int _digitLength = 6;
    private static int _windowSize = 3;
    private static int _intervalLength = 10;


    public static int DigitLength
    {
        get { return _digitLength; }
        private set { _digitLength = value; }
    }
    public static int WindowSize
    {
        get { return _windowSize; }
        private set { _windowSize = value; }
    }
    public static int IntervalLength
    {
        get { return _intervalLength; }
       private set { _intervalLength = value; }
    }

    public static bool IsCodeExpired { get; set; } = false;

    public int RemainingSeconds { get; set; }

    static void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        IsCodeExpired = false;
    }

    public static string GenerateOTP(string secret, ulong counter)
    {

        _timer = new System.Timers.Timer(_intervalLength * 1000);


        _timer.Elapsed += Timer_Elapsed;

        _timer.Start();

        Task.Delay(_intervalLength * 1000).ContinueWith(t => _timer.Stop());

        byte[] counterBytes = BitConverter.GetBytes(counter);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(counterBytes);

        HMACSHA1 hmac = new HMACSHA1(Convert.FromBase64String(secret));
        byte[] hash = hmac.ComputeHash(counterBytes);

        int offset = hash[hash.Length - 1] & 0x0F;
        int binary = ((hash[offset] & 0x7f) << 24) |
                     ((hash[offset + 1] & 0xff) << 16) |
                     ((hash[offset + 2] & 0xff) << 8) |
                     (hash[offset + 3] & 0xff);
        int otp = binary % (int)Math.Pow(10, _digitLength);

        _startTime = GetCurrentTime();
        return otp.ToString().PadLeft(_digitLength, '0');
    }

    public static bool ValidateOTP(string secret, string userInput, out int remainingSeconds)
    {
        ulong counter = GetCurrentCounter();
        ulong elapsedTime = GetCurrentTime() - GetStartTime();

        remainingSeconds = (int)(_intervalLength - (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds % _intervalLength);

        if (IsExpired(elapsedTime))
        {
            Console.ForegroundColor = ConsoleColors.Error;
            Console.WriteLine("Invalid because time has expired.");
            Console.ForegroundColor = ConsoleColors.Default;
            return false; 
        }

        IsExpired(elapsedTime);

        for (int i = -_windowSize; i <= _windowSize; i++)
        {
            ulong currentCounter = counter + (ulong)i;
            string expectedOTP = GenerateOTP(secret, currentCounter);
            if (userInput == expectedOTP)
            {
                return true;
            }
        }
        return false;
    }


    public static bool IsExpired(ulong elapsedTime)
    {
        if (elapsedTime >= (ulong)_intervalLength)
            return true; 
        return false;
    }

    public static ulong GetExpirationTime()
    {
        ulong elapsedTime = GetCurrentTime() - GetStartTime();

        return GetCurrentTime() + (ulong)_intervalLength - (elapsedTime % (ulong)_intervalLength);
    }
    public static ulong GetExpirationTime(ulong elapsedTime)
    {
        return GetCurrentTime() + (ulong)_intervalLength - (elapsedTime % (ulong)_intervalLength);
    }

    public static ulong GetCurrentTime()
    {
        TimeSpan unixTimeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return Convert.ToUInt64(unixTimeSpan.TotalSeconds);
    }

    private static ulong GetStartTime()
    {
        return _startTime;
    }

    public static ulong GetCurrentCounter()
    {
        TimeSpan unixTimeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return Convert.ToUInt64(unixTimeSpan.TotalSeconds / _intervalLength);

        //return (ulong)((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds / _intervalLength);
    }
}
