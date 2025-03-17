using General;
using SO;

namespace Title.Handler
{
    internal sealed class BackButtonManager : ASceneChangeButtonManager
    {
        protected override string toSceneName => SO_SceneName.Entity.Title;
    }
}