using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Unity.Netcode;

public class ClientSocket : NetworkBehaviour
{
    private TcpClient _client;
    private NetworkStream _stream;
    private bool _isConnected = false;

    void Start()
    {
        if(!IsOwner) return;
        ConnectToServer("127.0.0.1", 5000);
    }

    void Update()
    {
        if (_isConnected && _stream.DataAvailable)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = _stream.Read(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Debug.Log($"Mensagem do servidor: {message}");
        }
        
        if(!IsOwner) return;
        // Enviar mensagens de teste usando a tecla espaço
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessageToServer("Mensagem do jogador");
        }
    }

    private void ConnectToServer(string ipAddress, int port)
    {
        try
        {
            _client = new TcpClient(ipAddress, port);
            _stream = _client.GetStream();
            _isConnected = true;
            Debug.Log("Conectado ao servidor!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro ao conectar: {ex.Message}");
        }
    }

    private void SendMessageToServer(string message)
    {
        if (_isConnected)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            _stream.Write(buffer, 0, buffer.Length);
        }
    }

    private void OnApplicationQuit()
    {
        if(!IsOwner) return;
        if (_isConnected)
        {
            _stream.Close();
            _client.Close();
        }
    }
}
