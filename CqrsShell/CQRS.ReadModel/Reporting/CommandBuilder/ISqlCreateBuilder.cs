using System;

namespace CQRS.ReadModel.Reporting.CommandBuilder
{
    public interface ISqlCreateBuilder
    {
        string CreateSqlCreateStatementFromDto(Type dtoType);
    }
}