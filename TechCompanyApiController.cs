using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain.TechCompanies;
using Sabio.Models.Requests.TechCompanies;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/techcompanies")]
    [ApiController]
    public class TechCompanyApiController : BaseApiController
    {
        private ITechCompaniesService _service = null;
        private IAuthenticationService<int> _authService = null;

        public TechCompanyApiController(ITechCompaniesService service, ILogger<TechCompanyApiController> logger, IAuthenticationService<int> authService) : base(logger)
        {
            _service = service; 
            _authService = authService; 
        }
        
        [HttpGet]
        public ActionResult<ItemsResponse<TechCompanies>> GetAll()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<TechCompanies> list = _service.GetAll();

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemsResponse<TechCompanies> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);

        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<TechCompanies>> Get(int id)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                TechCompanies comp = _service.Get(id);

                if (comp == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found");

                }
                else
                {
                    response = new ItemResponse<TechCompanies> { Item = comp };
                }
            }
            catch (SqlException sqlEx)
            {
                iCode = 500;
                response = new ErrorResponse(sqlEx.Message);
                base.Logger.LogError(sqlEx.ToString());
            }
            catch (ArgumentException argEx)
            {
                iCode = 500;
                response = new ErrorResponse(argEx.Message);
                base.Logger.LogError(argEx.ToString());
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(iCode, response);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                _service.Delete(id);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.Message.ToString());
            }

            return StatusCode(code, response);
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(TechCompanyAddRequest model)
        {
            ObjectResult result = null;

            try
            {

                int id = _service.Add(model);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };

                result = Created201(response);

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }

            return result;
        }

        [HttpPut("{id:Int}")]
        public ActionResult<ItemResponse<int>> Update(TechComapnyUpdateRequest model)
        {

            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.Update(model);
                response = new SuccessResponse();

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<TechCompanies>>> Pagination(int PageIndex, int PageSize)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<TechCompanies> paged = _service.Pagination(PageIndex, PageSize);
                if (paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("Records Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<TechCompanies>> { Item = paged };

                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message.ToString());
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }
        
        [HttpGet("search")]
        public ActionResult<ItemResponse<Paged<TechCompanies>>> Search(int PageIndex, int PageSize, string Query)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<TechCompanies> paged = _service.Search(PageIndex, PageSize, Query);
                if (paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("Records Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<TechCompanies>> { Item = paged };

                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message.ToString());
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        
        }

    }
}
