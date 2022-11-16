

using Microsoft.AspNetCore.SignalR;

namespace StormNet
{
    public class DataProxy
    {
        private readonly IHubContext<SignalRHub> _hubcontext;
        private string _data;

        public DataProxy(IHubContext<SignalRHub> hubcontext)
        {
            _hubcontext = hubcontext;
        }
        
        public void UpdateFromStormworks(string data)
        {
            _data = data;
            
            // Send to Controllers
            _hubcontext.Clients.All.SendAsync("ReceiveMessage",  "From stormnet.net");
        }

        public string GetForStormworks()
        {
            return _data;
        }
        
    }

}