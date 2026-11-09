using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Models.MeterReadings;

namespace OctopusEnergy.Client.Services.MeterReadings;

/// <summary>
/// GraphQL meter reading mutations and consent lookup for the authenticated dashboard API key.
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

        ValidateElectricityRequest(request);

        CreateElectricityMeterReadingVariables variables = new()
        {
            Mpan = request.Mpan.Trim(),
            SerialNumber = request.SerialNumber.Trim(),
            ReadAt = MeterReadingWireFormatting.FormatReadAtDate(request.ReadAt),
            Readings = MapReadings(request.Readings),
        };

        CreateElectricityMeterReadingMutationData data = await _graphQl.ExecuteAsync<CreateElectricityMeterReadingMutationData, CreateElectricityMeterReadingVariables>(
            MeterReadingMutations.CreateElectricity,
            variables,
            cancellationToken).ConfigureAwait(false);

        CreateMeterReadingOutputWire? output = data.CreateElectricityMeterReading
            ?? throw new OctopusEnergyParseException("The electricity meter reading mutation returned no data.");

        return new ElectricityMeterReadingResult { ReadingErrors = MapReadingErrors(output) };
    }

    /// <inheritdoc />
    public async Task<GasMeterReadingResult> CreateGasAsync(
        GasMeterReadingCreate request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (_graphQl is null)
        {
            throw new OctopusEnergyRequestException(
                "A dashboard API key is required to submit meter readings.");
        }

        ValidateGasRequest(request);

        CreateGasMeterReadingVariables variables = new()
        {
            Mprn = request.Mprn.Trim(),
            SerialNumber = request.SerialNumber.Trim(),
            ReadAt = MeterReadingWireFormatting.FormatReadAtDate(request.ReadAt),
            Reading = request.Reading,
        };

        CreateGasMeterReadingMutationData data = await _graphQl.ExecuteAsync<CreateGasMeterReadingMutationData, CreateGasMeterReadingVariables>(
            MeterReadingMutations.CreateGas,
            variables,
            cancellationToken).ConfigureAwait(false);

        CreateMeterReadingOutputWire? output = data.CreateGasMeterReading
            ?? throw new OctopusEnergyParseException("The gas meter reading mutation returned no data.");

        return new GasMeterReadingResult { ReadingErrors = MapReadingErrors(output) };
    }

    /// <inheritdoc />
    public async Task<ReadingConsentGranularity> GetReadingConsentGranularityAsync(
        string mpan,
        CancellationToken cancellationToken = default)
    {
        if (_graphQl is null)
        {
            throw new OctopusEnergyRequestException(
                "A dashboard API key is required to query reading consent granularity.");
        }

        if (string.IsNullOrWhiteSpace(mpan))
        {
            throw new OctopusEnergyRequestException("MPAN is required.");
        }

        ReadingConsentGranularityQueryVariables variables = new() { Mpan = mpan.Trim() };

        ReadingConsentGranularityQueryData data = await _graphQl.ExecuteAsync<ReadingConsentGranularityQueryData, ReadingConsentGranularityQueryVariables>(
            MeterReadingQueries.ReadingConsentGranularity,
            variables,
            cancellationToken).ConfigureAwait(false);

        return data.ReadingConsentGranularity
            ?? throw new OctopusEnergyParseException("The readingConsentGranularity query returned no data.");
    }

    private static void ValidateElectricityRequest(ElectricityMeterReadingCreate request)
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

    private static void ValidateGasRequest(GasMeterReadingCreate request)
    {
        if (string.IsNullOrWhiteSpace(request.Mprn))
        {
            throw new OctopusEnergyRequestException("MPRN is required.");
        }

        if (string.IsNullOrWhiteSpace(request.SerialNumber))
        {
            throw new OctopusEnergyRequestException("Meter serial number is required.");
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

    private static List<MeterReadingFieldError> MapReadingErrors(CreateMeterReadingOutputWire output)
    {
        if (output.ReadingErrors is null or { Count: 0 })
        {
            return [];
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

        return errors;
    }
}
