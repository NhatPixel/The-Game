using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Text.RegularExpressions;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputFieldHostPort;
    [SerializeField] private TMP_InputField inputFieldSeverIP;

    const string DefaultPort = "7777";

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void StartHost()
    {
        ushort port;

        string hostPort = inputFieldHostPort.text.Trim();
        if (string.IsNullOrEmpty(hostPort))
        {
            inputFieldHostPort.text = DefaultPort;
            hostPort = DefaultPort;
        }

        if (!ushort.TryParse(hostPort, out port) || port == 0)
        {
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("0.0.0.0", port, "0.0.0.0");

        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        string serverIP = inputFieldSeverIP.text.Trim();

        if (string.IsNullOrWhiteSpace(serverIP))
        {
            return;
        }

        string[] parts = serverIP.Split(':');
        if (parts.Length != 2)
        {
            return;
        }

        string ip = parts[0];
        string portStr = parts[1];

        if (!Regex.IsMatch(ip, @"^(\d{1,3}\.){3}\d{1,3}$"))
        {
            return;
        }

        string[] ipParts = ip.Split('.');
        foreach (string octet in ipParts)
        {
            if (!int.TryParse(octet, out int num) || num < 0 || num > 255)
            {
                return;
            }
        }
        
        if (!ushort.TryParse(portStr, out ushort port) || port == 0)
        {
            return;
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, port);

        NetworkManager.Singleton.StartClient();
    }
}
