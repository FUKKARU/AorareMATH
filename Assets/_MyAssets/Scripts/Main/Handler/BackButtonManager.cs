using General;
using General.Button;

namespace Title.Handler
{
    internal sealed class BackButtonManager : ASceneChangeButtonManager
    {
        protected sealed override Scene toScene => Scene.Title;
    }
}