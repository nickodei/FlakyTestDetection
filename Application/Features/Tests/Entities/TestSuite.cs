using System.Collections;

namespace Application.Features.Tests.Entities;

public class TestSuite
{
    public int Id { get; set; }
    public ICollection<Test> Tests { get; set; }
}