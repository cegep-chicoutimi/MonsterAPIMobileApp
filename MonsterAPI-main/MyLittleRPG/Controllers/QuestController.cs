using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MyLittleRPG_ElGuendouz.Data.Context;
using MyLittleRPG_ElGuendouz.Models;
using MyLittleRPG_ElGuendouz.DTOs;

namespace MyLittleRPG_ElGuendouz.Controllers
{
    [Route("api/[controller]")]
    public class QuestController : Controller
    {
        private readonly MonsterContext _context;

        public QuestController(MonsterContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Quest>>> Get()
        {
            return Ok(_context.Quest.ToList());
        }

        [HttpGet("Get/{id}")]
        public async Task<ActionResult<List<Quest>>> GetByCharacterId(int id)
        {
            if (!_context.Character.Any(c => c.idPersonnage == id)) return NotFound();

            else
            {
                return Ok(_context.Quest.Where(q => q.idPersonnage == id).ToList());
            }
        }

        [HttpGet("Timer")]
        public async Task<ActionResult<QuestTimer>> GetTimer()
        {
            var timer = await _context.QuestTimer.FirstOrDefaultAsync();
            
            if (timer == null)
            {
                return NotFound("Le timer de quêtes n'a pas encore été initialisé.");
            }

            return Ok(timer);
        }

        [HttpGet("TimeRemaining")]
        public async Task<ActionResult<object>> GetTimeRemaining()
        {
            var timer = await _context.QuestTimer.FirstOrDefaultAsync();
            
            if (timer == null)
            {
                return NotFound("Le timer de quêtes n'a pas encore été initialisé.");
            }

            var timeRemaining = timer.NextGenerationTime - DateTime.UtcNow;
            
            // Si le timer est expiré, calculer combien de temps jusqu'au prochain cycle
            if (timeRemaining.TotalSeconds <= 0)
            {
                // Calculer combien de cycles de 10 minutes se sont écoulés depuis la dernière génération
                var timeSinceLastGen = DateTime.UtcNow - timer.LastGenerationTime;
                var cyclesElapsed = (int)(timeSinceLastGen.TotalMinutes / timer.IntervalMinutes);
                
                // Calculer le prochain temps de génération
                var nextGenTime = timer.LastGenerationTime.AddMinutes((cyclesElapsed + 1) * timer.IntervalMinutes);
                timeRemaining = nextGenTime - DateTime.UtcNow;
            }
            
            var secondsRemaining = Math.Max(0, (int)timeRemaining.TotalSeconds);
            var minutesRemaining = secondsRemaining / QuestTimer.SECONDS_PER_MINUTE;

            // Format: "MM:SS"
            string formattedTime = $"{minutesRemaining:D2}:{(secondsRemaining % QuestTimer.SECONDS_PER_MINUTE):D2}";
            
            return Ok(new { FormattedTimeRemaining = formattedTime });
        }
    }
}
