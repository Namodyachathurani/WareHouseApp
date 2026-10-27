using System;

namespace WareHouseApp.Models
{
    /// <summary>
    /// Base interface for domain entities.
    /// This interface provides a primary key property for all entities.
    /// </summary>
    public interface IEntity
    {
        int Id { get; set; }
    }
}
