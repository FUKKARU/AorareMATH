using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using General.Extension;
using UnityEngine;

namespace Main.Data
{
    public static class FixedQuestions
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // 問題のテキストファイルを全て読み込む
            TextAsset[] textFiles = Resources.LoadAll<TextAsset>("FixedQuestions");
            if (textFiles.Length <= 0) return;

            foreach (TextAsset textFile in textFiles)
            {
                // それぞれのテキストファイルについて、１行ずつ問題をパースしてコレクションに格納し、
                // 最後にデータのディクショナリに格納する

                string[] lines = textFile.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                (int[] Numbers, int Target, string Answer)[] theData = new (int[] Numbers, int Target, string Answer)[lines.Length];

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    string[] elements = line.Split(", ");
                    if (elements.Length < 3) continue;

                    // 最後の1つは問題の答え
                    string answer = elements[^1];

                    // その前の1つは目標の数字
                    int target = int.Parse(elements[^2]);

                    // 残ったものは問題の数字
                    int[] numbers = new int[elements.Length - 2];
                    for (int j = 0; j < numbers.Length; j++)
                        numbers[j] = int.Parse(elements[j]);

                    theData[i] = (numbers, target, answer);
                }

                theData.ShuffleSelf(); // ここで問題のシャッフルも行っておく
                data[textFile.name] = Array.AsReadOnly(theData);
            }
        }

        /* 事前生成のデータ
        *
        * 1     問目 2-1   数字２個、演算子1個
        * 2 - 5 問目 3-2-0 数字３個、演算子２個(+,- 2個)
        * 6 -10 問目 3-2   数字３個、演算子２個
        * 11-15 問目 4-3-0 数字４個、演算子３個(+,- ３個)
        * 16-20 問目 4-2-1 数字４個、演算子３個(+,- 2個、*,/ 1個)
        * 21-   問目 4-3   数字４個、演算子３個
        */
        private static Dictionary<string, ReadOnlyCollection<(int[] Numbers, int Target, string Answer)>> data = new();

        # region 新しい問題を取得する時に使う

        private static Dictionary<string, int> _currentIndex = null;
        private static Dictionary<string, int> currentIndex
        {
            get
            {
                if (data.Count <= 0) return null;

                if (_currentIndex == null)
                {
                    _currentIndex = new();

                    foreach (string key in data.Keys)
                        _currentIndex[key] = 0;
                }

                return _currentIndex;
            }
        }

        internal enum Type
        {
            _2_1,
            _3_2_0,
            _3_2,
            _4_3_0,
            _4_2_1,
            _4_3,
            Invalid,
        }

        private static string ToKeyString(this Type type) => type switch
        {
            Type._2_1 => "2-1",
            Type._3_2_0 => "3-2-0",
            Type._3_2 => "3-2",
            Type._4_3_0 => "4-3-0",
            Type._4_2_1 => "4-2-1",
            Type._4_3 => "4-3",
            Type.Invalid => string.Empty,
            _ => string.Empty,
        };

        /// <summary>
        /// 問題のインデックスは、0始まり
        /// </summary>
        internal static Type ToQuestionType(this int preparingQuestionIndex) => preparingQuestionIndex switch
        {
            < 0 => Type.Invalid,
            < 1 => Type._2_1,
            < 5 => Type._3_2_0,
            < 10 => Type._3_2,
            < 15 => Type._4_3_0,
            < 20 => Type._4_2_1,
            _ => Type._4_3,
        };

        /// <summary>
        /// 取得に成功したらtrue、失敗したらfalseを返す
        /// </summary>
        internal static bool GetNewQuestion(this Type type, out int[] numbers, out int target, out string answer)
        {
            numbers = null;
            target = 0;
            answer = string.Empty;

            string key = type.ToKeyString();
            if (data.Count <= 0) return false;
            if (string.IsNullOrEmpty(key)) return false;
            if (!data.ContainsKey(key)) return false;

            var theData = data[key];
            var gotQuestion = theData[currentIndex[key]];
            numbers = gotQuestion.Numbers;
            target = gotQuestion.Target;
            answer = gotQuestion.Answer;
            currentIndex[key] = (currentIndex[key] + 1) % theData.Count;

            return true;
        }

        #endregion
    }
}
