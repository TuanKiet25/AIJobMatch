using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Requests
{
    public class JobSearchRequest
    {
        public List<string> candidateSkills;
        public string candidateLocation;
        public int candidateLevel;
    }
}
