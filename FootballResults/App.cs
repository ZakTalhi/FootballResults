using ClassLibrary.BusinessLogic;
using ClassLibrary.Models;

namespace FootballResults
{

    public class App
    {
        private readonly IMessages _messages;

        public App(IMessages messages)
        {
            _messages = messages;
            _messages.MessageSent += callBack;
        }

        public void Run(string[] args)
        {         

            _messages.SelectMenu();
           
        }


        private void callBack(object sender, MessageEventArgs message)
        {
            var result = message.Content;
            ScoreDisplay(result);
        }
         

        static void ScoreDisplay(List<Results> results)
        {
            Console.WriteLine("Team\t\t\t| MP | W | D | L | P | Win%");
            Console.WriteLine("-----------------------------------------------");

            foreach (var result in results)
            {
                Console.WriteLine($"{result.Team,-20} | {result.MatchesPlayed,2} | {result.MatchesWon,2} | {result.MatchesDrawn,2} | {result.MatchesLost,2} | {result.Points,2} | {result.WinPercentage,5:F2}");
            }
        }

    }
}