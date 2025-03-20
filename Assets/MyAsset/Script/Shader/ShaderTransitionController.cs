using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ShaderTransitionController : MonoBehaviour
{
    [Header("Settings")]
    public Material transitionMaterial; // トランジション用のマテリアル
    public float transitionSpeed = 1.0f; // トランジションの速度

    public  float Value = 0.0f; // トランジションの進行状況
    private bool isTransitioning = false; // トランジション中かどうか
    private System.Action onTransitionComplete; // トランジション完了時のコールバック

    /// <summary>
    /// トランジションを開始する
    /// </summary>
    /// <param name="callback">トランジション完了時に呼び出すコールバック（任意）</param>
    public async UniTask StartTransition()
    {
        if (isTransitioning) return; // すでにトランジション中なら無視

        isTransitioning = true;
        Value = 2.0f;

        while (Value > -1.0f)
        {
            Value -= Time.deltaTime * transitionSpeed;
            //Debug.Log(Value);
            transitionMaterial.SetFloat("_Height", Value);

            await UniTask.Yield(PlayerLoopTiming.Update); // フレーム待ち
        }

        // トランジション完了状態を設定
        Value = -1.0f;
        transitionMaterial.SetFloat("_Height", Value);
        isTransitioning = false;
    }


    public async UniTask EndTransition()
    {
        if (isTransitioning) return; // すでにトランジション中なら無視

        isTransitioning = true;
        Value = -1.0f;

        while (Value < 2f)
        {
            Value += Time.deltaTime * transitionSpeed;
            //Debug.Log(Value);
            transitionMaterial.SetFloat("_Height", Value);

            await UniTask.Yield(PlayerLoopTiming.Update); // フレーム待ち
        }

        // トランジション完了状態を設定
        Value = 2f;
        transitionMaterial.SetFloat("_Height", Value);
        isTransitioning = false;
    }


   

    /// <summary>
    /// トランジションをリセット（値を初期化）
    /// </summary>
    public void ResetTransition()
    {
        isTransitioning = false;
        Value = 3.0f;
        transitionMaterial.SetFloat("_Value", Value);
    }
}






//public ShaderTransitionController transitionController; // トランジション管理スクリプトの参照

//private void Start()
//{
//    // トランジションを開始し、完了後に処理を実行
//    transitionController.StartTransition(OnTransitionComplete);
//}

//private void OnTransitionComplete()
//{
//    Debug.Log("トランジションが完了しました！");
//    // トランジション完了後の処理をここに記述
//}

//private void Update()
//{
//    // 任意のタイミングでトランジションをリセット可能
//    if (Input.GetKeyDown(KeyCode.R))
//    {
//        transitionController.ResetTransition();
//        Debug.Log("トランジションをリセットしました！");
//    }
//}