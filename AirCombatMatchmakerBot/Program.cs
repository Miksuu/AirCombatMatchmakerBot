using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Net.WebSockets;
using System.IO.Pipes;

public class Program
{
    public static Task Main(string[] args) => new BotRuntime().BotRuntimeTask();
}