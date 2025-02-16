using EnergyManagementSystemMonitoring.Data;
using EnergyManagementSystemMonitoring.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnergyManagementSystemMonitoring.Repositories
{
    public class MeasurementRepository : IMeasurementRepository
    {
        private readonly IDbContextFactory<EnergyManagementSystemMonitoringDbContext> _contextFactory;

        public MeasurementRepository(IDbContextFactory<EnergyManagementSystemMonitoringDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<ActionResult<IEnumerable<MeasurementData>>> GetMeasurements()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Measurements.ToListAsync();
            }
        }

        public async Task<ActionResult<IEnumerable<MeasurementData>>> GetMeasurementsByDate(DateTime date)
        {
            DateTime startDate = date.Date;
            DateTime endDate = startDate.AddDays(1);

            using (var context = _contextFactory.CreateDbContext())
            {
                var measurementsByDate = await context.Measurements
                    .Where(m => m.TimeStamp >= startDate && m.TimeStamp < endDate)
                    .ToListAsync();

                return measurementsByDate;
            }
        }

        public async Task<ActionResult<IEnumerable<MeasurementData>>> GetMeasurementsByIdAndDate(DateTime date, Guid id)
        {
            DateTime startDate = date.Date;
            DateTime endDate = startDate.AddDays(1);

            using (var context = _contextFactory.CreateDbContext())
            {
                var measurementsByIdAndDate = await context.Measurements
                    .Where(m => (m.TimeStamp >= startDate && m.TimeStamp < endDate) && m.DeviceId.Equals(id))
                    .ToListAsync();

                return measurementsByIdAndDate;
            }
        }
    }
}
