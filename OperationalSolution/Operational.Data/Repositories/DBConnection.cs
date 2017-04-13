namespace Operational.Data.Repositories
{
    using System;
    using System.Data;

    public class DBConnection : IDisposable
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        private IDbConnection _connection;

        /// <summary>
        /// 
        /// </summary>
        protected IDbConnection Connection
        {
            get
            {
                if (_connection.State != ConnectionState.Open && _connection.State != ConnectionState.Connecting)
                    _connection.Open();

                return _connection;
            }

            set
            {
                _connection = value;
            }
        }


        #endregion

        public DBConnection()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public DBConnection(IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection is null");            
            _connection = connection;
        }

        /// <summary>
        /// Close the connection if this is open
        /// </summary>
        public void Dispose()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
                _connection.Close();
        }
    }
}
