using PhotoApp.Models;
using PhotoApp.Services;

namespace PhotoApp.Services
{
    public class AuthService
    {
        private readonly DataService _dataService;
        private ApplicationUser? _currentUser;

        public AuthService(DataService dataService)
        {
            _dataService = dataService;
        }

        public ApplicationUser? CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;
        public bool IsAdmin => _currentUser?.Role?.Name == "admin";
        public bool IsOperator => _currentUser?.Role?.Name == "operator";
        public bool IsClient => _currentUser?.Role?.Name == "client";

        public async Task<bool> LoginAsync(string login, string password)
        {
            try
            {
                // Просто проверяем логин/пароль в базе
                var user = await _dataService.GetUserByLoginAsync(login, password);

                if (user != null)
                {
                    _currentUser = user; // Сохраняем пользователя в текущей сессии
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка входа: {ex.Message}");
                return false;
            }
        }

        public void Logout()
        {
            _currentUser = null; // Просто очищаем текущего пользователя
        }
    }
}