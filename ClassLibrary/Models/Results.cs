namespace ClassLibrary.Models
{
    public class Results
    {
        public string Team { get; set; }
        public int MatchesPlayed { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesDrawn { get; set; }
        public int MatchesLost { get; set; }
        public int Points { get; set; }
        public double WinPercentage { get; set; }
    }
}
