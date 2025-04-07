using System.Collections.Generic;
using System.Linq;
using General.Debug;
using General.Extension;

namespace Main.Data.Formula
{
    internal sealed record IntStr
    {
        private int _int;
        internal int Int => _int;

        private string _str;
        internal string Str => _str;

        internal IntStr(int _int)
        {
            this._int = _int;
        }

        internal IntStr(string _str)
        {
            this._str = _str;
        }

        internal IntStr(int _int, string _str)
        {
            this._int = _int;
            this._str = _str;
        }

        public static implicit operator FltStr(IntStr intStr) => intStr is null ? null : new(intStr.Int, intStr.Str);
        public static bool operator ==(IntStr left, FltStr right) => left is not null && right is not null && left.Int == right.Flt && left.Str == right.Str;
        public static bool operator !=(IntStr left, FltStr right) => !(left == right);
    }

    internal sealed record FltStr
    {
        private float _flt;
        internal float Flt => _flt;

        private string _str;
        internal string Str => _str;

        internal FltStr(float _flt)
        {
            this._flt = _flt;
        }

        internal FltStr(string _str)
        {
            this._str = _str;
        }

        internal FltStr(float _flt, string _str)
        {
            this._flt = _flt;
            this._str = _str;
        }

        public static implicit operator IntStr(FltStr fltStr) => fltStr is null ? null : new((int)fltStr.Flt, fltStr.Str);
        public static bool operator ==(FltStr left, IntStr right) => left is not null && right is not null && left.Flt == right.Int && left.Str == right.Str;
        public static bool operator !=(FltStr left, IntStr right) => !(left == right);
    }

    internal static class Symbol
    {
        internal static readonly IntStr N0 = new(0);
        internal static readonly IntStr N1 = new(1);
        internal static readonly IntStr N2 = new(2);
        internal static readonly IntStr N3 = new(3);
        internal static readonly IntStr N4 = new(4);
        internal static readonly IntStr N5 = new(5);
        internal static readonly IntStr N6 = new(6);
        internal static readonly IntStr N7 = new(7);
        internal static readonly IntStr N8 = new(8);
        internal static readonly IntStr N9 = new(9);
        internal static readonly IntStr OA = new("+");
        internal static readonly IntStr OS = new("-");
        internal static readonly IntStr OM = new("*");
        internal static readonly IntStr OD = new("/");
        internal static readonly IntStr PL = new("(");
        internal static readonly IntStr PR = new(")");
        internal static readonly IntStr NONE = new("_");

