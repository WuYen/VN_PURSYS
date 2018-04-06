using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ERP_V2.Services
{
    //泛型介面
    //https://docs.microsoft.com/zh-tw/dotnet/csharp/programming-guide/concepts/covariance-contravariance/creating-variant-generic-interfaces
    //https://docs.microsoft.com/zh-tw/dotnet/csharp/programming-guide/generics/generic-interfaces

    public interface IServiceMD<MD, M, D, in TKeyM, TKeyD>
    {
        M GetM(Expression<Func<M, bool>> predicate);

        D GetD(Expression<Func<D, bool>> predicate);

        IQueryable<M> GetAllM();

        IQueryable<D> GetAllD(TKeyM key);

        MD GetMD(TKeyM key);

        /// <summary>Master:Update only; Detail: Create、Update、Delete</summary>
        /// <param name="data">Master entity and List<Detail></param>
        /// <returns>Error massage</returns>
        string UpdateMD(M UpdateM, List<D> CreateD, List<D> UpdateD, List<TKeyD> DeleteD);

        /// <summary>Delete detail first then master</summary>
        /// <param name="key">Master key</param>
        /// <returns>Error massage</returns>
        string DeleteM(M data);
    }
}
