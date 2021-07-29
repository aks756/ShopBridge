using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ShopBridge.Service
{
    public abstract class ServiceBase
    {
        private readonly IConfiguration _configuration;

        protected ServiceBase(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected IDbConnection CreateConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("InventoryDemo"));
        }
    }
}
