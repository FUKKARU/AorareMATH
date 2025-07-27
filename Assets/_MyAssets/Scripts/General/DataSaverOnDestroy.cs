namespace General
{
    // 全てのシーンに配置したい
    internal sealed class DataSaverOnDestroy : MonoBehaviour
    {
        private void OnDestroy()
        {
            SaveDataHolder.Save();
            "SaveData was saved because this object was destroyed.".Log();
        }
    }
}