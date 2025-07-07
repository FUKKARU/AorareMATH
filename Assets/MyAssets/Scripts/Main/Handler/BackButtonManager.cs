using General.Button;
using SO;

namespace Title.Handler
{
    internal sealed class BackButtonManager : ASceneChangeButtonManager
    {
        protected sealed override string toSceneName => SO_SceneName.Entity.Title;
    }
}