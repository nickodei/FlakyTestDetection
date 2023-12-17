namespace Application.LogParsers.Models;

public class TestRun
{
    public required TestIdentifier identifier { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorLocation { get; set; }
    public string StackTrace { get; set; }
}