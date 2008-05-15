using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace OMLEngine
{
    public class RegexUtils
    {
        public static Match[] FindSubstrings(string source, string matchPattern, bool findAllUnique)
        {
            SortedList uniqueMatches = new SortedList();
            Match[] retArray = null;

            Regex RE = new Regex(matchPattern, RegexOptions.Multiline);
            MatchCollection theMatches = RE.Matches(source);

            if (findAllUnique)
            {
                for (int counter = 0; counter < theMatches.Count; counter++)
                {
                    if (!uniqueMatches.ContainsKey(theMatches[counter].Value))
                    {
                        uniqueMatches.Add(theMatches[counter].Value,
                                          theMatches[counter]);
                    }
                }

                retArray = new Match[uniqueMatches.Count];
                uniqueMatches.Values.CopyTo(retArray, 0);
            }
            else
            {
                retArray = new Match[theMatches.Count];
                theMatches.CopyTo(retArray, 0);
            }

            return (retArray);
        }
        public static ArrayList ExtractGroupings(string source, string matchPattern, bool wantInitialMatch)
        {
            ArrayList keyedMatches = new ArrayList();
            int startingElement = 1;
            if (wantInitialMatch)
            {
                startingElement = 0;
            }

            Regex RE = new Regex(matchPattern, RegexOptions.Multiline);
            MatchCollection theMatches = RE.Matches(source);

            foreach (Match m in theMatches)
            {
                Hashtable groupings = new Hashtable();

                for (int counter = startingElement;
                  counter < m.Groups.Count; counter++)
                {
                    // If we had just returned the MatchCollection directly, the
                    //  GroupNameFromNumber method would not be available to use
                    groupings.Add(RE.GroupNameFromNumber(counter),
                                   m.Groups[counter]);
                }

                keyedMatches.Add(groupings);
            }

            return (keyedMatches);
        }
        public static bool VerifyRegEx(string testPattern)
        {
            bool isValid = true;

            if ((testPattern != null) && (testPattern.Trim().Length > 0))
            {
                try
                {
                    Regex.Match("", testPattern);
                }
                catch (ArgumentException)
                {
                    // BAD PATTERN: Syntax error
                    isValid = false;
                }
            }
            else
            {
                //BAD PATTERN: Pattern is null or blank
                isValid = false;
            }

            return (isValid);
        }
        public static string Replace(string source, char matchPattern, string replaceStr)
        {
            return (Replace(source, matchPattern.ToString(), replaceStr, -1, 0));
        }
        public static string Replace(string source, char matchPattern, string replaceStr, int count)
        {
            return (Replace(source.ToString(), matchPattern.ToString(), replaceStr,
                                 count, 0));
        }
        public static string Replace(string source, char matchPattern, string replaceStr, int count, int startPos)
        {
            return (Replace(source.ToString(), matchPattern.ToString(), replaceStr,
                            count, startPos));
        }
        public static string Replace(string source, string matchPattern, string replaceStr)
        {
            return (Replace(source, matchPattern, replaceStr, -1, 0));
        }
        public static string Replace(string source, string matchPattern, string replaceStr, int count)
        {
            return (Replace(source, matchPattern, replaceStr, count, 0));
        }
        public static string Replace(string source, string matchPattern, string replaceStr, int count, int startPos)
        {
            Regex RE = new Regex(matchPattern);
            string newString = RE.Replace(source, replaceStr, count, startPos);

            return (newString);
        }
        public static string MatchHandler(Match theMatch)
        {
            // Handle Top property of the Property tag
            if (theMatch.Value.StartsWith("<Property"))
            {
                long topPropertyValue = 0;

                // Obtain the numeric value of the Top property
                Match topPropertyMatch = Regex.Match(theMatch.Value,
                                                     "Top=\"([-]*\\d*)");
                if (topPropertyMatch.Success)
                {
                    if (topPropertyMatch.Groups[1].Value.Trim().Equals(""))
                    {
                        // If blank, set to zero
                        return (theMatch.Value.Replace("Top=\"\"", "Top=\"0\""));
                    }
                    else if (topPropertyMatch.Groups[1].Value.Trim().Equals("-"))
                    {
                        // If only a negative sign (syntax error), set to zero
                        return (theMatch.Value.Replace("Top=\"-\"", "Top=\"0\""));
                    }
                    else
                    {
                        // We have a valid number
                        // Convert the matched string to a numeric value
                        topPropertyValue = long.Parse(
                                           topPropertyMatch.Groups[1].Value,
                                           System.Globalization.NumberStyles.Any);

                        // If the Top property is out of the specified 
                        //    range, set it to zero
                        if (topPropertyValue < 0 || topPropertyValue > 5000)
                        {
                            return (theMatch.Value.Replace("Top=\"" +
                                                           topPropertyValue +
                                                           "\"", "Top=\"0\""));
                        }
                    }
                }
            }

            return (theMatch.Value);
        }
        public static void ComplexReplace(string matchPattern, string source)
        {
            MatchEvaluator replaceCallback = new MatchEvaluator(MatchHandler);
            Regex RE = new Regex(matchPattern, RegexOptions.Multiline);
            string newString = RE.Replace(source, replaceCallback);

            Console.WriteLine("Replaced String = " + newString);
        }
        public static void CreateRegExDLL(string assmName)
        {
            RegexCompilationInfo[] RE = new RegexCompilationInfo[2] 
        {new RegexCompilationInfo("PATTERN", RegexOptions.Compiled, 
                                  "CompiledPATTERN", "Chapter_Code", true),
         new RegexCompilationInfo("NAME", RegexOptions.Compiled, 
                                  "CompiledNAME", "Chapter_Code", true)};

            System.Reflection.AssemblyName aName =
                 new System.Reflection.AssemblyName();
            aName.Name = assmName;

            Regex.CompileToAssembly(RE, aName);
        }
        public static long LineCount(string source, bool isFileName)
        {
            if (source != null)
            {
                string text = source;

                if (isFileName)
                {
                    FileStream FS = new FileStream(source, FileMode.Open,
                                                 FileAccess.Read, FileShare.Read);
                    StreamReader SR = new StreamReader(FS);
                    text = SR.ReadToEnd();
                    SR.Close();
                    FS.Close();
                }

                Regex RE = new Regex("\n", RegexOptions.Multiline);
                MatchCollection theMatches = RE.Matches(text);

                // Needed for files with zero length
                //   Note that a string will always have a line terminator 
                //        and thus will always have a length of 1 or more
                if (isFileName)
                {
                    return (theMatches.Count);
                }
                else
                {
                    return (theMatches.Count) + 1;
                }
            }
            else
            {
                // Handle a null source here
                return (0);
            }
        }
        public static ArrayList GetLines(string source, string pattern, bool isFileName)
        {
            string text = source;
            ArrayList matchedLines = new ArrayList();

            // If this is a file, get the entire file's text
            if (isFileName)
            {
                FileStream FS = new FileStream(source, FileMode.Open,
                                               FileAccess.Read, FileShare.Read);
                StreamReader SR = new StreamReader(FS);

                while (text != null)
                {
                    text = SR.ReadLine();

                    if (text != null)
                    {
                        // Run the regex on each line in the string
                        Regex RE = new Regex(pattern, RegexOptions.Multiline);
                        MatchCollection theMatches = RE.Matches(text);

                        if (theMatches.Count > 0)
                        {
                            // Get the line if a match was found
                            matchedLines.Add(text);
                        }
                    }
                }

                SR.Close();
                FS.Close();
            }
            else
            {
                // Run the regex once on the entire string
                Regex RE = new Regex(pattern, RegexOptions.Multiline);
                MatchCollection theMatches = RE.Matches(text);

                // Get the line for each match
                foreach (Match m in theMatches)
                {
                    int lineStartPos = GetBeginningOfLine(text, m.Index);
                    int lineEndPos = GetEndOfLine(text, (m.Index + m.Length - 1));
                    string line = text.Substring(lineStartPos,
                                                 lineEndPos - lineStartPos);
                    matchedLines.Add(line);
                }
            }

            return (matchedLines);
        }
        public static int GetBeginningOfLine(string text, int startPointOfMatch)
        {
            if (startPointOfMatch > 0)
            {
                --startPointOfMatch;
            }

            if (startPointOfMatch >= 0 && startPointOfMatch < text.Length)
            {
                // Move to the left until the first '\n char is found
                for (int index = startPointOfMatch; index >= 0; index--)
                {
                    if (text[index] == '\n')
                    {
                        return (index + 1);
                    }
                }

                return (0);
            }

            return (startPointOfMatch);
        }
        public static int GetEndOfLine(string text, int endPointOfMatch)
        {
            if (endPointOfMatch >= 0 && endPointOfMatch < text.Length)
            {
                // Move to the right until the first '\n char is found
                for (int index = endPointOfMatch; index < text.Length; index++)
                {
                    if (text[index] == '\n')
                    {
                        return (index);
                    }
                }

                return (text.Length);
            }

            return (endPointOfMatch);
        }
        public static Match FindOccurrenceOf(string source, string pattern, int occurrence)
        {
            if (occurrence < 1)
            {
                throw (new ArgumentException("Cannot be less than 1",
                                             "occurrence"));
            }

            // Make occurrence zero-based
            --occurrence;

            // Run the regex once on the source string
            Regex RE = new Regex(pattern, RegexOptions.Multiline);
            MatchCollection theMatches = RE.Matches(source);

            if (occurrence >= theMatches.Count)
            {
                return (null);
            }
            else
            {
                return (theMatches[occurrence]);
            }
        }
        public static ArrayList FindEachOccurrenceOf(string source, string pattern, int occurrence)
        {
            ArrayList occurrences = new ArrayList();

            // Run the regex once on the source string
            Regex RE = new Regex(pattern, RegexOptions.Multiline);
            MatchCollection theMatches = RE.Matches(source);

            for (int index = (occurrence - 1);
              index < theMatches.Count; index += occurrence)
            {
                occurrences.Add(theMatches[index]);
            }

            return (occurrences);
        }
    }
}
