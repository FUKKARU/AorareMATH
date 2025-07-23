using General;
using General.Button;

namespace Title.Handler
{
    internal sealed class StartButtonManager : ASceneChangeButtonManager
    {
        protected sealed override Scene toScene => Scene.Main;
    }
}