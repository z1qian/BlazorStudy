using System.ComponentModel.DataAnnotations;

namespace BlazingPizza.Model;

public class UserModel
{
    [Required(ErrorMessage = "请输入用户名")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "请输入邮箱")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string Email { get; set; } = string.Empty;
}
