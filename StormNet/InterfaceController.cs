using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace StormNet
{
    [Route("Get")]
    [ApiController]
    public class InterfaceController : ControllerBase
    {
        private readonly DataHandler _dataHandler;

        public InterfaceController(DataHandler dataHandler)
        {
            _dataHandler = dataHandler;
        }

        [HttpGet("/")]
        public async Task<IActionResult> Get([FromQuery]IDictionary<string, string> query)
        {
            Console.WriteLine(DateTime.Now.ToString("O"));
            Console.WriteLine(JsonConvert.SerializeObject(query, Formatting.Indented));
            if (query.ContainsKey("startup"))
            {
                return Ok("storm.net.connected");
            }

            try
            {
                _dataHandler.Handle(query);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Ok("storm.net.error: "+ e.Message);
            }
            
            return Ok("storm.net.saved");
        }
    }
}