namespace TransactionApp.Models.DTO
{
    public class UserProfileModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Username { get; set; }

        public string? ProfilePicture { get; set; }

        public string PhoneNumber { get; set; }
    }
}
