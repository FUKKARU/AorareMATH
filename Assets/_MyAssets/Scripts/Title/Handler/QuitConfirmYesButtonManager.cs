using General.Button;
using General;

namespace Title.Handler
{
    internal sealed class QuitConfirmYesButtonManager : ATextButtonManager
    {
        protected sealed override void OnClickSucceeded()
        {
            GameQuitter.Quit();
        }
    }
}