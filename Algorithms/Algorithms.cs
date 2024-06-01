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

namespace NumberGamePlus.Algorithms
{
    public static class Algorithms
    {
#if WITHOUTBSOD == false
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
#endif

        public static int[][] Get0Groups(NumberValue[] values)
        {
            var result = new List<int[]>();
            for (var i = 1; i < values.Length; i++)
            {
                var list = PermutationCombinations(Enumerable.Range(0, values.Length).ToList(), i);
                foreach (var ls in list)
                {
                    var c_values = new List<NumberValue>();
                    foreach (var l in ls)
                        c_values.Add(values[l]);
                    var value_status = GetCheckedStatus(c_values.ToArray());
                    if (value_status.Overall)
                        result.Add(ls.ToArray());
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

/*        public static List<int> Get0GroupsWithSignum(List<int> sgn, List<int> num)
        {
            var result = new List<int>();
            if (sgn.Count <= 0)
                return new List<int> { num.Sum() };
            if (sgn.Count == 1)
            {
                foreach (var n in num)
                {
                    result.Add(sgn[0] + n);
                    result.Add(n - sgn[0]);
                }
                return result;
            }
            var signum = sgn[0];
            sgn.RemoveAt(0);
            var res = Get0GroupsWithSignum(sgn, num);
            foreach (var r in res)
            {
                result.Add(signum + r);
                result.Add(r - signum);
            }
            return result;
        }*/

        public static List<int> Get0GroupsWithSignums(int[] signums, int[] numbers)
        {
            List<int> result = new List<int>() { numbers.Sum() };
            foreach (var signum in signums)
            {
                List<int> dup_result = new List<int>(result);
                for (var i = 0; i < result.Count(); i++)
                    result[i] += signum;
                for (var j = 0; j < dup_result.Count(); j++)
                    dup_result[j] += -signum;
                result.AddRange(dup_result);
            }
            return result;
        }



        public static CheckedStatusClass GetCheckedStatus(NumberValue[] numberValue)
        {
            var checkstatus = new CheckedStatusClass();
            var sum = int.MinValue;
            var numbers = new List<int>();
            var signums = new List<int>();
            foreach (var n_value in numberValue)
            {

                if (n_value.Type != NumberValue.NumberType.Common)
                {
                    checkstatus.ContainsUncommonNumber = true;
                    if (n_value.Type != NumberValue.NumberType.Double && n_value.Type != NumberValue.NumberType.Null)
                        checkstatus.ContainsIrregularNumber = true;
                }
                switch (n_value.Type)
                {
                    case NumberValue.NumberType.Common:
                        if (sum == int.MinValue)
                            sum = n_value.Value;
                        else
                            sum += n_value.Value;
                        numbers.Add(n_value.Value);
                        break;
                    case NumberValue.NumberType.Signum:
                        signums.Add(n_value.Value);
                        checkstatus.ContainsSignums = true;
                        break;
                    case NumberValue.NumberType.Infinitive:
                        checkstatus.ContainsInfinitives = true;
                        break;
                    case NumberValue.NumberType.Double:
                        checkstatus.ContainsDouble = true;
                        break;
                    case NumberValue.NumberType.Unknown:
                        checkstatus.ContainsUnknown = true;
                        break;
                    case NumberValue.NumberType.Null:
                        checkstatus.ContainsNull = true;
                        break;
                    case NumberValue.NumberType.TimesZero:
                        checkstatus.Overall = true;
                        checkstatus.ContainsTimesZero = true;
                        break;
                    default:
                        break;
                }
            }
            checkstatus.Signums = signums.ToArray();
            checkstatus.Numbers = numbers.ToArray();
            if (checkstatus.Overall) return checkstatus;
            if (!checkstatus.ContainsIrregularNumber)
            {
                if (sum == int.MinValue)
                {
                    checkstatus.Overall = false;
                    checkstatus.Possibilities = new int[] { };
                    return checkstatus;
                }
                checkstatus.Overall = sum == 0;
                checkstatus.Possibilities = new int[] { sum };
                return checkstatus;
            }
            checkstatus.Possibilities = Get0GroupsWithSignums(checkstatus.Signums, checkstatus.Numbers).ToArray();
            if (signums.Count <= 0) return checkstatus;
            checkstatus.Overall = checkstatus.Possibilities.Contains(0);
            return checkstatus;
        }

        public struct CheckedStatusClass
        {
            public bool Overall;
            public int[] Numbers;
            public int[] Signums;
            public int[] Possibilities;
            public bool ContainsSignums;
            public bool ContainsInfinitives;
            public bool ContainsDouble;
            public bool ContainsUnknown;
            public bool ContainsNull;
            public bool ContainsTimesZero;
            public bool ContainsUncommonNumber;
            public bool ContainsIrregularNumber;
        }
    }
}
