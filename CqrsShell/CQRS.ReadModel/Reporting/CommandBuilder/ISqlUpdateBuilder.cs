namespace CQRS.ReadModel.Reporting.CommandBuilder
{
    public interface ISqlUpdateBuilder
    {
        string GetUpdateString<TDto>(object update, object where) where TDto : class;
    }
}