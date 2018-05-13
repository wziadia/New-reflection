using System;
using System.Net;
using System.Web.Http;
//using refactor_me.Models;
using ProductsDataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Net.Http;

namespace refactor_me.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        public class ProductList
        {
            public object Items;

            public ProductList()
            {
            }

            public ProductList(List<Product> item)
            {
                Items = new List<Product>(item);
            }

            public ProductList(List<ProductOption> item)
            {
                Items = new List<ProductOption>(item);
            }
        }

        [HttpGet, Route]
        //1. GET /products - gets all products.
        public HttpResponseMessage Get()
        {
            using (DatabaseEntities entity = new DatabaseEntities())
            {
                var products = entity.Products.ToList();
                if(products != null)
                {
                    ProductList items = new ProductList(products);
                    return Request.CreateResponse(HttpStatusCode.OK, items);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No products are found.");
                }
            }
        }

        [HttpGet, Route]
        //2. GET /products?name={name} - finds all products matching the specified name.
        public HttpResponseMessage Get(string name)
        {
            using (DatabaseEntities entity = new DatabaseEntities())
            {
                var product = entity.Products.FirstOrDefault(p => p.Name != null && p.Name == name);
                if (product != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,product);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Product with name = " + name + " not found");
                }
            }
        }

        [HttpGet, Route("{id}")]
        //3. GET /products/{id} - gets the project that matches the specified ID - ID is a GUID.
        public HttpResponseMessage Get(Guid id)
        {
            using (DatabaseEntities entity = new DatabaseEntities())
            {
                var product = entity.Products.FirstOrDefault(p => p.Id == id);
                if (product != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, product);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Product with id = " + id + " not found");
                }
            }
        }

        [HttpPost, Route]
        //4. POST /products - creates a new product.
        public HttpResponseMessage Post(Product product)
        {
            try
            {
                using (DatabaseEntities entity = new DatabaseEntities())
                {
                    entity.Products.Add(product);
                    entity.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, product);
                    message.Headers.Location = new Uri(Request.RequestUri + product.Id.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPut, Route("{id}")]
        //5. PUT /products/{id} - updates a product.
        public HttpResponseMessage Put(Guid id, [FromBody] Product product)
        {
            try
            {
                using (DatabaseEntities entity = new DatabaseEntities())
                {
                    var productToUpdate = entity.Products.FirstOrDefault(p => p.Id == id);
                    if(productToUpdate != null)
                    {
                        productToUpdate.Name = product.Name;
                        productToUpdate.Description = product.Description;
                        productToUpdate.Price = product.Price;
                        productToUpdate.DeliveryPrice = product.DeliveryPrice;
                        productToUpdate.ProductOptions = product.ProductOptions;

                        entity.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, productToUpdate);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Product with id = " + id + " not found to update.");
                    }                    
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpDelete, Route("{id}")]
        //6. DELETE /products/{id} - deletes a product and its options.
        public HttpResponseMessage Delete(Guid id)
        {
            try
            {
                using (DatabaseEntities entity = new DatabaseEntities())
                {
                    var product = entity.Products.FirstOrDefault(p => p.Id == id);
                    if (product != null)
                    {
                        var productOption = entity.ProductOptions.Where(p => p.ProductId == id).ToList();
                        if (productOption != null)
                        {
                            foreach (ProductOption prOp in productOption)
                                entity.ProductOptions.Remove(prOp);
                            entity.SaveChanges();
                        }
                        entity.Products.Remove(product);
                        entity.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Product with Id = " + id + " not found to delete.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet, Route("{id}/options")]
        //7. GET /products/{id}/options - finds all options for a specified product.
        public HttpResponseMessage GetOptions(Guid id)
        {
            using (DatabaseEntities entity = new DatabaseEntities())
            {
                var productOptions = entity.ProductOptions.Where(op => op.ProductId == id).ToList();
                if (productOptions != null)
                {
                    ProductList items = new ProductList(productOptions);
                    return Request.CreateResponse(HttpStatusCode.OK, items);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No product options are found.");
                }
            }
        }

        [HttpGet, Route("{id}/options/{optionId}")]
        //8. GET /products/{id}/options/{optionId} - finds the specified product option for the specified product.
        public HttpResponseMessage Get(Guid id, Guid optionId)
        {
            using (DatabaseEntities entity = new DatabaseEntities())
            {
                var productOption = entity.ProductOptions.FirstOrDefault(p => p.Id == optionId && p.ProductId == id);
                if (productOption != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, productOption);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Product Option with id = " + id + " not found");
                }
            }
        }

        [HttpPost, Route("{id}/Options")]
        //9. POST /products/{id}/options - adds a new product option to the specified product.
        public HttpResponseMessage Post(Guid id, ProductOption productOption)
        {
            try
            {
                using (DatabaseEntities entity = new DatabaseEntities())
                {
                    productOption.ProductId = id;
                    entity.ProductOptions.Add(productOption);
                    entity.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, productOption);
                    message.Headers.Location = new Uri(Request.RequestUri + productOption.Id.ToString());
                    return message;
                }
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
                using (DatabaseEntities entity = new DatabaseEntities())
                {
                    var productOptionToUpdate = entity.ProductOptions.FirstOrDefault(p => p.Id == optionId);
                    if (productOptionToUpdate != null)
                    {
                        productOptionToUpdate.Name = productOption.Name;
                        productOptionToUpdate.Description = productOption.Description;

                        entity.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, productOptionToUpdate);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Product Option with id = " + optionId + " not found to update.");
                    }
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
                using (DatabaseEntities entity = new DatabaseEntities())
                {
                    var productOption = entity.ProductOptions.FirstOrDefault(p => p.Id == optionId && p.ProductId == id);
                    if (productOption != null)
                    {
                        entity.ProductOptions.Remove(productOption);
                        entity.SaveChanges();
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "ProductOption with Id = " + optionId + " not found to delete.");
                    }
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

    }
}
