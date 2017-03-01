using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using webAPI.Models;

namespace webAPI.Controllers
{
    public class PersonDAO
    {
        private static PersonDAO instance;
        private List<Person> personStorage;
        private PersonDAO() { }

        public static PersonDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PersonDAO();
                }
                return instance;
            }
        }
        private void initList()
        {
            personStorage = new List<Person>();
            personStorage.Add(new Person { Id = 1, FirstName = "Mario", LastName = "Perez" });
            personStorage.Add(new Person { Id = 2, FirstName = "Vladimir", LastName = "black" });
            personStorage.Add(new Person { Id = 3, FirstName = "Mike", LastName = "Hernandez" });
            personStorage.Add(new Person { Id = 4, FirstName = "Juan", LastName = "Lopez" });
            personStorage.Add(new Person { Id = 5, FirstName = "Ron", LastName = "Jack" });
        }
        private bool validate(object entity, out string message)
        {
            bool success = true;
            message = null;
            var context = new ValidationContext(entity, null, null);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(entity, context, results, true))
            {
                success = false;
                StringBuilder sb = new StringBuilder("");
                //Validation Failed
                foreach (var failedValidation in results)
                {
                    sb.Append(failedValidation.ErrorMessage);
                }
                message = sb.ToString();

            }

            return success;
        }

        #region CRUD
        public Person deletePerson(int id)
        {
            List<Person> people = getPersonList();
            Person deletedPerson = people.Where(x => x.Id == id).FirstOrDefault();
            if (deletedPerson == null)
            {
                throw new Exception("Person could not be found");
            }
            else
            {
                people.Remove(deletedPerson);
                return deletedPerson;
            }

        }

        public List<Person> getPersonList()
        {

            if (personStorage == null)
            {
                //initialize the list
                initList();

            }



            return personStorage;
        }


        public Person insertPerson(Person newPerson)
        {
            if (newPerson == null)
            {
                throw new Exception("Person cannot be null");
            }
            string message = null;
            if (!validate(newPerson, out message))
            {
                throw new Exception(message);
            }
            List<Person> people = getPersonList();
            if (people.Count > 0)
            {
                int lastId = people.Max(x => x.Id);
                newPerson.Id = lastId + 1;
            }
            else
            {
                newPerson.Id = 1;
            }

            people.Add(newPerson);
            return newPerson;
        }
        public Person updatePerson(int id, Person newPerson)
        {
            if (newPerson == null)
            {
                throw new Exception("Person cannot be null");
            }
            string message = null;
            if (!validate(newPerson, out message))
            {
                throw new Exception(message);
            }


            List<Person> people = getPersonList();
            Person prevPerson = people.Where(x => x.Id == id).FirstOrDefault();
            if (prevPerson == null)
            {
                throw new Exception("Person could not be found");
            }

            prevPerson.FirstName = newPerson.FirstName;
            prevPerson.LastName = newPerson.LastName;

            return prevPerson;
        }
        #endregion
    }
}