using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.CCS;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrıssCuttingConcerns.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using static Core.Aspects.Autofac.Caching.CacheAspect;

namespace Business.Concrete
{
    public partial class ProductManager : IProductService
    {
        IProductDal _productDal;
        ICategoryService _categoryService;
            
        // 1 Manager Sadece Kendi Dalını Enjekte eder
        public ProductManager(IProductDal productDal , ICategoryService categoryService)
        {
            _productDal = productDal;
            _categoryService = categoryService;
        }

         //Claim 
         [SecuredOperation("product.add,admin")]
         [ValidationAspect(typeof(ProductValidator))]
         [CacheRemoveAspect("IProductService.Get")] // Iproductservice'de ki bütün Get Cachelerini sil
        public IResult Add(Product product)
        {
            if (CheckIfProductCountOfCategoryCorrect(product.CategoryId).Success  ) 
            {
                if (CheckIfProductNameExists(product.ProductName).Success)
                {
                    _productDal.Add(product);
                    return new SuccessResult(Messages.ProductAdded);

                }

            }
            return new ErrorResult();
           
            //business codes 
            //validation codes BİRBİRİYLE KARIŞTIRMA !

            //if (product.UnitPrice <= 0)
            //{
            //    return new ErrorResult(Messages.UnitPriceInvalid);

            //}

            //if (product.ProductName.Length<2)
            //{
            //    return new ErrorResult(Messages.ProductNameInvalid);

            //}

            
            
        }
        [ValidationAspect(typeof(ProductValidator))]
        [CacheRemoveAspect("IProductService.Get")] // Iproductservice'de ki bütün Get Cachelerini sil
        public IResult Update(Product product)
        {

            IResult result= BusinessRules.Run(CheckIfProductCountOfCategoryCorrect(product.CategoryId),
                CheckIfProductNameExists(product.ProductName),
                CheckIfCategoryLimitExceded());
                
            if (result !=null)
            {
                return result;
            }
                      _productDal.Update(product);
                      return new SuccessResult(Messages.ProductAdded);

            //  if (CheckIfProductCountOfCategoryCorrect(product.CategoryId).Success)
            //  {
            //      if (CheckIfProductNameExists(product.ProductName).Success)
            //      {
            //          _productDal.Update(product);
            //          return new SuccessResult(Messages.ProductAdded);
            //  
            //      }
            //  
            //  }
            return new ErrorResult();
        }
        [CacheAspect] //key,value
        public IDataResult<List<Product>> GetAll()
        {
            if (DateTime.Now.Hour==23)
            {
                return new ErrorDataResult<List<Product>>(Messages.MaintenanceTime);
            }
            return new SuccessDataResult<List<Product>> (_productDal.GetAll(),Messages.ProductsListed);

        }
        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p=>p.CategoryId==id), Messages.ProductsListed);
        }
        [CacheAspect] //key,value
        [PerformanceAspect(5)] //Parametre olarak saniye veriyoruz  Method Verilen Saniyeden uzun çalışırsa sistem yavaşlığını uyar
        public IDataResult<Product> GetById(int productId)
        {
        
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductId == productId));
                }

        public IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.UnitPrice >= min && p.UnitPrice <= max));
        }

        public IDataResult<List<ProductDetailDto>> GetProductDetails()
        {
            if (DateTime.Now.Hour == 14)
            {
                return new ErrorDataResult<List<ProductDetailDto>>(Messages.MaintenanceTime);
            }
            return new SuccessDataResult<List<ProductDetailDto>>(_productDal.GetProductDetails());
        }

        private IResult CheckIfProductCountOfCategoryCorrect(int categoryId)
        {
            // Select count(*) from products where  categoryid=1
            var result = _productDal.GetAll(p => p.CategoryId == categoryId).Count;
            if (result >= 20)
            {
                return new ErrorResult(Messages.ProductCountOfCategoryError);
            }
            return new SuccessResult();
        }
        private IResult CheckIfProductNameExists(string productName)
        {
            // Any = Şart Sağlanıyor mu sorusuna karşılık bool değer dondürür 

            var result = _productDal.GetAll(p => p.ProductName == productName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
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
            return new SuccessResult();
        }
        [TransactionScopeAspect]
        public IResult AddTransactionalTest(Product product)
        {
            Add(product);
            if (product.UnitPrice<10)
            {
                throw new Exception("");
            }

            Add(product);
            return null;
        }
    }
} 
