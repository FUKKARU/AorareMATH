using General.Button;
using General;

namespace Title.Handler.Menu
{
    internal sealed class QuitConfirmYesButtonManager : ATextButtonManager
    {
        private protected sealed override void OnClickSucceeded()
        {
            GameQuitter.Quit();
        }
    }
}