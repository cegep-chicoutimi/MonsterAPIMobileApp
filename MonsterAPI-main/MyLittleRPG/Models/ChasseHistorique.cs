using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLittleRPG_ElGuendouz.Models
{
    [Table("ChasseHistorique")]
    public class ChasseHistorique
    {
        [Key]
        public int Id { get; set; }

        public int idPersonnage { get; set; }

        public int idMonstre { get; set; }

        public DateTime DateChasse { get; set; }

        public bool Vaincu { get; set; }

        public ChasseHistorique()
        {
            DateChasse = DateTime.UtcNow;
        }

        public ChasseHistorique(int idPersonnage, int idMonstre, bool vaincu)
        {
            this.idPersonnage = idPersonnage;
            this.idMonstre = idMonstre;
            this.Vaincu = vaincu;
            this.DateChasse = DateTime.UtcNow;
        }
    }
}
