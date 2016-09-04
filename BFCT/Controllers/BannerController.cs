using BFCT.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BFCT.Controllers
{
    public class BannerController : ApiController
    {

        private MongoClient client = new MongoClient();

        private List<Banner> bannerStore = new List<Banner>
        {
            new Banner
            {
                Id = 1,
                Html = "<div>Banner 1</div>",
                Created = DateTime.Now,
                Modified = DateTime.Now
            },
            new Banner
            {
                Id = 2,
                Html = "<div>Banner 2</div>",
                Created = DateTime.Now,
                Modified = DateTime.Now
            }
        };

        // GET api/banner
        public IEnumerable<Banner> Get()
        {
            var database = client.GetDatabase("local");
            var coll = database.GetCollection<Banner>("banners");

            var res = coll.Find(new BsonDocument()).ToList();

            return res;
        }

        // GET api/banner/5
        public HttpResponseMessage Get(int id)
        {
            var database = client.GetDatabase("local");
            var coll = database.GetCollection<Banner>("banners");

            var filter = Builders<Banner>.Filter.Eq("_id", id);
            var result = coll.Find(filter).FirstOrDefault();

            if(result == null) { 
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // POST api/banner
        public HttpResponseMessage Post([FromBody]Banner banner)
        {
            var database = client.GetDatabase("local");
            var coll = database.GetCollection<Banner>("banners");

            banner.Created = DateTime.Now;
            banner.Modified = DateTime.Now; 

            try
            {
                coll.InsertOne(banner);
            }
            catch(MongoWriteException writeException)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }

            return Request.CreateResponse(HttpStatusCode.Created);
        }

        // PUT api/banner/5
        public HttpResponseMessage Put(int id, [FromBody]Banner banner)
        {
            if (banner == null)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }

            var database = client.GetDatabase("local");
            var coll = database.GetCollection<Banner>("banners");

            var filter = Builders<Banner>.Filter.Eq("_id", id);
            var result = coll.Find(filter).FirstOrDefault();

            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var update = Builders<Banner>.Update.Set("Html", banner.Html).Set("Modified", DateTime.Now);
            coll.UpdateOne(filter, update);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/banner/5
        public HttpResponseMessage Delete(int id)
        {
            var database = client.GetDatabase("local");
            var coll = database.GetCollection<Banner>("banners");

            var filter = Builders<Banner>.Filter.Eq("_id", id);

            if (coll.Find(filter).FirstOrDefault() == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            coll.DeleteOne(filter);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("api/banner/{id}/view")]
        public HttpResponseMessage View(int id)
        {
            var database = client.GetDatabase("local");
            var coll = database.GetCollection<Banner>("banners");

            var filter = Builders<Banner>.Filter.Eq("_id", id);
            var result = coll.Find(filter).FirstOrDefault();

            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var response = new HttpResponseMessage();
            response.Content = new StringContent(result.Html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}
