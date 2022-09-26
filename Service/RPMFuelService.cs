using RPMFuelService.Data;

// ReSharper disable CheckNamespace
namespace RPMFuelService 
// ReSharper restore CheckNamespace
{
    public interface IRPMFuelService
    {
         void SyncRPMFuelData(IReadOnlyCollection<FuelData> sourceEntities);
    }
    public class RPMFuelService : IRPMFuelService
    {
        private readonly IRPMFuelRepository _repository;

        public RPMFuelService(IRPMFuelRepository repository)
        {
            _repository = repository;
        }

        public void SyncRPMFuelData(IReadOnlyCollection<FuelData> sourceEntities)
        {
            var existingEntities = _repository.GetFuelData();
            AddFuelData(sourceEntities, existingEntities);
        }
        
        private void CreateFuelData(FuelData fuelData)
        {
            var model = new FuelData();
            model.RecordDate = fuelData.RecordDate;
            model.Price = fuelData.Price;
            _repository.CreateFuelData(model);
        }

        private void AddFuelData(IReadOnlyCollection<FuelData> sourceEntities,
            IReadOnlyCollection<FuelData> existingEntities)
        {
            // add new records
            var newRecords = sourceEntities
                .Where(c => !existingEntities.Select(x => x.RecordDate).Contains(c.RecordDate))
                .Select(c => {  return c; });

            foreach (var fuelData in newRecords)
            {
                CreateFuelData(fuelData);
            }
        }



}
}
