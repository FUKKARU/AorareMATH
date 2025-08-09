using General;

namespace Main.Data.Formula;

internal readonly record struct Element
{
    // デフォルトはNone
    internal readonly int Id { get; init; } = 0;

    internal Element(int id) => Id = id;

    internal Type Type => Id switch
    {
        0 or 1 or 2 or 3 or 4 or 5 or 6 or 7 or 8 or 9 => Type.Number,
        101 or 102 or 103 or 104 => Type.Operator,
        105 or 106 => Type.Paragraph,
        -1 => Type.None,
        _ => throw new Exception("不正な要素です")
    };

#if UNITY_EDITOR
    internal string ToStr() => Type switch
    {
        Type.Number => Id.ToString(),
        Type.Operator => Id switch
        {
            101 => "+",
            102 => "-",
            103 => "*",
            104 => "/",
            _ => throw new Exception("不正な演算子です")
        },
        Type.Paragraph => Id switch
        {
            105 => "(",
            106 => ")",
            _ => throw new Exception("不正な括弧です")
        },
        Type.None => ".",
        _ => throw new Exception("不正な要素です")
    };
#endif
}

internal enum Type : byte
{
    Number = 0,
    Operator = 1,
    Paragraph = 2,
    None = 3
}

internal static class Symbol
{
    internal static readonly Element N0 = new(0);
    internal static readonly Element N1 = new(1);
    internal static readonly Element N2 = new(2);
    internal static readonly Element N3 = new(3);
    internal static readonly Element N4 = new(4);
    internal static readonly Element N5 = new(5);
    internal static readonly Element N6 = new(6);
    internal static readonly Element N7 = new(7);
    internal static readonly Element N8 = new(8);
    internal static readonly Element N9 = new(9);
    internal static readonly Element OA = new(101);
    internal static readonly Element OS = new(102);
    internal static readonly Element OM = new(103);
    internal static readonly Element OD = new(104);
    internal static readonly Element PL = new(105);
    internal static readonly Element PR = new(106);
    internal static readonly Element NONE = new(-1);
}



internal sealed class Formula
{
    internal Element[] Data { get; set; } = null;
    internal const int MaxLength = 12;

    // 計算部で使用する参照型オブジェクト
    // 事前に作成して、使いまわす
    private readonly List<Element> _onRemoveNoneList = new(MaxLength);
    private readonly List<Element> _onConnectNumbersList = new(MaxLength);
    private readonly List<double> _onConvertToDoubleList = new(MaxLength);

    internal Formula()
    {
        Data = new Element[MaxLength];
        Reset();
    }

    internal void Reset()
    {
        for (int i = 0; i < Data.Length; ++i)
            Data[i] = Symbol.NONE;
    }

#if UNITY_EDITOR
    internal string Dump()
    {
        string[] dataStr = new string[Data.Length];
        for (int i = 0; i < Data.Length; ++i)
            dataStr[i] = Data[i].ToStr();
        return string.Join("", dataStr);
    }
#endif

    internal double? Calcurate()
    {
        try
        {
            RemoveNone(Data, _onRemoveNoneList);

            if (!IsArrayOK(_onRemoveNoneList) || !IsNumberOK(_onRemoveNoneList)
                || !IsOperatorOK(_onRemoveNoneList) || !IsParagraphOK(_onRemoveNoneList))
                throw new Exception("不正な形式です");

            ConnectNumbers(_onRemoveNoneList, _onConnectNumbersList);
            ConvertToDouble(_onConnectNumbersList, _onConvertToDoubleList);
            double result = Calcurate(_onConvertToDoubleList);
            return Math.Clamp(result, short.MinValue, short.MaxValue);
        }
        catch (Exception)
        {
            return null;
        }
    }

    #region バリデーション部

    /// <summary>
    /// コレクションがnullでないか、配列の要素数が0でないか
    /// srcから読み取りのみ行う
    /// </summary>
    private bool IsArrayOK(IReadOnlyList<Element> src)
        => src is not null and { Count: > 0 };

    /// <summary>
    /// かっこのすぐ外側に数字が来ていないか
    /// srcから読み取りのみ行う
    /// </summary>
    private bool IsNumberOK(IReadOnlyList<Element> src)
    {
        for (int i = 0; i < src.Count - 1; i++)
        {
            Element e = src[i], f = src[i + 1];
            if (e.Type == Type.Number && f == Symbol.PL) return false;
            else if (e == Symbol.PR && f.Type == Type.Number) return false;
        }

        return true;
    }

