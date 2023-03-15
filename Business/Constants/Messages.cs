using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants
{
    public static class Messages
    {
        public static string ProductAdded = "Ürün Eklendi";
        public static string ProductNameInvalid= "Ürün ismi geçersiz";
        public static string MaintenanceTime = "Sistem Bakımda";
        public static string ProductsListed = "Ürünler Listelendi";
        internal static string UnitPriceInvalid = "Ürün fiyatı geçersiz";
        internal static string ProductCountOfCategoryError="Kategori Maks Ürün Sayısına Ulaştı";
        internal static string ProductNameAlreadyExists = "Bu isimde Zaten Başka Bir Ürün Var ";
        internal static string CategoryLimitExceded ="Kategori Sayısı Maksimum Sayıya Ulaştı";
        internal static string AuthorizationDenied="Yetkiniz Yok";
        internal static string UserRegistered="Kayıt olundu";
        internal static string UserNotFound="Kullanıcı Bulunamadı";
        internal static string PasswordError="Hatalı Şifre";
        internal static string SuccessfulLogin="Giriş Yapıldı";
        internal static string UserAlreadyExists="Kullanıcı Adı Zaten Mevcut";
        internal static string AccessTokenCreated="Token Oluşturuldu";
    }
}
