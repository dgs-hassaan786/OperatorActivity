namespace Operational.Data.Repositories
{
    using Dapper;
    using MicroOrm.Pocos.SqlGenerator;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using IContract;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class DataRepositoryBase<TEntity> : DBConnection, IDataRepository<TEntity> where TEntity : new()
    {
        private string ConnectionString { get; set; }

        #region Constructors

        protected DataRepositoryBase()
        {
            //ConnectionString = DBConfigurationManager.Current.DBSettings.ReadConnectionString;//AppConfigurationManager.Current.DialerAppSettings.ProdDBConnectionString
            Connection = new SqlConnection(ConnectionString);
            SqlGenerator = new MicroOrm.Pocos.SqlGenerator.SqlGenerator<TEntity>();
        }

        public DataRepositoryBase(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connection string is null");            
            ConnectionString = connectionString;
            Connection = new SqlConnection(ConnectionString);
            SqlGenerator = new MicroOrm.Pocos.SqlGenerator.SqlGenerator<TEntity>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        protected DataRepositoryBase(IDbConnection connection, ISqlGenerator<TEntity> sqlGenerator)
            : base(connection)
        {
            if (sqlGenerator == null)
                throw new ArgumentNullException("sqlGenerator is null");            
            SqlGenerator = sqlGenerator;
        }


        #endregion

        #region Properties

        protected ISqlGenerator<TEntity> SqlGenerator { get; private set; }

        #endregion

        #region Repository sync base actions

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            var sql = SqlGenerator.GetSelectAll();
            var result = Connection.Query<TEntity>(sql);
            Connection.Dispose();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetWhere(object filters)
        {
            var sql = SqlGenerator.GetSelect(filters);

            var result = Connection.Query<TEntity>(sql, filters);
            Connection.Dispose();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual TEntity GetFirst(object filters)
        {
            var result = this.GetWhere(filters).FirstOrDefault();
            Connection.Dispose();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual bool Insert(TEntity instance)
        {
            bool added = false;
            var sql = SqlGenerator.GetInsert();

            if (SqlGenerator.IsIdentity)
            {
                var newId = Connection.Query<decimal>(sql, instance).Single();
                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentityProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentityProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                added = Connection.Execute(sql, instance) > 0;
            }

            Connection.Dispose();
            return added;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool Delete(object key)
        {
            var sql = SqlGenerator.GetDelete();
            var result = Connection.Execute(sql, key) > 0;
            Connection.Dispose();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual bool Update(TEntity instance)
        {
            var sql = SqlGenerator.GetUpdate();
            var result = Connection.Execute(sql, instance) > 0;
            Connection.Dispose();
            return result;
        }

        #endregion

        #region Repository async base action

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var sql = SqlGenerator.GetSelectAll();
            try
            {
                SetConnectionState();
                var data = await Connection.QueryAsync<TEntity>(sql);
                return data;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                Connection.Dispose();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetWhereAsync(object filters)
        {
            var sql = SqlGenerator.GetSelect(filters);
            SetConnectionState();
            var result = await Connection.QueryAsync<TEntity>(sql, filters);
            Connection.Dispose();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetFirstAsync(object filters)
        {
            var sql = SqlGenerator.GetSelect(filters);
            SetConnectionState();
            Task<IEnumerable<TEntity>> queryTask = Connection.QueryAsync<TEntity>(sql, filters);
            IEnumerable<TEntity> data = await queryTask;
            var result = data.FirstOrDefault();
            Connection.Dispose();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<bool> InsertAsync(TEntity instance)
        {
            bool added = false;
            var sql = SqlGenerator.GetInsert();

            if (SqlGenerator.IsIdentity)
            {
                SetConnectionState();
                Task<IEnumerable<decimal>> queryTask = Connection.QueryAsync<decimal>(sql, instance);
                IEnumerable<decimal> result = await queryTask;
                var newId = result.Single();
                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentityProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentityProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                Task<IEnumerable<int>> queryTask = Connection.QueryAsync<int>(sql, instance);
                IEnumerable<int> result = await queryTask;
                added = result.Single() > 0;
            }

            Connection.Dispose();
            return added;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(object key)
        {
            var sql = SqlGenerator.GetDelete();
            SetConnectionState();
            Task<IEnumerable<int>> queryTask = Connection.QueryAsync<int>(sql, key);
            IEnumerable<int> results = await queryTask;
            var result = results.SingleOrDefault() > 0;
            Connection.Dispose();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(TEntity instance)
        {
            var sql = SqlGenerator.GetUpdate();
            SetConnectionState();
            Task<IEnumerable<int>> queryTask = Connection.QueryAsync<int>(sql, instance);
            IEnumerable<int> results = await queryTask;
            var result = results.SingleOrDefault() > 0;
            return result;
        }

        #endregion

        #region ConnectionBased
        public void SetConnectionState()
        {
            var check = true;
            try
            {
                switch (Connection.State)
                {
                    case ConnectionState.Closed:
                        check = false;
                        break;
                    case ConnectionState.Broken:
                        Connection.Dispose();
                        check = false;
                        break;
                    default:
                        return;
                }
            }
            catch (InvalidOperationException)
            {
                //Exception of Connection Not Initialized
                //Or ConnectionString Property not found
                //Open a new connection in that case
                check = false;
            }

            if (!check)
            {
                Connection = new SqlConnection(ConnectionString);
                return;
            }

        }

        protected async Task<IEnumerable<T>> ProcessAsync<T>(string storedProcedureName, DynamicParameters p = null, CommandType? commandType = null)
        {
            try
            {
                SetConnectionState();
                var result = await Connection.QueryAsync<T>(storedProcedureName, p, commandType: commandType);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Connection.Dispose();
            }

        }

        protected IEnumerable<T> ProcessSync<T>(string storedProcedureName, DynamicParameters p = null, CommandType? commandType = null)
        {
            try
            {
                SetConnectionState();
                var result = Connection.Query<T>(storedProcedureName, p, commandType: commandType);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Connection.Dispose();
            }

        }

        #endregion


    }
}
