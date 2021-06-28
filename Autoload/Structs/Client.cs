using Godot.Collections;
namespace bit_shuter_server.Autoload.Structs
{
    public struct Client {
        public enum ClientState {
            WAITING_FOR_CREDENTIALS = 0,
            TIME_SYNCING = 1,
            JOINING_WORLD = 2,
            LOBBY_NOT_READY = 3,
            LOBBY_READY = 4,
            LOADED_WORLD = 5,
            SYNCING = 6,
        }
        public string ClientName {get; set;}
        public ClientState State {get; set;}
        public int id {get; set;}

        public Dictionary<string, object> AsGDDict {
            get {
                return new Dictionary<string, object> {
                    { "id", id },
                    { "ClientName", ClientName},
                    { "State", State }
                };
            }
        }
    }
}