using UnityEngine;

namespace Helpers
{
    public class ExtendedPlayerPrefs : PlayerPrefs
    {
        public static bool TryGetString(string key, out string value)
        {
            value = string.Empty;

            if (HasKey(key))
            {
                value = GetString(key);
                return true;
            }

            return false;
        }
    }
}
