using Microsoft.EntityFrameworkCore.Design;

namespace UIConcepts.Core.Model.Context
{
    public class ManagerContextFactory : IDesignTimeDbContextFactory<ManagerContext>
    {
        public ManagerContext CreateDbContext(string[] args)
        {
            return new ManagerContext(ManagerContext.DB_PATH);
        }
    }
}
