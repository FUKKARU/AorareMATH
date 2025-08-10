using General;

namespace Title.Handler
{
    internal sealed class BillMover : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private Transform billPrefab;

        // 5度刻みで配置し、(75, 105)度の範囲にしか置かないので、高々7個で十分
        private static readonly int billCount = 7;
        private readonly Transform[] bills = new Transform[billCount];

        private float radius = 45.0f;
        private float rotation = 0.0f; // Deg

        private void Awake()
        {
            for (int i = 0; i < billCount; i++)
            {
                Transform bill = Instantiate(billPrefab, root);
                bills[i] = bill;
            }

            UpdateBills();
        }

        internal async UniTaskVoid Play(Ct ct)
        {
            if (SaveDataHolder.Data.DoFastenDirections)
            {
                radius = 51.5f;
                UpdateBills();
            }
            else
            {
                await 2.6f.SecAwait(ct: ct);
                await DOVirtual.Float(45.0f, 51.5f, 0.6f, UpdateRadius)
                    .SetEase(Ease.InQuad).WithCancellation(ct);
                await 0.8f.SecAwait(ct: ct);
            }

            await DOVirtual.Float(0.0f, -360.0f, 60.0f, UpdateRotation)
                .SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental).WithCancellation(ct);
        }

        private void UpdateRadius(float value)
        {
            radius = value;
            UpdateBills();
        }

        private void UpdateRotation(float value)
        {
            rotation = value;
            UpdateBills();
        }

        private void UpdateBills()
        {
            for (int i = 0; i < billCount; i++)
            {
                Transform bill = bills[i];
                if (bill == null) continue;

                float realAngleDeg = rotation + 105.0f - i * 5.0f;
                while (realAngleDeg <= 75.0f) realAngleDeg += 30.0f;
                while (realAngleDeg >= 105.0f) realAngleDeg -= 30.0f;
                float realAngleRad = realAngleDeg * Mathf.Deg2Rad;

                Vector2 pos = new Vector2(Mathf.Cos(realAngleRad), Mathf.Sin(realAngleRad)) * radius;
                bill.SetLocalPosXY(pos.x, pos.y);
                bill.SetLocalRotZ(realAngleDeg - 90.0f);
            }
        }
    }
}