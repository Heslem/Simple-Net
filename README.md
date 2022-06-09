# Simple-Network-framework
It's framework for working with TCP protocols. You can send packets.
____
### Example server
```C#
  Server server = Server.GetServer(); // get a server, need for working with all clients
  server.Init();

  server.ClientConnect += (client) =>
  {
      Console.WriteLine("User join.");
      client.Send(new Packet("hi", "welcome to server"));
  }; // subscribe to clientConnect, and send to client the packet with id "hi", and value "welcome to server"
  
  server.ClientDisconnect += (c) =>
  {
      Console.WriteLine("User left.");
  };

  while (true)
  {
      if (server.CountConnections != 0)
          server.Broadcast(new Packet("message", "123")); // sending all clients a packet 
  }
  server.Stop();
```
___
### Example client
```C#
  Client client = new Client("127.0.0.1", 8008); // ip adress, and port 


  client.Recieve += (message) =>
  {
      Console.WriteLine(message.Value); 
  }; // subscribe to recieve messages
  
  while (true)
  {
      client.Send(new Packet("client", "im here")); // send to server a packet
  }
```
