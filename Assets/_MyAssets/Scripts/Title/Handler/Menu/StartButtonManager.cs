using General.Button;
using SO;

namespace Title.Handler.Menu
{
    internal sealed class StartButtonManager : ASceneChangeButtonManager
    {
        protected sealed override string toSceneName => SO_SceneName.Entity.Main;
    }
}