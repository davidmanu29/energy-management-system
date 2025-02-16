using EnergyManagementSystemMonitoring.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnergyManagementSystemMonitoring.Repositories
{
    public interface IMeasurementRepository
    {
        Task<ActionResult<IEnumerable<MeasurementData>>> GetMeasurements();
        Task<ActionResult<IEnumerable<MeasurementData>>> GetMeasurementsByDate(DateTime date);
        Task<ActionResult<IEnumerable<MeasurementData>>> GetMeasurementsByIdAndDate(DateTime date, Guid id);
    }
}