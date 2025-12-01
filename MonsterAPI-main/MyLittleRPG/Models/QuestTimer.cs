using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLittleRPG_ElGuendouz.Models
{
    [Table("QuestTimer")]
    public class QuestTimer
    {
        public const int DEFAULT_INTERVAL_MINUTES = 10;
        public const int SECONDS_PER_MINUTE = 60;

        [Key]
        public int Id { get; set; }
        
        public DateTime NextGenerationTime { get; set; }
        
        public DateTime LastGenerationTime { get; set; }
        
        public int IntervalMinutes { get; set; }

        public QuestTimer()
        {
            IntervalMinutes = DEFAULT_INTERVAL_MINUTES;
        }
    }
}
