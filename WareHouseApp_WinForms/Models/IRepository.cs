using System;
using System.Collections.Generic;

namespace WareHouseApp.Models
{
    /// <summary>
    /// Generic repository interface for database operations.
    /// This interface defines standard CRUD operations for domain objects.
    /// </summary>
    /// <typeparam name="T">Entity type that implements IEntity.</typeparam>
    public interface IRepository<T> where T : class, IEntity
    {
        void Insert(T entity);
        T GetById(int id);
        List<T> GetAll();
        void Update(T entity);
        void Delete(int id);
    }
}
