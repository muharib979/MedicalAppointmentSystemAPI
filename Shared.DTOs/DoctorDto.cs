using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }            
        public string FullName { get; set; }          
        public string Specialization { get; set; }   
        public string ContactNumber { get; set; }
        public int DepartmentId { get; set; }
    }

}
