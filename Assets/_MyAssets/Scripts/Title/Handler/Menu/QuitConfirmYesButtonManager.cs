using General.Button;
using General;

namespace Title.Handler.Menu
{
    internal sealed class QuitConfirmYesButtonManager : ATextButtonManager
    {
        protected sealed override void OnClickSucceeded()
        {
            GameQuitter.Quit();
        }
    }
}