using System.Collections.Generic;

namespace CQRS.ReadModel.Reporting.CommandBuilder
{
    public interface ISqlSelectBuilder
    {
        string CreateSqlSelectStatementFromDto<TDto>();

        string CreateSqlSelectStatementFromDto<TDto>(IEnumerable<KeyValuePair<string, object>> example)
            where TDto : class;
    }
}