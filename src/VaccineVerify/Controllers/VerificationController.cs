using VaccineVerify.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace VaccineVerify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerificationController : Controller
    {
        private readonly VaccineVerifyDbService _VaccineVerifyDbService;

        private readonly IHubContext<MattrVerifiedSuccessHub> _hubContext;

        public VerificationController(VaccineVerifyDbService VaccineVerifyDbService,
            IHubContext<MattrVerifiedSuccessHub> hubContext)
        {
            _hubContext = hubContext;
            _VaccineVerifyDbService = VaccineVerifyDbService;
        }

        /// <summary>
        /// {
        ///  "presentationType": "QueryByExample",
        ///  "challengeId": "GW8FGpP6jhFrl37yQZIM6w",
        ///  "verified": true,
        ///  "holder": "did:key:z6MkgmEkNM32vyFeMXcQA7AfQDznu47qHCZpy2AYH2Dtdu1d"
        /// }
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("[action]")]
        //public async Task<IActionResult> VerificationDataCallback([FromBody] VerifiedVaccinationData body)
        //{
        //    string connectionId;
        //    var found = MattrVerifiedSuccessHub.Challenges
        //        .TryGetValue(body.ChallengeId, out connectionId);

        //    // test Signalr
        //    //await _hubContext.Clients.Client(connectionId).SendAsync("MattrCallbackSuccess", $"{body.ChallengeId}");
        //    //return Ok();

        //    var exists = await _VaccineVerifyDbService.ChallengeExists(body.ChallengeId);

        //    if (exists)
        //    {
        //        await _VaccineVerifyDbService.PersistVerification(body);

        //        if (found)
        //        {
        //            //$"/VerifiedUser?challengeid={body.ChallengeId}"
        //            await _hubContext.Clients
        //                .Client(connectionId)
        //                .SendAsync("MattrCallbackSuccess", $"{body.ChallengeId}");
        //        }

        //        return Ok();
        //    }

        //    return BadRequest("unknown verify request");
        //}

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> VerificationDataCallback([FromBody] object body)
        {
            var test = body.ToString();
            return Ok();

        }
    }
}