using UnityEngine;
using System.Collections.Generic;

namespace Main.Handler
{
    internal sealed class ShowQuestion : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI licensePlateText;
        [SerializeField] private TMPro.TextMeshProUGUI targetNumberText;

        private void Update()
        {
            if (GameManager.Instance.State == GameState.OnGoing)
            {
                (int n1, int n2, int n3, int n4) = GameManager.Instance.Question.N;
                int target = GameManager.Instance.Question.Target;

                licensePlateText.text = $"{n1}{n2}{n3}{n4}";
                targetNumberText.text = target.ToString();
            }
            else
            {
                licensePlateText.text = "";
                targetNumberText.text = "";
            }
        }
    }

    internal static class RandomGeneration
    {
        internal static ((int N1, int N2, int N3, int N4) N, int Target) RandomGenerate()
        {
            return (CreateLicensePlateNumbers(), CreateTargetNumber());
        }

        private static (int N1, int N2, int N3, int N4) CreateLicensePlateNumbers()
        {
            return CreateRandomNumbers
                    (
                    e => e.Count(0) <= 1,  // 0の数が1個以下
                    e => e.Count().Max() <= 2,  // 重複している数が2個以下
                    e => e.Count(0, 1, 7).Sum() <= 2  // 使いにくい数の合計が2個以下
                    );
        }

        private static int CreateTargetNumber()
        {
            return Random.Range(5, 16);
        }

        /// <summary>
        /// 条件を全て満たす組を生成
        /// </summary>
        private static (int N1, int N2, int N3, int N4) CreateRandomNumbers
            (params System.Func<(int N1, int N2, int N3, int N4), bool>[] functions)
        {
            int cnt = 0;
            while (true)
            {
                var n = CreateRandomNumbers();
                if (n.All(functions)) return n;
                if (++cnt > byte.MaxValue) return (0, 0, 0, 0);
            }
        }

        /// <summary>
        /// ランダムに組を生成
        /// </summary>
        private static (int N1, int N2, int N3, int N4) CreateRandomNumbers()
        {
            return (Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10));
        }
    }

    internal static class Iterator
    {
        /// <summary>
        /// 組の中で、targetの個数を数える
        /// </summary>
        internal static int Count(this (int N1, int N2, int N3, int N4) n, int target)
        {
            int ret = 0;
            if (n.N1 == target) ret++;
            if (n.N2 == target) ret++;
            if (n.N3 == target) ret++;
            if (n.N4 == target) ret++;
            return ret;
        }

        /// <summary>
        /// 組の中で、targetsの個数を順に数える
        /// </summary>
        internal static IEnumerable<int> Count(this (int N1, int N2, int N3, int N4) n, params int[] targets)
        {
            foreach (int e in targets)
            {
                yield return n.Count(e);
            }
        }

        /// <summary>
        /// 組の中で、0~9の個数を順に数える
        /// </summary>
        internal static IEnumerable<int> Count(this (int N1, int N2, int N3, int N4) n)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return n.Count(i);
            }
        }

        /// <summary>
        /// 全て条件を満たすか？
        /// </summary>
        internal static bool All<T>(this T e, params System.Func<T, bool>[] functions)
        {
            foreach (var f in functions)
            {
                if (!f(e))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// ひとつでも条件を満たすか？
        /// </summary>
        internal static bool Any<T>(this T e, params System.Func<T, bool>[] functions)
        {
            foreach (var f in functions)
            {
                if (f(e))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 最小を求める
        /// </summary>
        internal static int Min(this IEnumerable<int> itr)
        {
            int ret = int.MaxValue;
            foreach (int e in itr) ret = Mathf.Min(ret, e);
            return ret;
        }

        /// <summary>
        /// 最大を求める
        /// </summary>
        internal static int Max(this IEnumerable<int> itr)
        {
            int ret = int.MinValue;
            foreach (int e in itr) ret = Mathf.Max(ret, e);
            return ret;
        }

        /// <summary>
        /// 合計を求める
        /// </summary>
        internal static int Sum(this IEnumerable<int> itr)
        {
            int ret = 0;
            foreach (int e in itr) ret += e;
            return ret;
        }
    }
}