using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Constants
{
    public static class Messages
    {
        public static string ProductAdded = "Urun Eklendi";
        public static string ProductNameInvalid = "Urun ismi gecersiz";
        public static string MaintenanceTime = "Sistem Bakim Saatinde";
        public static string ProductListed = "Urunler Listelendi";
        public static string ProductCountOfCategoryError = "Bir kategoride en fazla 10 urun olabilir";
        public static string ProductNameAlreadyExist = "Bu isimde urun zaten var";
        public static string CategoryLimitExceded = "Kategori limiti asildi";
    }
}
