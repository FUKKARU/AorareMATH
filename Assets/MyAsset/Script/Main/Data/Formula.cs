using System.Collections.Generic;
using General.Extension;

namespace Main.Data.Formula
{
    internal sealed class IntStr
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
    }

    internal sealed class FltStr
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

        internal Formula()
        {
            Data = new();
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

        internal float? Calcurate()
        {
            try
            {
                var data = Data.RemoveNone();

                if (!IsListOK(data) || !IsNumberOK(data) || !IsOperatorOK(data) || !IsParagraphOK(data))
                    throw new System.Exception("�s���Ȍ`���ł�");

                return
                    UnityEngine.Mathf.Clamp(data.ConnectNumbers().Convert().Calcurate(), short.MinValue, short.MaxValue);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// ���X�g��null�łȂ����A���X�g�̗v�f����0�łȂ���
        /// </summary>
        private bool IsListOK(List<IntStr> list)
        {
            if (list == null) return false;
            if (list.Count == 0) return false;
            return true;
        }

        /// <summary>
        /// �������̂����O���ɐ��������Ă��Ȃ���
        /// </summary>
        private bool IsNumberOK(List<IntStr> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                IntStr e = list[i], f = list[i + 1];
                if (Symbol.IsNumber(e) == true && f.Str == Symbol.PL.Str) return false;
                else if (e.Str == Symbol.PR.Str && Symbol.IsNumber(f) == true) return false;
            }

            return true;
        }

        /// <summary>
        /// ���Z�q���[�ɂȂ��A(�����̐�) - (���Z�q�̐�) >= 1 �ł��邩
        /// �������𖳎����鎞�A���Z�q��2�A�����Ă��Ȃ���
        /// </summary>
        private bool IsOperatorOK(List<IntStr> list)
        {
            if (Symbol.IsOperator(list[0]) == true) return false;
            if (Symbol.IsOperator(list[^1]) == true) return false;

            int n = 0;
            foreach (var e in list)
            {
                if (Symbol.IsNumber(e) == true) n++;
                else if (Symbol.IsOperator(e) == true) n--;
            }
            if (n < 1) return false;

            for (int i = 0; i < list.Count; i++)
            {
                if (Symbol.IsOperator(list[i]) == true)
                {
                    for (int j = i + 1; j < list.Count; j++)
                    {
                        if (Symbol.IsNumber(list[j]) == true) break;
                        else if (Symbol.IsParagraph(list[j]) == true) continue;
                        else if (Symbol.IsOperator(list[j]) == true) return false;
                        else return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// ()���S�đΉ����Ă��邩�A�܂����̏��Ԃł��邩
        /// ()�̒���1�ȏ�̐����������Ă��邩
        /// </summary>
        private bool IsParagraphOK(List<IntStr> list)
        {
            int n = 0;
            foreach (var e in list)
            {
                if (e.Str == Symbol.PL.Str) n++;
                else if (e.Str == Symbol.PR.Str) n--;

                if (n < 0) return false;
            }
            if (n != 0) return false;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Str == Symbol.PL.Str)
                {
                    int j = i + 1;
                    while (j < list.Count)
                    {
                        if (list[j].Str == Symbol.PR.Str) break;
                        j++;
                    }

                    if (list.GetRange(i, j - i + 1).All(e => Symbol.IsNumber(e) != true)) return false;
                }
            }

            return true;
        }
    }

    internal static class Ex
    {
        /// <summary>
        /// None�������ċl�߂�
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
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
        /// �������������āA�V�������X�g�Ƃ��ĕԂ�
        /// </summary>
        internal static List<IntStr> ConnectNumbers(this List<IntStr> list)
        {
            List<IntStr> ret = new();

            for (int i = 0; i < list.Count; i++)
            {
                // �����ׂĂ���v�f�������łȂ��Ȃ�A�������Ȃ�
                IntStr s = list[i];
                if (!s.IsNumber()) { ret.Add(s); continue; }

                // ��̗v�f�����Ɍ��Ă����A�������������Ă���
                int n = s.Int;
                for (int j = i + 1; j < list.Count; i++, j++)
                {
                    IntStr _s = list[j];
                    if (!_s.IsNumber()) break;
                    else n = n * 10 + _s.Int;
                }
                ret.Add(new(n));
            }

            return ret;
        }

        /// <summary>
        /// IntStr��FltStr�ɕϊ�����(���X�g)
        /// </summary>
        internal static List<FltStr> Convert(this List<IntStr> intStr)
        {
            return intStr.Map(e => e.Convert()).ToList();
        }

        /// <summary>
        /// �����v�Z����
        /// </summary>
        internal static float Calcurate(this List<FltStr> list)
        {
            List<FltStr> _list = new(list);

            // �������𖳂���
            int i = 0, n = 0;
            // �����猩��"("��T��
            while (i < _list.Count)
            {
                if (_list[i].Str != Symbol.PL.Str) { i++; continue; }

                // �E���猩��")"��T��
                for (int j = _list.Count - 1; i < j; j--)
                {
                    if (_list[j].Str != Symbol.PR.Str) continue;

                    // "()"�̊Ԃ��ċA�I�Ɍv�Z���A_list���X�V����
                    float val = _list.GetRange(i + 1, (j - 1) - (i + 1) + 1).Calcurate();
                    _list.RemoveRange(i, j - i + 1);
                    _list.Insert(i, new(val));
                    break;
                }

                if (++n >= byte.MaxValue) throw new System.Exception("�������[�v�̉\��������܂�");
            }

            // �������������Ȃ���(���邢�͂��������A����������������)�̂ŁA�l�����Z���s��
            return _list.CalcurateRaw();
        }

        /// <summary>
        /// �������������O��ŁA�����v�Z����
        /// </summary>
        private static float CalcurateRaw(this List<FltStr> list)
        {
            List<FltStr> _list = new(list);

            // �揜
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].Str == Symbol.OM.Str)
                {
                    (_list[i - 1].Flt * _list[i + 1].Flt).ReplaceAroundOperator(ref _list, ref i);
                }
                else if (_list[i].Str == Symbol.OD.Str)
                {
                    if (_list[i + 1].Flt == 0) throw new System.Exception("0���Z");
                    (_list[i - 1].Flt / _list[i + 1].Flt).ReplaceAroundOperator(ref _list, ref i);
                }
            }

            // ����
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].Str == Symbol.OA.Str)
                {
                    (_list[i - 1].Flt + _list[i + 1].Flt).ReplaceAroundOperator(ref _list, ref i);
                }
                else if (_list[i].Str == Symbol.OS.Str)
                {
                    (_list[i - 1].Flt - _list[i + 1].Flt).ReplaceAroundOperator(ref _list, ref i);
                }
            }

            return _list[0].Flt;
        }

        /// <summary>
        /// ���X�g�̃C���f�b�N�X�Ԗڂ̗v�f�ɂ��āA�O����܂߂�3�������A�^����ꂽ�l�ɒu��������B�C���f�b�N�X�ԍ����␳����B
        /// </summary>
        private static void ReplaceAroundOperator(this float val, ref List<FltStr> list, ref int operatorIndex)
        {
            list.RemoveRange(operatorIndex - 1, 3);
            list.Insert(operatorIndex - 1, new(val));
            operatorIndex--;
        }

        /// <summary>
        /// IntStr��FltStr�ɕϊ�����
        /// </summary>
        private static FltStr Convert(this IntStr intStr)
        {
            return new(intStr.Int, intStr.Str);
        }

        /// <summary>
        /// �������ǂ������肷��
        /// </summary>
        private static bool IsNumber(this IntStr var)
        {
            bool? isNum = Symbol.IsNumber(var);
            return isNum.HasValue && isNum.Value;
        }
    }
}