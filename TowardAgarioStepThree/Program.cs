using Communications;
using AgarioModels;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;
using TowardAgarioStepThree;

try
{
    Networking channel = new(NullLogger.Instance, onConnect, onDisconnect, onMessage, '\n');
    channel.Connect("localhost", 11000);
    Console.WriteLine("Connected to localhost!");
    channel.AwaitMessagesAsync(infinite: true);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return;
}

Console.ReadLine();

void onConnect(Networking channel)
{

}

void onDisconnect(Networking channel)
{
    channel.Disconnect();
}

void onMessage(Networking channel, string message)
{
    if (message.StartsWith(Protocols.CMD_Food))
    {
        //Console.WriteLine($"{message}");

        Food[] food = JsonSerializer.Deserialize<Food[]>(message.Substring(Protocols.CMD_Food.Length)) ?? throw new Exception("bad json");
        
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine($"{i}: color - {food[i].ARGBcolor}, mass - {food[i].Mass}, X - {food[i].X}, Y - {food[i].Y}");
        }
    }

    //string[] messages = JsonSerializer.Deserialize<string[]>
    //    (Protocols.CMD_Food,
    //     Protocols.CMD_Dead_Players,
    //     Protocols.CMD_Eaten_Food,
    //     Protocols.CMD_HeartBeat,
    //     Protocols.CMD_Update_Players,
    //     Protocols.CMD_Start_Game,
    //     Protocols.CMD_Start_Recognizer,
    //     Protocols.CMD_Move,
    //     Protocols.CMD_Move_Recognizer,
    //     Protocols.CMD_Split,
    //     Protocols.CMD_Split_Recognizer) ?? Array.Empty<string>();

    //Food[] Foods = JsonSerializer.Deserialize<Food[]>(
    //     Protocols.CMD_Food,
    //     Protocols.CMD_Dead_Players,
    //     Protocols.CMD_Eaten_Food,
    //     Protocols.CMD_HeartBeat,
    //     Protocols.CMD_Update_Players,
    //     Protocols.CMD_Start_Game,
    //     Protocols.CMD_Start_Recognizer,
    //     Protocols.CMD_Move,
    //     Protocols.CMD_Move_Recognizer,
    //     Protocols.CMD_Split,
    //     Protocols.CMD_Split_Recognizer) ?? Array.Empty<Food>();
    //Console.WriteLine($"The message is: {message}");
}
