using System.Web.Http;
using Common.Entities;

namespace WebApplication1.Controllers
{
    public class InvationController : ApiController
    {
        /*public InvationController (IDataBase DataBase)
        {
            dataBase = DataBase;
        }

        private IDataBase dataBase { get; set; }*/

        [HttpPost]
        [Route("Invation/{RoomName}/{UserFrom}")]
        public void InvateUserInRoom([FromUri] int RoomName, [FromUri] int UserFrom, [FromBody] int UserToId)
        {
            
        }
    }
}