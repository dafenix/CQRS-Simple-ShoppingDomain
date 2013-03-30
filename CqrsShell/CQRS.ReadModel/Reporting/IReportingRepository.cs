using System;
using System.Collections.Generic;

namespace CQRS.ReadModel.Reporting
{
    public interface IReportingRepository
    {
        void Bootstrap(IEnumerable<Type> reportingTypes);
        IEnumerable<TDto> GetByExample<TDto>(object example) where TDto : class;
        void Save<TDto>(TDto dto) where TDto : class;
        void Update<TDto>(object update, object where) where TDto : class;
        void Delete<TDto>(object example) where TDto : class;
    }
}