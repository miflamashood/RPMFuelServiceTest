using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RPMFuelService.Data;

namespace RPMFuelService
{
    public interface IRPMFuelRepository
    {
        IReadOnlyCollection<FuelData> GetFuelData();
        void CreateFuelData(FuelData model);
       
    }
    public class RPMFuelRepository : IRPMFuelRepository
    {
        private readonly FuelDbContext _context;

        public RPMFuelRepository(FuelDbContext context)
        {
            _context = context;
        }

        public IReadOnlyCollection<FuelData> GetFuelData() => _context.FuelDatas.ToList();

        public void CreateFuelData(FuelData model)
        {
            _context.FuelDatas.Add(model);
            _context.SaveChanges();
        }
    }
}
