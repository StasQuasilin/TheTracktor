using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

public partial class MainController : MonoBehaviour
{
    // Start is called before the first frame update
    public string address = "192.168.0.100";
    //private IPEndPoint _remoteEndPoint;
    private TcpClient _tcpClient;
    private float _lastSend;

    private AbstractController _controller;
    public Dictionary<ControlType, AbstractController> _controllers = new Dictionary<ControlType, AbstractController>();
    
    public int dutyL, dutyR;
    [Range(0.2f, 1)]
    public float throttle = 1;
    
    [FormerlySerializedAs("_controlType")] public ControlType controlType = ControlType.Joystick;

    private void Start()
    {
        _isControllerNull = _controller == null;
    }

    private void Awake()
    {

        var controllers = FindObjectsOfType<AbstractController>();
        foreach (var abstractController in controllers)
        {
            _controllers.Add(abstractController.GetControlType(), abstractController);
            abstractController.gameObject.SetActive(false);
        }
#if UNITY_EDITOR
        if (_controllers.ContainsKey(ControlType.Keyboard))
        {
            controlType = ControlType.Keyboard;
        }
#endif
        ChangeControl(controlType);
    }

    private void Receive() {
        Debug.Log("Receive Thread start");
        while (_tcpClient != null && _tcpClient.Connected) {
            int available = _tcpClient.Client.Available;
            if (available > 0) {
                byte[] buffer = new byte[available];
                _tcpClient.Client.Receive(buffer);
                //Debug.Log(Encoding.UTF8.GetString(buffer));
                SendDriveCommand();
            }
        }
        Debug.Log("Receive Thread stop");
    }

    private void Update() {
        if(_tcpClient?.Client == null) return;
        
        if (_tcpClient == null || !_tcpClient.Connected || !(Time.time > _lastSend + 0.01f)) return;

        if (_isControllerNull) return;
        
        var controllerInput = _controller.GetInput();

        float x = controllerInput.x;
        float y = controllerInput.y;

        dutyL = (int) ((x - y) * 255);
        dutyR = (int) ((x + y) * 255);

        _lastSend = Time.time;
    }

    private void SendDriveCommand() {
        //{"dl":-71,"dr":71}
        Send("{\"dl\":" + (int)(dutyL * throttle) + ",\"dr\":" + (int)(dutyR * throttle) + "}");
    }

    private Rect _addressFieldRect = new Rect(0, 2, 100, 20);
    private Rect _connectButtonRect = new Rect( 0, 22, 100, 40 );

    private Rect _leftStick = new Rect(2, 20, 20, 0);
    private Rect _throtRect = new Rect(2, 20, 100, 0);

    private Rect _controllTypeButtons = new Rect(0, 0, 20, 20);
    private bool _isControllerNull;

    private void OnGUI() {
        _addressFieldRect.x = (Screen.width - _addressFieldRect.width ) / 2;
        _connectButtonRect.x = (Screen.width - _connectButtonRect.width ) / 2;
        address = GUI.TextField( _addressFieldRect, address );
        if (_tcpClient != null && _tcpClient.Connected) {
            if (GUI.Button( _connectButtonRect, "Disconnect" )) {
                Disconnect();
            }
        } else {
            if (GUI.Button( _connectButtonRect, "Connect" )) {
                Connect();
            }
        }

        _throtRect.height = Screen.height - 40;
        _throtRect.x = Screen.width - 60;
        throttle = GUI.VerticalSlider(_throtRect, throttle, 1, 0.2f);

        _controllTypeButtons.x = 2;
        _controllTypeButtons.y = Screen.height - 22;

        foreach (var value in _controllers.Keys)
        {
            if (GUI.Button(_controllTypeButtons, value == controlType ? "O" : "X"))
            {
                ChangeControl(value);
                
            }
            _controllTypeButtons.x += 22;
        }
    }

    private void ChangeControl(ControlType value)
    {
        Debug.Log(value);
        if (_controller != null)
            _controller.gameObject.SetActive(false);
        _controller = _controllers[value];
        _controller.gameObject.SetActive(true);
        controlType = value;
    }

    public void OnApplicationQuit() {
        Disconnect();
    }

    void Connect() {
        Disconnect();
        _tcpClient = new TcpClient();
        _tcpClient.Connect(address, 80);
        new Thread(Receive){IsBackground = true}.Start();
    }

    void Disconnect() {
        if (_tcpClient != null && _tcpClient.Connected)         {
            _tcpClient.Close();
        }
        _tcpClient = null;
    }

    private void Send(String msg) {
        _tcpClient.Client.Send( Encoding.UTF8.GetBytes(msg) );
    }
}

