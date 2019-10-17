using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TechTweaking.Bluetooth;

[System.Serializable]
public class InputReceivedEvent : UnityEvent<char>
{

}

public class InputController : MonoBehaviour
{
    public bool bluetoothControl;
    public InputReceivedEvent inputReceivedEvent;

    public TextMesh debug;

    private BluetoothDevice device;

    void Awake()
    {
        BluetoothAdapter.enableBluetooth();
        device = new BluetoothDevice();
        device.Name = "LEBROWNJAMES";
        device.connect();
        device.setEndByte(10);
        debug.text = device.IsConnected.ToString();
        device.ReadingCoroutine = ManageConnection;
    }

    IEnumerator ManageConnection(BluetoothDevice device) {
        while (true) {
            BtPackets packets = device.readAllPackets();
            if (packets != null) {
                for (int i = 0; i < packets.Count; i++) {
                    int idx = packets.get_packet_offset_index(i);
                    int size = packets.get_packet_size(i);
                    string content = System.Text.Encoding.ASCII.GetString(packets.Buffer, idx, size);
                    //char direction;
                    //char.TryParse(content, out direction);
                    debug.text = content[0].ToString();
                    inputReceivedEvent.Invoke(content[0]);
                }
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!bluetoothControl)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                inputReceivedEvent.Invoke('U');
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                inputReceivedEvent.Invoke('D');
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                inputReceivedEvent.Invoke('L');
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                inputReceivedEvent.Invoke('R');
            if (Input.GetKeyDown(KeyCode.Space))
                inputReceivedEvent.Invoke('P');
        }
    }
}
