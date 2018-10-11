using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace RsaChat.Hubs
{
  public class ChatHub : Hub
  {
    private const int KeySize = 1024;   
    private static readonly EncryptorRSAKeys Keys;

    static ChatHub()
    {
      Keys = EncryptorRSA.GenerateKeys(KeySize);
    }
    
    public async Task Send(string message)
    {
      var decrypted = EncryptorRSA.DecryptText(message, Keys.PrivateKey);
      var parts = decrypted.Split("_|_");
      
      await Clients.All.SendAsync("Receive", parts.First(), parts.Last());
    }

    public async Task RequestPublicKey()
    {
      await Clients.Caller.SendAsync("PublicKey", Keys.PublicKey);
    }
  }
}