using VaccineVerify.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VaccineVerify.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VerificationRedirectController : Controller
    {
        public static Dictionary<string, string> WalletUrls { get; set; } = new Dictionary<string, string>();

        [HttpGet]
        [Route("{challengeId}")]
        public IActionResult WalletUrl(string challengeId)
        {
            var walletUrl = WalletUrls.GetValueOrDefault(challengeId);
            // var didcommUrl = `didcomm://${ngrokUrl}/qr`;
            return Redirect(walletUrl);

        }
    }
}