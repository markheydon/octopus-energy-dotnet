using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Models.MeterReadings;

namespace OctopusEnergy.Client.Services.MeterReadings;

/// <summary>
/// GraphQL meter reading mutations for the authenticated dashboard API key.
/// </summary>
public sealed class MeterReadingService : IMeterReadingService
{
    private readonly GraphQlClient? _graphQl;

    internal MeterReadingService(GraphQlClient? graphQl)
    {
        _graphQl = graphQl;
    }

    /// <inheritdoc />
    public async Task<ElectricityMeterReadingResult> CreateElectricityAsync(
        ElectricityMeterReadingCreate request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (_graphQl is null)
        {
            throw new OctopusEnergyRequestException(
                "A dashboard API key is required to submit meter readings.");
        }

        ValidateRequest(request);

        CreateElectricityMeterReadingVariables variables = new()
        {
            Mpan = request.Mpan.Trim(),
            SerialNumber = request.SerialNumber.Trim(),
            ReadAt = MeterReadingWireFormatting.FormatReadAtDate(request.ReadAt),
            Readings = MapReadings(request.Readings),
        };

        CreateElectricityMeterReadingMutationData data = await _graphQl.ExecuteAsync<CreateElectricityMeterReadingMutationData>(
            MeterReadingMutations.CreateElectricity,
            variables,
            cancellationToken).ConfigureAwait(false);

        CreateMeterReadingOutputWire? output = data.CreateElectricityMeterReading
            ?? throw new OctopusEnergyParseException("The electricity meter reading mutation returned no data.");

        return MapResult(output);
    }

    private static void ValidateRequest(ElectricityMeterReadingCreate request)
    {
        if (string.IsNullOrWhiteSpace(request.Mpan))
        {
            throw new OctopusEnergyRequestException("MPAN is required.");
        }

        if (string.IsNullOrWhiteSpace(request.SerialNumber))
        {
            throw new OctopusEnergyRequestException("Meter serial number is required.");
        }

        if (request.Readings is null or { Count: 0 })
        {
            throw new OctopusEnergyRequestException("At least one register reading is required.");
        }
    }

    private static List<ElectricityReadingInputWire> MapReadings(
        IReadOnlyList<ElectricityRegisterReading> readings)
    {
        List<ElectricityReadingInputWire> wireReadings = new(readings.Count);

        foreach (ElectricityRegisterReading reading in readings)
        {
            ArgumentNullException.ThrowIfNull(reading);
            wireReadings.Add(new ElectricityReadingInputWire
            {
                RegisterId = reading.RegisterId,
                Reading = reading.Reading,
            });
        }

        return wireReadings;
    }

    private static ElectricityMeterReadingResult MapResult(CreateMeterReadingOutputWire output)
    {
        if (output.ReadingErrors is null or { Count: 0 })
        {
            return new ElectricityMeterReadingResult();
        }

        List<MeterReadingFieldError> errors = new(output.ReadingErrors.Count);

        foreach (MeterReadingFieldErrorWire wire in output.ReadingErrors)
        {
            errors.Add(new MeterReadingFieldError
            {
                Field = wire.Field,
                Errors = wire.Errors ?? [],
            });
        }

        return new ElectricityMeterReadingResult { ReadingErrors = errors };
    }
}
