using System.ComponentModel.DataAnnotations;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.GamesManager;


namespace WebApp.Pages.CreateGame;

public class CreateModel(AppDbContext context) : PageModel
{
    [Required]
    [MinLength(1, ErrorMessage = "Nickname must be between 1 and 20 characters.")]
    [MaxLength(20, ErrorMessage = "Nickname must be between 1 and 20 characters.")]
    [BindProperty]
    public required string Nickname { get; set; }
        
    [Required]
    [Range(2, 7, ErrorMessage = "Players amount must be between 2 and 7.")]
    [BindProperty]
    public int PlayersMaxAmount { get; set; }

    [Required]
    [Range(2, 7, ErrorMessage = "Cards amount must be between 2 and 7.")]
    [BindProperty]
    public int CardsMaxInHand { get; set; }

    [BindProperty]
    public UnoCard.Value? CardValueToAvoid { get; set; }
        

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (ModelState.IsValid)
        {
            var gameManager = new GameManager(context);
            var data = gameManager.CreateTheGame(Nickname, PlayersMaxAmount, CardsMaxInHand, CardValueToAvoid);
            
            return RedirectToPage($"/GameWait/Index", new { data.gameId, data.playerId, isBoss = 1, maxAmount = PlayersMaxAmount });
                
        }
            
        return Page();
    }
}