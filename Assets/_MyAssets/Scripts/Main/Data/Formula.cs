using System.Runtime.CompilerServices;
using foriver4725.FormulaCalculator;

namespace Main.Data.Formula;

internal static class FormulaElement
{
    internal const char N0 = '0';
    internal const char N1 = '1';
    internal const char N2 = '2';
    internal const char N3 = '3';
    internal const char N4 = '4';
    internal const char N5 = '5';
    internal const char N6 = '6';
    internal const char N7 = '7';
    internal const char N8 = '8';
    internal const char N9 = '9';
    internal const char OA = '+';
    internal const char OS = '-';
    internal const char OM = '*';
    internal const char OD = '/';
    internal const char PL = '(';
    internal const char PR = ')';
    internal const char NONE = ' ';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsNumber(this char c) => c is (>= N0 and <= N9);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static char ToFormulaElement(this int n) => n switch
    {
        0 => N0,
        1 => N1,
        2 => N2,
        3 => N3,
        4 => N4,
        5 => N5,
        6 => N6,
        7 => N7,
        8 => N8,
        9 => N9,
        _ => throw new Exception("Invalid number for FormulaElement.")
    };
}

internal sealed class Formula
{
    internal const int MaxLength = 12;
    private readonly char[] data = null;

    private bool dirty = true; // これがtrueなら、data が更新されたので、再度最初から計算する
    private double lastResult = double.NaN; // 最後に計算した結果 (dirty が立つ度に、再度計算して更新する)

    internal Formula()
    {
        data = new char[MaxLength];
        ClearData();
    }

    internal char GetData(int index) => data[index];

    internal void SetData(int index, char value)
    {
        dirty |= true;
        data[index] = value;
    }

    internal void ClearData()
    {
        dirty |= true;
        Array.Fill(data, FormulaElement.NONE);
    }

    internal double Calculate()
    {
        if (!dirty) return lastResult;

        ReadOnlySpan<char> dataSpan = data;
        double result = dataSpan.Calculate();

        lastResult = result;
        dirty &= false;

        return result;
    }
}
