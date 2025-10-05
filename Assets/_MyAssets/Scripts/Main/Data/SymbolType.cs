using Main.Data.Formula;
using SO;

namespace Main.Data
{
    internal enum SymbolType : byte
    {
        N0, N1, N2, N3, N4, N5, N6, N7, N8, N9,
        OA, OS, OM, OD,
        PL, PR
    }

    internal static class SymbolTypeEx
    {
        internal static char GetElement(this SymbolType type) => type switch
        {
            SymbolType.N0 => FormulaElement.N0,
            SymbolType.N1 => FormulaElement.N1,
            SymbolType.N2 => FormulaElement.N2,
            SymbolType.N3 => FormulaElement.N3,
            SymbolType.N4 => FormulaElement.N4,
            SymbolType.N5 => FormulaElement.N5,
            SymbolType.N6 => FormulaElement.N6,
            SymbolType.N7 => FormulaElement.N7,
            SymbolType.N8 => FormulaElement.N8,
            SymbolType.N9 => FormulaElement.N9,
            SymbolType.OA => FormulaElement.OA,
            SymbolType.OS => FormulaElement.OS,
            SymbolType.OM => FormulaElement.OM,
            SymbolType.OD => FormulaElement.OD,
            SymbolType.PL => FormulaElement.PL,
            SymbolType.PR => FormulaElement.PR,
            _ => throw new Exception("Invalid symbol type.")
        };

        internal static Sprite GetSprite(this SymbolType type) => type switch
        {
            SymbolType.N0 => SO_Sprite.Entity.N0,
            SymbolType.N1 => SO_Sprite.Entity.N1,
            SymbolType.N2 => SO_Sprite.Entity.N2,
            SymbolType.N3 => SO_Sprite.Entity.N3,
            SymbolType.N4 => SO_Sprite.Entity.N4,
            SymbolType.N5 => SO_Sprite.Entity.N5,
            SymbolType.N6 => SO_Sprite.Entity.N6,
            SymbolType.N7 => SO_Sprite.Entity.N7,
            SymbolType.N8 => SO_Sprite.Entity.N8,
            SymbolType.N9 => SO_Sprite.Entity.N9,
            SymbolType.OA => SO_Sprite.Entity.OA,
            SymbolType.OS => SO_Sprite.Entity.OS,
            SymbolType.OM => SO_Sprite.Entity.OM,
            SymbolType.OD => SO_Sprite.Entity.OD,
            SymbolType.PL => SO_Sprite.Entity.PL,
            SymbolType.PR => SO_Sprite.Entity.PR,
            _ => throw new Exception("Invalid symbol type.")
        };
    }
}
