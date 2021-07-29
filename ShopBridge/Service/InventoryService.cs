using Microsoft.Extensions.Configuration;
using Dapper;
using ShopBridge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace ShopBridge.Service
{
    public interface IInventoryService
    {
        Task<int> AddInventory(InventoryDetail inventoryDetail);
        Task<List<InventoryDetail>> GetAllInventory();
        Task<int> DeleteInventory(int id);
        Task<int> UpdateInventory(InventoryDetail inventoryDetail);
    }
    public class InventoryService : ServiceBase, IInventoryService
    {
        public InventoryService(IConfiguration configuration)
           : base(configuration)
        { }

        public async Task<int> AddInventory(InventoryDetail inventoryDetail)
        {
            var query = "INSERT INTO [dbo].[InventoryDetail] (Name, Description, Price, Quantity, AddedOn, AddedBy, ModifiedOn, ModifiedBy) Output Inserted.Id VALUES (@Name, @Description, @Price , @Quantity, @AddedOn, @AddedBy, @ModifiedOn, @ModifiedBy  )";

            using (var db = CreateConnection())
            {
                return await db.ExecuteScalarAsync<int>(query, inventoryDetail);
            }
        }

        public async Task<int> DeleteInventory(int id)
        {
            var query = "DELETE FROM [dbo].[InventoryDetail] WHERE Id = @Id";
            using (var db = CreateConnection())
            {
                return await db.ExecuteAsync(query, new { Id = @id });
            }
        }

        public async Task<List<InventoryDetail>> GetAllInventory()
        {
            var query = "SELECT * FROM [dbo].[InventoryDetail]";
            using (var db = CreateConnection())
            {
                return (await db.QueryAsync<InventoryDetail>(query)).ToList();
            }
        }

        public async Task<int> UpdateInventory(InventoryDetail inventoryDetail)
        {
            using (var db = CreateConnection())
            {
                return await db.ExecuteAsync("sp_UpdateInventory", new { Id = inventoryDetail.Id, name = inventoryDetail.Name, description = inventoryDetail.Description, price = inventoryDetail.Price, quantity = inventoryDetail.Quantity }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
