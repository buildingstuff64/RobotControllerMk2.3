using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.UI;

public class SerialManager : MonoBehaviour
{
    private SerialController SerialController;
    [SerializeField] private UIManager UI;

    [Tooltip("Baud rate that the serial device is using to transmit data.")]
    public int baudRate = 115200;

    [Tooltip("After an error in the serial communication, or an unsuccessful " +
             "connect, how many milliseconds we should wait.")]
    public int reconnectionDelay = 50;

    [Tooltip("Maximum number of unread data messages in the queue. " +
             "New messages will be discarded.")]
    public int maxUnreadMessages = 3;

    // Constants used to mark the start and end of a connection. There is no
    // way you can generate clashing messages from your serial device, as I
    // compare the references of these strings, no their contents. So if you
    // send these same strings from the serial device, upon reconstruction they
    // will have different reference ids.
    public const string SERIAL_DEVICE_CONNECTED = "__Connected__";
    public const string SERIAL_DEVICE_DISCONNECTED = "__Disconnected__";

    // Internal reference to the Thread and the object that runs in it.
    protected Thread thread;
    protected SerialThreadLines serialThread;
    private bool _serialconnected = false;
    public string lastMsg = "";
    private string lastCom = "COM14";


    void Start()
    {
        SerialController = GetComponent<SerialController>();
        string[] ports = SerialPort.GetPortNames();
        UI.setCommPortOptions(new List<string>(ports));


    }

    private void Update()
    {
        if (_serialconnected == false) return;

        string s = ReadSerial();
        if (s == null) return;
        s.TrimEnd();
        lastMsg = s;
        UI.serialIn(s);
    }


    // Update is called once per frame
    public void ConnectSerial()
    {
        connectToSerial(UI.getComm());
    }

    private void connectToSerial(string com)
    {
        lastCom = com;
        DisconnectSerial();
        try
        {
            serialThread = new SerialThreadLines(com,
                                                 baudRate,
                                                 reconnectionDelay,
                                                 maxUnreadMessages);
            thread = new Thread(new ThreadStart(serialThread.RunForever));
            thread.Start();
        }
        catch (Exception e) { h = false; Debug.LogError(e); throw; }
        StartCoroutine(Iconnect());
    }

    IEnumerator Iconnect()
    {
        _serialconnected = false;
        for (int i = 0; i < 100; i++)
        {
            string str = (string)serialThread.ReadMessage();
            if (str != null) { Debug.Log(str); }
            if (str == SERIAL_DEVICE_CONNECTED)
            {
                Debug.Log("_CONNECTED_");
                UI.hideConnect(false);
                _serialconnected = true;
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
        }
        if (_serialconnected == false)
        {
            Debug.Log("_CONNECTION_TIMEOUT_");
            DisconnectSerial();
            yield return null;
        }
    }

    public void DisconnectSerial()
    {
        _serialconnected = false;
        if (userDefinedTearDownFunction != null)
            userDefinedTearDownFunction();

        // The serialThread reference should never be null at this point,
        // unless an Exception happened in the OnEnable(), in which case I've
        // no idea what face Unity will make.
        if (serialThread != null)
        {
            serialThread.RequestStop();
            serialThread = null;
        }

        // This reference shouldn't be null at this point anyway.
        if (thread != null)
        {
            thread.Join();
            thread = null;
        }
    }

    public void refresh()
    {
        UI.clearSerial();
        connectToSerial(lastCom);
    }

    private void OnDisable()
    {
        DisconnectSerial();
    }

    public string ReadSerial()
    {
        if (!_serialconnected) return null;
        return (string)serialThread.ReadMessage();
    }

    public void SendSerial(string str)
    {
        if (!_serialconnected) return;
        serialThread.SendMessage(str);
    }

    public void SendInstruction(string str)
    {
        SendSerial(string.Format("I#{0}", str));
    }


    public delegate void TearDownFunction();
    private TearDownFunction userDefinedTearDownFunction;
    public void SetTearDownFunction(TearDownFunction userFunction)
    {
        this.userDefinedTearDownFunction = userFunction;
    }
}