    /// <summary>
    /// 「+」「-」の演算子について、「一つ前の要素が存在しそれが 数字,(,) のいずれかである、または一つ前の要素が存在しない」かつ「一つ後の要素が存在しそれが 数字,( のいずれかである」であるか
    /// 「+」「-」を除いた演算子について、「一つ前の要素が存在しそれが 数字,) のいずれかである」かつ「一つ後の要素が存在しそれが 数字,( のいずれかである」であるか
    /// srcから読み取りのみ行う
    /// </summary>
    private bool IsOperatorOK(IReadOnlyList<Element> src)
    {
        int n = src.Count;

        for (int i = 0; i < n; i++)
        {
            Element e = src[i];

            if (e.Type != Type.Operator) continue;

            if (e == Symbol.OA || e == Symbol.OS)
            {
                if (i > 0)
                {
                    Element left = src[i - 1];
                    if (left.Type != Type.Number && left != Symbol.PL && left != Symbol.PR) return false;
                }

                if (i < n - 1)
                {
                    Element right = src[i + 1];
                    if (right.Type != Type.Number && right != Symbol.PL) return false;
                }
                else return false;
            }
            else
            {
                if (i > 0)
                {
                    Element left = src[i - 1];
                    if (left.Type != Type.Number && left != Symbol.PR) return false;
                }
                else return false;

                if (i < n - 1)
                {
                    Element right = src[i + 1];
                    if (right.Type != Type.Number && right != Symbol.PL) return false;
                }
                else return false;
            }
        }

        return true;
    }

    /// <summary>
    /// ()が全て対応しているか、またこの順番であるか
    /// ()の中に1つ以上の数字が入っているか
    /// )(の配置が存在しないか
    /// srcから読み取りのみ行う
    /// </summary>
    private bool IsParagraphOK(IReadOnlyList<Element> src)
    {
        int len = src.Count;

        int n = 0;
        foreach (var e in src)
        {
            if (e == Symbol.PL) n++;
            else if (e == Symbol.PR) n--;

            if (n < 0) return false;
        }
        if (n != 0) return false;

        for (int i = 0; i < len; i++)
        {
            if (src[i] == Symbol.PL)
            {
                int j = i + 1;
                while (j < len)
                {
                    if (src[j] == Symbol.PR) break;
                    j++;
                }

                bool hasNumber = false;
                for (int k = i, _n = 0; _n < j - i + 1; k++, _n++)
                {
                    Element e = src[k];
                    if (e.Type == Type.Number)
                    {
                        hasNumber = true;
                        break;
                    }
                }
                if (!hasNumber) return false;
            }
        }

        for (int i = 0; i < len - 1; i++)
        {
            Element e = src[i], f = src[i + 1];
            if (e == Symbol.PR && f == Symbol.PL) return false;
        }

        return true;
    }

    #endregion

    #region 計算部

    /// <summary>
    /// Noneを消して詰める
    /// resultに結果を書き込む、capacityは高々MaxLengthの前提
    /// </summary>
    private void RemoveNone(IReadOnlyList<Element> src, List<Element> result)
    {
        result.Clear();
        for (int i = 0; i < src.Count; i++)
        {
            if (src[i] == Symbol.NONE) continue;
            result.Add(src[i]);
        }
    }

    /// <summary>
    /// 数字を結合して、新しいコレクションとして返す
    /// 12桁全て結合、などのケースは想定していない
    /// resultに結果を書き込む、capacityは高々MaxLengthの前提
    /// </summary>
    private void ConnectNumbers(IReadOnlyList<Element> src, List<Element> result)
    {
        checked
        {
            int len = src.Count;

            result.Clear();
            for (int i = 0; i < len; i++)
            {
                // 今調べている要素が数字でないなら、結合しない
                Element s = src[i];
                if (s.Type != Type.Number)
                {
                    result.Add(s);
                    continue;
                }

                // 先の要素を順に見ていき、数字を結合していく
                int n = s.Id;
                for (int j = i + 1; j < len; i++, j++)
                {
                    Element _s = src[j];
                    if (_s.Type != Type.Number) break;
                    else n = n * 10 + _s.Id;
                }
                result.Add(new(n));
            }
        }
    }

