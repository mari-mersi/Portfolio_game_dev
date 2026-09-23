using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services.Abstractions;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Services.Implementations;

public class ContactService : IContactService {
    private readonly AppDbContext _db;

    public ContactService(AppDbContext db) {
        _db = db;
    }

    public async Task SaveAsync(ContactViewModel vm, string? ipHash, CancellationToken ct = default) {
        var message = new ContactMessage {
            Name = vm.Name.Trim(),
            Email = vm.Email.Trim().ToLowerInvariant(),
            Message = vm.Message.Trim(),
            IpHash = ipHash,
            IsRead = false
        };

        _db.ContactMessages.Add(message);
        await _db.SaveChangesAsync(ct);
    }
}