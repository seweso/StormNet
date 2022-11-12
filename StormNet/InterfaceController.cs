using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QRCoder;

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
        
        [HttpGet("/qrcode")]
        public async Task<IActionResult> QrCode([FromQuery]IDictionary<string, string> query)
        {
            var ipAddress = GetLocalIPv4();
            var href = query["href"]
                .Replace("[ipAddress]", ipAddress);
            
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(href, QRCodeGenerator.ECCLevel.L);
            var rawBytes = qrCodeData.GetRawData(QRCodeData.Compression.Uncompressed);
            var base64StringRaw = Convert.ToBase64String(rawBytes).
                Replace('+', '@').Replace('/', '~');
            return Ok(base64StringRaw);
        }
        
        [HttpGet("/controller")]
        public async Task<IActionResult> Controller([FromQuery]IDictionary<string, string> query)
        {
            return Ok("storm.net.data:" + query["data"]);
        }
        
        
        public string GetLocalIPv4()
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || item.NetworkInterfaceType == NetworkInterfaceType.Ethernet) && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }
    }
}