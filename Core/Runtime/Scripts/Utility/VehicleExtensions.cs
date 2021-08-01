using Tesla.API.Core;
using System.Threading.Tasks;

public static class VehicleExtensions
{
    /// <summary>
    /// Opens the charge port.
    /// </summary>
    /// <param name="onRequestComplete"></param>
    public static async Task<Command> OpenChargePortAsync(this Vehicle vehicle) {

        return await vehicle.RequestAsync<Command>($"/api/1/vehicles/{vehicle.Id}/command/charge_port_door_open");
    }
}
