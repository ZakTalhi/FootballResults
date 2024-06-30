//using ClassLibrary.BusinessLogic;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Abstractions;


//namespace FootballResultsTests.BusinessLogic_Test;

//public class MessagesTests
//{
//    [Fact]
//    public void Greeting_InEnglish()
//    {
//        ILogger<Messages> logger = new NullLogger<Messages>();
//        Messages messages = new(logger);

//        string expected = "Hello, World!";
//        string actual = messages.Greeting("en");

//        Assert.Equal(expected, actual);

//    }

//    [Fact]
//    public void Greeting_InSpanish()
//    {
//        ILogger<Messages> logger = new NullLogger<Messages>();
//        Messages messages = new(logger);

//        string expected = "Hola, Mundo!";
//        string actual = messages.Greeting("es");

//        Assert.Equal(expected, actual);

//    }

//    [Fact]
//    public void Greeting_Invalid()
//    {
//        ILogger<Messages> logger = new NullLogger<Messages>();
//        Messages messages = new(logger);



//        Assert.Throws<InvalidOperationException>(
//            () => messages.Greeting("fr"));

//    }
//}



using ClassLibrary.BusinessLogic;
using ClassLibrary.Models;
using Microsoft.Extensions.Logging;
using Moq;

public class MessagesTests
{
    private readonly Messages _messages;
    private readonly Mock<ILogger<Messages>> _mockLogger;

    public MessagesTests()
    {
        _mockLogger = new Mock<ILogger<Messages>>();
        _messages = new Messages(_mockLogger.Object);
    }

    [Fact]
    public void SelectMenu_FolderExists_ListsFiles()
    {
        // Arrange
        string folderPath = @"C:\YourFolderPathHere";
        Directory.CreateDirectory(folderPath);
        string fileName1 = Path.Combine(folderPath, "file1.txt");
        string fileName2 = Path.Combine(folderPath, "file2.txt");
        File.WriteAllText(fileName1, "```text\nTeamA;TeamB;win\n```");
        File.WriteAllText(fileName2, "```text\nTeamC;TeamD;draw\n```");

        using (var sw = new StringWriter())
        {
            Console.SetOut(sw);

            using (var sr = new StringReader(fileName1))
            {
                Console.SetIn(sr);

                // Act
                _messages.SelectMenu();

                // Assert
                string result = sw.ToString();
                Assert.Contains("file1.txt", result);
                Assert.Contains("file2.txt", result);                
            }
        }
    }

    [Fact]
    public void SelectMenu_InvalidFile_ShowsErrorMessage()
    {
        // Arrange
        string folderPath = @"C:\YourFolderPathHere";
        Directory.CreateDirectory(folderPath);
        string invalidFile = Path.Combine(folderPath, "invalidFile.txt");
        File.WriteAllText(invalidFile, "Invalid content");

        using (var sw = new StringWriter())
        {
            Console.SetOut(sw);

            using (var sr = new StringReader(invalidFile))
            {
                Console.SetIn(sr);

                // Act
                _messages.SelectMenu();

                // Assert
                string result = sw.ToString();
                Assert.Contains("does not meet the criteria", result);
            }
        }
    }

    [Fact]
    public void CheckFileFormat_ValidFile_ReturnsTrue()
    {
        // Arrange
        string fileName = "validFile.txt";
        File.WriteAllLines(fileName, new[]
        {
            "```text",
            "TeamA;TeamB;win",
            "TeamC;TeamD;draw",
            "```"
        });

        // Act
        bool result = _messages.CheckFileFormat(fileName);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CheckFileFormat_InvalidFile_MissingMarkers_ReturnsFalse()
    {
        // Arrange
        string fileName = "invalidFile.txt";
        File.WriteAllLines(fileName, new[]
        {
            "TeamA;TeamB;win",
            "TeamC;TeamD;draw"
        });

        // Act
        bool result = _messages.CheckFileFormat(fileName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CheckFileFormat_InvalidFile_IncorrectPartsCount_ReturnsFalse()
    {
        // Arrange
        string fileName = "invalidFile2.txt";
        File.WriteAllLines(fileName, new[]
        {
            "```text",
            "TeamA;TeamB;win;ExtraPart",
            "TeamC;TeamD;draw",
            "```"
        });

        // Act
        bool result = _messages.CheckFileFormat(fileName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CompetitionResults_ValidFile_SendsCorrectResults()
    {
        // Arrange
        string fileName = "resultsFile.txt";
        File.WriteAllLines(fileName, new[]
        {
            "```text",
            "TeamA;TeamB;win",
            "TeamC;TeamD;draw",
            "TeamA;TeamC;loss",
            "```"
        });

        List<Results> expectedResults = new List<Results>
        {
            new Results { Team = "TeamA", MatchesPlayed = 2, MatchesWon = 1, MatchesDrawn = 0, MatchesLost = 1, Points = 3, WinPercentage = 50.0 },
            new Results { Team = "TeamB", MatchesPlayed = 1, MatchesWon = 0, MatchesDrawn = 0, MatchesLost = 1, Points = 0, WinPercentage = 0.0 },
            new Results { Team = "TeamC", MatchesPlayed = 2, MatchesWon = 0, MatchesDrawn = 1, MatchesLost = 1, Points = 1, WinPercentage = 0.0 },
            new Results { Team = "TeamD", MatchesPlayed = 1, MatchesWon = 0, MatchesDrawn = 1, MatchesLost = 0, Points = 1, WinPercentage = 0.0 }
        };

        List<Results> actualResults = null;
        _messages.MessageSent += (sender, e) =>
        {
            actualResults = e.Content;
        };

        // Act
        _messages.CompetitionResults(fileName);

        // Assert
        Assert.NotNull(actualResults);
        Assert.Equal(expectedResults.Count, actualResults.Count);

        for (int i = 0; i < expectedResults.Count; i++)
        {
            Assert.Equal(expectedResults[i].Team, actualResults[i].Team);
            Assert.Equal(expectedResults[i].MatchesPlayed, actualResults[i].MatchesPlayed);
            Assert.Equal(expectedResults[i].MatchesWon, actualResults[i].MatchesWon);
            Assert.Equal(expectedResults[i].MatchesDrawn, actualResults[i].MatchesDrawn);
            Assert.Equal(expectedResults[i].MatchesLost, actualResults[i].MatchesLost);
            Assert.Equal(expectedResults[i].Points, actualResults[i].Points);
            Assert.Equal(expectedResults[i].WinPercentage, actualResults[i].WinPercentage);
        }
    }
}