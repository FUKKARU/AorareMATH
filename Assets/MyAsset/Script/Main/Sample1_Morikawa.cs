// �K�v��using�ȊO�͏����Ă���
using System.Collections.Generic;
using UnityEngine;

// �S�Ė��O��Ԃň͂��Ă��������B���O��ԂƂ́A�N���X��֐����̃o�b�e�B���O������邽�߂ɁA�t�H���_�𕪂��Đ�������݂����Ȃ��́B
// ���̏������̏ꍇ�AMain�Ƃ������O��Ԃ̒��́ASample�Ƃ������O��Ԃ̒��ɓ���A�Ƃ����Ӗ��B
namespace Main.Sample
{
    // ��{�I�ɁApublic�̑����internal���g���A�N���X�ɂ͑S��sealed��t���邱�Ƃ𐄏��B
    // ���yinternal�z�O�����W���[������̃A�N�Z�X���֎~����B���W���[���Ƃ�DLL�t�@�C���̂��ƁB
    // �@�@�@�@�@�@�@ �f�t�H���g�ł̓X�N���v�g�S�Ă�1��DLL�t�@�C���ɃR���p�C�������̂ŁA���ړI�ȈӋ`�͂Ȃ��B
    //�@�@�@�@�@�@�@�@�����A�J�v�Z�������Ă������Ƃɂ��������Ƃ͂Ȃ��̂ŁA�������ɂ��邱�Ƃ𐄏����܂��B
    // ���ysealed�z���̃N���X�̌p�����֎~����A�Ƃ����Ӗ��B
    //            internal�Ɠ��l�ɁA���sealed���O���̂͊ȒP�����t����̂͑�ςȂ̂ŁA�Ƃ肠�����t���Ă����Ɨǂ��B
    internal sealed class Sample1 : MonoBehaviour
    {
        // �R�[�f�B���O�K��Ɋ�Â��āA�A�N�Z�X�C���q�͖������Ăق����ł��B
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
            // �����̒l
            private int _integer;
            internal int Integer => _integer;

            // ������̒l
            private string _string;
            internal string String => _string;

            // �R���X�g���N�^1
            internal IntStr(int _integer)
            {
                this._integer = _integer;
            }

            // �R���g���N�^2
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
            // ��Ԃ���y�B�����A�����ƕ�����ȊO�̌^�����蓾��̂ŁA�戵���ӁB
            List<object> list1 = new() { 1, 2, "+", "(", 5 };

            // �����ƕ����񂵂�����Ȃ����Ƃ��A�m���ɕۏ؂����B�����A�����̂��ʓ|�B
            List<IntStr> list2 = new() { new(1), new(2), new("+"), new("("), new(5) };

            // ��2�̒��Ԃ��Ƃ����A�o�����X�^�B�����A�����l�ł��̂܂܌v�Z���邱�Ƃ͂ł��Ȃ��B
            List<N> list3 = new() { N.N1, N.N2, N.OA, N.PL, N.N5 };
        }
    }
}