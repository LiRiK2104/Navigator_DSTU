
using AR;

namespace UI.AR.Buttons
{
    public class RetryButton : CloseErrorButton
    {
        private ARMain ARMain => Global.Instance.ArMain;


        protected override void Action()
        {
            base.Action();
            EnterToAR();
        }
        
        private void EnterToAR()
        {
            ARMain.Enter();    
        }
    }
}
