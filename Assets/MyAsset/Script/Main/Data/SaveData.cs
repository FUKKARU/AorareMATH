using System;
using CI.QuickSave;

namespace Main.Data
{
    internal sealed class SaveData<T> : IDisposable
    {
        private QuickSaveReader reader = null;
        private QuickSaveWriter writer = null;

        // 暗号キーを直書きするとか正気か？
        private static readonly string password = "Iput_Fukkaru";
        private static readonly string root = "userData";
        private string key = null;

        internal T Data { get; set; } = default;

        internal SaveData(string key, T initValue = default)
        {
            this.key = key;
            Data = initValue;

            var settings = new QuickSaveSettings()
            {
                SecurityMode = SecurityMode.Aes,
                Password = password,
                CompressionMode = CompressionMode.Gzip
            };
            reader = QuickSaveReader.Create(root, settings);
            writer = QuickSaveWriter.Create(root, settings);
        }

        internal void Load()
        {
            if (!reader.Exists(root)) return;
            Data = reader.Read<T>(key);
        }

        internal void Save()
        {
            writer.Write(key, Data);
            writer.Commit();
        }

        public void Dispose()
        {
            reader = null;
            writer = null;
            key = null;
        }
    }
}