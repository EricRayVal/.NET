using Sabio.Models;
using Sabio.Models.Domain.TechCompanies;
using Sabio.Models.Requests.TechCompanies;
using System.Collections.Generic;

namespace Sabio.Services
{
    public interface ITechCompaniesService
    {
        int Add(TechCompanyAddRequest techModel);
        void Delete(int id);
        TechCompanies Get(int id);
        List<TechCompanies> GetAll();
        Paged<TechCompanies> Pagination(int PageIndex, int PageSize);
        Paged<TechCompanies> Search(int PageIndex, int PageSize, string Query);
        void Update(TechComapnyUpdateRequest techModel);
    }
}