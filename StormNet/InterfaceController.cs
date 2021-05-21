using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace StormNet
{
    [Route("Get")]
    [ApiController]
    public class InterfaceController : ControllerBase
    {
        [HttpGet("/")]
        public async Task<IActionResult> Get([FromQuery]IDictionary<string, string> query)
        {
            Console.WriteLine(DateTime.Now.ToString("O"));
            Console.WriteLine(JsonConvert.SerializeObject(query, Formatting.Indented));
            return Ok();
        }
    }
}