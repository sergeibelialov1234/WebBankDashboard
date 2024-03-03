using System.Text;
using System.Text.Json;

namespace WebBankDashboard.BankClient;

public interface IBankClient
{
    Task<BankAccount> GetAccountAsync(string accountNumber);
    Task<IEnumerable<BankAccount>> GetAccountsAsync();
    Task CreateAccountAsync(CreateBankAccount account);
    Task UpdateAccountAsync(string accountId,BankAccount account);
    Task DeleteAccountAsync(string accountNumber);
    Task<BankAccount> DepositAsync(string accountNumber, BankClient.DepositTransaction depositTransaction);
}

public class BankAccount : CreateBankAccount
{
    public int id { get; set; }
    public DateTime openDate { get; set; } = DateTime.Now;
    public int number { get; set; }
    public int balance { get; set; }
    public List<object> transactions { get; set; } = new List<object>();
}

public class CreateBankAccount
{
    public string owner { get; set; }
}

public class BankClient : IBankClient
{
    private readonly HttpClient _httpClient;

    public BankClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BankAccount> GetAccountAsync(string accountNumber)
    {
        var response = await _httpClient.GetAsync($"/accounts/{accountNumber}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<BankAccount>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        return data;
    }

    public async Task<IEnumerable<BankAccount>> GetAccountsAsync()
    {
        var response = await _httpClient.GetAsync("/accounts");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<IEnumerable<BankAccount>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        return data;
    }

    public async Task CreateAccountAsync(CreateBankAccount account)
    {
        var response = await _httpClient.PostAsync("/accounts", new StringContent(JsonSerializer.Serialize(account), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAccountAsync(string accountId, BankAccount account)
    {
        var response = await _httpClient.PutAsync($"/accounts/{accountId}", new StringContent(JsonSerializer.Serialize(account), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAccountAsync(string accountNumber)
    {
        var response = await _httpClient.DeleteAsync($"/accounts/{accountNumber}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<BankAccount> DepositAsync(string accountNumber, DepositTransaction depositTransaction)
    {
        var response = await _httpClient.PostAsync($"/accounts/{accountNumber}/deposit", new StringContent(JsonSerializer.Serialize(depositTransaction), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<BankAccount>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return data;
    }

    public class DepositTransaction
    {
       public string DepositAmount { get;set; }
    }
}