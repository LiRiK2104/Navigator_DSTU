using System;
using AR;
using Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AR.Buttons
{
    [RequireComponent(typeof(Button))]
    public class AREnableButton : SelfInitializingButton
    {
        [SerializeField] private bool _arEnable;

        private ARMain ARMain => Global.Instance.ArMain;


        protected override void Action() => SetAREnable();

        private void SetAREnable()
        {
            if (_arEnable)
                ARMain.Enter();
            else
                ARMain.Exit();
        }
    }
}
