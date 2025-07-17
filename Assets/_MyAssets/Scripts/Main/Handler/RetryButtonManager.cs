using General.Button;
using SO;

namespace Title.Handler
{
    internal sealed class RetryButtonManager : ASceneChangeButtonManager
    {
        protected sealed override string toSceneName => SO_SceneName.Entity.Main;
    }
}