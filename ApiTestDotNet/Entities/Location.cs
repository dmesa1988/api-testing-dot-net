using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTestsTraining.Entities
{
    class Location
    {
        public int Id { get; set; }
        public String Name { get; set;}
        public String Type { get; set; }
        public String Dimension { get; set; }
        public List<String> Residents { get; set; }
        public String Url { get; set; }
        public String Created { get; set; }
    }
}
