using ClassLibrary.Models;
using Microsoft.Extensions.Logging; 
using static ClassLibrary.BusinessLogic.IMessages;

namespace ClassLibrary.BusinessLogic
{
    public class Messages : IMessages
    {
        private readonly ILogger<Messages> _log;

        public event EventHandler<MessageEventArgs> MessageSent;

        public Messages(ILogger<Messages> log)
        {
            _log = log;
        }

        public void SelectMenu()
        {

            string folderPath = @"C:\YourFolderPathHere"; 

            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath);

                Console.WriteLine($"Files in folder '{folderPath}':");
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    Console.WriteLine(fileName);

                }

                string selectedFile = "";
                bool fileFound = false;

                while (!fileFound)
                {
                    Console.WriteLine("\nEnter the file name you want to display first: ");
                    selectedFile = Console.ReadLine();

                    string firstFilePath = Path.Combine(folderPath, selectedFile);

                    if (File.Exists(firstFilePath))
                    {
                        string firstFileContent = File.ReadAllText(firstFilePath);
                        Console.WriteLine($"Content of file '{selectedFile}':");
                        Console.WriteLine(firstFileContent);
                        fileFound = true;

                        bool fileAFormat = CheckFileFormat(firstFilePath);

                        if (fileAFormat)
                        {

                            CompetitionResults(firstFilePath);
                        }
                        else
                        {
                            Console.WriteLine($"File '{selectedFile}' does not meet the criteria, check the file format!");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"File '{selectedFile}' not found in the folder. Please try again.");
                    }
                }


            }
            else
            {
                Console.WriteLine($"Folder '{folderPath}' does not exist.");
            }            
        }



        public bool CheckFileFormat(string fileName)
        {
            List<string> partsList = new List<string>();
            int lineCount = 0;


            string[] lines = File.ReadAllLines(fileName);

            if (lines.Length > 3 && lines[0].Trim() == "```text" && lines[lines.Length - 1].Trim() == "```")
            {
                foreach (string line in lines)
                {
                    if (line.Trim() != "```" && line.Trim() != "```text")
                    {
                        string[] parts = line.Split(';');

                        if (parts.Length != 3)
                        {
                            return false;
                        }
                    }
                }

            }
            else
            {
                return false;
            }

            return true;
        }



        public void CompetitionResults(string fileName)
        {
            List<Results> teamsScore = new List<Results>();


            string[] lines = File.ReadAllLines(fileName);

            for (int i = 1; i < lines.Length - 1; i++)
            {
                string[] parts = lines[i].Split(';');

                bool isWin = parts[2] == "win";
                bool isLoss = parts[2] == "loss";
                bool isDraw = parts[2] == "draw";

                if (parts.Length == 3 && (parts[2] == "win" || parts[2] == "loss" || parts[2] == "draw"))
                {

                    for (int k = 0; k < 2; k++)
                    {
                        Results? teamAlreadyExist = teamsScore.Find(t => t.Team == parts[k]);

                        
                        if (teamAlreadyExist is not null)
                        {
                            
                            teamAlreadyExist.MatchesPlayed++;

                            switch (parts[2])
                            {
                                case "win":
                                    
                                    teamAlreadyExist.MatchesWon += k == 0 ? 1 : 0;
                                    break;
                                case "draw":
                                    teamAlreadyExist.MatchesDrawn++;
                                    break;
                                case "loss":
                                    
                                    teamAlreadyExist.MatchesLost += k == 0 ? 1 : 0;
                                    break;
                            }
                            
                            if (parts[2] == "win" && k == 1 || parts[2] == "loss" && k == 1)
                            {
                                teamAlreadyExist.MatchesWon += parts[2] == "win" ? 1 : 0;
                                teamAlreadyExist.MatchesLost += parts[2] == "loss" ? 1 : 0;
                            }
                        }
                        else
                        {                            
                                int matchesWon = (isWin && k == 0) || (isLoss && k == 1) ? 1 : 0;
                                int matchesLost = (isLoss && k == 0) || (isWin && k == 1) ? 1 : 0;
                                int matchesDrawn = isDraw ? 1 : 0;

                                
                                teamsScore.Add(new Results
                                {
                                    Team = parts[k],
                                    MatchesPlayed = 1,
                                    MatchesWon = matchesWon,
                                    MatchesDrawn = matchesDrawn,
                                    MatchesLost = matchesLost,
                                    Points = 0, 
                                    WinPercentage = 0.0 
                                });                           

                        }
                    }
                }
            }
            foreach (var score in teamsScore)
            {
                score.WinPercentage = (score.MatchesPlayed == 0) ? 0 : (double)score.MatchesWon / score.MatchesPlayed * 100;
                score.Points = score.MatchesWon * 3 + score.MatchesDrawn;
                
            }

            var e = new MessageEventArgs(teamsScore);
            MessageSent?.Invoke(this, e);            
        } 

    }
}
