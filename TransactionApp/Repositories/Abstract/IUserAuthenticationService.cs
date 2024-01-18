using TransactionApp.Models.DTO;

namespace TransactionApp.Repositories.Abstract
{
    public interface IUserAuthenticationService
    {

        Task<Status> LoginAsync(LoginModel model);
        Task LogoutAsync();
        Task<Status> RegisterAsync(RegistrationModel model);
        Task<Status> ChangePasswordAsync(ChangePassword model, string username);


        Task<UserProfileModel> GetProfileAsync(string username);
        Task<Status> UpdateProfileAsync(UserProfileModel model, string username);

    }
}
