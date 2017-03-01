using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        public static List<string> distinctDepartmentList;
        static void Main(string[] args)
        {
            #region json 
            string json = @"{
                            'personal':[
                                {
                                    'name': 'Person',
                                    'last': 'A',
                                    'job': 'Accountant',
                                    'department': {
                                        'name': 'Finance',
                                        'personal': 3,
                                        'hasBudget': true,
                                        'understaffed': false,
                                        location:{
                                            'floor': 1,
                                            'building': 1
                                        }
                                    }
                                },{
                                    'name': 'Person',
                                    'last': 'B',
                                    'job': 'Engineer',
                                    'department': {
                                        'name': 'Finance',
                                        'personal': 3,
                                        'hasBudget': true,
                                        'understaffed': false,
                                        location:{
                                            'floor': 1,
                                            'building': 1
                                        }
                                    }
                                },{
                                    'name': 'Person',
                                    'last': 'C',
                                    'job': 'Accountant',
                                    'department': {
                                        'name': 'Finance',
                                        'personal': 3,
                                        'hasBudget': true,
                                        'understaffed': false,
                                        location:{
                                            'floor': 3,
                                            'building': 4
                                        }
                                    }
                                },{
                                    'name': 'Person',
                                    'last': 'D',
                                    'job': 'Engineer',
                                    'department': {
                                        'name': 'Engineering',
                                        'personal': 1,
                                        'hasBudget': false,
                                        'understaffed': true,
                                        location:{
                                            'floor': 2,
                                            'building': 1
                                        }
                                    }
                                }
                            ]
                        }";
            #endregion


            JObject o = JObject.Parse(json);

            //under staffed count
            int underStaffedCount = understaffedDepartmentCount(o as JToken);


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
            JObject distinctLocationsJson = distinctLocations(o as JToken);
            Console.WriteLine("3) Location Array:");
            Console.WriteLine(distinctLocationsJson);
            Console.ReadLine();
        }

        
        static int understaffedDepartmentCount(JToken obj)
        {
            int count = 0;
            if (obj == null)
            {
                return 0;
            }
            bool isArray = obj.Type == JTokenType.Array;
            if (isArray)
            {
                //search for understaffed departments               
                foreach (JToken child in obj.Children())
                {
                    //count the understaffed departments
                    count += understaffedDepartmentCount(child);
                }

            }
            else
            {

                JToken department = obj["department"];
                if (department != null)
                {
                    //is department
                    bool understaffed = (bool)department["understaffed"];
                    if (understaffed)
                    {
                        count = 1;
                    }
                }
                else
                {
                    JToken personal = obj["personal"];
                    count = understaffedDepartmentCount(personal);
                }

            }


            return count;
        }

       
        static void distinctDepartments(JToken obj)
        {

            if (obj == null)
            {
                return;
            }

            bool isArray = obj.Type == JTokenType.Array;
            if (isArray)
            {
                //iterate over the personal array
                foreach (JToken child in obj.Children())
                {
                    distinctDepartments(child);
                }
            }
            else
            {
                //check if it is a person or a person array
                JToken department = obj["department"];
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
                    JToken personal = obj["personal"];
                    distinctDepartments(personal);
                }

            }


            return;
        }

        
        static JObject distinctLocations(JToken obj)
        {
            JObject objectResult = null;
            if (obj == null)
            {
                return obj as JObject;
            }
            //check if object is array
            bool isArray = obj.Type == JTokenType.Array;
            if (isArray)
            {
                JArray locationArray = new JArray();
                List<string> locationKeys = new List<string>();

                foreach (JToken child in obj.Children())
                {
                    //obtains the location for the current object
                    JObject location = distinctLocations(child);
                    if (location != null)
                    {
                        int floor = (int)location["floor"];
                        int building = (int)location["building"];

                        //creates a unique key for the location
                        string key = $"{floor},{building}";

                        //checks if the key has been previously added
                        if (!locationKeys.Contains(key))
                        {
                            locationArray.Add(location);
                            locationKeys.Add(key);
                        }

                    }

                }
                locationKeys.Clear();

                //Result object contining the data array
                objectResult = new JObject(new JProperty("data", locationArray));

            }
            else
            {

                JToken department = obj["department"];
                JToken location = obj["location"];

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
                    JToken personal = obj["personal"];
                    objectResult = distinctLocations(personal);
                }
            }


            return objectResult;
        }
    }
}
