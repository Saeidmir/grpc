using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Apress.Sample.gRPC;
namespace CountryService.Web.Services;

public class CountryGrpcService : Apress.Sample.gRPC.CountryService.CountryServiceBase
{
    private readonly CountryManagementService _countryManagementService;
    private readonly ILogger<CountryGrpcService> _logger;

    public CountryGrpcService(CountryManagementService countryManagementService, ILogger<CountryGrpcService> logger)
    {
        _countryManagementService = countryManagementService;
        _logger = logger;
    }
    
    public override async Task GetAll(Empty request, IServerStreamWriter<CountryReply> responseStream,
        ServerCallContext context)
    {
 
        throw new TimeoutException("Something got really wrong here");
        var countries = await _countryManagementService.GetAllAsync();
        foreach (var country in countries)
        {
            await responseStream.WriteAsync(country);
        }
        await Task.CompletedTask;
        
        // catch (Exception e)
        // {
        //     var correlationId = Guid.NewGuid();
        //     _logger.LogError(e, "CorrelationId: {0}", correlationId);
        //     var trailers = new Metadata();
        //     trailers.Add("CorrelationId", correlationId.ToString());
        //     throw new RpcException(
        //         new Status(StatusCode.Internal,
        //             $"Error message sent to the client with a CorrelationId: {correlationId}"), trailers,
        //         "Error message that will appear in log server");
        // }
   
    }

    
    public override async Task<CountryReply> Get(CountryIdRequest request,
        ServerCallContext context)
    {
        return await _countryManagementService.GetAsync(request);
    }

    
    public override async Task<Empty> Delete(IAsyncStreamReader<CountryIdRequest> requestStream,
        ServerCallContext context)
    {
        var countryIdRequestList = new List<CountryIdRequest>();
        await foreach (var countryIdRequest in requestStream.ReadAllAsync())
        {
            countryIdRequestList.Add(countryIdRequest);
        }

        await _countryManagementService.DeleteAsync(countryIdRequestList);
        return new Empty();
    }

    
    public override async Task<Empty> Update(CountryUpdateRequest request, ServerCallContext context)
    {
        await _countryManagementService.UpdateAsync(request);
        return new Empty();
    }

    
    public override async Task Create(IAsyncStreamReader<CountryCreationRequest> requestStream,
        IServerStreamWriter<CountryCreationReply> responseStream, ServerCallContext context)
    {
        var countryCreationRequestList = new List<CountryCreationRequest>();
        await foreach (var countryCreationRequest in requestStream.ReadAllAsync())
        {
            countryCreationRequestList.Add(countryCreationRequest);
        }

        var createdCountries = await _countryManagementService.CreateAsync(countryCreationRequestList);
        foreach (var country in createdCountries)
        {
            await responseStream.WriteAsync(country);
        }
    }
}