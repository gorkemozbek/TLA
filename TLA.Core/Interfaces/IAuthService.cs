using TLA.Core.DTOs;
using TLA.Core.DTOs.Response;

namespace TLA.Core.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Kullanıcı giriş işlemini gerçekleştirir
        /// </summary>
        /// <param name="loginDto">Giriş bilgileri (email, password)</param>
        /// <returns>Login başarılıysa token ve kullanıcı bilgileri döner</returns>
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Yeni kullanıcı kaydı oluşturur
        /// </summary>
        /// <param name="registerDto">Kayıt bilgileri (email, password, confirmPassword)</param>
        /// <returns>Kayıt başarılıysa kullanıcı bilgileri döner</returns>
        Task<ApiResponse<UserDto>> RegisterAsync(RegisterDto registerDto);

        /// <summary>
        /// JWT token'ın geçerli olup olmadığını kontrol eder
        /// </summary>
        /// <param name="token">Kontrol edilecek JWT token</param>
        /// <returns>Token geçerliyse true döner</returns>
        Task<ApiResponse<bool>> ValidateTokenAsync(string token);

        /// <summary>
        /// Kullanıcının sahip olduğu tüm izinleri getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Kullanıcının izin listesi</returns>
        Task<List<string>> GetUserPermissionsAsync(int userId);

        /// <summary>
        /// Refresh token ile yeni access token oluşturur (opsiyonel - gelecekte eklenebilir)
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>Yeni access token</returns>
        // Task<ApiResponse<string>> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Kullanıcının şifresini değiştirir (opsiyonel - gelecekte eklenebilir)
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <param name="currentPassword">Mevcut şifre</param>
        /// <param name="newPassword">Yeni şifre</param>
        /// <returns>İşlem sonucu</returns>
        // Task<ApiResponse<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}