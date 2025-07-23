using TLA.Core.DTOs;
using TLA.Core.DTOs.Response;
using TLA.Core.Entities;

namespace TLA.Core.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// ID'ye göre kullanıcı getirir
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        /// <returns>Kullanıcı bilgileri (DTO formatında)</returns>
        Task<ApiResponse<UserDto>> GetByIdAsync(int id);

        /// <summary>
        /// Email adresine göre kullanıcı getirir
        /// </summary>
        /// <param name="email">Kullanıcı email adresi</param>
        /// <returns>Kullanıcı bilgileri (DTO formatında)</returns>
        Task<ApiResponse<UserDto>> GetByEmailAsync(string email);

        /// <summary>
        /// Tüm kullanıcıları getirir
        /// </summary>
        /// <returns>Kullanıcı listesi (DTO formatında)</returns>
        Task<ApiResponse<List<UserDto>>> GetAllAsync();

        /// <summary>
        /// Email adresine göre kullanıcı entity'sini getirir (Authentication için)
        /// </summary>
        /// <param name="email">Kullanıcı email adresi</param>
        /// <returns>User entity (şifreli hali ile)</returns>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Belirtilen email adresinde kullanıcı var mı kontrol eder
        /// </summary>
        /// <param name="email">Kontrol edilecek email adresi</param>
        /// <returns>Kullanıcı varsa true, yoksa false</returns>
        Task<bool> UserExistsAsync(string email);

        /// <summary>
        /// Yeni kullanıcı oluşturur
        /// </summary>
        /// <param name="email">Kullanıcı email adresi</param>
        /// <param name="password">Kullanıcı şifresi (hashlenecek)</param>
        /// <returns>Oluşturulan kullanıcı entity'si</returns>
        Task<User> CreateUserAsync(string email, string password);

        /// <summary>
        /// Kullanıcı bilgilerini günceller (opsiyonel - gelecekte eklenebilir)
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        /// <param name="updateDto">Güncellenecek bilgiler</param>
        /// <returns>Güncellenmiş kullanıcı bilgileri</returns>
        // Task<ApiResponse<UserDto>> UpdateAsync(int id, UpdateUserDto updateDto);

        /// <summary>
        /// Kullanıcıyı siler (opsiyonel - gelecekte eklenebilir)
        /// </summary>
        /// <param name="id">Silinecek kullanıcı ID'si</param>
        /// <returns>İşlem sonucu</returns>
        // Task<ApiResponse<bool>> DeleteAsync(int id);

        /// <summary>
        /// Kullanıcının aktif/pasif durumunu değiştirir (opsiyonel - gelecekte eklenebilir)
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        /// <param name="isActive">Aktif durumu</param>
        /// <returns>İşlem sonucu</returns>
        // Task<ApiResponse<bool>> ToggleActiveStatusAsync(int id, bool isActive);

        /// <summary>
        /// Kullanıcının rollerini getirir (opsiyonel - gelecekte eklenebilir)
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Kullanıcının rol listesi</returns>
        // Task<ApiResponse<List<string>>> GetUserRolesAsync(int userId);

        /// <summary>
        /// Kullanıcının izinlerini getirir (opsiyonel - gelecekte eklenebilir)
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Kullanıcının izin listesi</returns>
        // Task<ApiResponse<List<string>>> GetUserPermissionsAsync(int userId);
    }
}