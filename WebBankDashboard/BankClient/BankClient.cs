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
    Task<BankAccount> WithdrawAsync(string accountNumber, BankClient.WithdrawTransaction withdrawTransaction);
    Task<BankAccount> TransferAsync(BankClient.TransferTransaction deposit);
}

public class BankAccount : CreateBankAccount
{
    public int id { get; set; }
    public DateTime openDate { get; set; } = DateTime.Now;
    public int number { get; set; }
    public int balance { get; set; }
    public List<Transaction> transactions { get; set; } = new List<Transaction>();
}

public class Transaction
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public int Amount { get; set; }
    public int Account { get; set; }
    public int OldBalance { get; set; }
    public int NewBalance { get; set; }
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

    public async Task<BankAccount> WithdrawAsync(string accountNumber, WithdrawTransaction withdrawTransaction)
    {
        var response = await _httpClient.PostAsync($"/accounts/{accountNumber}/withdraw", new StringContent(JsonSerializer.Serialize(withdrawTransaction), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<BankAccount>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return data;
    }

    public async Task<BankAccount> TransferAsync(TransferTransaction transfer)
    {
        var response = await _httpClient.PostAsync($"/accounts/transfer", new StringContent(JsonSerializer.Serialize(transfer), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<BankAccount>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return data;
    }

    public class DepositTransaction
    {
       public string DepositAmount { get;set; }
    }
    
    public class WithdrawTransaction
    {
        public string WithdrawAmount { get;set; }
    }

    public class TransferTransaction
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public int Amount { get; set; }
    }
}