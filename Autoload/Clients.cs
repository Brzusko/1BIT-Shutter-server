using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using bit_shuter_server.Autoload.Structs;

public class Clients : Node
{
    public IList ClientsToSync {
        get {
            var clients = _clients.Where(client => client.Value.State == Client.ClientState.SYNCING);
            return clients.ToArray();
        }
    }

    public bool CanStartGame {
        get => (_clients.Where(client => client.Value.State == Client.ClientState.LOBBY_READY).ToArray().Length == _clients.Count) && _clients.Count >= 2;
    }

    public int ClientsCount {
        get => _clients.Count;
    }
    private Dictionary<int, Client> _clients = new Dictionary<int, Client>();

    public void RegisterClient(int id) {
        if (_clients.ContainsKey(id)) return;
        var newClient = new Client{
            ClientName = "",
            State = Client.ClientState.WAITING_FOR_CREDENTIALS
        };
        _clients[id] = newClient;
    }

    public void SetClientCredentials(Credentials creds, int id) {
        if (!_clients.ContainsKey(id)) return;
        var client = _clients[id];
        client.State = Client.ClientState.TIME_SYNCING;
        client.ClientName = creds.ClientName;
    }

    public void ClientFinishedClockSync(int id) {
        if (!_clients.ContainsKey(id)) return ;
        var client = _clients[id];
        client.State = Client.ClientState.LOBBY_NOT_READY;
    }
    public void DestroyClientOnDisconnect(int id) {
        if (!_clients.ContainsKey(id)) return;
        // Niszczenie gracza w świecie
        _clients.Remove(id);
    }
}
