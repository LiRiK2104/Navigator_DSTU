using AR.Calibration;
using Helpers;

namespace UI.AR.Buttons
{
    public class CalibrationButton : SelfInitializingButton
    {
        private Calibrator Calibrator => Global.Instance.ArMain.Calibrator;

        protected override void Action() => StartCalibration();

        private void StartCalibration()
        {
            Calibrator.StartCalibration();
        }
    }
}
