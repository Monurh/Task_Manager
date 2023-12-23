namespace Task_Manager.Model
{
    public class Users
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Rolle {  get; set; }
    }
    public  class LoginModel
    {
        public string UserName { get; set; }    
        public string Password { get; set; }    
    }
}
