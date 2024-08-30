using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Sprite", fileName = "SO_Sprite")]
    public class SO_Sprite : ScriptableObject
    {
        #region
        public const string PATH = "SO_Sprite";

        private static SO_Sprite _entity = null;
        public static SO_Sprite Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_Sprite>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [SerializeField] private Sprite _n0;
        internal Sprite N0 => _n0;

        [SerializeField] private Sprite _n1;
        internal Sprite N1 => _n1;

        [SerializeField] private Sprite _n2;
        internal Sprite N2 => _n2;

        [SerializeField] private Sprite _n3;
        internal Sprite N3 => _n3;

        [SerializeField] private Sprite _n4;
        internal Sprite N4 => _n4;

        [SerializeField] private Sprite _n5;
        internal Sprite N5 => _n5;

        [SerializeField] private Sprite _n6;
        internal Sprite N6 => _n6;

        [SerializeField] private Sprite _n7;
        internal Sprite N7 => _n7;

        [SerializeField] private Sprite _n8;
        internal Sprite N8 => _n8;

        [SerializeField] private Sprite _n9;
        internal Sprite N9 => _n9;

        [SerializeField] private Sprite _oa;
        internal Sprite OA => _oa;

        [SerializeField] private Sprite _os;
        internal Sprite OS => _os;

        [SerializeField] private Sprite _om;
        internal Sprite OM => _om;

        [SerializeField] private Sprite _od;
        internal Sprite OD => _od;

        [SerializeField] private Sprite _pl;
        internal Sprite PL => _pl;

        [SerializeField] private Sprite _pr;
        internal Sprite PR => _pr;
    }
}
