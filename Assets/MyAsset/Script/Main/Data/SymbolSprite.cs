using Main.Data.Formula;
using SO;
using UnityEngine;

namespace Main.Data
{
    internal sealed class SymbolSprite : MonoBehaviour
    {
        private enum SymbolType
        {
            N0, N1, N2, N3, N4, N5, N6, N7, N8, N9,
            OA, OS, OM, OD,
            PL, PR
        }

        [SerializeField] private SymbolType type;

        [SerializeField] private SpriteRenderer spriteRenderer;

        private void OnEnable()
        {
            spriteRenderer.sprite = type switch
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
                _ => throw new System.Exception("ïsê≥Ç»éÌóﬁÇ≈Ç∑")
            };
        }

        private void OnDisable()
        {
            spriteRenderer = null;
        }

        internal IntStr GetSymbol()
        {
            return type switch
            {
                SymbolType.N0 => Symbol.N0,
                SymbolType.N1 => Symbol.N1,
                SymbolType.N2 => Symbol.N2,
                SymbolType.N3 => Symbol.N3,
                SymbolType.N4 => Symbol.N4,
                SymbolType.N5 => Symbol.N5,
                SymbolType.N6 => Symbol.N6,
                SymbolType.N7 => Symbol.N7,
                SymbolType.N8 => Symbol.N8,
                SymbolType.N9 => Symbol.N9,
                SymbolType.OA => Symbol.OA,
                SymbolType.OS => Symbol.OS,
                SymbolType.OM => Symbol.OM,
                SymbolType.OD => Symbol.OD,
                SymbolType.PL => Symbol.PL,
                SymbolType.PR => Symbol.PR,
                _ => throw new System.Exception("ïsê≥Ç»éÌóﬁÇ≈Ç∑")
            };
        }
    }
}