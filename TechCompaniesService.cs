using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain.TechCompanies;
using Sabio.Models.Requests.TechCompanies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class TechCompaniesService : ITechCompaniesService
    {
        IDataProvider _data = null;

        public TechCompaniesService(IDataProvider dataProvider)
        {
            _data = dataProvider;
        }

        public void Delete(int id)
        {
            string procName = "[dbo].[TechCompanies_Delete]";

            _data.ExecuteNonQuery(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Id", id);
            });
        }
        public int Add(TechCompanyAddRequest techModel)
        {
            int id = 0;
            string procName = "[dbo].[TechCompanies_Insert]";
            DataTable image = MapImagesToTable(techModel.Images);
            DataTable url = MapUrlsToTable(techModel.Urls);
            DataTable tag = MapTagsToTable(techModel.Tags);
            DataTable friend = MapFriendsToTable(techModel.Friends);

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(techModel, col, image, url, tag, friend);

                SqlParameter idOut = new SqlParameter("@TechCompanyId", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;

                col.Add(idOut);
            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@TechCompanyId"].Value;
                int.TryParse(oId.ToString(), out id);
            });
            return id;
        }

        public void Update(TechComapnyUpdateRequest techModel)
        {
            
            string procName = "[dbo].[TechCompanies_Update]";
            DataTable image = MapImagesToTable(techModel.Images);
            DataTable url = MapUrlsToTable(techModel.Urls);
            DataTable tag = MapTagsToTable(techModel.Tags);
            DataTable friend = MapFriendsToTable(techModel.Friends);

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(techModel, col, image, url, tag, friend);
                col.AddWithValue("@TechCompanyId", techModel.Id);

            }, returnParameters: null);
        }

        private DataTable MapImagesToTable(List<ImageAddRequest> imagesToMap)
        {
            DataTable image = new DataTable();
            image.Columns.Add("TypeId", typeof(int));
            image.Columns.Add("Url", typeof(String));

            if (imagesToMap != null)
            {
                foreach (ImageAddRequest singleImage in imagesToMap)
                {
                    DataRow dr = image.NewRow();
                    int index = 0;
                    dr.SetField(index++, singleImage.TypeId);
                    dr.SetField(index++, singleImage.Url);
                    image.Rows.Add(dr);
                }
            }
            return image;
        }

        private DataTable MapUrlsToTable(List<string> urlsToMap)
        {
            DataTable url = new DataTable();

            url.Columns.Add("Url", typeof(string));

            if (urlsToMap != null)
            {

                foreach (string singleUrl in urlsToMap)
                {
                    DataRow dr = url.NewRow();
                    int index = 0;
                    dr.SetField(index++, singleUrl);
                    url.Rows.Add(dr);

                }
            }
            return url;
        }
        private DataTable MapTagsToTable(List<string> tagsToMap)
        {
            DataTable tag = new DataTable();

            tag.Columns.Add("Tags", typeof(String));

            if (tagsToMap != null)
            {

                foreach (string singleTag in tagsToMap)
                {
                    DataRow dr = tag.NewRow();
                    int index = 0;
                    dr.SetField(index++, singleTag);
                    tag.Rows.Add(dr);

                }
            }
            return tag;
        }
        private DataTable MapFriendsToTable(List<int> friendsToMap)
        {
            DataTable friend = new DataTable();

            friend.Columns.Add("Id", typeof(int));

            if (friendsToMap != null)
            {

                foreach (int singleFriend in friendsToMap)
                {
                    DataRow dr = friend.NewRow();
                    int index = 0;
                    dr.SetField(index++, singleFriend);
                    friend.Rows.Add(dr);

                }
            }
            return friend;
        }

        private static void AddCommonParams(TechCompanyAddRequest techModel, SqlParameterCollection col, DataTable image, DataTable url, DataTable tag, DataTable friend)
        {


            col.AddWithValue("@Name", techModel.Name);
            col.AddWithValue("@Profile", techModel.Profile);
            col.AddWithValue("@Summary", techModel.Summary);
            col.AddWithValue("@Headline", techModel.Headline);
            col.AddWithValue("@ContactInformation", techModel.ContactInformation);
            col.AddWithValue("@Slug", techModel.Slug);
            col.AddWithValue("@StatusId", techModel.StatusId);
            col.AddWithValue("@newImages", image);
            col.AddWithValue("@newURls", url);
            col.AddWithValue("@newTags", tag);
            col.AddWithValue("@newFriends", friend);

        }

        public TechCompanies Get(int id)
        {
            string procName = "[dbo].[TechCompanies_SelectById]";

            TechCompanies tech = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                tech = MapSingleTechCompany(reader, ref startingIndex);
            });
            return tech;
        }

        public List<TechCompanies> GetAll()
        {
            List<TechCompanies> list = null;
            string procName = "[dbo].[TechCompanies_SelectAll]";

            _data.ExecuteCmd(procName, inputParamMapper: null, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                TechCompanies tech = MapSingleTechCompany(reader, ref startingIndex);

                if (list == null)
                {
                    list = new List<TechCompanies>();
                }
                list.Add(tech);
            });
            return list;
        }

        public Paged<TechCompanies> Pagination(int PageIndex, int PageSize)
        {
            Paged<TechCompanies> pagedResult = null;
            List<TechCompanies> listResult = null;
            int totalCount = 0;

            string procName = "[dbo].[TechCompanies_Pagination]";
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@PageIndex", PageIndex);
                parameterCollection.AddWithValue("@PageSize", PageSize);
            }, delegate (IDataReader reader, short set)
            {

                int startingIndex = 0;
                TechCompanies techComp = MapSingleTechCompany(reader, ref startingIndex);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }

                if (listResult == null)
                {
                    listResult = new List<TechCompanies>();
                }
                listResult.Add(techComp);
            });
            if (listResult != null)
            {
                pagedResult = new Paged<TechCompanies>(listResult, PageIndex, PageSize, totalCount);
            };
            return pagedResult;
        }

        public Paged<TechCompanies> Search(int PageIndex, int PageSize, string Query)
        {
            Paged<TechCompanies> pagedResult = null;
            List<TechCompanies> listResult = null;
            int totalCount = 0;

            string procName = "[dbo].[TechCompanies_SearchPagination]";
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@PageIndex", PageIndex);
                parameterCollection.AddWithValue("@PageSize", PageSize);
                parameterCollection.AddWithValue("@Query", Query);

            }, delegate (IDataReader reader, short set)
            {

                int startingIndex = 0;
                TechCompanies tech = MapSingleTechCompany(reader, ref startingIndex);


                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                };

                if (listResult == null)
                {
                    listResult = new List<TechCompanies>();
                }
                listResult.Add(tech);
            });
            if (listResult != null)
            {
                pagedResult = new Paged<TechCompanies>(listResult, PageIndex, PageSize, totalCount);
            };
            return pagedResult;
        }

        private static TechCompanies MapSingleTechCompany(IDataReader reader, ref int startingIndex)
        {
            TechCompanies tech = new TechCompanies();

            tech.Id = reader.GetInt32(startingIndex++);
            tech.Name = reader.GetString(startingIndex++);
            tech.Profile = reader.GetString(startingIndex++);
            tech.Summary = reader.GetString(startingIndex++);
            tech.Headline = reader.GetString(startingIndex++);
            tech.ContactInformation = reader.GetString(startingIndex++);
            tech.Slug = reader.GetString(startingIndex++);
            tech.StatusId = reader.GetInt32(startingIndex++);
            tech.DateCreated = reader.GetDateTime(startingIndex++);
            tech.DateModified = reader.GetDateTime(startingIndex++);
            tech.Images = reader.DeserializeObject<List<Image>>(startingIndex++);
            tech.Urls = reader.DeserializeObject<List<Urls>>(startingIndex++);
            tech.Tags = reader.DeserializeObject<List<Tag>>(startingIndex++);
            tech.Friends = reader.DeserializeObject<List<Friend>>(startingIndex++);

            return tech;
        }
    }
}
