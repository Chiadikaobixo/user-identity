using BCryptNet = BCrypt.Net.BCrypt;

namespace Hash
{
    public class Hashed
    {
        public string hashedPassword(string password)
        {
            string salt = BCryptNet.GenerateSalt();
            string hashedPassword = BCryptNet.HashPassword(password, salt);
            return hashedPassword;
        }
        
        public bool validPassword(string password, string hashedPassword){
            bool isPasswordValid = BCryptNet.Verify(password, hashedPassword);
            return isPasswordValid;
        }
    }
}
