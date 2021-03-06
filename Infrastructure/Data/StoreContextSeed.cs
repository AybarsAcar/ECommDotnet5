using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
  public class StoreContextSeed
  {
    public static async Task SeedAsync(StoreContext context, ILoggerFactory loggerFactory)
    {
      try
      {
        if (!context.ProductBrands.Any())
        {
          var brandsData = File.ReadAllText("../Infrastructure/Data/SeedData/brands.json");

          var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

          // add them to db
          foreach (var brand in brands)
          {
            context.ProductBrands.Add(brand);
          }

          await context.SaveChangesAsync();
        }

        if (!context.ProductTypes.Any())
        {
          var typesData = File.ReadAllText("../Infrastructure/Data/SeedData/types.json");

          var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);

          // add them to db
          foreach (var type in types)
          {
            context.ProductTypes.Add(type);
          }

          await context.SaveChangesAsync();
        }

        if (!context.Products.Any())
        {
          var productsData = File.ReadAllText("../Infrastructure/Data/SeedData/products.json");

          var products = JsonSerializer.Deserialize<List<Product>>(productsData);

          // add them to db
          foreach (var product in products)
          {
            context.Products.Add(product);
          }

          await context.SaveChangesAsync();
        }

        if (!context.DeliveryMethods.Any())
        {
          var dmData = File.ReadAllText("../Infrastructure/Data/SeedData/delivery.json");

          var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);

          // add them to db
          foreach (var deliveryMethod in deliveryMethods)
          {
            context.DeliveryMethods.Add(deliveryMethod);
          }

          await context.SaveChangesAsync();
        }
      }
      catch (Exception ex)
      {
        var logger = loggerFactory.CreateLogger<StoreContextSeed>();
        logger.LogError(ex.Message);
      }
    }
  }
}