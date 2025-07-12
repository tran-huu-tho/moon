namespace moon.Models
{
    public class User
    {
        public string Id { get; set; }            // ID người dùng
        public string Name { get; set; }          // Tên
        public string Email { get; set; }         // Email đăng nhập
        public string Password { get; set; }      // Mật khẩu
        public string Phone { get; set; }         // Số điện thoại
        public string Role { get; set; }          // Vai trò: "True - Admin" hoặc "False - Customer"
    }
}
