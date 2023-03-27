using Calibration;
using Helpers;
using Navigation;
using UI;
using UnityEngine;

public class Global : Singleton<Global>
{
    [SerializeField] private ARMain _arMain;
    [SerializeField] private ARValidator _validator;
    [SerializeField] private Calibrator _calibrator;
    [SerializeField] private AREnvironment _arEnvironment;
    [SerializeField] private Navigator _navigator;
    [SerializeField] private DataBase _dataBase;
    [SerializeField] private UISetter _uiSetter;
    [SerializeField] private UISetterV2 _uiSetterV2;
    [SerializeField] private CameraContainer _cameraContainer;
    [SerializeField] private BordersSetter _bordersSetter;
    

    public ARMain ArMain => _arMain;
    public ARValidator Validator => _validator;
    public Calibrator Calibrator => _calibrator;
    public AREnvironment ArEnvironment => _arEnvironment;
    public Navigator Navigator => _navigator;
    public DataBase DataBase => _dataBase;
    public UISetter UiSetter => _uiSetter;
    public UISetterV2 UISetterV2 => _uiSetterV2;
    public CameraContainer CameraContainer => _cameraContainer;
    public BordersSetter BordersSetter => _bordersSetter;
}
