namespace bit_shuter_server.Autoload.Structs
{
    public struct Client {
        public enum ClientState {
            WAITING_FOR_CREDENTIALS,
            TIME_SYNCING,
            JOINING_WORLD,
            LOBBY_NOT_READY,
            LOBBY_READY,
            SYNCING,
        }
        public string ClientName {get; set;}
        public ClientState State {get; set;}
    }
}