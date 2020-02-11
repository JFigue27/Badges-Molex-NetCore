using Reusable.CRUD.Contract;
using Reusable.Rest;
using ServiceStack;
using ServiceStack.Caching;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using ServiceStack.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Reusable.CRUD.Implementations.SS
{
    public abstract class ReadOnlyLogic<Entity> : BaseLogic, ILogicReadOnly<Entity>, ILogicReadOnlyAsync<Entity> where Entity : class, IEntity, new()
    {
        protected string CACHE_GET_ALL { get { return typeof(Entity).Name + "_GetAll_"; } }
        protected string CACHE_GET_LIST { get { return typeof(Entity).Name + "_GetList_"; } }
        protected string CACHE_GET_BY_ID { get { return typeof(Entity).Name + "_GetById_"; } }
        protected string CACHE_GET_PAGED { get { return typeof(Entity).Name + "_GetPaged_"; } }
        protected string CACHE_GET_SINGLE_WHERE { get { return typeof(Entity).Name + "_GetSingleWhere_"; } }

        protected string CACHE_CONTAINER_GET_PAGED { get { return typeof(Entity).Name + "_Container_GetPaged_"; } }
        protected string CACHE_CONTAINER_GET_SINGLE_WHERE { get { return typeof(Entity).Name + "_Container_GetSingleWhere_"; } }

        #region HOOKS
        virtual protected SqlExpression<Entity> OnGetList(SqlExpression<Entity> query)
        {
            Entity track = new Entity();
            if (track is Trackable) query.Where(e => !(e as Trackable).IsDeleted);
            return query;
        }
        virtual protected SqlExpression<Entity> OnGetSingle(SqlExpression<Entity> query)
        {
            return OnGetList(query);
        }
        virtual protected List<Entity> AdapterOut(params Entity[] entities) { return entities.ToList(); }
        virtual protected List<Entity> BeforePaginate(List<Entity> entities) { return entities; }
        virtual protected bool BeforeSearch(List<Entity> entities) { return false; } // return true to avoid calling AdapterOut when getPage because they are the same.
        #endregion

        virtual public List<Entity> GetAll()
        {
            var cache = CacheDisabled ? null : Cache.Get<List<Entity>>(CACHE_GET_ALL);
            if (cache != null)
                return cache;

            var query = Db.From<Entity>();
            var entities = Db.LoadSelect(query);
            if (!CacheDisabled) Cache.Set(CACHE_GET_ALL, entities);
            return entities;
        }

        virtual public async Task<List<Entity>> GetAllAsync()
        {
            var cache = CacheDisabled ? null : Cache.Get<List<Entity>>(CACHE_GET_ALL);
            if (cache != null)
                return cache;

            var query = Db.From<Entity>();
            var entities = await Db.LoadSelectAsync(query);
            if (!CacheDisabled) Cache.Set(CACHE_GET_ALL, entities);
            return entities;
        }
        virtual public List<Entity> GetAllAdapted()
        {
            var cache = CacheDisabled ? null : Cache.Get<List<Entity>>(CACHE_GET_LIST);
            if (cache != null)
                return AdapterOut(cache.ToArray());

            var query = OnGetList(Db.From<Entity>());
            var entities = Db.LoadSelect(query);
            if (!CacheDisabled) Cache.Set(CACHE_GET_LIST, entities);
            return AdapterOut(entities.ToArray());
        }
        virtual public async Task<List<Entity>> GetAllAdaptedAsync()
        {
            var cache = CacheDisabled ? null : Cache.Get<List<Entity>>(CACHE_GET_LIST);
            if (cache != null)
                return AdapterOut(cache.ToArray());

            var query = OnGetList(Db.From<Entity>());
            var entities = await Db.LoadSelectAsync(query);
            if (!CacheDisabled) Cache.Set(CACHE_GET_LIST, entities);
            return AdapterOut(entities.ToArray());
        }

        virtual public Entity GetById(long id)
        {
            var cacheKey = CACHE_GET_BY_ID + id;
            var cache = CacheDisabled ? null : Cache.Get<Entity>(cacheKey);
            if (cache != null)
                return AdapterOut(cache)[0];

            var query = OnGetSingle(Db.From<Entity>())
                    .Where(e => e.Id == id);

            var entity = Db.LoadSelect(query).FirstOrDefault();

            if (entity != null) AdapterOut(entity);

            var response = entity;
            if (!CacheDisabled) Cache.Set(cacheKey, response);
            return response;
        }

        virtual public async Task<Entity> GetByIdAsync(long id)
        {
            var cacheKey = CACHE_GET_BY_ID + id;
            var cache = CacheDisabled ? null : Cache.Get<Entity>(cacheKey);
            if (cache != null)
                return AdapterOut(cache)[0];

            var query = OnGetSingle(Db.From<Entity>())
                    .Where(e => e.Id == id);

            var entity = (await Db.LoadSelectAsync(query)).FirstOrDefault();

            if (entity != null) AdapterOut(entity);

            var response = entity;
            if (!CacheDisabled) Cache.Set(cacheKey, response);
            return response;
        }

        virtual public CommonResponse GetPaged(int perPage = 0, int page = 1, string generalFilter = "", SqlExpression<Entity> query = null, string cacheKey = null, bool requiresKeysInJsons = false)
        {
            if (string.IsNullOrWhiteSpace(cacheKey)) cacheKey = CACHE_GET_PAGED;
            cacheKey += $"_{perPage}_{page}_{generalFilter}";
            if (requiresKeysInJsons) cacheKey += "_requiresKeysInJsons";
            var cacheContainer = CacheDisabled ? null : Cache.Get<Dictionary<string, CommonResponse>>(CACHE_CONTAINER_GET_PAGED);
            if (cacheContainer == null) cacheContainer = new Dictionary<string, CommonResponse>();

            var paramsNotFoundAsProps = new Dictionary<string, string>();

            var allParams = Request.GetRequestParams();
            foreach (var param in allParams)
            {
                if (IsValidJSValue(param.Value))
                    cacheKey += $"_{param.Key}_{param.Value}";
            }

            //bool jsonKeyRequired = Request.QueryString.GetValues(null)?
            //    .Select(q => q.ToLower()).Contains("filterjson") ?? false;

            //string filterJson = Request.QueryString["filterjson"];
            //bool jsonKeysRequired = IsValidJSValue(filterJson);
            //if(jsonKeysRequired)
            //    paramsNotFoundAsProps = JSON.parse(filterJson).ToStringDictionary();

            //cachePrefix += filterJson;

            //var cacheFromContainer = false;
            //IQueryable<Entity> allEntities = new List<Entity>().AsQueryable();

            if (query == null) query = Db.From<Entity>();
            //    allEntities = GetAll().AsQueryable();
            //else
            //cacheFromContainer = true;

            #region Apply Database Filtering
            foreach (var queryParam in Request.QueryString.AllKeys)
            {
                string queryParamValue = Request.QueryString[queryParam];
                if (IsValidParam(queryParam) && IsValidJSValue(queryParamValue))
                {
                    string sPropertyName = queryParam;

                    PropertyInfo oProp = typeof(Entity).GetProperty(sPropertyName);
                    if (oProp == null)
                    {
                        paramsNotFoundAsProps.Add(queryParam, queryParamValue);
                        continue; //Ignore non-existing properties, they could be just different query parameters.
                    }

                    //Type tProp = oProp.PropertyType;
                    ////Nullable properties have to be treated differently, since we
                    ////  use their underlying property to set the value in the object
                    //if (tProp.IsGenericType
                    //    && tProp.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    //    //Get the underlying type property instead of the nullable generic
                    //    tProp = new NullableConverter(oProp.PropertyType).UnderlyingType;

                    //ParameterExpression entityParameter = Expression.Parameter(typeof(Entity), "entityParameter");
                    //Expression childProperty = Expression.PropertyOrField(entityParameter, sPropertyName);

                    //var value = Expression.Constant(Convert.ChangeType(queryParamValue, tProp));

                    ////if (cacheFromContainer)
                    ////{
                    //if (tProp == typeof(string))
                    //    query.Where($"{query.SqlColumn(sPropertyName)} like '%{value.Value}%'");
                    //else
                    //    query.Where($"{query.SqlColumn(sPropertyName)} = {value.Value}");
                    ////}
                    ////else //Cache From Memory
                    ////{
                    ////    // let's perform the conversion only if we really need it
                    ////    var converted = value.Type != childProperty.Type
                    ////        ? Expression.Convert(value, childProperty.Type)
                    ////        : (Expression)value;

                    ////    Expression<Func<Entity, bool>> lambda;
                    ////    if (tProp == typeof(String))
                    ////    {
                    ////        MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    ////        lambda = Expression.Lambda<Func<Entity, bool>>(Expression.Call(converted, method, childProperty), entityParameter);
                    ////    }
                    ////    else
                    ////    {
                    ////        Expression comparison = Expression.Equal(childProperty, converted);
                    ////        lambda = Expression.Lambda<Func<Entity, bool>>(comparison, entityParameter);
                    ////    }
                    ////    allEntities = allEntities.Where(lambda);
                    ////}
                }
            }
            #endregion

            #region From Container Cache
            //if (cacheFromContainer)
            //{
            cacheContainer.TryGetValue(cacheKey, out CommonResponse cache);
            if (cache != null)
            {
                var cacheList = cache.Result as IEnumerable<Entity>;
                AdapterOut(cacheList.ToArray());
                return cache;
            }
            //}
            #endregion

            //var entities = allEntities.ToList();
            query = OnGetList(query);
            var entities = Db.LoadSelect(query);

            var filterResponse = new FilterResponse
            {
                total_items = entities.Count()
            };

            #region Apply General Search Filter
            var usingBeforeSearch = BeforeSearch(entities);

            var filtered = new HashSet<Entity>();
            if (!string.IsNullOrEmpty(generalFilter) || (paramsNotFoundAsProps.Count > 0))
            {
                var searchableProps = typeof(Entity).GetProperties().Where(prop => !prop.HasAttribute<IsJson>()
                                    && new[] { "String" }.Contains(prop.PropertyType.Name)).ToList();
                var jsonProps = typeof(Entity).GetPublicProperties().Where(p => p.HasAttribute<IsJson>()).ToList();

                foreach (var entity in entities)
                    if (SearchInStringProps(entity, generalFilter)
                            && SearchInJsonProps(entity, paramsNotFoundAsProps, requiresKeysInJsons, jsonProps))
                        filtered.Add(entity);
            }
            else
                filtered = new HashSet<Entity>(entities);

            filterResponse.total_filtered_items = filtered.Count();
            #endregion

            #region Pagination
            IEnumerable<Entity> afterPaginate;
            if (perPage != 0)
            {
                var totalPagesCount = (filterResponse.total_filtered_items + perPage - 1) / perPage;
                if (page > totalPagesCount)
                    page = totalPagesCount;

                afterPaginate = BeforePaginate(filtered.ToList()).Skip((page - 1) * perPage).Take(perPage);
                filterResponse.page = page;
            }
            else
                afterPaginate = BeforePaginate(filtered.ToList());
            #endregion

            #region AdapterOut Hook
            if (!usingBeforeSearch)
                AdapterOut(afterPaginate.ToArray());
            #endregion

            var response = new CommonResponse { Result = afterPaginate, AdditionalData = filterResponse };

            //if (cacheFromContainer)
            //{
            cacheContainer[cacheKey] = response;
            if (!CacheDisabled) Cache.Replace(CACHE_CONTAINER_GET_PAGED, cacheContainer);
            //}
            return response;
        }

        virtual public async Task<CommonResponse> GetPagedAsync(int perPage = 0, int page = 1, string generalFilter = "", SqlExpression<Entity> query = null, string cachePrefix = null, bool requiresKeysInJsons = false)
        {
            if (string.IsNullOrWhiteSpace(cachePrefix)) cachePrefix = CACHE_GET_PAGED;
            cachePrefix += $"_{perPage}_{page}_{generalFilter}";

            if (requiresKeysInJsons) cachePrefix += "_requiresKeysInJsons";
            var cacheContainer = CacheDisabled ? null : Cache.Get<Dictionary<string, CommonResponse>>(CACHE_CONTAINER_GET_PAGED);
            if (cacheContainer == null) cacheContainer = new Dictionary<string, CommonResponse>();

            var paramsNotFoundAsProps = new Dictionary<string, string>();

            //var cacheFromContainer = false;

            if (query == null) query = Db.From<Entity>();
            //    query = Db.From<Entity>();
            //else
            //    cacheFromContainer = true;

            #region Apply Database Filtering
            foreach (var queryParam in Request.QueryString.AllKeys)
            {
                string queryParamValue = Request.QueryString[queryParam];
                if (IsValidParam(queryParam) && IsValidJSValue(queryParamValue))
                {
                    cachePrefix += $"_{queryParam}_{queryParamValue}";

                    string sPropertyName = queryParam;

                    PropertyInfo oProp = typeof(Entity).GetProperty(sPropertyName);
                    if (oProp == null)
                    {
                        paramsNotFoundAsProps.Add(queryParam, queryParamValue);
                        continue; //Ignore non-existing properties, they could be just different query parameters.
                    }

                    Type tProp = oProp.PropertyType;
                    //Nullable properties have to be treated differently, since we
                    //  use their underlying property to set the value in the object
                    if (tProp.IsGenericType
                        && tProp.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                        //Get the underlying type property instead of the nullable generic
                        tProp = new NullableConverter(oProp.PropertyType).UnderlyingType;

                    ParameterExpression entityParameter = Expression.Parameter(typeof(Entity), "entityParameter");
                    Expression childProperty = Expression.PropertyOrField(entityParameter, sPropertyName);

                    var value = Expression.Constant(Convert.ChangeType(queryParamValue, tProp));

                    //if (cacheFromContainer)
                    //{
                    if (tProp == typeof(string))
                        query.Where($"{query.SqlColumn(sPropertyName)} like '%{value.Value}%'");
                    else
                        query.Where($"{query.SqlColumn(sPropertyName)} = {value.Value}");
                    //}
                    //else //Cache From Memory
                    //{
                    //    // let's perform the conversion only if we really need it
                    //    var converted = value.Type != childProperty.Type
                    //        ? Expression.Convert(value, childProperty.Type)
                    //        : (Expression)value;

                    //    Expression<Func<Entity, bool>> lambda;
                    //    if (tProp == typeof(String))
                    //    {
                    //        MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    //        lambda = Expression.Lambda<Func<Entity, bool>>(Expression.Call(converted, method, childProperty), entityParameter);
                    //    }
                    //    else
                    //    {
                    //        Expression comparison = Expression.Equal(childProperty, converted);
                    //        lambda = Expression.Lambda<Func<Entity, bool>>(comparison, entityParameter);
                    //    }
                    //    allEntities = allEntities.Where(lambda);
                    //}
                }
            }
            #endregion

            #region From Container Cache
            //if (cacheFromContainer)
            //{
            cacheContainer.TryGetValue(cachePrefix, out CommonResponse cache);
            if (cache != null)
            {
                var cacheList = cache.Result as IEnumerable<Entity>;
                AdapterOut(cacheList.ToArray());
                return cache;
            }
            //}
            #endregion

            query = OnGetList(query);
            var entities = await Db.LoadSelectAsync(query);
            //var entities = await GetAllAsync();

            var usingBeforeSearch = BeforeSearch(entities);

            var filterResponse = new FilterResponse
            {
                total_items = entities.Count()
            };

            #region Apply General Search Filter


            var filtered = new HashSet<Entity>();
            if (!string.IsNullOrEmpty(generalFilter) || (paramsNotFoundAsProps.Count > 0))
            {
                var searchableProps = typeof(Entity).GetProperties().Where(prop => !prop.HasAttribute<IsJson>()
                                    && new[] { "String" }.Contains(prop.PropertyType.Name)).ToList();
                var jsonProps = typeof(Entity).GetPublicProperties().Where(p => p.HasAttribute<IsJson>()).ToList();

                foreach (var entity in entities)
                    if (SearchInStringProps(entity, generalFilter)
                            && SearchInJsonProps(entity, paramsNotFoundAsProps, requiresKeysInJsons, jsonProps))
                        filtered.Add(entity);
            }
            else
                filtered = new HashSet<Entity>(entities);

            filterResponse.total_filtered_items = filtered.Count();
            #endregion

            #region Pagination
            IEnumerable<Entity> afterPaginate;
            if (perPage != 0)
            {
                var totalPagesCount = (filterResponse.total_filtered_items + perPage - 1) / perPage;
                if (page > totalPagesCount)
                    page = totalPagesCount;

                afterPaginate = BeforePaginate(filtered.ToList()).Skip((page - 1) * perPage).Take(perPage);
                filterResponse.page = page;
            }
            else
                afterPaginate = BeforePaginate(filtered.ToList());
            #endregion

            #region AdapterOut Hook
            if (!usingBeforeSearch)
                AdapterOut(afterPaginate.ToArray());
            #endregion

            var response = new CommonResponse { Result = afterPaginate, AdditionalData = filterResponse };

            //if (cacheFromContainer)
            //{
            cacheContainer[cachePrefix] = response;
            if (!CacheDisabled) Cache.Replace(CACHE_CONTAINER_GET_PAGED, cacheContainer);
            //}
            return response;
        }

        virtual public Entity GetSingleWhere(string Property, object Value, SqlExpression<Entity> query = null, string cachePrefix = null)
        {
            if (string.IsNullOrWhiteSpace(cachePrefix)) cachePrefix = CACHE_GET_SINGLE_WHERE;
            cachePrefix += $"_{Property}_{Value}";
            var cacheContainer = CacheDisabled ? null : Cache.Get<Dictionary<string, Entity>>(CACHE_CONTAINER_GET_SINGLE_WHERE);
            if (cacheContainer == null) cacheContainer = new Dictionary<string, Entity>();

            //var cacheFromContainer = false;
            //IQueryable<Entity> allEntities = new List<Entity>().AsQueryable();

            if (query == null) query = Db.From<Entity>();
            //    allEntities = GetAll().AsQueryable();
            //else
            //    cacheFromContainer = true;

            var queryParams = Request.QueryString.ToDictionary().ConvertTo<Dictionary<string, object>>();
            if (Property != null && Value != null)
            {
                queryParams.Add(Property, Value);
            }
            #region Apply Database Filtering
            foreach (var queryParam in queryParams.Keys)
            {
                var queryParamValue = queryParams[queryParam];
                if (IsValidParam(queryParam) && IsValidJSValue(queryParamValue.ToString()))
                {
                    cachePrefix += $"_{queryParam}_{queryParamValue}";

                    string sPropertyName = queryParam;

                    PropertyInfo oProp = typeof(Entity).GetProperty(sPropertyName);
                    if (oProp == null) continue; //Ignore non-existing properties, they could be just different query parameters.

                    Type tProp = oProp.PropertyType;
                    //Nullable properties have to be treated differently, since we
                    //  use their underlying property to set the value in the object
                    if (tProp.IsGenericType
                        && tProp.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                        //Get the underlying type property instead of the nullable generic
                        tProp = new NullableConverter(oProp.PropertyType).UnderlyingType;

                    ParameterExpression entityParameter = Expression.Parameter(typeof(Entity), "entityParameter");
                    Expression childProperty = Expression.PropertyOrField(entityParameter, sPropertyName);

                    var value = Expression.Constant(Convert.ChangeType(queryParamValue, tProp));

                    //if (cacheFromContainer)
                    //{
                    if (tProp == typeof(string))
                        query.Where($"{query.SqlColumn(sPropertyName)} like '%{value.Value}%'");
                    else
                        query.Where($"{query.SqlColumn(sPropertyName)} = {value.Value}");
                    //}
                    //else //Cache From Memory
                    //{
                    //    // let's perform the conversion only if we really need it
                    //    var converted = value.Type != childProperty.Type
                    //        ? Expression.Convert(value, childProperty.Type)
                    //        : (Expression)value;

                    //    Expression<Func<Entity, bool>> lambda;
                    //    if (tProp == typeof(String))
                    //    {
                    //        MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    //        lambda = Expression.Lambda<Func<Entity, bool>>(Expression.Call(converted, method, childProperty), entityParameter);
                    //    }
                    //    else
                    //    {
                    //        Expression comparison = Expression.Equal(childProperty, converted);
                    //        lambda = Expression.Lambda<Func<Entity, bool>>(comparison, entityParameter);
                    //    }
                    //    allEntities = allEntities.Where(lambda);
                    //}
                }
            }
            #endregion

            #region From Container Cache
            //if (cacheFromContainer)
            //{
            cacheContainer.TryGetValue(cachePrefix, out Entity cache);
            if (cache != null) return AdapterOut(cache)[0];
            //}
            #endregion

            query = OnGetSingle(query);
            var entity = Db.LoadSelect(query).FirstOrDefault();
            //var entity = allEntities.FirstOrDefault();
            if (entity != null) AdapterOut(entity);

            var response = entity;

            //if (cacheFromContainer)
            //{
            cacheContainer[cachePrefix] = response;
            if (!CacheDisabled) Cache.Replace(CACHE_CONTAINER_GET_SINGLE_WHERE, cacheContainer);
            //}
            return response;
        }

        virtual public async Task<Entity> GetSingleWhereAsync(string Property, object Value, SqlExpression<Entity> query = null, string cachePrefix = null)
        {
            if (string.IsNullOrWhiteSpace(cachePrefix)) cachePrefix = CACHE_GET_SINGLE_WHERE;
            cachePrefix += $"_{Property}_{Value}";
            var cacheContainer = CacheDisabled ? null : Cache.Get<Dictionary<string, Entity>>(CACHE_CONTAINER_GET_SINGLE_WHERE);
            if (cacheContainer == null) cacheContainer = new Dictionary<string, Entity>();

            //var cacheFromContainer = false;
            //IQueryable<Entity> allEntities = new List<Entity>().AsQueryable();

            if (query == null) query = Db.From<Entity>();
            //    allEntities = (await GetAllAsync()).AsQueryable();
            //else
            //    cacheFromContainer = true;

            var queryParams = Request.QueryString.ToDictionary().ConvertTo<Dictionary<string, object>>();
            queryParams.Add(Property, Value);

            #region Apply Database Filtering
            foreach (var queryParam in queryParams.Keys)
            {
                var queryParamValue = queryParams[queryParam];
                if (IsValidParam(queryParam) && IsValidJSValue(queryParamValue.ToString()))
                {
                    cachePrefix += $"_{queryParam}_{queryParamValue}";

                    string sPropertyName = queryParam;

                    PropertyInfo oProp = typeof(Entity).GetProperty(sPropertyName);
                    if (oProp == null) continue; //Ignore non-existing properties, they could be just different query parameters.

                    Type tProp = oProp.PropertyType;
                    //Nullable properties have to be treated differently, since we
                    //  use their underlying property to set the value in the object
                    if (tProp.IsGenericType
                        && tProp.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                        //Get the underlying type property instead of the nullable generic
                        tProp = new NullableConverter(oProp.PropertyType).UnderlyingType;

                    ParameterExpression entityParameter = Expression.Parameter(typeof(Entity), "entityParameter");
                    Expression childProperty = Expression.PropertyOrField(entityParameter, sPropertyName);

                    var value = Expression.Constant(Convert.ChangeType(queryParamValue, tProp));

                    //if (cacheFromContainer)
                    //{
                    if (tProp == typeof(string))
                        query.Where($"{query.SqlColumn(sPropertyName)} like '%{value.Value}%'");
                    else
                        query.Where($"{query.SqlColumn(sPropertyName)} = {value.Value}");
                    //}
                    //else //Cache From Memory
                    //{
                    //    // let's perform the conversion only if we really need it
                    //    var converted = value.Type != childProperty.Type
                    //        ? Expression.Convert(value, childProperty.Type)
                    //        : (Expression)value;

                    //    Expression<Func<Entity, bool>> lambda;
                    //    if (tProp == typeof(String))
                    //    {
                    //        MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    //        lambda = Expression.Lambda<Func<Entity, bool>>(Expression.Call(converted, method, childProperty), entityParameter);
                    //    }
                    //    else
                    //    {
                    //        Expression comparison = Expression.Equal(childProperty, converted);
                    //        lambda = Expression.Lambda<Func<Entity, bool>>(comparison, entityParameter);
                    //    }
                    //    allEntities = allEntities.Where(lambda);
                    //}
                }
            }
            #endregion

            #region From Container Cache
            //if (cacheFromContainer)
            //{
            cacheContainer.TryGetValue(cachePrefix, out Entity cache);
            if (cache != null) return AdapterOut(cache)[0];
            //}
            #endregion

            query = OnGetSingle(query);
            var entity = (await Db.LoadSelectAsync(query)).FirstOrDefault();
            //var entity = allEntities.FirstOrDefault();
            if (entity != null) AdapterOut(entity);

            var response = entity;

            //if (cacheFromContainer)
            //{
            cacheContainer[cachePrefix] = response;
            if (!CacheDisabled) Cache.Replace(CACHE_CONTAINER_GET_SINGLE_WHERE, cacheContainer);
            //}
            return response;
        }

        public bool SearchInStringProps(Entity entity, string criteria = "", List<PropertyInfo> searchableProps = null)
        {
            if (string.IsNullOrWhiteSpace(criteria)) return true;

            var splitGeneralFilter = criteria.ToLower().Split(' ').Select(e => e.Trim()).ToList();
            searchableProps = searchableProps ?? typeof(Entity).GetProperties().Where(prop => !prop.HasAttribute<IsJson>()
                                    && new[] { "String" }.Contains(prop.PropertyType.Name)).ToList();

            foreach (var keyword in splitGeneralFilter)
            {
                bool bAtLeastOnePropertyContainsIt = false;
                foreach (var prop in searchableProps)
                {
                    string value = (string)prop.GetValue(entity, null);
                    if (value != null && value.ToLower().Contains(keyword))
                    {
                        bAtLeastOnePropertyContainsIt = true;
                        break;
                    }
                }

                if (!bAtLeastOnePropertyContainsIt)
                    return false;
            }

            return true;
        }

        public bool SearchInJsonProps(Entity entity, Dictionary<string, string> properties, bool keysAreRequired = false, List<PropertyInfo> jsonProps = null)
        {
            jsonProps = jsonProps ?? typeof(Entity).GetPublicProperties().Where(p => p.HasAttribute<IsJson>()).ToList();
            if (jsonProps.Count == 0 || properties == null || properties.Count == 0) return true;

            foreach (var prop in properties)
                foreach (var jsonProp in jsonProps)
                {
                    var jsonValue = JsonObject.Parse(jsonProp.GetValue(entity) as string);
                    if (jsonValue != null && jsonValue.ContainsKey(prop.Key))
                    {
                        //As value
                        string value = jsonValue[prop.Key];
                        if (value != null)
                        {
                            if (value != null && value.Trim().ToLower().Contains(prop.Value.Trim().ToLower()))
                                return true;
                        }
                        else
                        {
                            //As array
                            var splitValue = prop.Value.Split(',');
                            var list = jsonValue.ArrayObjects(prop.Key);
                            var found = list.Any(e => splitValue.Any(s => e["Value"].Trim().ToLower().Contains(s.Trim().ToLower())));
                            if (found != false)
                                return true;
                        }

                        return false;
                    }
                    else if (keysAreRequired)
                        return false;
                }

            return true;
        }
    }
}
