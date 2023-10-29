using CoMon.Debugging;

namespace CoMon
{
    public class CoMonConsts
    {
        public const string LocalizationSourceName = "CoMon";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = false;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "3d4024248a58410285a8264ef64b5a95";
    }
}
