namespace CQRS.ReadModel.Reporting.CommandBuilder
{
    public interface ISqlInsertBuilder
    {
        string CreateSqlInsertStatementFromDto<TDto>() where TDto : class;
    }
}