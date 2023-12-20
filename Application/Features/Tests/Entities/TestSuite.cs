using Application.Features.Github.Entities;

namespace Application.Features.Tests.Entities;

public class TestSuite
{
    public int Id { get; set; }
    
    public long JobId { get; set; }
    public Job? Job { get; set; }
    
    public int CountFlakyTests { get; set; }
    public int CountPassedTests { get; set; }
    public int CountFailedTests { get; set; }
    public int CountSkippedTests { get; set; }
    
    public ICollection<Test> Tests { get; set; } = [];
}