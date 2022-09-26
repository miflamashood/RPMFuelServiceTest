using System.Globalization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RPMFuelService.Data;

// ReSharper disable once CheckNamespace
namespace RPMFuelService
{
    public interface IRPMFuelDataProvider
    {
        Task<IReadOnlyCollection<FuelData>> ReadFuelData();
    }

    internal class RPMFuelDataProvider : IRPMFuelDataProvider
    {
        private readonly IOptions<RPMFuelServiceConfig> _rpmFuelServiceConfig;
        public RPMFuelDataProvider(IOptions<RPMFuelServiceConfig> rpmFuelServiceConfig) => _rpmFuelServiceConfig = rpmFuelServiceConfig ?? throw new ArgumentNullException(nameof(rpmFuelServiceConfig));

        public async Task<IReadOnlyCollection<FuelData>> ReadFuelData()
        {
            using var httpClient = new HttpClient();
           
            httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
           
            var httpResponseAsync = await httpClient.GetAsync(_rpmFuelServiceConfig.Value.FuelDataUrl);
            
            var jsonString = await httpResponseAsync.Content.ReadAsStringAsync();

            var rawFuelData = JsonConvert.DeserializeObject<RawFuelData>(jsonString);

            var fuelData = new List<FuelData>();

            DateTime dateToCompare = DateTime.Today.AddDays(-Int32.Parse(_rpmFuelServiceConfig.Value.DaysCount)).Date;

            if (rawFuelData!=null)
                fuelData=  rawFuelData.series[0].data.Select(fuelitem => new FuelData()
                {
                    Price = Convert.ToSingle(fuelitem[1]),
                    RecordDate = Convert.ToString(fuelitem[0]) ?? string.Empty
                }).Where(x=> DateTime.ParseExact(x.RecordDate, "yyyyMMdd", CultureInfo.InvariantCulture).Date > dateToCompare).ToList();

            return fuelData;
        }
    }
}