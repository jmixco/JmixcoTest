using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        public static List<string> distinctDepartmentList;
        public static Dictionary<string, JObject> departmentDictionary;
        public static Dictionary<string, JObject> locationDictionary;
        static void Main(string[] args)
        {
            #region read json from file 
            string json = null;
            string path = $"{System.AppDomain.CurrentDomain.BaseDirectory }personal.json";

            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                Console.WriteLine("FILE NOT FOUND: ~/personal.json");
                Console.ReadLine();
                return;
            }

            // Open the file to read from.
            try
            {
                json = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading JSON: ~/personal.json");
                Console.WriteLine(e.Message);
                Console.ReadLine();
                return;
            }

            #endregion


            JObject o = JObject.Parse(json);

            departmentDictionary = new Dictionary<string, JObject>();
            //under staffed count
            int underStaffedCount = understaffedDepartmentCount(o);

            departmentDictionary = new Dictionary<string, JObject>();


            //disctinct departments
            distinctDepartmentList = new List<string>();
            distinctDepartments(o);

            #region print distinct Departments
            Console.WriteLine($"1) Understaffed departments count: { underStaffedCount}");
            StringBuilder sb = new StringBuilder();
            sb.Append("2) Distinct departments: ");

            foreach (var department in distinctDepartmentList)
            {
                sb.Append($"| {department} |");


            }
            Console.WriteLine(sb);
            #endregion


            // distinct locations
            locationDictionary = new Dictionary<string, JObject>();
            JObject distinctLocationsJson = distinctLocations(o as JObject);
            Console.WriteLine("3) Location Array:");
            Console.WriteLine(distinctLocationsJson);
            Console.ReadLine();
        }

        //This function counts only the understaffed departments. 
        //If an understaffed department appears multiple times, it wil be counted as 1
        //This does not count the personal related to a understaffed department 
        static int understaffedDepartmentCount(JObject obj)
        {
            int count = 0;
            if (obj == null)
            {
                return 0;
            }

            JObject department = obj["department"]?.Value<JObject>();
            if (department != null)
            {
                string departmentName = (string)department["name"];
                bool understaffed = (bool)department["understaffed"];
                JObject departmentFromDictionary = null;

                //add the department, if it has not been previously added
                if (!departmentDictionary.TryGetValue(departmentName, out departmentFromDictionary))
                {
                    departmentDictionary.Add(departmentName, department);
                    if (understaffed)
                    {
                        //department is unstaffed, count it
                        return 1;
                    }
                }
            }
            else
            {
                var personal = obj["personal"];
                //iterate over personal
                foreach (JObject person in personal.Children())
                {
                    count += understaffedDepartmentCount(person);
                }
                return count;
            }

            return count;
        }


        static void distinctDepartments(JObject obj)
        {

            if (obj == null)
            {
                return;
            }
            //check if it is a person or a person array
            JObject department = obj["department"]?.Value<JObject>();
            if (department != null)
            {
                string departmentName = (string)department["name"];
                if (!distinctDepartmentList.Contains(departmentName))
                {
                    //add the department to the list
                    distinctDepartmentList.Add(departmentName);
                }
            }
            else
            {
                var personal = obj["personal"];
                foreach (JObject person in personal.Children())
                {
                    distinctDepartments(person);
                }
            }

            return;
        }


        static JObject distinctLocations(JObject obj)
        {
            JObject objectResult = null;
            if (obj == null)
            {
                return obj as JObject;
            }
            //check if object is array


            JObject department = obj["department"]?.Value<JObject>();
            JObject location = obj["location"]?.Value<JObject>();

            if (department != null)
            {
                //if the current object contains department, the current object is a person
                objectResult = distinctLocations(department);
            }
            else if (location != null)
            {
                //if the current object contains location, the current object is a department
                objectResult = location as JObject;
            }
            else
            {
                //root
                var personal = obj["personal"];

                foreach (JObject child in personal.Children())
                {
                    //obtains the location for the current object
                    location = distinctLocations(child);
                    if (location != null)
                    {
                        int floor = (int)location["floor"];
                        int building = (int)location["building"];

                        //creates a unique key for the location
                        string key = $"{floor},{building}";
                        JObject addedLocation = null;

                        //checks if the key has been previously added
                        if (!locationDictionary.TryGetValue(key, out addedLocation))
                        {
                            locationDictionary.Add(key, location);
                        }
                    }
                }
                //Result object contining the data array
                objectResult = new JObject(new JProperty("data", locationDictionary.Values.ToArray()));
            }


            return objectResult;
        }
    }
}
