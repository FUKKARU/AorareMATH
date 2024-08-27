// 必要なusing以外は消していい
using System.Collections.Generic;
using UnityEngine;

// 全て名前空間で囲ってください。名前空間とは、クラスや関数名のバッティングを避けるために、フォルダを分けて整理するみたいなもの。
// ↓の書き方の場合、Mainという名前空間の中の、Sampleという名前空間の中に入る、という意味。
namespace Main.Sample
{
    // 基本的に、publicの代わりにinternalを使い、クラスには全てsealedを付けることを推奨。
    // ※【internal】外部モジュールからのアクセスを禁止する。モジュールとはDLLファイルのこと。
    // 　　　　　　　 デフォルトではスクリプト全てが1つのDLLファイルにコンパイルされるので、直接的な意義はない。
    //　　　　　　　　ただ、カプセル化しておくことにこしたことはないので、こっちにすることを推奨します。
    // ※【sealed】このクラスの継承を禁止する、という意味。
    //            internalと同様に、後でsealedを外すのは簡単だが付けるのは大変なので、とりあえず付けておくと良い。
    internal sealed class Sample1 : MonoBehaviour
    {
        // コーディング規約に基づいて、アクセス修飾子は明示してほしいです。
        private void Start()
        {

        }

        private void Update()
        {

        }
    }

    internal sealed class Sample2 : MonoBehaviour
    {
        private class IntStr
        {
            // 整数の値
            private int _integer;
            internal int Integer => _integer;

            // 文字列の値
            private string _string;
            internal string String => _string;

            // コンストラクタ1
            internal IntStr(int _integer)
            {
                this._integer = _integer;
            }

            // コントラクタ2
            internal IntStr(string _string)
            {
                this._string = _string;
            }
        }

        private enum N
        {
            N0, N1, N2, N3, N4, N5, N6, N7, N8, N9,  // 0~9
            OA, OS, OM, OD,  // +,-,*,/
            PL, PR  // (,)
        }

        private void Start()
        {
            // 一番お手軽。ただ、整数と文字列以外の型も入り得るので、取扱注意。
            List<object> list1 = new() { 1, 2, "+", "(", 5 };

            // 整数と文字列しか入らないことが、確実に保証される。ただ、書くのが面倒。
            List<IntStr> list2 = new() { new(1), new(2), new("+"), new("("), new(5) };

            // 上2つの中間をとった、バランス型。ただ、整数値でそのまま計算することはできない。
            List<N> list3 = new() { N.N1, N.N2, N.OA, N.PL, N.N5 };
        }
    }
}