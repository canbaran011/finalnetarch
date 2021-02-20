using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.InMemory;
using DataAccess.Concrete.EntityFramework;
using System;

namespace ConsoleUI
{
    class Program
    {
        // SOLID
        // OpenClosed Principle -- IoC container
        // DTO Data Transformation Object
        static void Main(string[] args)
        {
            ProductTest();
            //CategoryTest();


             Console.WriteLine("Hello World!");
        }

        private static void CategoryTest()
        {
            CategoryManager categoryManager = new CategoryManager(new EfCategoryDal());

            foreach (var category in categoryManager.GetAll())
            {
                Console.WriteLine(category.CategoryName);
            }
        }

        private static void ProductTest()
        {
            ProductManager productManager = new ProductManager(new EfProductDal());


            var result = productManager.GetProductDetails();

            if (result.Success)
            {
                foreach (var product in result.Data)
                {
                    Console.WriteLine("Product Name : " + product.ProductName);
                    Console.WriteLine("Category Name: " + product.CategoryName);
                    Console.WriteLine("---------------------------------------");
                }
            }
            else
            {
                Console.WriteLine(result.Message);
            }

        }
    }
}
