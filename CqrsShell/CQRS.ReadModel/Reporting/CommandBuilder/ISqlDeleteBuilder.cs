using System.Collections.Generic;

namespace CQRS.ReadModel.Reporting.CommandBuilder
{
    public interface ISqlDeleteBuilder
    {
        string CreateSqlDeleteStatementFromDto<TDto>();

        string CreateSqlDeleteStatementFromDto<TDto>(IEnumerable<KeyValuePair<string, object>> example)
            where TDto : class;
    }
}