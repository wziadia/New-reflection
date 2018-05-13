using ProductDataAccess;
using refactor_me.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace refactor_me.Controllers
{
    [RoutePrefix("products")]
    public class ProductOptionsController : ApiController
    {
        public class ProductOptionList
        {
            public List<ProductOption> Items;

            public ProductOptionList()
            {
            }

            public ProductOptionList(List<ProductOption> item)
            {
                Items = new List<ProductOption>(item);
            }
        }

        ProductOptionModel productOptions = new Models.ProductOptionModel();

        [HttpGet, Route("{id}/options")]
        //7. GET /products/{id}/options - finds all options for a specified product.
        public HttpResponseMessage GetOptions(Guid id)
        {
            if (productOptions != null)
            {
                ProductOptionList items = new ProductOptionList(productOptions.GetOptions(id));
                return Request.CreateResponse(HttpStatusCode.OK, items);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No product options are found.");
            }
        }


        [HttpGet, Route("{id}/options/{optionId}")]
        //8. GET /products/{id}/options/{optionId} - finds the specified product option for the specified product.
        public HttpResponseMessage Get(Guid id, Guid optionId)
        {
            if (productOptions.GetOption(id, optionId) != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, productOptions.GetOption(id, optionId));
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Product Option with id = " + id + " not found");
            }
        }

        [HttpPost, Route("{id}/Options")]
        //9. POST /products/{id}/options - adds a new product option to the specified product.
        public HttpResponseMessage Post(Guid id, ProductOption productOption)
        {
            try
            {
                productOptions.Insert(id, productOption);
                var message = Request.CreateResponse(HttpStatusCode.Created, productOption);
                message.Headers.Location = new Uri(Request.RequestUri + productOption.Id.ToString());
                return message;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPut, Route("{id}/options/{optionId}")]
        //10. PUT /products/{id}/options/{optionId} - updates the specified product option.
        public HttpResponseMessage Put(Guid id, Guid optionId, [FromBody] ProductOption productOption)
        {
            try
            {
                if (productOptions.Update(id, optionId, productOption))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, productOptions.Update(id, optionId, productOption));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Product Option with id = " + optionId + " not found to update.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpDelete, Route("{id}/Options/{optionId}")]
        //11. DELETE /products/{id}/options/{optionId} - deletes the specified product option.
        public HttpResponseMessage Delete(Guid id, Guid optionId)
        {
            try
            {
                if (productOptions.Delete(id, optionId))
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound, "ProductOption with Id = " + optionId + " not found to delete.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
