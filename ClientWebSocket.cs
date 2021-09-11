using System;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

class Stager
{
    private static string GUID = GenerateStagerName();

    public static void Main(string[] args)
    {
        Task ws = Task.Run(() => ConnectWS());
        Task hb = Task.Run(() => Heartbeat());
        Task.WaitAll(hb, ws);
    }

    private static string GenerateStagerName()
    {
        Random random = new Random();

        string alphanumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string stagerName = "";

        for (int i = 0; i < 6; i++)
        {
            int pos = random.Next(0, alphanumeric.Length);
            stagerName += alphanumeric.ToCharArray()[pos];
        }

        return stagerName;
    }

    private static async Task Heartbeat()
    {
        using (ClientWebSocket ws = new ClientWebSocket())
        {
            Uri serverUri = new Uri("ws://127.0.0.1:8080/socket");

            await ws.ConnectAsync(serverUri, CancellationToken.None);

            while (ws.State == WebSocketState.Open)
            {
                Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                /*
                using var ms = new MemoryStream();
                using var writer = new Utf8JsonWriter(ms);
                writer.WriteStartObject();
                writer.WriteString("guid", GUID);
                writer.WriteString("heartbeat", unixTimestamp.ToString());
                writer.WriteEndObject();
                writer.Flush();
                

                string json = Encoding.UTF8.GetString(ms.ToArray());

                string msg = json.ToString();
                */

                
                await Send(ws, JsonMessage(unixTimestamp.ToString()));

                // Sending (Legacy)
                //ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
                //await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);

                // Receive (Legacy)
                //ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                //WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                //Console.WriteLine("Response: " + Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));

                await Task.Delay(3000);
            }
        }
    }

    private static string JsonMessage(string msg)
    {
        using var ms = new MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        writer.WriteStartObject();
        writer.WriteString("guid", GUID);
        writer.WriteString("message", msg);
        writer.WriteEndObject();
        writer.Flush();

        string json = Encoding.UTF8.GetString(ms.ToArray());

        return json.ToString();
    }

    private static async Task ConnectWS()
    {
        using (ClientWebSocket ws = new ClientWebSocket())
        {
            Uri serverUri = new Uri("ws://127.0.0.1:8080/socket");

            await ws.ConnectAsync(serverUri, CancellationToken.None);

            while (ws.State == WebSocketState.Open)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }

                /*
                using var ms = new MemoryStream();
                using var writer = new Utf8JsonWriter(ms);
                writer.WriteStartObject();
                writer.WriteString("guid", GUID);
                writer.WriteString("message", input);
                writer.WriteEndObject();
                writer.Flush();

                string json = Encoding.UTF8.GetString(ms.ToArray());

                string msg = json.ToString();
                */

                await Send(ws, JsonMessage(input));
                await Receive(ws);

                // Sending (Legacy)
                //ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
                //await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);

                // Receiving (Legacy)
                //ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                //WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                //Console.WriteLine("Response: " + Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));
            }
        }
    }

    static async Task Send(ClientWebSocket ws, String message)
    {
        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    static async Task Receive(ClientWebSocket ws) 
    {
        ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
        WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
        Console.WriteLine("Response: " + Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));
    }

}

