using System;

namespace CoMon.Common
{
    public static class UriExtensions
    {
        public static string ToMaskedString(this Uri uri)
        {
            string userInfo = uri.UserInfo;

            if (string.IsNullOrEmpty(userInfo))
            {
                return uri.ToString();
            }

            int separatorIndex = userInfo.IndexOf(':');
            if (separatorIndex == -1)
            {
                return uri.ToString();
            }

            return new UriBuilder(uri)
            {
                UserName = userInfo.Substring(0, separatorIndex),
                Password = "****"
            }.ToString();
        }
    }
}
