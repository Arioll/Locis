using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using SongMangment.Application;
using WebApplication1.Extensions;

namespace WebApplication1.Controllers
{
    //[Authorization(AuthStatus.Authorized)]
    public class SongController : ApiController
    {
        public SongController(ISongManager songManager)
        {
            _songManager = songManager;
        }

        private readonly ISongManager _songManager;

        [HttpGet]
        [Route("songs/{query}")]
        public HttpResponseMessage SearchSongsByQuery([FromUri]string query)
        {
            return string.IsNullOrWhiteSpace(query) ? 
                Request.CreateResponse(HttpStatusCode.BadRequest, "Request in null or empty") : 
                Request.CreateResponse(HttpStatusCode.OK, _songManager.GetSongsByQuery(query).SongsToSongProfiles());
        }

        [HttpGet]
        [Route("songs/stream/{songId}")]
        public HttpResponseMessage GetSongsStream([FromUri]uint songId)
        {
            if (songId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, songId + " id is invalid");

            if (_songManager.GetSongById(songId) == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Song with specified id is not found");
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(_songManager.GetSongStream(songId));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
            return response;
        }

        [HttpPost]
        [Route("songs")]
        public async Task<HttpResponseMessage> UploadFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid file format");
            }
            var data = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(data);
            if (data.Contents.Count == 0) return Request.CreateResponse(HttpStatusCode.OK);

            var wrong = _songManager.SaveSongFiles(data.Contents).Result;
            return Request.CreateResponse(HttpStatusCode.OK, wrong);
        }
    }
}