    /// <summary>
    /// 計算の事前準備として、値をdoubleに変換する
    /// 記号を表すIDも、そのまま変換する
    /// resultに結果を書き込む、capacityは高々MaxLengthの前提
    /// </summary>
    private void ConvertToDouble(IReadOnlyList<Element> src, List<double> result)
    {
        result.Clear();
        foreach (var e in src)
            result.Add(e.Id);
    }

    /// <summary>
    /// 計算する
    /// srcから読み取りのみ行う
    /// </summary>
    private double Calcurate(IReadOnlyList<double> src)
    {
        Span<double> srcSpan = stackalloc double[src.Count];
        for (int i = 0; i < src.Count; i++)
            srcSpan[i] = src[i];

        return CalcurateImpl(srcSpan);
    }

    /// <summary>
    /// 計算のコアロジック
    /// srcから読み取りのみ行う
    /// </summary>
    private double CalcurateImpl(ReadOnlySpan<double> src)
    {
        Span<double> _src = stackalloc double[src.Length];
        for (int _i = 0; _i < src.Length; _i++)
            _src[_i] = src[_i];

        // かっこを無くす

        // 左から見て"("を探す
        int i = 0, cnt = 0;
        while (i < _src.Length)
        {
            if (_src[i] != Symbol.PL.Id) { i++; continue; }

            // その右を順に探索し、対応する")"を探す
            int n = 0;
            for (int j = i + 1; j < _src.Length; j++)
            {
                double e = _src[j];
                if (e != Symbol.PR.Id) { if (e == Symbol.PL.Id) n++; continue; }
                if (n >= 1) { n--; continue; }

                // "()"の間を再帰的に計算し、_srcを更新する
                {
                    // "()"の中を計算する
                    Span<double> newSpan = stackalloc double[j - i - 1];
                    for (int k = i + 1; k <= j - 1; k++)
                        newSpan[k - (i + 1)] = _src[k];
                    double value = CalcurateImpl(newSpan);

                    // "()"を削除し、計算結果を_srcに挿入する
                    newSpan = stackalloc double[_src.Length - (j - i)];
                    for (int k = 0; k < i; k++) newSpan[k] = _src[k];
                    newSpan[i] = value;
                    for (int k = j + 1; k < _src.Length; k++)
                        newSpan[k - (j - i)] = _src[k];
                    _src = newSpan;
                }

                break;
            }

            if (++cnt >= byte.MaxValue) throw new Exception("無限ループの可能性があります");
        }

        // かっこが無くなった(あるいはそもそも、かっこが無かった)ので、四則演算を行う
        return CalcurateRaw(_src);



        /// <summary>
        /// かっこが無い前提で、式を計算する
        /// srcから読み取りのみ行う
        /// </summary>
        static double CalcurateRaw(ReadOnlySpan<double> src)
        {
            Span<double> _src = stackalloc double[src.Length];
            for (int i = 0; i < src.Length; i++)
                _src[i] = src[i];

            // 正負の符号
            double first = _src[0];
            if (first == Symbol.OA.Id)
            {
                _src = _src[1..];
            }
            else if (first == Symbol.OS.Id)
            {
                _src[1] = -_src[1];
                _src = _src[1..];
            }

            // 乗除
            for (int i = 0; i < _src.Length; i++)
            {
                if (_src[i] == Symbol.OM.Id)
                {
                    Span<double> newSpan = stackalloc double[_src.Length - 2];
                    for (int j = 0; j < i - 1; j++) newSpan[j] = _src[j];
                    newSpan[i - 1] = _src[i - 1] * _src[i + 1];
                    for (int j = i + 2; j < _src.Length; j++) newSpan[j - 2] = _src[j];
                    _src = newSpan;
                    i--;
                }
                else if (_src[i] == Symbol.OD.Id)
                {
                    if (_src[i + 1] == 0) throw new Exception("0除算");

                    Span<double> newSpan = stackalloc double[_src.Length - 2];
                    for (int j = 0; j < i - 1; j++) newSpan[j] = _src[j];
                    newSpan[i - 1] = _src[i - 1] / _src[i + 1];
                    for (int j = i + 2; j < _src.Length; j++) newSpan[j - 2] = _src[j];
                    _src = newSpan;
                    i--;
                }
            }

            // 加減
            for (int i = 0; i < _src.Length; i++)
            {
                if (_src[i] == Symbol.OA.Id)
                {
                    Span<double> newSpan = stackalloc double[_src.Length - 2];
                    for (int j = 0; j < i - 1; j++) newSpan[j] = _src[j];
                    newSpan[i - 1] = _src[i - 1] + _src[i + 1];
                    for (int j = i + 2; j < _src.Length; j++) newSpan[j - 2] = _src[j];
                    _src = newSpan;
                    i--;
                }
                else if (_src[i] == Symbol.OS.Id)
                {
                    Span<double> newSpan = stackalloc double[_src.Length - 2];
                    for (int j = 0; j < i - 1; j++) newSpan[j] = _src[j];
                    newSpan[i - 1] = _src[i - 1] - _src[i + 1];
                    for (int j = i + 2; j < _src.Length; j++) newSpan[j - 2] = _src[j];
                    _src = newSpan;
                    i--;
                }
            }

            return _src[0];
        }
    }

