﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ChoHanClient.ClientForms;
using SharedUtilities;
using Timer = System.Timers.Timer;

namespace ChoHanClient
{
    public class Client
    {
        public LogInForm LogInForm { get; set; }
        public PlayerForm PlayerForm { get; set; }
        private readonly IPAddress _localIpAddress;
        private TcpClient _client;
        public string Name { get; set; }

        public Client(string name, LogInForm form)
        {
            Console.WriteLine("SENPAI!");
            Name = name;
            LogInForm = form;
            IPAddress localIP = GetLocalIpAddress();

            bool IpOk = IPAddress.TryParse(localIP.ToString(), out _localIpAddress);
            if (!IpOk)
            {
                Console.WriteLine("Couldn't parse the IP address. Exiting code.");
                Environment.Exit(1);
            }
            _client = new TcpClient();

            Console.WriteLine("I want to connect with Senpai!");
<<<<<<< HEAD
            
=======
>>>>>>> ebba8d8f8792945b45d9e4a6c5eb785cadfe6562
            TryConnection();
        }
        public Client(string name, LogInForm form, IPAddress IP)
        {
            Name = name;
            LogInForm = form;
            _client = new TcpClient();
        }

        public void TryConnection()
        {
            try
            {
                Console.WriteLine("Senpai, connect with me!");
                _client.Connect(_localIpAddress, 1337);
                SharedUtil.SendMessage(_client, new
                {
                    id = "send/name",
                    data = new
                    {
                        name = Name
                    }
                });
                Thread thread = new Thread(StartLoop);
                thread.Start();
<<<<<<< HEAD
                LogInForm.Close();
                LogInForm.Dispose();
                Console.WriteLine("YAY! Senpai and I connected!");
                LogInForm.Visible = false;
                PlayerForm = new PlayerForm();
=======
                Console.WriteLine("YAY! Senpai and I connected!");
                LogInForm.Visible = false;
                PlayerForm = new PlayerForm();
                PlayerForm.Show();
>>>>>>> ebba8d8f8792945b45d9e4a6c5eb785cadfe6562
            }
            catch (Exception e)
            {
                LogInForm.setServerText("Can't connect to this server.");
                Console.WriteLine(e.StackTrace);
            }
        }

        public void StartLoop()
        {
            while (_client.Connected)
            {
                dynamic message = SharedUtil.ReadMessage(_client);
                switch ((string)message.id)
                {
                    case "give/confirmation":
                        SharedUtil.SendMessage(_client, new
                        {
                            id = "send",
                            data = new
                            {
                                confirmation = PlayerForm.ConfirmAnswer
                            }
                        });
                        break;
                    case "recieve/answer":
                        PlayerForm.Update((bool) message.data.answer, (int) message.data.score);
                        break;
                    case "give/answer":
                        SharedUtil.SendMessage(_client, new
                        {
                            id = "send",
                            data = new
                            {
                                answer = PlayerForm.Answer
                            }
                        });
                        break;
                    case "update/panel":
                        PlayerForm.UpdateMessageLabel((string)message.data.text);
                        break;
                    case "send/session":
                        List<string> sessions = new List<string>();
                        for (var i = 0; i < message.data.sessions.Count; i++)
                        {
                            sessions.Add((string)message.data.sessions[i]);
                        }
                        PlayerForm.FillSessionBox(sessions);
                        break;
                    case "disconnect":
                        _client.GetStream().Close();
                        _client.Close();
                        break;
                    default:
                        Console.WriteLine("You're not suposse to be here.");
                        break;
                }
            }
        }

        public static IPAddress GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;
            throw new Exception("Local IP Address Not Found!");
        }
    }
}
