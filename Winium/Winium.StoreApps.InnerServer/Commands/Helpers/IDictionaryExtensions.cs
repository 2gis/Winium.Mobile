namespace Winium.StoreApps.InnerServer.Commands.Helpers
{
    #region

    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    #endregion

    public static class DictionaryExtensions
    {
        #region Public Methods and Operators

        public static bool TryGetValue<T>(this IDictionary<string, JToken> dictionary, string key, out T value)
        {
            JToken jsonValue;
            value = default(T);
            if (!dictionary.TryGetValue(key, out jsonValue))
            {
                return false;
            }

            value = jsonValue.ToObject<T>();
            return true;
        }

        #endregion
    }
}