    #endregion
}

#if UNITY_EDITOR && false
internal static class Test
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Run()
    {
        Formula formula = new();

        // 1+3*(4-5)
        // = -2
        formula.Data = new Element[]
        {
            Symbol.N1, Symbol.OA, Symbol.N3, Symbol.OM, Symbol.PL, Symbol.N4,
            Symbol.OS, Symbol.N5, Symbol.PR, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == -2).Log();

        // 1 23
        // = 123
        formula.Data = new Element[]
        {
            Symbol.N1, Symbol.NONE, Symbol.N2, Symbol.N3, Symbol.NONE, Symbol.NONE,
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == 123).Log();

        // +12-21
        // = -9
        formula.Data = new Element[]
        {
            Symbol.OA, Symbol.N1, Symbol.N2, Symbol.OS, Symbol.N2, Symbol.N1,
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == -9).Log();

        // -123 4
        // = -1234
        formula.Data = new Element[]
        {
            Symbol.OS, Symbol.N1, Symbol.N2, Symbol.N3, Symbol.NONE, Symbol.N4,
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == -1234).Log();

        // 2(3+4)
        // = null
        formula.Data = new Element[]
        {
            Symbol.N2, Symbol.PL, Symbol.N3, Symbol.OA, Symbol.N4, Symbol.PR,
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == null).Log();

        // (1+2)(3+4)
        // = null
        formula.Data = new Element[]
        {
            Symbol.PL, Symbol.N1, Symbol.OA, Symbol.N2, Symbol.PR, Symbol.PL,
            Symbol.N3, Symbol.OA, Symbol.N4, Symbol.PR, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == null).Log();

        // 2*((4-2)*3)
        // = 12
        formula.Data = new Element[]
        {
            Symbol.N2, Symbol.OM, Symbol.PL, Symbol.PL, Symbol.N4, Symbol.OS,
            Symbol.N2, Symbol.PR, Symbol.OM, Symbol.N3, Symbol.PR, Symbol.NONE
        };
        (formula.Calcurate() == 12).Log();

        // 2*(4-2)*3
        // = 12
        formula.Data = new Element[]
        {
            Symbol.N2, Symbol.OM, Symbol.PL, Symbol.N4, Symbol.OS, Symbol.N2,
            Symbol.PR, Symbol.OM, Symbol.N3, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == 12).Log();

        // 1 2 3 -4
        // = 119
        formula.Data = new Element[]
        {
            Symbol.N1, Symbol.NONE, Symbol.N2, Symbol.NONE, Symbol.N3, Symbol.NONE,
            Symbol.OS, Symbol.N4, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == 119).Log();

        // 1+2+3+4
        // = 10
        formula.Data = new Element[]
        {
            Symbol.N1, Symbol.OA, Symbol.N2, Symbol.OA, Symbol.N3, Symbol.OA,
            Symbol.N4, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == 10).Log();

        // (2+3-4)
        // = 1
        formula.Data = new Element[]
        {
            Symbol.PL, Symbol.N2, Symbol.OA, Symbol.N3, Symbol.OS, Symbol.N4,
            Symbol.PR, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == 1).Log();

        // 3/(7-4)
        // = 1
        formula.Data = new Element[]
        {
            Symbol.N3, Symbol.OD, Symbol.PL, Symbol.N7, Symbol.OS, Symbol.N4,
            Symbol.PR, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == 1).Log();

        // 4/(6-(2*3))
        // = null
        formula.Data = new Element[]
        {
            Symbol.N4, Symbol.OD, Symbol.PL, Symbol.N6, Symbol.OS, Symbol.PL,
            Symbol.N2, Symbol.OM, Symbol.N3, Symbol.PR, Symbol.PR, Symbol.NONE
        };
        (formula.Calcurate() == null).Log();

        // (3)
        // = 3
        formula.Data = new Element[]
        {
            Symbol.PL, Symbol.N3, Symbol.PR, Symbol.NONE, Symbol.NONE, Symbol.NONE,
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == 3).Log();

        // ((1+2))
        // = 3
        formula.Data = new Element[]
        {
            Symbol.PL, Symbol.PL, Symbol.N1, Symbol.OA, Symbol.N2, Symbol.PR,
            Symbol.PR, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == 3).Log();

        // 8/4/2
        // = 1
        formula.Data = new Element[]
        {
            Symbol.N8, Symbol.OD, Symbol.N4, Symbol.OD, Symbol.N2, Symbol.NONE,
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == 1).Log();

        // 0+0*0
        // = 0
        formula.Data = new Element[]
        {
            Symbol.N0, Symbol.OA, Symbol.N0, Symbol.OM, Symbol.N0, Symbol.NONE,
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == 0).Log();

        // ()
        // = null
        formula.Data = new Element[]
        {
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.PL, Symbol.PR,
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == null).Log();

        // (-)
        // = null
        formula.Data = new Element[]
        {
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.PL, Symbol.OS, Symbol.PR,
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == null).Log();

        // (+)
        // = null
        formula.Data = new Element[]
        {
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.PL, Symbol.OA, Symbol.PR,
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == null).Log();

        // 1-
        // = null
        formula.Data = new Element[]
        {
            Symbol.N1, Symbol.OS, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE,
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == null).Log();

        // *123
        // = null
        formula.Data = new Element[]
        {
            Symbol.OM, Symbol.N1, Symbol.N2, Symbol.N3, Symbol.NONE, Symbol.NONE,
            Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == null).Log();

        // 23++34
        // = null
        formula.Data = new Element[]
        {
            Symbol.NONE, Symbol.NONE, Symbol.N2, Symbol.N3, Symbol.OA, Symbol.OA,
            Symbol.N3, Symbol.N4, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
        };
        (formula.Calcurate() == null).Log();

        // 123456789123
        // = null
        formula.Data = new Element[]
        {
            Symbol.N1, Symbol.N2, Symbol.N3, Symbol.N4, Symbol.N5, Symbol.N6,
            Symbol.N7, Symbol.N8, Symbol.N9, Symbol.N1, Symbol.N2, Symbol.N3,
        };
        (formula.Calcurate() == null).Log();

        // (-3)+4-(-5)
        // = 6
        formula.Data = new Element[]
        {
            Symbol.PL, Symbol.OS, Symbol.N3, Symbol.PR, Symbol.OA, Symbol.N4,
            Symbol.OS, Symbol.PL, Symbol.OS, Symbol.N5, Symbol.PR, Symbol.NONE,
        };
        (formula.Calcurate() == 6).Log();
    }
}
#endif

#if UNITY_EDITOR && false
internal static class Profile
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static async UniTaskVoid Run()
    {
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        Formula formula = new()
        {
            // 1+2*3/(4-5)
            // = -5
            Data = new Element[]
            {
                Symbol.N1, Symbol.OA, Symbol.N2, Symbol.OM, Symbol.N3, Symbol.OD,
                Symbol.PL, Symbol.N4, Symbol.OS, Symbol.N5, Symbol.PR, Symbol.NONE
            }
        };

        double? result = double.NaN;
        const ulong LoopAmount = 1_000_000;

        UnityEngine.Profiling.Profiler.BeginSample("Custom_Formula");

        for (ulong i = 0; i < LoopAmount; i++)
            result = formula.Calcurate();

        UnityEngine.Profiling.Profiler.EndSample();

        (result == -5).Log();
    }
}
#endif