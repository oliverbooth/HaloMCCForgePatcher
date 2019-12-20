namespace HaloMCCForgePatcher.Helpers
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

    #endregion

    internal static class JsonHelpers
    {
        /// <summary>
        /// Iterates through a <see cref="JToken"/> by fetching the values found in numeric <see cref="String"/> keys.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IEnumerable<string> IterateStringKeys(JToken json)
        {
            List<string> values = new List<string>();

            for (int i = 1;; i++)
            {
                JToken key = json[i.ToString()];
                if (key is null)
                {
                    // end of list
                    break;
                }

                values.Add(key.Value<string>());
            }

            return values;
        }
    }
}