        internal static bool? IsNumber(IntStr symbol)
        {
            if (symbol == N0 || symbol == N1 || symbol == N2 || symbol == N3 || symbol == N4 ||
                symbol == N5 || symbol == N6 || symbol == N7 || symbol == N8 || symbol == N9)
            {
                return true;
            }
            else if (symbol == OA || symbol == OS || symbol == OM || symbol == OD)
            {
                return false;
            }
            else if (symbol == PL || symbol == PR)
            {
                return false;
            }
            else if (symbol == NONE)
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        internal static bool? IsOperator(IntStr symbol)
        {
            if (symbol == N0 || symbol == N1 || symbol == N2 || symbol == N3 || symbol == N4 ||
                symbol == N5 || symbol == N6 || symbol == N7 || symbol == N8 || symbol == N9)
            {
                return false;
            }
            else if (symbol == OA || symbol == OS || symbol == OM || symbol == OD)
            {
                return true;
            }
            else if (symbol == PL || symbol == PR)
            {
                return false;
            }
            else if (symbol == NONE)
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        internal static bool? IsParagraph(IntStr symbol)
        {
            if (symbol == N0 || symbol == N1 || symbol == N2 || symbol == N3 || symbol == N4 ||
                symbol == N5 || symbol == N6 || symbol == N7 || symbol == N8 || symbol == N9)
            {
                return false;
            }
            else if (symbol == OA || symbol == OS || symbol == OM || symbol == OD)
            {
                return false;
            }
            else if (symbol == PL || symbol == PR)
            {
                return true;
            }
            else if (symbol == NONE)
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        internal static bool? IsNone(IntStr symbol)
        {
            if (symbol == N0 || symbol == N1 || symbol == N2 || symbol == N3 || symbol == N4 ||
                symbol == N5 || symbol == N6 || symbol == N7 || symbol == N8 || symbol == N9)
            {
                return false;
            }
            else if (symbol == OA || symbol == OS || symbol == OM || symbol == OD)
            {
                return false;
            }
            else if (symbol == PL || symbol == PR)
            {
                return false;
            }
            else if (symbol == NONE)
            {
                return true;
            }
            else
            {
                return null;
            }
        }
    }

    internal sealed class Formula
    {
        internal List<IntStr> Data { get; set; }
        internal static readonly int MaxLength = 12;

        internal Formula()
        {
            Data = new();
        }

        internal Formula(IntStr[] data)
        {
            if (data == null) return;
            if (data.Length != MaxLength) return;
            Data = data.ToList();
        }

        internal void Init()
        {
            Data = new()
            {
                Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE,
                Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE
            };
        }

        internal void Reset()
        {
            Data.Clear();
        }

#if UNITY_EDITOR
        internal string Dump() => string.Join("", Data.Select(e => e is null ? string.Empty : Symbol.IsNumber(e) == true ? e.Int.ToString() : e.Str));
#endif

        internal float? Calcurate()
        {
            try
            {
                var data = Data.RemoveNone();

                if (!IsListOK(data) || !IsNumberOK(data) || !IsOperatorOK(data) || !IsParagraphOK(data))
                    throw new System.Exception("不正な形式です");

                return
                    UnityEngine.Mathf.Clamp(data.ConnectNumbers().Convert().Calcurate(), short.MinValue, short.MaxValue);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// リストがnullでないか、リストの要素数が0でないか
        /// </summary>
        private bool IsListOK(List<IntStr> list)
        {
            if (list == null) return false;
            if (list.Count <= 0) return false;
            return true;
        }

        /// <summary>
        /// かっこのすぐ外側に数字が来ていないか
        /// </summary>
        private bool IsNumberOK(List<IntStr> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                IntStr e = list[i], f = list[i + 1];
                if (Symbol.IsNumber(e) == true && f == Symbol.PL) return false;
                else if (e == Symbol.PR && Symbol.IsNumber(f) == true) return false;
            }

            return true;
        }

        /// <summary>
        /// 「+」「-」の演算子について、「一つ前の要素が存在しそれが 数字,(,) のいずれかである、または一つ前の要素が存在しない」かつ「一つ後の要素が存在しそれが 数字,( のいずれかである」であるか
        /// 「+」「-」を除いた演算子について、「一つ前の要素が存在しそれが 数字,) のいずれかである」かつ「一つ後の要素が存在しそれが 数字,( のいずれかである」であるか
        /// </summary>
        private bool IsOperatorOK(List<IntStr> list)
        {
            int n = list.Count;

            for (int i = 0; i < n; i++)
            {
                IntStr e = list[i];

                if (Symbol.IsOperator(e) != true) continue;

                if (e == Symbol.OA || e == Symbol.OS)
                {
                    if (i > 0)
                    {
                        IntStr left = list[i - 1];
                        if (Symbol.IsNumber(left) != true && left != Symbol.PL && left != Symbol.PR) return false;
                    }

                    if (i < n - 1)
                    {
                        IntStr right = list[i + 1];
                        if (Symbol.IsNumber(right) != true && right != Symbol.PL) return false;
                    }
                    else return false;
                }
                else
                {
                    if (i > 0)
                    {
                        IntStr left = list[i - 1];
                        if (Symbol.IsNumber(left) != true && left != Symbol.PR) return false;
                    }
                    else return false;

                    if (i < n - 1)
                    {
                        IntStr right = list[i + 1];
                        if (Symbol.IsNumber(right) != true && right != Symbol.PL) return false;
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
        /// </summary>
        private bool IsParagraphOK(List<IntStr> list)
        {
            int n = 0;
            foreach (var e in list)
            {
                if (e == Symbol.PL) n++;
                else if (e == Symbol.PR) n--;

                if (n < 0) return false;
            }
            if (n != 0) return false;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == Symbol.PL)
                {
                    int j = i + 1;
                    while (j < list.Count)
                    {
                        if (list[j] == Symbol.PR) break;
                        j++;
                    }

                    if (list.GetRange(i, j - i + 1).All(e => Symbol.IsNumber(e) != true)) return false;
                }
            }

            for (int i = 0; i < list.Count - 1; i++)
            {
                IntStr e = list[i], f = list[i + 1];
                if (e == Symbol.PR && f == Symbol.PL) return false;
            }

            return true;
        }
    }

    internal static class Ex
    {
        /// <summary>
        /// Noneを消して詰める
        /// </summary>
        internal static List<IntStr> RemoveNone(this List<IntStr> list)
        {
            List<IntStr> ret = new(list);

            List<int> removeIndexList = new();

            for (int i = 0; i < ret.Count; i++)
            {
                if (ret[i].Str == Symbol.NONE.Str)
                {
                    removeIndexList.Add(i);
                }
            }

            for (int i = removeIndexList.Count - 1; 0 <= i; i--)
            {
                ret.RemoveAt(removeIndexList[i]);
            }

            return ret;
        }

        /// <summary>
        /// 数字を結合して、新しいリストとして返す
        /// </summary>
        internal static List<IntStr> ConnectNumbers(this List<IntStr> list)
        {
            List<IntStr> ret = new();

            for (int i = 0; i < list.Count; i++)
            {
                // 今調べている要素が数字でないなら、結合しない
                IntStr s = list[i];
                if (Symbol.IsNumber(s) != true) { ret.Add(s); continue; }

                // 先の要素を順に見ていき、数字を結合していく
                int n = s.Int;
                for (int j = i + 1; j < list.Count; i++, j++)
                {
                    IntStr _s = list[j];
                    if (Symbol.IsNumber(_s) != true) break;
                    else n = n * 10 + _s.Int;
                }
                ret.Add(new(n));
            }

            return ret;
        }

        /// <summary>
        /// IntStrをFltStrに変換する(リスト)
        /// </summary>
        internal static List<FltStr> Convert(this List<IntStr> intStr) => intStr.Select(e => (FltStr)e).ToList();

        /// <summary>
        /// 式を計算する
        /// </summary>
        internal static float Calcurate(this List<FltStr> list)
        {
            List<FltStr> _list = new(list);

            // かっこを無くす

            // 左から見て"("を探す
            int i = 0, cnt = 0;
            while (i < _list.Count)
            {
                if (_list[i] != Symbol.PL) { i++; continue; }

                // その右を順に探索し、対応する")"を探す
                int n = 0;
                for (int j = i + 1; j < _list.Count; j++)
                {
                    IntStr e = _list[j];
                    if (e != Symbol.PR) { if (e == Symbol.PL) n++; continue; }
                    if (n >= 1) { n--; continue; }

                    // "()"の間を再帰的に計算し、_listを更新する
                    float val = _list.GetRange(i + 1, (j - 1) - (i + 1) + 1).Calcurate();
                    _list.RemoveRange(i, j - i + 1);
                    _list.Insert(i, new(val));
                    break;
                }

                if (++cnt >= byte.MaxValue) throw new System.Exception("無限ループの可能性があります");
            }

            // かっこが無くなった(あるいはそもそも、かっこが無かった)ので、四則演算を行う
            return _list.CalcurateRaw();
        }

        /// <summary>
        /// かっこが無い前提で、式を計算する
        /// </summary>
        private static float CalcurateRaw(this List<FltStr> list)
        {
            List<FltStr> _list = new(list);

            // 正負の符号
            FltStr first = _list[0];
            if (first == Symbol.OA)
            {
                _list.RemoveAt(0);
            }
            else if (first == Symbol.OS)
            {
                _list[1] = new(-_list[1].Flt);
                _list.RemoveAt(0);
            }

            // 乗除
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i] == Symbol.OM)
                {
                    (_list[i - 1].Flt * _list[i + 1].Flt).ReplaceAroundOperator(ref _list, ref i);
                }
                else if (_list[i] == Symbol.OD)
                {
                    if (_list[i + 1].Flt == 0) throw new System.Exception("0除算");
                    (_list[i - 1].Flt / _list[i + 1].Flt).ReplaceAroundOperator(ref _list, ref i);
                }
            }

            // 加減
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i] == Symbol.OA)
                {
                    (_list[i - 1].Flt + _list[i + 1].Flt).ReplaceAroundOperator(ref _list, ref i);
                }
                else if (_list[i] == Symbol.OS)
                {
                    (_list[i - 1].Flt - _list[i + 1].Flt).ReplaceAroundOperator(ref _list, ref i);
                }
            }

            return _list[0].Flt;
        }

        /// <summary>
        /// リストのインデックス番目の要素について、前後を含めた3つを消し、与えられた値に置き換える。インデックス番号も補正する。
        /// </summary>
        private static void ReplaceAroundOperator(this float val, ref List<FltStr> list, ref int operatorIndex)
        {
            list.RemoveRange(operatorIndex - 1, 3);
            list.Insert(operatorIndex - 1, new(val));
            operatorIndex--;
        }
    }
}