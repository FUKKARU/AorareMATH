using UnityEngine;

namespace General
{
    // 全てのシーンに配置したい
    public sealed class DataSaverOnDestroy : MonoBehaviour
    {
        private void OnDestroy()
        {
            SaveDataHolder.Save();
            "SaveData was saved because this object was destroyed.".Log();
        }
    }
}