using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using webAPI.Models;

namespace webAPI.Controllers
{
    [RoutePrefix("api/persons")]
    public class PersonController : ApiController
    {
        private PersonDAO db = PersonDAO.Instance;

        // GET api/persons
        [HttpGet]
        [Route("")]// [Route("~/api/persons")] 
        [ResponseType(typeof(List<Person>))]
        public IHttpActionResult Get()
        {
            List<Person> people = db.getPersonList();
            return Ok(people);
        }


        // GET api/persons/
        [HttpGet]
        [Route("{id:int}")]
        [ResponseType(typeof(Person))]
        public IHttpActionResult Get(int id)
        {
            List<Person> people = db.getPersonList();
            Person selectedPerson = people.Where(x => x.Id == id).FirstOrDefault();
            if (selectedPerson == null)
            {
                return NotFound();
            }
            return Ok(selectedPerson);
        }

        [HttpPost]
        [ResponseType(typeof(Person))]
        [Route("")]
        public HttpResponseMessage Post([FromBody]Person newPerson)
        {
            HttpError err = null;
            try
            {
                Person createdPerson = db.insertPerson(newPerson);

                string location = $"{Request.RequestUri}/{createdPerson.Id.ToString()}";
                var response = Request.CreateResponse(HttpStatusCode.Created, createdPerson);
                response.Headers.Location = new Uri(location);

                return response;

            }
            catch (Exception e)
            {
                err = new HttpError(e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, err);
            }

        }

        [HttpPut]
        [Route("{id:int}")]
        public HttpResponseMessage Put(int id, [FromBody]Person newPerson)
        {
            HttpError err = null;
            
            try
            {
                Person updatedPerson = db.updatePerson(id, newPerson);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                err = new HttpError(e.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, err);
            }


        }

        [HttpDelete]
        [Route("{id:int}")]
        public HttpResponseMessage Delete(int id)
        {
            HttpError err = null;
            if (id > 0)
            {
                try
                {
                    Person deletedPerson = db.deletePerson(id);
                    return Request.CreateResponse(HttpStatusCode.NoContent);

                }
                catch (Exception e)
                {
                    err = new HttpError(e.Message);

                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                }
                //return Content(System.Net.HttpStatusCode.NoContent, "");
            }
            err = new HttpError("Person id should be > 0");
            return Request.CreateResponse(HttpStatusCode.BadRequest, err);

        }


    }
}
