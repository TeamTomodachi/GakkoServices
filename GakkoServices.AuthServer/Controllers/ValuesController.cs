using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RawRabbit;

namespace GakkoServices.AuthServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private class TestMessage {
            public string name;
        }
        private IBusClient _bus;

        public ValuesController(IBusClient bus)
        {
            _bus = bus;
            Listen();
        }

        private void Listen()
        {
            _bus.SubscribeAsync<TestMessage>(async (message, context) =>
            {
                Console.WriteLine($"I GOT IT {message.name}");
            });
        }

        // GET api/values
        [HttpGet]
        async public Task<IEnumerable<string>> Get()
        {
            // bus.SubscribeAsync<TestMessage>(async (msg, context) =>
            // {
            //     Console.WriteLine($"Received {msg.name}");
            // });
            await _bus.PublishAsync(new TestMessage { name = "hullo" });
            return new string[] { "Auth Server", "api1" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
