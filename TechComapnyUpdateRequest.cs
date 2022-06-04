using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.TechCompanies
{
    public class TechComapnyUpdateRequest : TechCompanyAddRequest, IModelIdentifier
    {
        public int Id { get; set; }
    }
}
