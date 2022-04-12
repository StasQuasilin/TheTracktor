using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using DitzeGames.MobileJoystick;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MainController : MonoBehaviour
{
    // Start is called before the first frame update
    public string address = "192.168.0.100";
    //private IPEndPoint _remoteEndPoint;
    private TcpClient _tcpClient;
    private float lastSend;
    
    public int dutyL, dutyR;
    [Range(0.2f, 1)]
    public float throttle = 1;

    private Joystick _joystick;
    private TouchField _touchField;

    private void Awake() {
        _joystick = FindObjectOfType<Joystick>();
        _touchField = FindObjectOfType<TouchField>();
    }

    void Start() {
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

    void Update() {
        if(_tcpClient?.Client == null) return;
        
        if (_tcpClient == null || !_tcpClient.Connected || !(Time.time > lastSend + 0.01f)) return;

        //float x = Input.GetAxis("Vertical");
        //float y = Input.GetAxis("Horizontal") * 0.5f;
        float x = _joystick.AxisNormalized.y;
        float y = _joystick.AxisNormalized.x;

        dutyL = (int) ((x - y) * 255);
        dutyR = (int) ((x + y) * 255);

        lastSend = Time.time;
        
    }

    void SendDriveCommand() {
        Send("{\"dl\":" + (int)(dutyL * throttle) + ",\"dr\":" + (int)(dutyR * throttle) + "}");
    }

    private Rect _addressFieldRect = new Rect(0, 2, 200, 40);
    private Rect _connectButtonRect = new Rect( 0, 2, 80, 20 );

    private Rect _leftStick = new Rect(2, 20, 20, 0);
    private Rect _throtRect = new Rect(2, 20, 50, 0);
    void OnGUI() {
        _addressFieldRect.x = (Screen.width - 200 ) / 2;
        _connectButtonRect.x = _addressFieldRect.x + _addressFieldRect.width + 2;
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

