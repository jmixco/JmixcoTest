using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace webAPI.Models
{
    public class Person
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [Required(ErrorMessage ="The field 'first' is required. ")]
        [JsonProperty(PropertyName = "first")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The field 'last' is required. ")]
        [JsonProperty(PropertyName = "last")]
        public string LastName { get; set; }
    }
}