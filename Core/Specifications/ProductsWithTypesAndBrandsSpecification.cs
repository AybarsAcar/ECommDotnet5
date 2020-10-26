using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  /* 
  specification for our eager loading for the Products class
   */
  public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
  {
    // without no criteria -- used for all the products
    public ProductsWithTypesAndBrandsSpecification(ProductSpecParams productParams) : base(
      x =>
        (string.IsNullOrEmpty(productParams.Search) || x.Name.ToLower().Contains(productParams.Search))
        && (!productParams.BrandId.HasValue || x.ProductBrandId == productParams.BrandId)
        && (!productParams.TypeId.HasValue || x.ProductTypeId == productParams.TypeId)
    )
    {
      AddInclude(x => x.ProductBrand);
      AddInclude(x => x.ProductType);
      // default ordering
      AddOrderBy(x => x.Name);

      ApplyPaging(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);

      if (!string.IsNullOrEmpty(productParams.Sort))
      {
        switch (productParams.Sort)
        {
          case "priceAsc":
            AddOrderBy(p => p.Price);
            break;
          case "priceDesc":
            AddOrderByDescending(p => p.Price);
            break;
          default:
            AddOrderBy(p => p.Name);
            break;
        }
      }
    }

    // with criteria where Product.Id = id in the request body
    public ProductsWithTypesAndBrandsSpecification(int id) : base(x => x.Id == id)
    {
      AddInclude(x => x.ProductBrand);
      AddInclude(x => x.ProductType);
    }

  }
}