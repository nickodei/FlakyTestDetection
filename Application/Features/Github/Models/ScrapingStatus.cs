namespace Application.Features.Github.Models;

public class ScrapingStatus
{
    public ScrapingStatus()
    {
        JobStatusList = [];
    }
    
    public ScrapingStatus(IEnumerable<string> jobNames)
    {
        JobStatusList = jobNames.Select(job => new JobStatus() { Name = job }).ToList();
    }
    
    public List<JobStatus> JobStatusList { get; set; }

    public JobStatus GetJobStatus(string name)
    {
        return JobStatusList.Find(x => x.Name == name)!;
    }

    public void IncreaseJobNotFount(string job)
    {
        var jobStatus = GetJobStatus(job);
        jobStatus.AmountJobNotFound++;
    }
    
    public void IncreaseUnexpectedError(string job)
    {
        var jobStatus = GetJobStatus(job);
        jobStatus.AmountUnexpectedError++;
    }
    
    public void IncreaseParsingError(string job)
    {
        var jobStatus = GetJobStatus(job);
        jobStatus.AmountUnexpectedError++;
    }
    
    
}

public class JobStatus
{
    public required string Name { get; set; }
    public int AmountJobNotFound { get; set; } = 0;
    public int AmountParsingError { get; set; } = 0;
    public int AmountUnexpectedError { get; set; } = 0;
}