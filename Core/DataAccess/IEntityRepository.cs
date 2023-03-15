﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.DataAccess
{   // Generic constraint
    //class : referans tip olabilir anlamına geliyor
    //IEntity : IEntity  ya da IEntity  implemente eden bi veri tipi olabiir
    // new(): new'lenebilir olmalı

    public interface IEntityRepository<T> where T: class,IEntity,new()
    {
        List<T> GetAll(Expression<Func<T,bool>> filter=null);
        T Get(Expression<Func<T, bool>> filter);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        //List<T> GetAllByCategory(int categoryId);

    }
}
