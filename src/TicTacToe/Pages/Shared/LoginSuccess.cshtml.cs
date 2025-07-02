using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginSuccessModel : PageModel
{
    public Guid UserId { get; set; }

    public void OnGet(Guid userId)
    {
        UserId = userId;
    }
}
