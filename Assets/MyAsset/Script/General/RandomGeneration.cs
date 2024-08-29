using UnityEngine;
using TMPro;

namespace Main.Sample
{
    internal sealed class RandomGeneraation : MonoBehaviour
    {
        // 数字を表示するTextMeshProオブジェクト
        [SerializeField] private TextMeshProUGUI licensePlateText;
        [SerializeField] private TextMeshProUGUI targetNumberText;

        private void Update()
        {
            // キーボードのエンターが押されたとき
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // 0以上21未満の数をランダムに生成し表示
                int targetNumber = UnityEngine.Random.Range(0, 21);
                targetNumberText.text = "＜目標数字＞ \n" + targetNumber.ToString();

                int licensePlateNumber;
                bool foundSolution = false; //生成されたナンバーと目標が計算可能か

                // ナンバープレートの数字と目標数字が一致するまで繰り返し生成
                do
                {
                    // 1000以上10000未満の数をランダムに生成
                    licensePlateNumber = UnityEngine.Random.Range(1000, 10000);
                    string licensePlateString = licensePlateNumber.ToString();

                    // 数字を分割
                    int a = int.Parse(licensePlateString[0].ToString());
                    int b = int.Parse(licensePlateString[1].ToString());
                    int c = int.Parse(licensePlateString[2].ToString());
                    int d = int.Parse(licensePlateString[3].ToString());

                    // 演算子の全組み合わせを試す
                    string[] operators = { "+", "-", "*", "/" };
                    foreach (var op1 in operators)
                    {
                        foreach (var op2 in operators)
                        {
                            foreach (var op3 in operators)
                            {
                                int result = Calculate(a, b, c, d, op1, op2, op3);
                                if (result == targetNumber)
                                {
                                    foundSolution = true; // 解が見つかった　ヾ(≧∇≦*)/
                                    break;
                                }
                            }
                            if (foundSolution) break;
                        }
                        if (foundSolution) break;
                    }

                    // 解が見つかった場合、ナンバープレートを表示
                } while (!foundSolution);

                // 解が見つかったナンバープレートを表示
                licensePlateText.text = "＜ナンバープレート＞ \n" + licensePlateNumber.ToString();
            }
        }

        // 4つの数値と3つの演算子を使って計算
        private int Calculate(int a, int b, int c, int d, string op1, string op2, string op3)
        {
            int result = ApplyOperation(a, b, op1);
            result = ApplyOperation(result, c, op2);
            result = ApplyOperation(result, d, op3);
            return result;
        }

        // 指定された演算子で計算を実行
        private int ApplyOperation(int x, int y, string operation)
        {
            return operation switch
            {
                "+" => x + y,
                "-" => x - y,
                "*" => x * y,
                "/" => y != 0 ? x / y : 0, // どんな数字も０で割れない　(ŏ﹏ŏ。)
                _ => 0,
            };
        }
    }
}