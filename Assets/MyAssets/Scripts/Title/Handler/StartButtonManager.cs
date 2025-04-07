using General;
using SO;

namespace Title.Handler
{
    internal sealed class StartButtonManager : ASceneChangeButtonManager
    {
        protected override string toSceneName => SO_SceneName.Entity.Main;
    }
}