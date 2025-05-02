using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace BatchProcessing.WebApp.Services;

public class ApiClient
{
    public ApiClient(HttpClient httpClient, NavigationManager navigationManager)
    {
        HttpClient = httpClient;
        HttpClient.BaseAddress = new Uri(navigationManager.BaseUri);
    }

    public HttpClient HttpClient { get; }

    public async Task<Guid> StartBatchProcessingAsync(int records, string? region, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsJsonAsync($"/batchProcessing/?region={region}", records, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
    }

    public Task<EngineStatusRecord?> GetBatchProcessingStatusAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return HttpClient.GetFromJsonAsync<EngineStatusRecord>($"/batchProcessing/{id}/status", cancellationToken);
    }

    public Task<EngineStatusRecord?> GetBatchProcessingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return HttpClient.GetFromJsonAsync<EngineStatusRecord>($"/batchProcessing/{id}", cancellationToken);
    }

    public async Task<IEnumerable<EngineStatusRecord>> GetAllBatchProcessingStatusAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetFromJsonAsync<IEnumerable<EngineStatusRecord>>($"/batchProcessing", cancellationToken);
        return (response == null) ? [] : response.Select(record => record with { RecordsProcessed = record.Status == AnalysisStatus.Completed ? record.RecordCount : record.RecordsProcessed });
    }
}