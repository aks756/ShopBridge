using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopBridge.Models;
using ShopBridge.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShopBridge.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        IInventoryService _inventorySvc;
        public InventoryController(IInventoryService inventorySvc)
        {
            this._inventorySvc = inventorySvc;
        }

        [Route("add")]
        [HttpPost]
        public async Task<IActionResult> AddInventory([FromBody] InventoryDetail inventoryDetail)
        {
            inventoryDetail.AddedOn = DateTime.Now;
            inventoryDetail.AddedBy = "Product Admin";
            try
            {
                InventoryResponse inventoryResponse = new InventoryResponse();
                var errorMsg = ValidateRequest(inventoryDetail);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    inventoryResponse.Message = errorMsg;
                    return BadRequest(inventoryResponse);
                }
                var insertedId = await _inventorySvc.AddInventory(inventoryDetail);
                inventoryResponse.Id = insertedId;
                inventoryResponse.Message = "Product inserted successfully.";
                return Ok(inventoryResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("modify")]
        [HttpPost]
        public async Task<IActionResult> UpdateInventory([FromBody] InventoryDetail inventoryDetail)
        {
            try
            {
                InventoryResponse inventoryResponse = new InventoryResponse();
                if (inventoryDetail.Id.Equals(null) || inventoryDetail.Id == 0)
                {
                    inventoryResponse.Message = "Valid 'Id' field value is required to update the inventory.";
                    return BadRequest(inventoryResponse);
                }
                var inv = await _inventorySvc.UpdateInventory(inventoryDetail);
                inventoryResponse.Id = inventoryDetail.Id;
                if (inv == 0)
                    inventoryResponse.Message = $"No item has been updated for the Id {inventoryDetail.Id}. Please check the entered id value is valid or not.";
                else
                    inventoryResponse.Message = $"Inventory item has been updated for the Id {inventoryDetail.Id}";
                return Ok(inventoryResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            try
            {
                InventoryResponse inventoryResponse = new InventoryResponse();
                if (id.Equals(null) || id == 0)
                {
                    inventoryResponse.Message = "Valid 'Id' value is required to delete the inventory.";
                    return BadRequest(inventoryResponse);
                }

                var inv = await _inventorySvc.DeleteInventory(id);
                inventoryResponse.Id = id;
                if (inv == 0)
                    inventoryResponse.Message = $"No item has been deleted for the Id {id}. Please check the entered id value is valid or not.";
                else
                    inventoryResponse.Message = $"Inventory item has been deleted for the Id {id}";
                return Ok(inventoryResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("getitems")]
        [HttpGet]
        public async Task<IActionResult> GetInventoryDetails()
        {
            try
            {
                var inv = await _inventorySvc.GetAllInventory();
                return Ok(inv);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private string ValidateRequest(InventoryDetail inventoryDetail)
        {
            if (string.IsNullOrEmpty(inventoryDetail.Name))
            {
                return "'Name' is a mandatory field or value cannot be a blank.";
            }
            else if (inventoryDetail.Price.Equals(null) || inventoryDetail.Price == 0)
            {
                return "'Price' is a mandatory field or value cannot be 0.";
            }
            else if (inventoryDetail.Quantity.Equals(null) || inventoryDetail.Quantity == 0)
            {
                return "'Quantity' is a mandatory field or value cannot be 0.";
            }
            return null;
        }
    }
}
