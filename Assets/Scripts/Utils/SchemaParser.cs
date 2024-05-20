using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SchemaParser
{
    public class SchemaFunction
    {
        public string FuncName;
        public List<string> FuncParams;
    }

    public const char SPLITTER_NORMAL = ';';
    public const char SPLITTER_PARAM = ',';
    public const char SPLITTER_PAIR = '|';
    public const char SPLITTER_COM = '+';
    private const char SPLITTER_FUNC = ':';
    private const string SPLITTER_CONTINUE_NUMBER = "-->";
    public const string SPLITTER_DLG_SELECTION = "###";

    /// <summary>
    /// eg. 1001-->1010,1050-->1055,1070
    /// </summary>
    /// <param name="param"></param>
    /// <param name="splitter"></param>
    /// <returns></returns>
    public static List<int> ParseParamToInts(string param, char splitter = SPLITTER_PARAM)
    {
        List<int> resultList = new List<int>();
        if (!string.IsNullOrEmpty(param))
        {
            var items = param.Split(splitter);
            foreach (var item in items)
            {
                if (item.Contains(SPLITTER_CONTINUE_NUMBER))
                {
                    var pairNumbers = item.Split(new string[] { SPLITTER_CONTINUE_NUMBER }, StringSplitOptions.None);
                    if (pairNumbers.Length == 2)
                    {
                        int begin = int.Parse(pairNumbers[0]);
                        int end = int.Parse(pairNumbers[1]);
                        if (end > begin)
                        {
                            for (int i = begin; i <= end; i++)
                            {
                                resultList.Add(i);
                            }
                        }
                    }
                }
                else
                {
                    int result = 0;
                    if (int.TryParse(item, out result))
                    {
                        resultList.Add(result);
                    }
                }
            }
        }

        return resultList;
    }

    /// <summary>
    /// 按照字符串分割符解析具体的参数
    /// funcName1:funcParam1,funcParam2,...,funcParamN;funcName2:funcParam1,funcParam2,...,funcParamN
    /// 默认的函数分隔符为;符号
    /// 函数名和参数分隔符为:符号
    /// 参数之间分隔符为,符号
    /// </summary>
    /// <param name="param"></param>
    /// <param name="splitter"></param>
    /// <returns>返回N个函数以及参数</returns>
    public static List<SchemaFunction> ParseParamToFunctionsSingle(string param, char splitter = SPLITTER_NORMAL)
    {
        List<SchemaFunction> resultList = new List<SchemaFunction>();
        if (!string.IsNullOrEmpty(param))
        {
            var items = param.Split(splitter);
            foreach (var item in items)
            {
                var data = item.Split(SPLITTER_FUNC);
                if (data.Length == 1)
                {
                    var funcName = data[0];
                    resultList.Add(new SchemaFunction()
                    {
                        FuncName = funcName,
                        FuncParams = null
                    });
                }
                else if (data.Length == 2)
                {
                    var funcName = data[0];
                    List<string> funcParam = null;
                    List<string> funcFinalParams = null;
                    if (!string.IsNullOrEmpty(data[1]))
                    {
                        funcParam = new List<string>(data[1].Split(SPLITTER_PARAM));

                        funcFinalParams = new List<string>();
                        for (int i = 0; i < funcParam.Count; i++)
                        {
                            param = funcParam[i];
                            if (param.Contains(SPLITTER_CONTINUE_NUMBER))
                            {
                                var pairNumbers = param.Split(new string[] { SPLITTER_CONTINUE_NUMBER }, StringSplitOptions.None);
                                if (pairNumbers.Length == 2)
                                {
                                    int begin = int.Parse(pairNumbers[0]);
                                    int end = int.Parse(pairNumbers[1]);
                                    if (end > begin)
                                    {
                                        for (int j = begin; j <= end; j++)
                                        {
                                            funcFinalParams.Add(j.ToString());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                funcFinalParams.Add(param);
                            }
                        }
                    }
                    resultList.Add(new SchemaFunction()
                    {
                        FuncName = funcName,
                        FuncParams = funcFinalParams
                    });
                }
            }
        }

        return resultList;
    }

    /// <summary>
    /// 按照字符串分割符解析具体的参数
    /// funcName1:funcParam1|funcParam2|...|funcParamN;funcName2:funcParam1|funcParam2|...|funcParamN
    /// 默认的函数分隔符为;符号
    /// 函数名和参数分隔符为:符号
    /// 参数之间分隔符为|符号
    /// </summary>
    /// <param name="param"></param>
    /// <param name="splitter"></param>
    /// <returns>返回N个函数以及参数</returns>
    public static List<SchemaFunction> ParseParamToFunctionsMultiParam(string param, char splitter = SPLITTER_NORMAL)
    {
        List<SchemaFunction> resultList = new List<SchemaFunction>();
        if (!string.IsNullOrEmpty(param))
        {
            var items = param.Split(splitter);
            foreach (var item in items)
            {
                var data = item.Split(SPLITTER_FUNC);
                if (data.Length == 1)
                {
                    var funcName = data[0];
                    resultList.Add(new SchemaFunction()
                    {
                        FuncName = funcName,
                        FuncParams = null
                    });
                }
                else if (data.Length == 2)
                {
                    var funcName = data[0];
                    List<string> funcParam = null;
                    if (!string.IsNullOrEmpty(data[1]))
                    {
                        funcParam = new List<string>(data[1].Split(SPLITTER_PAIR));
                    }
                    resultList.Add(new SchemaFunction()
                    {
                        FuncName = funcName,
                        FuncParams = funcParam
                    });
                }
            }
        }

        return resultList;
    }

    public static List<string> ParseParamToStringList(string param, char splitter = SPLITTER_PARAM)
    {
        List<string> resultList = new List<string>();
        if (!string.IsNullOrEmpty(param))
        {
            var items = param.Split(splitter);
            resultList.AddRange(items.ToList());
        }

        return resultList;
    }

    public static List<string> ParseParamExcludeFlag(string param, char beginFlag = '[', char endFlg = ']')
    {
        List<string> resultList = new List<string>();
        if (!string.IsNullOrEmpty(param))
        {
            int beginIndex = param.IndexOf(beginFlag);
            int endIndex = param.LastIndexOf(endFlg);
            if (beginIndex != endIndex && beginIndex >= 0 && endIndex >= 0)
            {
                string strParams = param.Substring(beginIndex + 1, endIndex - beginIndex - 1);
                var listOfParams = ParseParamToStringList(strParams);
                resultList.AddRange(listOfParams);
            }
        }

        return resultList;
    }

    /// <summary>
    /// eg. 1002|20020001
    /// </summary>
    /// <param name="param"></param>
    /// <param name="item1"></param>
    /// <param name="item2"></param>
    /// <param name="splitter"></param>
    /// <returns></returns>
    public static bool ParseParamAsPairInt(string param, out int item1, out int item2, char splitter = SPLITTER_PAIR)
    {
        item1 = 0;
        item2 = 0;
        var data = param.Split(splitter);
        if (data != null && data.Length == 2)
        {
            item1 = int.Parse(data[0]);
            item2 = int.Parse(data[1]);
            return true;
        }

        return false;
    }

    public static string JoinIntsToString(int[] someInts, char splitter = SPLITTER_NORMAL)
    {
        string strSplitter = string.Format("{0}", splitter);
        return string.Join(strSplitter, someInts);
    }

    public static string JoinStringsToUISelection(string[] someStrs, string splitter = SPLITTER_DLG_SELECTION)
    {
        return string.Join(splitter, someStrs);
    }

    public static List<string> ParseUISelectionToList(string str, string splitter = SPLITTER_DLG_SELECTION)
    {
        List<string> result = new List<string>();
        if (string.IsNullOrEmpty(str))
        {
            return result;
        }
        var listOfSelections = str.Split(new string[] { splitter }, StringSplitOptions.None);
        return listOfSelections.ToList();
    }
}
