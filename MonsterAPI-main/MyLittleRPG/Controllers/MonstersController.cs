using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittleRPG_ElGuendouz.Data.Context;
using MyLittleRPG_ElGuendouz.Models;

namespace MyLittleRPG_ElGuendouz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonstersController : ControllerBase
    {
        private readonly MonsterContext _context;

        public MonstersController(MonsterContext context)
        {
            _context = context;
        }

        [HttpGet("GetInstances")]
        public ActionResult<IEnumerable<InstanceMonstre>> GetInstances()
        {
            return Ok(_context.InstanceMonstre);
        }


        [HttpGet("IsConnected")]
        public ActionResult<bool> IsConnected()
        {
            return Ok(true);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetMonstres(
            [FromQuery] int offset = 0, 
            [FromQuery] int limit = 20, 
            [FromQuery] string? nom = null,
            [FromQuery] string? type1 = null,
            [FromQuery] string? type2 = null)
        {
            const int MAX_LIMIT = 100;

            //limite maximale
            if (limit > MAX_LIMIT) limit = MAX_LIMIT;
            if (limit <= 0) limit = 20;
            if (offset < 0) offset = 0;

            IQueryable<Monster> query = _context.Monsters;

            // Filtration par nom
            if (!string.IsNullOrWhiteSpace(nom))
            {
                query = query.Where(m => m.nom.Contains(nom));
            }

            // Filtration par type1
            if (!string.IsNullOrWhiteSpace(type1))
            {
                query = query.Where(m => m.type1 == type1);
            }

            // Filtration par type2
            if (!string.IsNullOrWhiteSpace(type2))
            {
                query = query.Where(m => m.type2 == type2);
            }

            // Compter le total avant pagination
            int total = await query.CountAsync();

            //pagination
            var monstres = await query
                .OrderBy(m => m.idMonster)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                Total = total,
                Offset = offset,
                Limit = limit,
                Count = monstres.Count,
                Data = monstres
            });
        }

        [HttpGet("random/{count}")]
        public async Task<ActionResult<IEnumerable<Monster>>> GetRandomMonsters(int count)
        {
            int totalCount = await _context.Monsters.CountAsync();
            if (totalCount == 0)
            {
                return NotFound("Aucun monstre trouvé.");
            }

            // si le user demande plus que ce qu'on a on renvoie tout
            if (count >= totalCount)
            {
                return await _context.Monsters.ToListAsync();
            }

            List<int> allIds = await _context.Monsters
                .Select(m => m.idMonster)
                .ToListAsync();

            // tirage aléatoire de 300 IDs uniques
            Random random = new Random();
            List<int> randomIds = allIds
                .OrderBy(x => random.Next())
                .Take(count)
                .ToList();

            // on récupère ensuite les monstres correspondant à ces IDs
            List<Monster> monsters = await _context.Monsters
                .Where(m => randomIds.Contains(m.idMonster))
                .ToListAsync();

            return Ok(monsters);
        }

        [HttpGet("ChasseHistorique/{idPersonnage}")]
        public async Task<ActionResult<object>> GetChasseHistorique(int idPersonnage)
        {
            var character = await _context.Character.FirstOrDefaultAsync(c => c.idPersonnage == idPersonnage);
            if (character == null)
            {
                return NotFound("Personnage non trouvé.");
            }

            var historique = await _context.ChasseHistorique
                .Where(ch => ch.idPersonnage == idPersonnage)
                .Join(_context.Monsters,
                    ch => ch.idMonstre,
                    m => m.idMonster,
                    (ch, m) => new
                    {
                        Monster = m,
                        DateChasse = ch.DateChasse,
                        Vaincu = ch.Vaincu
                    })
                .ToListAsync();

            // Grouper par monstre avec stats
            var statistiques = historique
                .GroupBy(h => h.Monster.idMonster)
                .Select(g => new
                {
                    Monstre = g.First().Monster,
                    TotalRencontres = g.Count(),
                    Victoires = g.Count(h => h.Vaincu),
                    Defaites = g.Count(h => !h.Vaincu),
                    DerniereChasse = g.Max(h => h.DateChasse)
                })
                .OrderByDescending(s => s.TotalRencontres)
                .ToList();

            return Ok(new
            {
                IdPersonnage = idPersonnage,
                TotalMonstresChasses = statistiques.Count,
                TotalRencontres = historique.Count,
                TotalVictoires = historique.Count(h => h.Vaincu),
                TotalDefaites = historique.Count(h => !h.Vaincu),
                Statistiques = statistiques
            });
        }

        [HttpGet("Types")]
        public async Task<ActionResult<object>> GetTypes()
        {
            var types1 = await _context.Monsters
                .Where(m => !string.IsNullOrEmpty(m.type1))
                .Select(m => m.type1)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            var types2 = await _context.Monsters
                .Where(m => !string.IsNullOrEmpty(m.type2))
                .Select(m => m.type2)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            var allTypes = types1.Union(types2).Distinct().OrderBy(t => t).ToList();

            return Ok(new
            {
                Types = allTypes,
                Count = allTypes.Count
            });
        }

        // POST: api/Monsters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Monster>> PostMonster(Monster monster)
        {
            _context.Monsters.Add(monster);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMonster", new { id = monster.idMonster }, monster);
        }
    }
}
