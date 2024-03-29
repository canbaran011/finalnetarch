﻿using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.CCS;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        // Constructor Injection
        IProductDal _productDal;
        //ICategoryDal _categoryDal;  // olmaz kendisi harici Injection olmaz
        ICategoryService _categoryService;

        public ProductManager(IProductDal productDal, ICategoryService categoryService)
        {
            _productDal = productDal;
            _categoryService = categoryService;

        }
        //[LogAspect]
        //[Validate]
        //[RemoveCache]
        //[Transaction]
        //[Performance]
        //Claim
        //[SecuredOperation("admin")]//,product.add --last
        //[ValidationAspect(typeof(ProductValidator))] --last
        //[CacheRemoveAspect("IProductService.Get")] --last
        public IResult Add(Product product)
        {   //business codes // yetki --last
            //IResult result = BusinessRules.Run(CheckIfProductNameExist(product.ProductName),
            //    CheckIfProductCountOfCategory(product.CategoryId),
            //    CheckIfCategoryLimitExceded());
            //if (result != null)
            //{
            //    return result;
            //}

            _productDal.Add(product); 
            
            return new SuccessResult(Messages.ProductAdded);

        }
        //[CacheAspect] // key ,value // --last
        public IDataResult<List<Product>> GetAll()
        {
            // Bussiness code lari --
            // yetkisi vs vs
            // --last
            //if (DateTime.Now.Hour == 9)
            //{
            //    return new ErrorDataResult<List<Product>>(Messages.MaintenanceTime);
            //}

            return new SuccessDataResult<List<Product>>(_productDal.GetAll(), Messages.ProductListed);

        }

        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.CategoryId == id));
        }
        //[CacheAspect] --last
        //[PerformanceAspect(5)] --last
        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductId == productId));
        }

        public IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.UnitPrice <= min && p.UnitPrice <= max));
        }

        public IDataResult<List<ProductDetailDto>> GetProductDetails()
        {
            return new SuccessDataResult<List<ProductDetailDto>>(_productDal.GetProductDetails());
        }

        //[ValidationAspect(typeof(ProductValidator))] --last
        //[CacheRemoveAspect("IProductService.Get")] --last
        public IResult Update(Product product)
        {
            _productDal.Update(product);
            return new SuccessDataResult<Product>(Messages.ProductUpdated);
        }

        private IResult CheckIfProductCountOfCategory(int categoryId)
        {
            // Bir kategoride en fazla 10 urun olabilir
            var result = _productDal.GetAll(p => p.CategoryId == categoryId).Count;
            if (result >= 20)
            {
                return new ErrorResult(Messages.ProductCountOfCategoryError);
            }
            return new SuccessResult();
        }

        private IResult CheckIfProductNameExist(string productName)
        {
            // Ayni isimde urun eklenemez...
            var result = _productDal.GetAll(p => p.ProductName == productName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExist);
            }
            return new SuccessResult();
        }
        private IResult CheckIfCategoryLimitExceded()
        {
            var result = _categoryService.GetAll();
            if (result.Data.Count > 15)
            {
                return new ErrorResult(Messages.CategoryLimitExceded);

            }

            return default;
        }

        public IResult GetByReOrderLevel(int id)
        {
            throw new NotImplementedException();
        }
       // [TransactionScopeAspect]
        public IResult AddTransactionalTest(Product product)
        {
            //using (TransactionScope scope=new TransactionScope())
            //{
            //    try
            //    {
            //        Add(product);
            //        if (product.UnitPrice < 10)
            //        {
            //            throw new Exception("");
            //        }
            //        Add(product);
            //        scope.Complete();
            //    }
            //    catch (Exception ex)
            //    {
            //        scope.Dispose();
            //        Console.WriteLine(ex.Message);
            //        throw;
            //    }
            //}
            //return null;
            Add(product);
            if (product.UnitPrice < 10)
            {
                throw new Exception("");
            }
            Add(product);
            return null;
        }
    }
}
