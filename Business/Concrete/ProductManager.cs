﻿using Business.Abstract;
using Business.CCS;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        // Constructor Injection
        IProductDal _productDal;
        ILogger _logger;

        public ProductManager(IProductDal productDal , ILogger logger)
        {
            _productDal = productDal;
            _logger = logger;
        }
        //[LogAspect]
        //[Validate]
        //[RemoveCache]
        //[Transaction]
        //[Performance]

        [ValidationAspect(typeof(ProductValidator))]
        public IResult Add(Product product)
        {

                //business codes
                _productDal.Add(product);
                return new SuccessResult(Messages.ProductAdded);
           
        }

        public IDataResult<List<Product>> GetAll()
        {
            // Bussiness code lari --
            // yetkisi vs vs

            if (DateTime.Now.Hour == 9)
            {
                return new ErrorDataResult<List<Product>>(Messages.MaintenanceTime);
            }

            return new SuccessDataResult<List<Product>>( _productDal.GetAll(), Messages.ProductListed);

        }

        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>>( _productDal.GetAll(p => p.CategoryId == id)); 
        }

        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p=> p.ProductId == productId)); 
        }

        public IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll( p => p.UnitPrice <= min && p.UnitPrice <= max)); 
        }

        public IDataResult<List<ProductDetailDto>> GetProductDetails()
        {
            return new SuccessDataResult<List<ProductDetailDto>>(_productDal.GetProductDetails());
        }
    }
}
