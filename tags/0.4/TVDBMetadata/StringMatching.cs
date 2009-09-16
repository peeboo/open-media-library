using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVDBMetadata
{
    static class StringMatching
    {
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        /*public static string SoundEX(string str)
        {
            StringBuilder result = new StringBuilder();

            if (!string.IsNullOrEmpty(str))
            {
                string previousCode = "";
                string currentCode = "";
                string currentLetter = "";

                result.Append(str.Substring(0,1));

                for (int it= 1; int < str.Length; int++)
                {
                    currentLetter = str.Substring(int, 1).ToLower();
                    currentCode = "";

                    if ("bfpv".IndexOf(currentLetter) > -1)
                        currentCode = "1";
                    else
                        if ("cgjkqsxz".IndexOf(currentLetter) > -1)
                        currentCode = "2";
                    else
                        if ("dt".IndexOf(currentLetter) > -1)
                        currentCode = "3";
                    else
                        if ("l".IndexOf(currentLetter) > -1)
                        currentCode = "4";
                    else
                        if ("mn".IndexOf(currentLetter) > -1)
                        currentCode = "5";
                    else
                        if ("r".IndexOf(currentLetter) > -1)
                        currentCode = "6";
                
                    if (currentCode != previousCode)
                        result.Append(currentCode);

                    if (result.Length == 4) break;



                }
            }
        }*/
    }
}
