using GenerativeAI.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ChurchSystem
{
    public class ChatHub : Hub
    {
            public void Send(string name, string message)
            {
                Clients.All.addNewMessageToPage(name, message);

                if (name != "Bear Bot")
                {
                    // Trigger Bear Bot response
                     bearBot(message);
                }
            }


            public void post(string name, string message)
            {
                Clients.All.addNewMessageToPage(name, message);
            }
        public  void bearBot(string message)
            {
                
                // Consider moving this to a config file or environment variable
                var apiKey = System.Configuration.ConfigurationManager.AppSettings["GoogleGeminiApiKey"];

                var model = new GenerativeModel(apiKey);
                // Or var model = new GeminiProModel(apiKey);

                /*if(model != null)
            {
                post("Gmodel", "notnull");
            }*/

                try
                {
                   //post("aye", model.GenerateContentAsync(message).ToString());
                    //var res = model.GenerateContentAsync(message).GetAwaiter().GetResult();
                var res = Task.Run(() => model.GenerateContentAsync(message)).Result;

                    // Ensure the response is not null or empty
                    if (string.IsNullOrWhiteSpace(res?.ToString()))
                    {
                        // Log or handle failure after a reasonable retry attempt
                        Send("Bear Bot", "Sorry, I'm having trouble responding right now.");
                    }
                    else
                    {
                        Send("Bear Bot", res.ToString());
                    }
                }
                catch (Exception ex)
                {
                    // Log the error and notify the user
                    Send("Bear Bot", "An error occurred while generating a response.");
                    // Optionally log the exception
                    Console.WriteLine(ex.Message);
                }
            }

        // Group chat method
        public void SendToGroup(string groupName, string name, string message)
        {
            Clients.Group(groupName).addNewMessageToPage(name, message);
        }

        // Join a group
        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }

        // Leave a group
        public void LeaveGroup(string groupName)
        {
            Groups.Remove(Context.ConnectionId, groupName);
        }


        //we shall overcome
        public void SendSignal(string roomId, string signal, string user)
        {
            Clients.Group(roomId).ReceiveSignal(signal, user);
        }

        public void JoinRoom(string roomId)
        {
            Groups.Add(Context.ConnectionId, roomId);
        }



    }
}