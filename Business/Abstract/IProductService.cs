using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IProductService
    {
        IDataResult<List<Product>> GetAll(); // List<Product> idi
        IDataResult<List<Product>> GetAllByCategoryId(int id);
        IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max);
        IDataResult<List<ProductDetailDto>> GetProductDetails(); //List<ProductDetailDto> idi
        IResult Add(Product product);
        IResult GetByReOrderLevel(int id);
        IResult Update(Product product);
        IDataResult<Product> GetById(int productId);
    }
}
