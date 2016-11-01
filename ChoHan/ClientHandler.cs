﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using SharedUtilities;

namespace ChoHan
{
    public class ClientHandler
    {
        //TODO implement logging
        public Player _client { get; set; }
        private readonly Log _sessionLog;

        public ClientHandler(Player client, Log sessionLog)
        {
            _client = client;
            _sessionLog = sessionLog;
        }

        public void HandleClientThread()
        {
            while (_client.Client.Connected)
            {
                if (_client.IsSession) continue;
                dynamic message = SharedUtil.ReadMessage(_client.Client);
                switch ((string) message.id)
                {
                    case "send/message":
                        break;
                    case "session/join":
                        string text = (string) message.data.sessionname;
                        string[] splitText = text.Split(':');
                        Server.FindSession(splitText[0]).AddPlayer(_client);
                        _client.IsSession = true;
                        break;
                    case "session/leave":
                        Server.FindSession((string)message.data.sessionname).DeletePlayerFromSession(_client);
                        break;
                    case "disconnect":
                        SharedUtil.SendMessage(_client.Client, new
                        {
                            id = "disconnect"
                        });

                        Console.WriteLine($"player: {_client.Naam} has disconnected");
                        _sessionLog.AddLogEntry(_client.Naam, " Disconnedted.");
                        _client.Client.GetStream().Close();
                        _client.Client.Close();

                        //sepukku
                        Server.Handlers.Remove(this);
                        break;
                    default:
                        Console.WriteLine("You're not suposse to be here.");
                        break;
                }
            }
        }

        public void SendAllSessions()
        {
            SharedUtil.SendMessage(_client.Client, new
            {
                id = "send/session",
                data = new
                {
                    sessions = Server.Sessions.Select(s => s
                    ._sessionName).ToList()
                }
            });
        }

        public void Disconnect()
        {
            SharedUtil.SendMessage(_client.Client, new
            {
                id = "disconnect",
                data = new
                {
                }
            });
        }

        public void SendAck()
        {
            SharedUtil.SendMessage(_client.Client,new
            {
                id = "ack",
                data = new
                {
                    ack = true
                }
            });
        }

        public void SendNotAck()
        {
            SharedUtil.SendMessage(_client.Client, new
            {
                id = "ack",
                data = new
                {
                    ack = false
                }
            });
        }
    }
}