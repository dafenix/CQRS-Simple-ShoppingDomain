using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRS.Infrastructure;
using CQRS.ReadModel.Reporting;
using CQRS.ReadModel.Repository;

namespace CQRS.WPFUI
{
    internal class BusFacade
    {
        public static Bus MessageBus;
        public static IReportingRepository ReportingRepository;
    }
}