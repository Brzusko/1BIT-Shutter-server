using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using bit_shuter_server.Autoload.Structs;

public class Clients : Node
{
    public IList<Client> ClientsToSync {
        get {
            var clients = _clients.Where(client => client.Value.State == Client.ClientState.SYNCING).Select(client => client.Value);
            return clients.ToList<Client>();
        }
    }

    public IList<Client> ClientsInLobby {
        get {
            var clients = _clients.Where(client => client.Value.State == Client.ClientState.LOBBY_NOT_READY || client.Value.State == Client.ClientState.LOBBY_READY).Select(client => client.Value);
            return clients.ToList<Client>();
        }
    }

    public bool CanStartGame {
        get => (_clients.Where(client => client.Value.State == Client.ClientState.LOBBY_READY).ToArray().Length == _clients.Count) && _clients.Count >= 1;
    }

    public int ClientsCount {
        get => _clients.Count;
    }
    private Dictionary<int, Client> _clients = new Dictionary<int, Client>();

    public void RegisterClient(int _id) {
        if (_clients.ContainsKey(_id)) return;
        var newClient = new Client{
            ClientName = "",
            State = Client.ClientState.WAITING_FOR_CREDENTIALS,
            id = _id
        };
        _clients[_id] = newClient;
    }

    public void SetClientCredentials(Credentials creds, int id) {
        if (!_clients.ContainsKey(id)) return;
        var client = _clients[id];
        client.State = Client.ClientState.TIME_SYNCING;
        client.ClientName = creds.ClientName;
        _clients[id] = client;
    }

    public void ClientFinishedClockSync(int id) {
        if (!_clients.ContainsKey(id)) return ;
        var client = _clients[id];
        client.State = Client.ClientState.LOBBY_NOT_READY;
        _clients[id] = client;
    }
    public void DestroyClientOnDisconnect(int id) {
        if (!_clients.ContainsKey(id)) return;
        // Niszczenie gracza w świecie
        _clients.Remove(id);
    }

    public void ChangePlayerReadyState(int id, bool state) {
        if (!_clients.ContainsKey(id)) return;
        var client = _clients[id];
        client.State = state ? Client.ClientState.LOBBY_READY : Client.ClientState.LOBBY_NOT_READY;
        _clients[id] = client;
    }
    public Client GetClientByID(int id) => _clients[id];
}
