using System.Threading.Tasks;

namespace Portfolio_game_dev.Services {
    public interface IPdfResumeService {
        Task<byte[]> GeneratePdfAsync();
    }
}