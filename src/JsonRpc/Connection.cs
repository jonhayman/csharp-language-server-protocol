﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JsonRpc.Server;
using JsonRpc.Server.Messages;
using Newtonsoft.Json.Linq;

namespace JsonRpc
{
    public class Connection : IDisposable
    {
        private readonly TextReader _input;

        private readonly TextWriter _output;

        private readonly Reciever _reciever;
        private readonly Responder _responder;
        private InputHandler _inputHandler;
        private readonly IIncomingRequestRouter _mediator;

        public Connection(TextReader input, TextWriter output)
        {
            _input = input;
            _output = output;
            _reciever = new Reciever();
            _responder = new Responder();
            _mediator = new IncomingRequestRouter(new HandlerResolver(AppDomain.CurrentDomain.GetAssemblies()), null);
        }

        private async void HandleRequest(string request)
        {
            JToken payload;
            try
            {
                payload = JToken.Parse(request);
            }
            catch
            {
                _responder.Respond(new ParseError());
                return;
            }

            if (!_reciever.IsValid(payload))
            {
                _responder.Respond(new InvalidRequest());
                return;
            }

            var (requests, hasResponse) = _reciever.GetRequests(payload);
            if (hasResponse)
            {
                // TODO: Find request to respond to
                // Deserialize and respond to task.
                throw new NotImplementedException();
            }
            else
            {
                await RespondTo(requests);
            }
        }

        private async Task RespondTo(IEnumerable<Renor> items)
        {
            var response = new List<Task<ErrorResponse>>();
            foreach (var item in items)
            {
                if (item.IsRequest)
                {
                    response.Add(_mediator.RouteRequest(item.Request));
                }
                else if (item.IsNotification)
                {
                    _mediator.RouteNotification(item.Notification);
                }
                else
                {
                    response.Add(Task.FromResult<ErrorResponse>(item.Error));
                }
            }

            // All notifications
            if (response.Count == 0)
            {
                return;
            }

            var result = await Task.WhenAll(response.ToArray());
            if (result.Length == 1)
            {

            }
            else
            {

            }
        }

        public void Open()
        {
            _inputHandler = new InputHandler(_input, HandleRequest);
        }

        public void Dispose()
        {
            _inputHandler?.Dispose();
        }
    }
}