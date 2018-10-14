using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace RsaChat.Hubs
{
  public class ChatHub : Hub
  {
    private const int KeySize = 1024;   
    private static ConcurrentDictionary<string, EncryptorRSAKeys> KeyPairs;
    private static IHubCallerClients CallerClients;
    
    static ChatHub()
    {
      KeyPairs = new ConcurrentDictionary<string, EncryptorRSAKeys>();

      Task.Run(RegenerateKeys);
    }
    
    public async Task Send(string message)
    {
      var decrypted = EncryptorRSA.DecryptText(message, KeyPairs[Context.ConnectionId].PrivateKey);
      var parts = decrypted.Split("_|_");
      
      await Clients.All.SendAsync("Receive", parts.First(), parts.Last());
    }

    public async Task RequestPublicKey()
    {
      var keyPair = EncryptorRSA.GenerateKeys(KeySize);
      if (KeyPairs.TryAdd(Context.ConnectionId, keyPair))
      {
        Console.WriteLine($"Generated new key pair for client with ConnectionId '{Context.ConnectionId}:" +
                          $"\n\tPublic key: {keyPair.PublicKey}" +
                          $"\n\tPrivate key: {keyPair.PrivateKey}");
        
        await Clients.Caller.SendAsync("PublicKey", keyPair.PublicKey);
      }
    }

    public override Task OnConnectedAsync()
    {
      CallerClients = Clients;
      
      return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
      if (KeyPairs.ContainsKey(Context.ConnectionId))
      {
        KeyPairs.TryRemove(Context.ConnectionId, out _);
      }
      
      return base.OnDisconnectedAsync(exception);
    }

    private static async Task RegenerateKeys()
    {
      while (true)
      {
        await Task.Delay(TimeSpan.FromMinutes(30));

        foreach (var keyValue in KeyPairs)
        {
          var newPair = EncryptorRSA.GenerateKeys(KeySize);

          keyValue.Value.PublicKey = newPair.PublicKey;
          keyValue.Value.PrivateKey = newPair.PrivateKey;

          Console.WriteLine($"Generated new key pair for client with ConnectionId '{keyValue.Key}:" +
                            $"\n\tPublic key: {keyValue.Value.PublicKey}" +
                            $"\n\tPrivate key: {keyValue.Value.PrivateKey}");
          
          await CallerClients.Caller.SendAsync("PublicKey", keyValue.Value.PublicKey);
        }
      }
    }
  }
}