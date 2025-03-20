using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ShaderTransitionController : MonoBehaviour
{
    [Header("Settings")]
    public Material transitionMaterial; // �g�����W�V�����p�̃}�e���A��
    public float transitionSpeed = 1.0f; // �g�����W�V�����̑��x

    public  float Value = 0.0f; // �g�����W�V�����̐i�s��
    private bool isTransitioning = false; // �g�����W�V���������ǂ���
    private System.Action onTransitionComplete; // �g�����W�V�����������̃R�[���o�b�N

    /// <summary>
    /// �g�����W�V�������J�n����
    /// </summary>
    /// <param name="callback">�g�����W�V�����������ɌĂяo���R�[���o�b�N�i�C�Ӂj</param>
    public async UniTask StartTransition()
    {
        if (isTransitioning) return; // ���łɃg�����W�V�������Ȃ疳��

        isTransitioning = true;
        Value = 2.0f;

        while (Value > -1.0f)
        {
            Value -= Time.deltaTime * transitionSpeed;
            //Debug.Log(Value);
            transitionMaterial.SetFloat("_Height", Value);

            await UniTask.Yield(PlayerLoopTiming.Update); // �t���[���҂�
        }

        // �g�����W�V����������Ԃ�ݒ�
        Value = -1.0f;
        transitionMaterial.SetFloat("_Height", Value);
        isTransitioning = false;
    }


    public async UniTask EndTransition()
    {
        if (isTransitioning) return; // ���łɃg�����W�V�������Ȃ疳��

        isTransitioning = true;
        Value = -1.0f;

        while (Value < 2f)
        {
            Value += Time.deltaTime * transitionSpeed;
            //Debug.Log(Value);
            transitionMaterial.SetFloat("_Height", Value);

            await UniTask.Yield(PlayerLoopTiming.Update); // �t���[���҂�
        }

        // �g�����W�V����������Ԃ�ݒ�
        Value = 2f;
        transitionMaterial.SetFloat("_Height", Value);
        isTransitioning = false;
    }


   

    /// <summary>
    /// �g�����W�V���������Z�b�g�i�l���������j
    /// </summary>
    public void ResetTransition()
    {
        isTransitioning = false;
        Value = 3.0f;
        transitionMaterial.SetFloat("_Value", Value);
    }
}






//public ShaderTransitionController transitionController; // �g�����W�V�����Ǘ��X�N���v�g�̎Q��

//private void Start()
//{
//    // �g�����W�V�������J�n���A������ɏ��������s
//    transitionController.StartTransition(OnTransitionComplete);
//}

//private void OnTransitionComplete()
//{
//    Debug.Log("�g�����W�V�������������܂����I");
//    // �g�����W�V����������̏����������ɋL�q
//}

//private void Update()
//{
//    // �C�ӂ̃^�C�~���O�Ńg�����W�V���������Z�b�g�\
//    if (Input.GetKeyDown(KeyCode.R))
//    {
//        transitionController.ResetTransition();
//        Debug.Log("�g�����W�V���������Z�b�g���܂����I");
//    }
//}