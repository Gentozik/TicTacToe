using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginSuccessModel : PageModel
{
    public LoginSuccessModel(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; set; }
}
